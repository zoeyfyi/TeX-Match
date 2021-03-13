using System.Runtime.InteropServices;

namespace TeX_Match.Views
{
    unsafe internal class Detextify
    {
        [DllImport(@"detexify.dll")]
        public static extern void* stroke_builder_new(uint capacity);

        [DllImport(@"detexify.dll")]
        public static extern void stroke_builder_add_point(void* builder, double x, double y);

        [DllImport(@"detexify.dll")]
        public static extern void* stroke_builder_build(void* builder);

        [DllImport(@"detexify.dll")]
        public static extern void* stroke_sample_new_builder(uint capacity);

        [DllImport(@"detexify.dll")]
        public static extern void stroke_sample_add_stroke(void* builder, void* stroke);

        [DllImport(@"detexify.dll")]
        public static extern void* stroke_sample_build(void* builder);

        [DllImport(@"detexify.dll")]
        public static extern void* classifier_new_default();

        [DllImport(@"detexify.dll")]
        public static extern void classifier_free(void* classifier);

        [DllImport(@"detexify.dll")]
        public static extern void* classify(void* classifier, void* sample);

        [DllImport(@"detexify.dll")]
        public static extern uint scores_length(void* scores);

        [DllImport(@"detexify.dll")]
        public static extern string scores_get_command(void* scores, uint i);

        [DllImport(@"detexify.dll")]
        public static extern string scores_get_package(void* scores, uint i);

        [DllImport(@"detexify.dll")]
        public static extern string scores_get_font_encoding(void* scores, uint i);

        [DllImport(@"detexify.dll")]
        public static extern void scores_free(string id);

        [DllImport(@"detexify.dll")]
        public static extern double scores_get_score(void* scores, uint i);
    }
}
