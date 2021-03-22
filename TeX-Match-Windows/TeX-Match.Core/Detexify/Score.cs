namespace TeX_Match.Core.Detexify
{
    public class Score
    {
        public Symbol Symbol { get; }
        public double Value { get; }

        internal Score(Symbol symbol, double score)
        {
            Symbol = symbol;
            Value = score;
        }
    }
}