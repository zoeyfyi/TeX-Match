#[macro_use]
extern crate shrinkwraprs;

use gio::prelude::*;
use gladis::Gladis;
use gtk::{prelude::*, Application, ApplicationWindow, DrawingArea, TreeView};

#[derive(Gladis, Clone, Shrinkwrap)]
struct App {
    #[shrinkwrap(main_field)]
    window: ApplicationWindow,
    drawing_area: DrawingArea,
    tree_view: TreeView,
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

        let app = App::from_resource("/uk/co/mrbenshef/TeX-Match/app.glade")
            .unwrap_or_else(|e| panic!("failed to load app.glade: {}", e));
        app.set_application(Some(application));
        app.show_all();
    });

    application.run(&[]);
}
