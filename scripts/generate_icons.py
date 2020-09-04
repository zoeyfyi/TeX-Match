#!/usr/bin/env python3

import yaml
import os
import base64

scripts_folder = os.path.dirname(os.path.abspath(__file__))
symbols_file = os.path.join(scripts_folder, "../symbols.yaml")

parsed_symbols = []

def add_symbol(symbol, package="latex2e", fontenc="OT1", textmode=True, mathmode=False):
    parsed_symbols.append((symbol, package, fontenc, textmode, mathmode))

with open(symbols_file) as file:
    symbols = yaml.load(file, Loader=yaml.FullLoader)

    for symbol in symbols:
        if type(symbol) is str:
            add_symbol(symbol)
        if type(symbol) is dict:
            package = "latex2e"
            fontenc = "OT1"
            if "package" in symbol:
                package = symbol["package"]
            if "fontenc" in symbol:
                fontenc = symbol["fontenc"]
            if "bothmodes" in symbol:
                for s in symbol["bothmodes"]:
                    add_symbol(s, package, fontenc, True, True)
            if "textmode" in symbol:
                for s in symbol["textmode"]:
                    add_symbol(s, package, fontenc, True, False)
            if "mathmode" in symbol:
                for s in symbol["mathmode"]:
                    add_symbol(s, package, fontenc, False, True)

for (symbol, package, fontenc, textmode, mathmode) in parsed_symbols:
    print("{:30} {:10} {:4} {} {}".format(symbol, package, fontenc, textmode, mathmode))
    
    command = ""
    if mathmode:
        command = "$" + symbol + "$"
    else:
        command = symbol

    p = ""
    if package != "latex2e":
        p = "\\usepackage{{{}}}".format(package)

    latex = """
        \\documentclass[10pt]{{article}}
        \\usepackage[utf8]{{inputenc}}

        \\usepackage[{}]{{fontenc}}
        {}

        \\pagestyle{{empty}}
        \\begin{{document}}

        {}

        \\end{{document}}
    """.format(fontenc, p, command)
    
    f = open("temp.latex", "w")
    f.write(latex)
    f.close()
    
    # latex to svg
    os.system("pdflatex temp.latex")
    os.system("pdf2svg temp.pdf temp.svg")
    # crop to drawing
    os.system("inkscape -D -o temp2.svg temp.svg")
    # resize to 64x64
    os.system("sed -i 's/height\=\\\"[0-9]*.[0-9]*pt\\\"/height=\\\"64px\\\"/g' temp2.svg")
    os.system("sed -i 's/width\=\\\"[0-9]*.[0-9]*pt\\\"/width=\\\"64px\\\"/g' temp2.svg")

    id = package + "-" + fontenc + "-" + symbol.replace("\\", "_")
    id = base64.b64encode(id.encode("utf-8"))
    id = str(id, "utf-8")
    
    os.system("mv temp2.svg resources/icons/scalable/actions/{}-symbolic.svg".format(id))
    



# cleanup files
os.system("rm temp.*")
