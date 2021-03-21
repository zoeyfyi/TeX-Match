import fs from 'fs';
import YAML from 'yaml'

const file = fs.readFileSync("./symbols.yaml", "utf-8");
const yaml = YAML.parse(file);

let symbols = [];

const defaultSymbol = {
    package: "latex2e",
    fontenc: "OT1",
    textmode: true,
    mathmode: false
};

for (let symbol of yaml) {
    if (typeof symbol === "string") {
        symbols.push({ ...defaultSymbol, symbol });
    } else {
        let s = {
            fontenc: symbol.fontenc || defaultSymbol.fontenc,
            package: symbol.package || defaultSymbol.package,
        };

        if ('bothmodes' in symbol) {
            for (let innerSymbol of symbol.bothmodes) {
                symbols.push({ ...s, symbol: innerSymbol, textmode: true, mathmode: true });
            }
        }
        if ('textmode' in symbol) {
            for (let innerSymbol of symbol.textmode) {
                symbols.push({ ...s, symbol: innerSymbol, textmode: true, mathmode: false });
            }
        }
        if ('mathmode' in symbol) {
            for (let innerSymbol of symbol.mathmode) {
                symbols.push({ ...s, symbol: innerSymbol, textmode: false, mathmode: true });
            }
        }
    }
}

console.log(symbols)

// const { extendDefaultPlugins } = require('svgo');
// module.exports = {
//     plugins: extendDefaultPlugins([
//         { name: 'dereferenceUses' }
//     ])
// }