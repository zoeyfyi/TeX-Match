import fs from 'fs/promises';
import YAML from 'yaml'
import ora from 'ora';
import base32 from 'base32';
import tmp from 'tmp-promise';
import { promisify } from 'util';
import { exec } from 'child_process';
import asyncPool from "tiny-async-pool";
import os from 'os';
import { dirname } from 'path';
import svgo from 'svgo';
import applyTranslations from './applyTranslations.js';

const CPUS = os.cpus().length;
console.log(`Running on ${CPUS} threads`)

/*

Utils

*/

function symbolId(symbol) {
    return base32.encode(`${symbol.package}-${symbol.fontenc}-${symbol.symbol.replace("\\", "_")}`)
}

function asyncPoolProgress(poolLimit, array, iteratorFn, tickCallback) {
    var len = array.length;
    var progress = 0;

    function tick() {
        progress++;
        tickCallback(progress, len);
    }

    return asyncPool(poolLimit, array, async (item) => {
        let value = await iteratorFn(item);
        tick();
        return value;
    });
}

function execute(command) {
    return promisify(exec)(command);
}

/*

1. Load symbols from symbols.yaml

*/

let spinner = ora('Parsing symbols.yaml').start();

let symbols = await (async function () {
    const file = await fs.readFile("./symbols.yaml", "utf-8");
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

    return symbols;
})();

// symbols = symbols.slice(0, 1);

spinner.succeed(`Found ${symbols.length} symbols`);

/*

2. Write LaTeX

*/

spinner = ora('Writing latex files').start();

const generateLatexFile = async (symbol) => {
    const { path } = await tmp.file({ postfix: "latex" });
    const handle = await fs.open(path, "w");

    let command = symbol.mathmode ? `$${symbol.symbol}$` : symbol.symbol;
    let usepackage = symbol.package == "latex2e" ? "" : `\\usepackage{${symbol.package}}`;
    const latex = `
        \\documentclass[10pt]{article}
        \\usepackage[utf8]{inputenc}
        \\usepackage[${symbol.fontenc}]{fontenc}
        ${usepackage}
        \\pagestyle{empty}
        \\begin{document}
        ${command}
        \\end{document}
    `;

    await handle.writeFile(latex);
    await handle.close();

    return { symbol, path };
};

const latexFiles = await asyncPoolProgress(CPUS, symbols, generateLatexFile, (progress, len) => {
    spinner.text = `Writing latex files (${progress}/${len})`;
});

spinner.succeed(`Written latex files`);

/* 

3. Render PDF's

*/

spinner = ora('Rendering PDFs').start();

const renderLatexFile = async ({ symbol, path }) => {
    await execute(`pdflatex -output-directory ${dirname(path)} ${path}`);
    return { symbol, path: `${path}.pdf` };
};

const pdfs = await asyncPoolProgress(CPUS, latexFiles, renderLatexFile, (progress, len) => {
    spinner.text = `Rendering PDFs (${progress}/${len})`;
});

spinner.succeed(`Rendered PDFs`);

/*

4. PDF to SVG

*/

spinner = ora('Converting PDFs').start();

const convertPDF = async ({ symbol, path }) => {
    let newpath = path.replace(".pdf", ".svg");
    await execute(`pdf2svg ${path} ${newpath}`);
    return { symbol, path: newpath };
}

const svgs = await asyncPoolProgress(CPUS, pdfs, convertPDF, (progress, len) => {
    spinner.text = `Converting PDFs (${progress}/${len})`;
});

spinner.succeed(`Converted PDFs`)

/*

5. Crop to drawing

*/

spinner = ora('Cropping SVGs').start()

const cropSVG = async ({ symbol, path }) => {
    let newpath = path.replace(".svg", ".cropped.svg");
    await execute(`inkscape -D -o ${newpath} ${path}`);
    return { symbol, path: newpath };
}

const croppedSvgs = await asyncPoolProgress(CPUS, svgs, cropSVG, (progress, len) => {
    spinner.text = `Cropping SVGs (${progress}/${len})`;
});

spinner.succeed(`Cropped SVGs`);

/*

6. Resizing SVGs

*/

spinner = ora('Resizing SVGs').start();

const resizeSVG = async ({ path }) => {
    let handle = await fs.open(path, "r");

    let svg = await handle.readFile({ encoding: 'utf-8' });
    svg = svg.replace(/height\=\"[0-9]*\.[0-9]*pt\"/g, `height="64px"`);
    svg = svg.replace(/width\=\"[0-9]*\.[0-9]*pt\"/g, `width="64px"`);

    await handle.close();

    handle = await fs.open(path, "w");
    await handle.writeFile(svg);
    await handle.close();
};

await asyncPoolProgress(CPUS, croppedSvgs, resizeSVG, (progress, len) => {
    spinner.text = `Resizing SVGs (${progress}/${len})`;
})

spinner.succeed(`Resized SVGs`);

/*

7. Minimize SVGs

We are using a fork of svgo, strarsis/svgo#dereferenceUses-plugin that has the dereferenceUses 
plugin. For some reason <use>'s do not play well with uwp <Image> controls, plus we save some
space by inlining them.

*/

spinner = ora('Minimizing SVGs').start();

const minimizeSVG = async ({ symbol, path }) => {
    let handle = await fs.open(path, "r");

    const svg = await handle.readFile({ encoding: 'utf-8' });
    const result = svgo.optimize(svg, {
        path,
        multipass: true,
        plugins: svgo.extendDefaultPlugins([
            {
                name: "dereferenceUses",
                params: {
                    symbolContainer: 'svg'
                }
            },
            applyTranslations
        ])
    });

    await handle.close();

    handle = await fs.open(path, "w");

    if (result.error) {
        spinner.fail(`Failed to minimize ${symbol.symbol}: ${result.error}`);
        return;
    }

    await handle.writeFile(result.data);
    await handle.close();

    return { symbol, path }
};

await asyncPoolProgress(1, croppedSvgs, minimizeSVG, (progress, len) => {
    spinner.text = `Minimizing SVGs (${progress}/${len})`;
})

spinner.succeed(`Minimized SVGs`);

/*

8. Rename SVGs

*/

const symbolsDir = `${process.cwd()}/symbols`;

await fs.mkdir(symbolsDir, { recursive: true });

spinner = ora('Renaming SVGs').start();

const renameSVG = async ({ symbol, path }) => {
    await execute(`mv ${path} ${symbolsDir}/${symbolId(symbol)}.svg`)
}

await asyncPoolProgress(1, croppedSvgs, renameSVG, (progress, len) => {
    spinner.text = `Renaming SVGs (${progress}/${len})`;
});

spinner.succeed(`Renamed SVGs`);
