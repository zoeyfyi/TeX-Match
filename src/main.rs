#[macro_use]
extern crate shrinkwraprs;

use detexify::{point::Point, Classifier, Stroke, StrokeSample, Symbol};
use gio::prelude::*;
use gladis::Gladis;
use glib::Type;
use gtk::{prelude::*, Application, ApplicationWindow, Button, DrawingArea, TreeView};
use itertools::Itertools;
use log::{info, warn};
use std::{
    iter,
    sync::{Arc, RwLock},
};

#[derive(Gladis, Clone, Shrinkwrap)]
struct App {
    #[shrinkwrap(main_field)]
    window: ApplicationWindow,
    drawing_area: DrawingArea,
    tree_view: TreeView,
    clear_button: Button,
}

#[derive(Default)]
struct DrawState {
    strokes: Vec<Stroke>,
    current_stroke: Stroke,
    is_down: bool,
    classifier: Classifier,
}
const ICON_COLUMN: u32 = 0;
const TEXT_COLUMN: u32 = 1;

const COLUMNS: [u32; 2] = [ICON_COLUMN, TEXT_COLUMN];
const COLUMN_TYPES: [Type; 2] = [Type::String, Type::String];

enum ListItem<'a> {
    Symbol(detexify::Symbol),
    Score(&'a detexify::Score),
}

fn main() {
    env_logger::init();

    let application = Application::new(Some("uk.co.mrbenshef.TeX-Match"), Default::default())
        .expect("failed to initialize GTK application");

    application.connect_activate(move |application| {
        // resources.gresources is created by build.rs
        // it includes all the files in the resources directory
        let resource_bytes =
            include_bytes!(concat!(env!("OUT_DIR"), "/resources/resources.gresource"));
        let resource_data = glib::Bytes::from(&resource_bytes[..]);
        gio::resources_register(&gio::Resource::from_data(&resource_data).unwrap());

        // add embedeed icons to theme
        let icon_theme = gtk::IconTheme::get_default().expect("failed to get default icon theme");
        icon_theme.add_resource_path("/uk/co/mrbenshef/TeX-Match/icons");

        let app: App = App::from_resource("/uk/co/mrbenshef/TeX-Match/app.glade")
            .unwrap_or_else(|e| panic!("failed to load app.glade: {}", e));
        app.set_application(Some(application));

        {
            let store = gtk::ListStore::new(&COLUMN_TYPES);

            app.tree_view.set_model(Some(&store));

            // icon column
            {
                let renderer = gtk::CellRendererPixbuf::new();
                renderer.set_padding(8, 16);
                renderer.set_property_stock_size(gtk::IconSize::Dnd);

                let column = gtk::TreeViewColumn::new();
                column.pack_start(&renderer, false);
                column.add_attribute(&renderer, "icon-name", ICON_COLUMN as i32);

                app.tree_view.append_column(&column);
            }

            {
                let renderer = gtk::CellRendererText::new();

                let column = gtk::TreeViewColumn::new();
                column.pack_start(&renderer, true);
                column.set_sizing(gtk::TreeViewColumnSizing::Autosize);
                column.add_attribute(&renderer, "markup", TEXT_COLUMN as i32);

                app.tree_view.append_column(&column);
            }

            update_store(store, detexify::iter_symbols().map(ListItem::Symbol))
        }

        let draw_state = Arc::new(RwLock::new(DrawState::default()));

        // on mouse down
        // set `is_down` to true
        {
            let draw_state = Arc::clone(&draw_state);
            app.drawing_area
                .connect_button_press_event(move |_area, _btn| {
                    let mut draw_state = draw_state.write().unwrap();
                    draw_state.is_down = true;

                    Inhibit(false)
                });
        }

        // on mouse up
        // set `is_down` to false and add `new_stroke` to `strokes`
        {
            let draw_state = Arc::clone(&draw_state);
            let tree_view = app.tree_view.clone();
            app.drawing_area
                .connect_button_release_event(move |_area, _btn| {
                    let mut draw_state = draw_state.write().unwrap();
                    draw_state.is_down = false;

                    let new_stroke = draw_state.current_stroke.clone();
                    draw_state.strokes.push(new_stroke);
                    draw_state.current_stroke = Stroke::default();

                    if let Some(sample) = StrokeSample::new(draw_state.strokes.clone()) {
                        if let Some(results) = draw_state.classifier.classify(sample) {
                            let store: gtk::ListStore =
                                tree_view.get_model().unwrap().downcast().unwrap();

                            update_store(store, results.iter().map(ListItem::Score));
                        }
                    }

                    Inhibit(false)
                });
        }

        // on mouse movement
        // if the mouse is down, add the current location to current stroke
        {
            let draw_state = Arc::clone(&draw_state);
            app.drawing_area
                .connect_motion_notify_event(move |area, motion| {
                    let mut draw_state = draw_state.write().unwrap();

                    if draw_state.is_down {
                        if let Some((x, y)) = motion.get_coords() {
                            draw_state.current_stroke.add_point(Point { x, y });
                            area.queue_draw(); // trigger draw event
                        }
                    }

                    Inhibit(false)
                });
        }

        // on clear button
        // remove strokes
        {
            let draw_state = Arc::clone(&draw_state);
            let drawing_area = app.drawing_area.clone();
            app.clear_button.connect_clicked(move |_button| {
                let mut draw_state = draw_state.write().unwrap();
                draw_state.strokes.clear();
                draw_state.current_stroke.clear();
                drawing_area.queue_draw(); // trigger draw event
            });
        }

        {
            let draw_state = Arc::clone(&draw_state);
            app.drawing_area.connect_draw(move |_area, ctx| {
                let draw_state = draw_state.read().unwrap();

                for stroke in draw_state
                    .strokes
                    .iter()
                    .chain(iter::once(&draw_state.current_stroke))
                {
                    for (p, q) in stroke.points().cloned().tuple_windows() {
                        ctx.set_line_width(3.0);
                        ctx.set_source_rgb(0.8, 0.8, 0.8);
                        ctx.set_line_cap(cairo::LineCap::Round);
                        ctx.move_to(p.x, p.y);
                        ctx.line_to(q.x, q.y);
                        ctx.stroke();
                    }
                }

                Inhibit(false)
            });
        }

        app.show_all();
    });

    application.run(&[]);
}

