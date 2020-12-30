#[macro_use]
extern crate shrinkwraprs;

use detexify::{point::Point, Classifier, Stroke, Symbol};
use gio::prelude::*;
use gladis::Gladis;
use glib::{SourceId, Type};
use gtk::{
    prelude::*, AboutDialog, Application, ApplicationWindow, Button, DrawingArea, Label, Revealer,
    TreeView,
};

use itertools::Itertools;
use log::*;
use std::{
    f64::consts::PI,
    iter,
    sync::{Arc, RwLock},
    thread,
    time::{Duration, Instant},
};

#[derive(Gladis, Clone, Shrinkwrap)]
struct App {
    #[shrinkwrap(main_field)]
    window: ApplicationWindow,
    drawing_area: DrawingArea,
    tree_view: TreeView,
    clear_button: Button,
    notification_revealer: Revealer,
    notification_button: Button,
    notification_label: Label,
    about_button: Button,
    about_dialog: AboutDialog,
}

#[derive(Clone, Default)]
struct AppState {
    strokes: Vec<Stroke>,
    current_stroke: Stroke,
    is_mouse_down: bool,
}

const ICON_COLUMN: u32 = 0;
const TEXT_COLUMN: u32 = 1;
const COMMAND_COLUMN: u32 = 2;
const PACKAGE_COLUMN: u32 = 3;

const COLUMNS: [u32; 4] = [ICON_COLUMN, TEXT_COLUMN, COMMAND_COLUMN, PACKAGE_COLUMN];
const COLUMN_TYPES: [Type; 4] = [Type::String, Type::String, Type::String, Type::String];

enum ListItem<'a> {
    Symbol(detexify::Symbol),
    Score(&'a detexify::Score),
}

