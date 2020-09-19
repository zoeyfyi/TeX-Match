<h1 align="center">TeX Match</h1>

<p align="center">
  <img src="TeX-Match.gif" width="660px">
</p>

<h2 align="center">A desktop version of <a href="https://detexify.kirelabs.org/classify.html">detexify</a></h2>

<br>

![Continuous integration](https://github.com/mrbenshef/TeX-Match/workflows/Continuous%20integration/badge.svg)
![Release](https://github.com/mrbenshef/TeX-Match/workflows/Release/badge.svg?branch=release)
[![Crates.io](https://img.shields.io/crates/v/tex-match)](https://crates.io/crates/tex-match)
<!--- [![tex-match](https://snapcraft.io//tex-match/badge.svg)](https://snapcraft.io/tex-match)
[![Flathub](https://img.shields.io/flathub/v/uk.co.mrbenshef.TeX-Match)](https://flathub.org/apps/details/uk.co.mrbenshef.TeX-Match)
--->

### What is TeX Match?

If you work with LaTeX, you know its difficult to memorize the names of all the symbols. TeX Match allows you to search through over 1000 different LaTeX symbols by sketching. TeX Match is based of [detexify](https://detexify.kirelabs.org/classify.html) and is powered by a port of the [detexify classifier](https://github.com/mrbenshef/detexify-rust).

### Features

- Over 1000 LaTeX symbols across multiple packages (same set as detexify) 
- Completely offline
- Crossplatform

### Get Tex Match

| Linux | Windows | MacOS |
| :--- | :--- | :--- |
| [tex-match.linux.amd64](https://github.com/mrbenshef/TeX-Match/releases/latest/download/tex-match.linux.amd64) </br> [tex-match.flatpak](https://github.com/mrbenshef/TeX-Match/releases/latest/download/tex-match.flatpak) </br> [tex-match.snap](https://github.com/mrbenshef/TeX-Match/releases/latest/download/tex-match.snap) <!---</br> [![Get it from the Snap Store](https://snapcraft.io/static/images/badges/en/snap-store-black.svg)](https://snapcraft.io/tex-match) </br> <a href='https://flathub.org/apps/details/uk.co.mrbenshef.TeX-Match'><img width='190' alt='Download on Flathub' src='https://flathub.org/assets/badges/flathub-badge-en.png'/></a>--> | [tex-match.windows.msi](https://github.com/mrbenshef/TeX-Match/releases/latest/download/tex-match.windows.msi) | You should really use the [detexify Mac app](https://gum.co/detexify), </br> but if you _really_ want to: </br> [tex-match.macos](https://github.com/mrbenshef/TeX-Match/releases/latest/download/tex-match.macos) |

Check out my other project, [Boop-GTK](https://github.com/mrbenshef/Boop-GTK): A scriptable scratchpad for developers.

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
sudo snap install tex-match_1.0.0_amd64.snap
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
flatpak-builder --repo=repo build-dir flatpak/uk.co.mrbenshef.TeX-Match.json
flatpak build-bundle ./repo tex-match.flatpak uk.co.mrbenshef.TeX-Match
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
