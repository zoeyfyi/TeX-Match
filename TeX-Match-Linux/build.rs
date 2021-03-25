use io::Write;
use std::{
    env, fs, io,
    path::{Path, PathBuf},
    process::Command,
};

const XML_HEADER: &str = r#"<?xml version="1.0" encoding="UTF-8"?>
<gresources>
    <gresource prefix="/fyi/zoey/TeX-Match">
"#;

const XML_FOOTER: &str = r#"    </gresource>
</gresources>
"#;

fn add_files(xml: &mut String, root_folder: PathBuf, folder: PathBuf) {
    for path in fs::read_dir(&folder).unwrap() {
        let path = path.as_ref().unwrap();

        // skip temporary files
        if path.path().display().to_string().ends_with('~') {
            continue;
        }

        if path.path().is_file() {
            xml.push_str(&format!(
                "\t\t<file>{}</file>\n",
                path.path()
                    .display()
                    .to_string()
                    .replace("\\", "/")
                    .trim_start_matches(&{
                        let mut s = root_folder.display().to_string();
                        println!("{}", s);
                        s.push('/');
                        s
                    })
            ));
        } else if path.path().is_dir() {
            add_files(xml, root_folder.clone(), path.path());
        } else {
            panic!("expected file or folder");
        }
    }
}

fn main() {
    let out_dir = env::var("OUT_DIR").unwrap();
    let mut resources = Path::new(&out_dir).to_path_buf();
    resources.push("resources");

    fs::create_dir_all(resources.clone()).unwrap();
    fs_extra::dir::copy("resources", out_dir, &{
        let mut options = fs_extra::dir::CopyOptions::new();
        options.copy_inside = true;
        options.overwrite = true;
        options
    })
    .unwrap();

    let mut icons = {
        let mut p = resources.clone();
        p.push("icons/scalable");
        p
    };

    fs_extra::dir::copy("../symbols", &icons, &{
        let mut options = fs_extra::dir::CopyOptions::new();
        options.copy_inside = true;
        options.overwrite = true;
        options
    })
    .unwrap();

    let symbols = {
        let mut p = icons.clone();
        p.push("symbols");
        p
    };

    icons.push("actions");

    fs::remove_dir_all(&icons).ok();
    fs::rename(symbols, &icons).unwrap();

    for icon in fs::read_dir(&icons)
        .unwrap()
        .collect::<Vec<io::Result<fs::DirEntry>>>()
        .into_iter()
    {
        let icon_path = icon.unwrap().path();
        let postfixed = icon_path.to_str().unwrap().replace(".svg", "-symbolic.svg");
        fs::rename(icon_path, postfixed).unwrap();
    }

    let mut xml = String::with_capacity(XML_HEADER.len() + XML_FOOTER.len() + 1024);

    xml.push_str(XML_HEADER);
    add_files(&mut xml, resources.clone(), resources.clone());
    xml.push_str(XML_FOOTER);

    let resource_xml = {
        let mut f = resources.clone();
        f.push("resources.xml");
        f
    };
    let mut file = fs::File::create(resource_xml).unwrap();
    file.write_all(xml.as_bytes()).unwrap();

    let mut cmd = Command::new(if let Ok(path) = env::var("GLIB_COMPILE_RESOURCES") {
        path
    } else if cfg!(target_os = "window") {
        "glib-compile-resources.exe".to_owned()
    } else {
        "glib-compile-resources".to_owned()
    });

    cmd.arg("resources.xml")
        .current_dir(resources)
        .output()
        .expect("failed to compile resources");

    #[cfg(windows)]
    {
        let mut res = winres::WindowsResource::new();
        res.set_icon("wix/tex-match.ico");
        res.compile().unwrap();
    }
}
