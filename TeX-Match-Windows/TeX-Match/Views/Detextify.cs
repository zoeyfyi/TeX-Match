using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace TeX_Match.Views
{
    unsafe internal class Detextify
    {
        // Stroke

        [DllImport(@"detexify.dll")]
        public static extern void* stroke_builder_new(uint capacity);
        [DllImport(@"detexify.dll")]
        public static extern void stroke_builder_add_point(void* builder, double x, double y);
        [DllImport(@"detexify.dll")]
        public static extern void* stroke_builder_build(void* builder);

        // Stroke sample

        [DllImport(@"detexify.dll")]
        public static extern void* stroke_sample_builder_new(uint capacity);
        [DllImport(@"detexify.dll")]
        public static extern void stroke_sample_builder_add_stroke(void* builder, void* stroke);
        [DllImport(@"detexify.dll")]
        public static extern void* stroke_sample_builder_build(void* builder);

        // Classifier

        [DllImport(@"detexify.dll")]
        public static extern void* classifier_new_default();
        [DllImport(@"detexify.dll")]
        public static extern void classifier_free(void* classifier);
        [DllImport(@"detexify.dll")]
        public static extern void* classify(void* classifier, void* sample);

        // Scores

        [DllImport(@"detexify.dll")]
        public static extern uint scores_length(void* scores);
        [DllImport(@"detexify.dll")]
        public static extern void* scores_get_symbol(void* scores, uint i);
        [DllImport(@"detexify.dll")]
        public static extern double scores_get_score(void* scores, uint i);
        [DllImport(@"detexify.dll")]
        public static extern void scores_free(string id);

        // Symbol

        [DllImport(@"detexify.dll")]
        public static extern void symbol_get_command(void* symbol, StringBuilder buffer, uint len);
        [DllImport(@"detexify.dll")]
        public static extern void symbol_get_package(void* symbol, StringBuilder buffer, uint len);
        [DllImport(@"detexify.dll")]
        public static extern void symbol_get_font_encoding(void* symbol, StringBuilder buffer, uint len);
        [DllImport(@"detexify.dll")]
        public static extern bool symbol_get_text_mode(void* symbol);
        [DllImport(@"detexify.dll")]
        public static extern bool symbol_get_math_mode(void* symbol);
        [DllImport(@"detexify.dll")]
        public static extern bool symbol_free(void* symbol);

        // Symbols

        [DllImport(@"detexify.dll")]
        public static extern uint symbols_count();

        [DllImport(@"detexify.dll")]
        public static extern void* symbols_get(uint i);
    }

    internal class StrokeBuilder
    {
        unsafe void* strokeBuilder;

        internal StrokeBuilder(uint capacity)
        {
            unsafe { strokeBuilder = Detextify.stroke_builder_new(capacity); }
        }

        internal void AddPoint(double x, double y)
        {
            unsafe { Detextify.stroke_builder_add_point(strokeBuilder, x, y); }
        }

        internal Stroke build()
        {
            unsafe { return new Stroke(Detextify.stroke_builder_build(strokeBuilder)); }
        }
    }

    internal class Stroke
    {
        internal unsafe Stroke(void* stroke)
        {
            Ptr = stroke;
        }

        internal unsafe void* Ptr { get; }
    }

    class StrokeSampleBuilder
    {
        unsafe void* builder;

        internal unsafe StrokeSampleBuilder(uint capacity)
        {
            builder = Detextify.stroke_sample_builder_new(capacity);
        }

        internal void AddStroke(Stroke stroke)
        {
            unsafe { Detextify.stroke_sample_builder_add_stroke(builder, stroke.Ptr); }
        }

        internal StrokeSample build()
        {
            unsafe { return new StrokeSample(Detextify.stroke_sample_builder_build(builder)); }
        }
    }

    internal class StrokeSample
    {
        internal unsafe StrokeSample(void* strokeSample)
        {
            Ptr = strokeSample;
        }

        internal unsafe void* Ptr { get; }
    }

    class Classifier
    {
        internal unsafe void* classifier;

        internal Classifier()
        {
            unsafe { classifier = Detextify.classifier_new_default(); }
        }

        ~Classifier()
        {
            unsafe { Detextify.classifier_free(classifier); }
        }

        internal Scores classify(StrokeSample sample)
        {
            unsafe { return new Scores(Detextify.classify(classifier, sample.Ptr)); }
        }
    }

    internal class Scores : IEnumerable
    {
        internal unsafe void* Ptr { get; }

        internal unsafe Scores(void* v)
        {
            Ptr = v;
        }

        private uint Length()
        {
            unsafe { return Detextify.scores_length(Ptr); };
        }

        private Symbol GetSymbol(uint i)
        {
            unsafe { return new Symbol(Detextify.scores_get_symbol(Ptr, i)); }
        }

        private double GetScore(uint i)
        {
            unsafe { return Detextify.scores_get_score(Ptr, i); }
        }

        public IEnumerator GetEnumerator()
        {
            for (uint i = 0; i < Length(); i++)
            {
                yield return new Score(GetSymbol(i), GetScore(i));
            }
        }
    }

    internal class Score
    {
        internal Symbol Symbol { get; }
        internal double Value { get; }

        public Score(Symbol symbol, double score)
        {
            Symbol = symbol;
            Value = score;
        }
    }

    internal class Symbol
    {
        const uint MAX_STRING_BYTES = 1024;

        internal unsafe void* Ptr { get; }

        internal unsafe Symbol(void* symbol)
        {
            Ptr = symbol;
        }

        ~Symbol()
        {
            unsafe { Detextify.symbol_free(Ptr); }
        }

        internal string Command
        {
            get
            {
                StringBuilder builder = new StringBuilder((int)MAX_STRING_BYTES);
                unsafe { Detextify.symbol_get_command(Ptr, builder, MAX_STRING_BYTES); }
                return builder.ToString();
            }
        }

        internal string Package
        {
            get
            {
                StringBuilder builder = new StringBuilder((int)MAX_STRING_BYTES);
                unsafe { Detextify.symbol_get_package(Ptr, builder, MAX_STRING_BYTES); }
                return builder.ToString();
            }
        }

        internal string FontEncoding
        {
            get
            {
                StringBuilder builder = new StringBuilder((int)MAX_STRING_BYTES);
                unsafe { Detextify.symbol_get_font_encoding(Ptr, builder, MAX_STRING_BYTES); }
                return builder.ToString();
            }
        }

        internal bool TextMode
        {
            get
            {
                unsafe { return Detextify.symbol_get_text_mode(Ptr); }
            }
        }

        internal bool MathMode
        {
            get
            {
                unsafe { return Detextify.symbol_get_math_mode(Ptr); }
            }
        }
    }
}