fn update_store<'a>(store: gtk::ListStore, results: impl Iterator<Item = ListItem<'a>>) {
    store.clear();

    info!("updating");

    for result in results {
        let (id, symbol, score) = match result {
            ListItem::Symbol(s) => (s.id().to_string(), s, -1.0f64),
            ListItem::Score(detexify::Score { id, score }) => {
                if let Some(symbol) = Symbol::from_id(id) {
                    (id.clone(), symbol, *score)
                } else {
                    warn!("could not find id: {}", id);
                    continue;
                }
            }
        };

        let mode = match (symbol.text_mode, symbol.math_mode) {
            (true, true) => "textmode & mathmode",
            (true, false) => "textmode",
            (false, true) => "mathmode",
            (false, false) => "",
        };

        let mut s = String::with_capacity(256);

        if symbol.package != "latex2e" {
            s.push_str(&format!(
                "<span size=\"smaller\">\\usepackage{{ {} }}</span>\n",
                symbol.package,
            ));
        }

        s.push_str(&format!(
            "<b>{}</b>\n<span size=\"x-small\">{}",
            symbol.command, mode
        ));

        if score >= 0.0 {
            s.push_str(&format!(" (score: {:.4})", score));
        }

        s.push_str("</span>");

        s = s.replace("&", "&amp;");

        let icon = format!("{}-symbolic", id);

        store.set(&store.append(), &COLUMNS, &[&icon, &s]);
    }
}
