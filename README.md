<h1 align="center">TeX Match</h1>

<p align="center">
  <img src="TeX-Match.gif" width="660px">
</p>

<h2 align="center">A desktop version of <a href="https://detexify.kirelabs.org/classify.html">detexify</a></h2>

<br>

![Continuous integration](https://github.com/zoeyfyi/TeX-Match/workflows/Continuous%20integration/badge.svg)
![Release](https://github.com/zoeyfyi/TeX-Match/workflows/Release/badge.svg?branch=release)
[![Crates.io](https://img.shields.io/crates/v/tex-match)](https://crates.io/crates/tex-match)
[![tex-match](https://snapcraft.io//tex-match/badge.svg)](https://snapcraft.io/tex-match)
[![Flathub](https://img.shields.io/flathub/v/fyi.zoey.TeX-Match)](https://flathub.org/apps/details/fyi.zoey.TeX-Match)

### What is TeX Match?

If you work with LaTeX, you know its difficult to memorize the names of all the symbols. TeX Match allows you to search through over 1000 different LaTeX symbols by sketching. TeX Match is based of [detexify](https://detexify.kirelabs.org/classify.html) and is powered by a port of the [detexify classifier](https://github.com/zoeyfyi/detexify-rust).

### Features

- Over 1000 LaTeX symbols across multiple packages (same set as detexify) 
- Completely offline
- Crossplatform

### Screenshots

| Adwaita | Adwaita-Dark |
| :---: | :---: |
| ![light](screenshots/light.png) | ![dark](screenshots/dark.png) |

### Get Tex Match

<table width="100%">
    <tr>
        <th width="33.333%">Linux</th>
        <th width="33.333%">Windows</th>
        <th width="33.333%">MacOS</th>
    </tr>
    <tr>
        <td>
            <a
                href="https://github.com/zoeyfyi/TeX-Match/releases/latest/download/tex-match.linux.amd64">tex-match.linux.amd64</a>
            </br>
            <a
                href="https://github.com/zoeyfyi/TeX-Match/releases/latest/download/tex-match.flatpak">tex-match.flatpak</a>
            </br>
            <a href="https://github.com/zoeyfyi/TeX-Match/releases/latest/download/tex-match.snap">tex-match.snap</a>
            </br>
            <a href="https://snapcraft.io/tex-match"><img
                    src="https://snapcraft.io/static/images/badges/en/snap-store-black.svg"
                    alt="Get it from the Snap Store"></a>
            </br>
            <a href="https://flathub.org/apps/details/fyi.zoey.TeX-Match"><img width='190'
                    alt='Download on Flathub' src='https://flathub.org/assets/badges/flathub-badge-en.png'></a>
        </td>
        <td>
            <a
                href="https://github.com/zoeyfyi/TeX-Match/releases/latest/download/tex-match.windows.msi">tex-match.windows.msi</a>
        </td>
        <td>
            You should really use the <a href="https://gum.co/detexify">detexify Mac app</a>,
            </br>
            </br>
            but if you <i>really</i> want to:
            </br>
            <a href="https://github.com/zoeyfyi/TeX-Match/releases/latest/download/tex-match.macos">tex-match.macos</a>
        </td>
    </tr>
</table>

Check out my other project, [Boop-GTK](https://github.com/zoeyfyi/Boop-GTK): A scriptable scratchpad for developers.

### Building

#### Linux

```shell
sudo apt-get install -y libgtk-3-dev
cargo build
```

#### Linux Snap

```shell
sudo apt-get install snap snapcraft
snapcraft snap
sudo snap install tex-match_1.1.0_amd64.snap
```

#### Linux Flatpak

```shell
sudo add-apt-repository ppa:alexlarsson/flatpak 
sudo apt-get update 
sudo apt-get install flatpak
sudo flatpak remote-add --if-not-exists flathub https://dl.flathub.org/repo/flathub.flatpakrepo
sudo flatpak install -y flathub org.freedesktop.Platform//20.08 org.freedesktop.Sdk//20.08 org.freedesktop.Sdk.Extension.rust-stable//20.08
wget https://github.com/flatpak/flatpak-builder/releases/download/1.0.10/flatpak-builder-1.0.10.tar.xz && tar -xvf flatpak-builder-1.0.10.tar.xz && cd flatpak-builder-1.0.10 && ./configure --disable-documentation && make && sudo make install
sudo apt-get install python3-toml
bash flatpak/gen-sources.sh
flatpak-builder --repo=repo build-dir flatpak/fyi.zoey.TeX-Match.json
flatpak build-bundle ./repo tex-match.flatpak fyi.zoey.TeX-Match
```

#### MacOS

```shell
brew install gtk+3
cargo build
```

#### Windows

```powershell
git clone https://github.com/wingtk/gvsbuild.git C:\gtk-build\github\gvsbuild
cd C:\gtk-build\github\gvsbuild; python .\build.py build -p=x64 --vs-ver=16 --msys-dir=C:\msys64 -k --enable-gi --py-wheel --py-egg gtk3 gdk-pixbuf
cargo build
```

#### Windows Installer

```powershell
# follow build steps above, then:
cargo install cargo-wix 
cargo wix -v
```