fn main() {
    env_logger::init();

    let application = Application::new(Some("fyi.zoey.TeX-Match"), Default::default())
        .expect("failed to initialize GTK application");

    application.connect_activate(move |application| {
        let (mut request_receiver, request_updater) = single_value_channel::channel::<AppState>();
        let (responce_sender, responce_receiver) =
            glib::MainContext::channel(glib::PRIORITY_DEFAULT);

        thread::spawn(move || {
            info!("classifier thread started");
            let classifier = Classifier::default();

            loop {
                let request = request_receiver.latest_mut();

                if let Some(app_state) = request.take() {
                    if let Some(sample) = detexify::StrokeSample::new(app_state.strokes) {
                        let start = Instant::now();
                        match classifier.classify(sample) {
                            Some(results) => {
                                responce_sender.send(results).expect("glib channel closed");
                            }
                            None => warn!("classifier returned None"),
                        }
                        info!(
                            "classification complete in {}ms",
                            start.elapsed().as_millis()
                        );
                    }
                }

                thread::sleep(Duration::from_millis(100));
            }
        });

        // resources.gresources is created by build.rs
        // it includes all the files in the resources directory
        let resource_bytes =
            include_bytes!(concat!(env!("OUT_DIR"), "/resources/resources.gresource"));
        let resource_data = glib::Bytes::from(&resource_bytes[..]);
        gio::resources_register(&gio::Resource::from_data(&resource_data).unwrap());

        // add embedded icons to theme
        let icon_theme = gtk::IconTheme::get_default().expect("failed to get default icon theme");
        icon_theme.add_resource_path("/fyi/zoey/TeX-Match/icons");

        let app: App = App::from_resource("/fyi/zoey/TeX-Match/app.glade")
            .unwrap_or_else(|e| panic!("failed to load app.glade: {}", e));
        app.set_application(Some(application));

        app.about_dialog
            .set_version(Some(env!("CARGO_PKG_VERSION")));

        let app_state = Arc::new(RwLock::new(AppState::default()));
        let notification_source_id: Arc<RwLock<Option<SourceId>>> = Arc::new(RwLock::new(None)); // SourceId doesnt implement clone, so must be seperate from AppState

        // setup symbols store
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

        // close notification on dismiss
        {
            let notification_revealer = app.notification_revealer.clone();
            app.notification_button
                .connect_button_press_event(move |_button, _event| {
                    notification_revealer.set_reveal_child(false);
                    Inhibit(false)
                });
        }

        // on about click, open about dialog
        {
            let about_dialog: AboutDialog = app.about_dialog.clone();
            app.about_button.connect_clicked(move |_| {
                let responce = about_dialog.run();
                if responce == gtk::ResponseType::DeleteEvent
                    || responce == gtk::ResponseType::Cancel
                {
                    about_dialog.hide();
                }
            });
        }

        // on symbol click
        // copy to clipboard and display a notification
        {
            let notification_label = app.notification_label.clone();
            let notification_revealer = app.notification_revealer.clone();
            let notification_source_id = Arc::clone(&notification_source_id);

            app.tree_view
                .connect_row_activated(move |tree_view, _path, _column| {
                    let store: gtk::ListStore = tree_view.get_model().unwrap().downcast().unwrap();

                    if let (Some(path), _) = tree_view.get_cursor() {
                        tree_view.get_selection().unselect_all();

                        let buffer = store
                            .get_value(
                                &store
                                    .get_iter(&path)
                                    .expect("failed to get list store iter"),
                                COMMAND_COLUMN as i32,
                            )
                            .downcast::<String>()
                            .expect("failed to downcast column to string")
                            .get()
                            .expect("column string was None");

                        info!("clicked: {}", buffer);

                        notification_label.set_text(&format!("Copied \"{}\" to clipboard", buffer));

                        notification_revealer.set_reveal_child(true);
                        {
                            let notification_revealer = notification_revealer.clone();
                            // dismiss after 3000ms

                            let mut source_id = notification_source_id.write().unwrap();

                            // cancel old notification timeout
                            if source_id.is_some() {
                                glib::source_remove(source_id.take().unwrap());
                            }

                            let new_source_id = glib::timeout_add_local(3000, move || {
                                notification_revealer.set_reveal_child(false);
                                Continue(false)
                            });

                            source_id.replace(new_source_id);
                        }

                        let clipboard = tree_view.get_clipboard(&gdk::SELECTION_CLIPBOARD);
                        clipboard.set_text(&buffer);
                    }
                });
        }

        // on receive classification result
        // update store
        {
            let tree_view = app.tree_view.clone();
            responce_receiver.attach(None, move |msg| {
                let store: gtk::ListStore = tree_view.get_model().unwrap().downcast().unwrap();
                update_store(store, msg.iter().map(ListItem::Score));
                glib::Continue(true)
            });
        }

        // on mouse down
        // set `is_down` to true
        {
            let app_state = Arc::clone(&app_state);
            app.drawing_area
                .connect_button_press_event(move |area, btn| {
                    let mut app_state = app_state.write().unwrap();
                    app_state.is_mouse_down = true;
                    if let Some((x, y)) = btn.get_coords() {
                        app_state.current_stroke.add_point(Point { x, y });
                        area.queue_draw(); // trigger draw event
                    }
                    Inhibit(false)
                });
        }

        // on mouse up
        // set `is_down` to false and add `new_stroke` to `strokes`
        {
            let app_state = Arc::clone(&app_state);
            app.drawing_area
                .connect_button_release_event(move |_area, _btn| {
                    let mut app_state = app_state.write().unwrap();
                    app_state.is_mouse_down = false;

                    let new_stroke = app_state.current_stroke.clone();
                    app_state.strokes.push(new_stroke);
                    app_state.current_stroke = Stroke::default();

                    request_updater
                        .update(Some(app_state.clone()))
                        .expect("classification channel closed");

                    Inhibit(false)
                });
        }

        // on mouse movement
        // if the mouse is down, add the current location to current stroke
        {
            let app_state = Arc::clone(&app_state);
            app.drawing_area
                .connect_motion_notify_event(move |area, motion| {
                    let mut app_state = app_state.write().unwrap();

                    if app_state.is_mouse_down {
                        if let Some((x, y)) = motion.get_coords() {
                            app_state.current_stroke.add_point(Point { x, y });
                            area.queue_draw(); // trigger draw event
                        }
                    }

                    Inhibit(false)
                });
        }

        // on clear button
        // remove strokes
        {
            let app_state = Arc::clone(&app_state);
            let drawing_area = app.drawing_area.clone();
            app.clear_button.connect_clicked(move |_button| {
                let mut app_state = app_state.write().unwrap();
                app_state.strokes.clear();
                app_state.current_stroke.clear();
                drawing_area.queue_draw(); // trigger draw event
            });
        }

        {
            let app_state = Arc::clone(&app_state);
            app.drawing_area.connect_draw(move |_area, ctx| {
                let app_state = app_state.read().unwrap();

                for stroke in app_state
                    .strokes
                    .iter()
                    .chain(iter::once(&app_state.current_stroke))
                {
                    let mut looped = false;
                    for (p, q) in stroke.points().cloned().tuple_windows() {
                        ctx.set_line_width(3.0);
                        ctx.set_source_rgb(0.8, 0.8, 0.8);
                        ctx.set_line_cap(cairo::LineCap::Round);
                        ctx.move_to(p.x, p.y);
                        ctx.line_to(q.x, q.y);
                        ctx.stroke();
                        looped = true;
                    }

                    if !looped && stroke.points().count() == 1 {
                        let p = stroke.points().next().unwrap();
                        ctx.set_source_rgb(0.8, 0.8, 0.8);
                        ctx.arc(p.x, p.y, 1.5, 0.0, 2.0 * PI);
                        ctx.fill();
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

        store.set(
            &store.append(),
            &COLUMNS,
            &[&icon, &s, &symbol.command, &symbol.package],
        );
    }
}
