using System.Runtime.InteropServices;
using System.Text;

namespace TeX_Match.Core.Detexify
{
    unsafe internal class Bindings
    {
        [DllImport(@"detexify.dll")]
        public static extern void classifier_free(void* classifier);

        // Classifier

        [DllImport(@"detexify.dll")]
        public static extern void* classifier_new_default();
        [DllImport(@"detexify.dll")]
        public static extern void* classify(void* classifier, void* sample);
        [DllImport(@"detexify.dll")]
        public static extern void scores_free(string id);
        [DllImport(@"detexify.dll")]
        public static extern double scores_get_score(void* scores, uint i);
        [DllImport(@"detexify.dll")]
        public static extern void* scores_get_symbol(void* scores, uint i);

        // Scores

        [DllImport(@"detexify.dll")]
        public static extern uint scores_length(void* scores);
        [DllImport(@"detexify.dll")]
        public static extern void stroke_builder_add_point(void* builder, double x, double y);
        [DllImport(@"detexify.dll")]
        public static extern void* stroke_builder_build(void* builder);
        // Stroke

        [DllImport(@"detexify.dll")]
        public static extern void* stroke_builder_new(uint capacity);
        [DllImport(@"detexify.dll")]
        public static extern void stroke_sample_builder_add_stroke(void* builder, void* stroke);
        [DllImport(@"detexify.dll")]
        public static extern void* stroke_sample_builder_build(void* builder);

        // Stroke sample

        [DllImport(@"detexify.dll")]
        public static extern void* stroke_sample_builder_new(uint capacity);

        // Symbols

        [DllImport(@"detexify.dll")]
        public static extern uint symbols_count();

        [DllImport(@"detexify.dll")]
        public static extern void* symbols_get(uint i);
        [DllImport(@"detexify.dll")]
        public static extern bool symbol_free(void* symbol);

        // Symbol

        [DllImport(@"detexify.dll")]
        public static extern void symbol_get_command(void* symbol, StringBuilder buffer, uint len);
        [DllImport(@"detexify.dll")]
        public static extern void symbol_get_font_encoding(void* symbol, StringBuilder buffer, uint len);
        [DllImport(@"detexify.dll")]
        public static extern bool symbol_get_math_mode(void* symbol);
        [DllImport(@"detexify.dll")]
        public static extern void symbol_get_package(void* symbol, StringBuilder buffer, uint len);
        [DllImport(@"detexify.dll")]
        public static extern bool symbol_get_text_mode(void* symbol);
    }
}