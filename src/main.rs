#[macro_use]
extern crate shrinkwraprs;

use cairo::LineCap;
use gio::prelude::*;
use gladis::Gladis;
use gtk::{prelude::*, Application, ApplicationWindow, Button, DrawingArea, TreeView};
use itertools::Itertools;
use std::{
    f64::consts::PI,
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
    strokes: Vec<Vec<(f64, f64)>>,
    current_stroke: Vec<(f64, f64)>,
    is_down: bool,
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

        let app: App = App::from_resource("/uk/co/mrbenshef/TeX-Match/app.glade")
            .unwrap_or_else(|e| panic!("failed to load app.glade: {}", e));
        app.set_application(Some(application));

        let draw_state = Arc::new(RwLock::new(DrawState::default()));

        // on mouse down, set `is_down` to true
        {
            let draw_state = Arc::clone(&draw_state);
            app.drawing_area
                .connect_button_press_event(move |area, btn| {
                    let mut draw_state = draw_state.write().unwrap();
                    draw_state.is_down = true;

                    Inhibit(false)
                });
        }

        // on mouse up, set `is_down` to false and add `new_stroke` to `strokes`
        {
            let draw_state = Arc::clone(&draw_state);
            app.drawing_area
                .connect_button_release_event(move |area, btn| {
                    let mut draw_state = draw_state.write().unwrap();
                    draw_state.is_down = false;

                    let new_stroke = draw_state.current_stroke.drain(0..).collect();
                    draw_state.strokes.push(new_stroke);

                    Inhibit(false)
                });
        }

        // on mouse movement, if the mouse is down, add the current location to current stroke
        {
            let draw_state = Arc::clone(&draw_state);
            app.drawing_area
                .connect_motion_notify_event(move |area, motion| {
                    let mut draw_state = draw_state.write().unwrap();

                    if draw_state.is_down {
                        if let Some((x, y)) = motion.get_coords() {
                            draw_state.current_stroke.push((x, y));
                            area.queue_draw(); // trigger draw event
                        }
                    }

                    Inhibit(false)
                });
        }

        // on clear, remove strokes
        {
            let draw_state = Arc::clone(&draw_state);
            let drawing_area = app.drawing_area.clone();
            app.clear_button.connect_clicked(move |button| {
                let mut draw_state = draw_state.write().unwrap();
                draw_state.strokes.clear();
                draw_state.current_stroke.clear();
                drawing_area.queue_draw(); // trigger draw event
            });
        }

        {
            let draw_state = Arc::clone(&draw_state);
            app.drawing_area.connect_draw(move |area, ctx| {
                let draw_state = draw_state.read().unwrap();

                for stroke in draw_state
                    .strokes
                    .iter()
                    .chain(iter::once(&draw_state.current_stroke))
                {
                    for ((x1, y1), (x2, y2)) in stroke.iter().cloned().tuple_windows() {
                        ctx.set_line_width(3.0);
                        ctx.set_source_rgb(0.8, 0.8, 0.8);
                        ctx.set_line_cap(cairo::LineCap::Round);
                        ctx.move_to(x1, y1);
                        ctx.line_to(x2, y2);
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
