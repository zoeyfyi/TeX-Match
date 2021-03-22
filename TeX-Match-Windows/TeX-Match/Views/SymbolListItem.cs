using System.Text;
using TeX_Match.Core.Helpers;

namespace TeX_Match.Views
{
    public class SymbolListItem
    {
        private readonly string command;
        private readonly bool textMode;
        private readonly bool mathMode;
        private readonly string package;
        private readonly string fontEnc;
        private readonly double score;

        internal SymbolListItem(Symbol symbol) : this(symbol, 0.0) { }

        internal SymbolListItem(Symbol symbol, double score)
        {
            this.command = symbol.Command;
            this.textMode = symbol.TextMode;
            this.mathMode = symbol.MathMode;
            this.package = symbol.Package;
            this.fontEnc = symbol.FontEncoding;
            this.score = score;
        }

        public string Command => command;
        public string Mode
        {
            get
            {
                if (textMode && !mathMode)
                {
                    return "textmode";
                }
                else if (!textMode && mathMode)
                {
                    return "mathmode";
                }
                else if (textMode && mathMode)
                {
                    return "textmode & mathmode";
                }
                else
                {
                    return "";
                }
            }
        }
        public string Package => string.Format("\\usepackage{{ {0} }}", package);
        public double Score => score;
        public string ModeAndScore => string.Format("{0} (score: {1:F4})", Mode, Score);

        //private string id => Base32.ToBase32String(Encoding.ASCII.GetBytes(string.Format("{0}-{1}-{2}", package, fontEnc, command.Replace('\\', '_')))).ToLower();
        private string id => Base32.ToBase32String(Encoding.UTF8.GetBytes(string.Format("{0}-{1}-{2}", package, fontEnc, command.Replace('\\', '_')))).ToLower();
        public string SourceURI => string.Format("ms-appx:///Assets/symbols/{0}.svg", id);
    }
}
