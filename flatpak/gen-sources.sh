# Generates sources file for flatpak build
# run whenever dependencies change

SCRIPT=`realpath $0`
SCRIPTPATH=`dirname $SCRIPT`

python3 $SCRIPTPATH/../submodules/flatpak-builder-tools/cargo/flatpak-cargo-generator.py $SCRIPTPATH/../Cargo.lock

mv $PWD/generated-sources.json $SCRIPTPATH/generated-sources.json
cp $SCRIPTPATH/generated-sources.json $SCRIPTPATH/uk.co.mrbenshef.TeX-Match/generated-sources.json