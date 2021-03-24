
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TeX_Match.Core.Detexify
{
    unsafe class Bindings
    {

                        
        /*
         * ClassifierFree
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "classifier_free")]
        private static extern void ClassifierFree86(void* classifier);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "classifier_free")]
        private static extern void ClassifierFree64(void* classifier);
        internal static void ClassifierFree(void* classifier)
        {
            if (Environment.Is64BitProcess)
            {
                ClassifierFree64(classifier);
            }
            else
            {
                ClassifierFree86(classifier);
            }
        }

            
        /*
         * ClassifierNewDefault
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "classifier_new_default")]
        private static extern void* ClassifierNewDefault86();
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "classifier_new_default")]
        private static extern void* ClassifierNewDefault64();
        internal static void* ClassifierNewDefault()
        {
            if (Environment.Is64BitProcess)
            {
                return ClassifierNewDefault64();
            }
            else
            {
                return ClassifierNewDefault86();
            }
        }

            
        /*
         * Classify
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "classify")]
        private static extern void* Classify86(void* classifier, void* sample);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "classify")]
        private static extern void* Classify64(void* classifier, void* sample);
        internal static void* Classify(void* classifier, void* sample)
        {
            if (Environment.Is64BitProcess)
            {
                return Classify64(classifier, sample);
            }
            else
            {
                return Classify86(classifier, sample);
            }
        }

            
        /*
         * ScoresFree
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "scores_free")]
        private static extern void ScoresFree86(string id);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "scores_free")]
        private static extern void ScoresFree64(string id);
        internal static void ScoresFree(string id)
        {
            if (Environment.Is64BitProcess)
            {
                ScoresFree64(id);
            }
            else
            {
                ScoresFree86(id);
            }
        }

            
        /*
         * ScoresGetScore
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "scores_get_score")]
        private static extern double ScoresGetScore86(void* scores, uint i);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "scores_get_score")]
        private static extern double ScoresGetScore64(void* scores, uint i);
        internal static double ScoresGetScore(void* scores, uint i)
        {
            if (Environment.Is64BitProcess)
            {
                return ScoresGetScore64(scores, i);
            }
            else
            {
                return ScoresGetScore86(scores, i);
            }
        }

            
        /*
         * ScoresGetSymbol
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "scores_get_symbol")]
        private static extern void* ScoresGetSymbol86(void* scores, uint i);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "scores_get_symbol")]
        private static extern void* ScoresGetSymbol64(void* scores, uint i);
        internal static void* ScoresGetSymbol(void* scores, uint i)
        {
            if (Environment.Is64BitProcess)
            {
                return ScoresGetSymbol64(scores, i);
            }
            else
            {
                return ScoresGetSymbol86(scores, i);
            }
        }

            
        /*
         * ScoresLength
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "scores_length")]
        private static extern uint ScoresLength86(void* scores);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "scores_length")]
        private static extern uint ScoresLength64(void* scores);
        internal static uint ScoresLength(void* scores)
        {
            if (Environment.Is64BitProcess)
            {
                return ScoresLength64(scores);
            }
            else
            {
                return ScoresLength86(scores);
            }
        }

            
        /*
         * StrokeBuilderNew
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "stroke_builder_new")]
        private static extern void* StrokeBuilderNew86(uint capacity);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "stroke_builder_new")]
        private static extern void* StrokeBuilderNew64(uint capacity);
        internal static void* StrokeBuilderNew(uint capacity)
        {
            if (Environment.Is64BitProcess)
            {
                return StrokeBuilderNew64(capacity);
            }
            else
            {
                return StrokeBuilderNew86(capacity);
            }
        }

            
        /*
         * StrokeBuilderAddPoint
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "stroke_builder_add_point")]
        private static extern void StrokeBuilderAddPoint86(void* builder, double x, double y);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "stroke_builder_add_point")]
        private static extern void StrokeBuilderAddPoint64(void* builder, double x, double y);
        internal static void StrokeBuilderAddPoint(void* builder, double x, double y)
        {
            if (Environment.Is64BitProcess)
            {
                StrokeBuilderAddPoint64(builder, x, y);
            }
            else
            {
                StrokeBuilderAddPoint86(builder, x, y);
            }
        }

            
        /*
         * StrokeBuilderBuild
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "stroke_builder_build")]
        private static extern void* StrokeBuilderBuild86(void* builder);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "stroke_builder_build")]
        private static extern void* StrokeBuilderBuild64(void* builder);
        internal static void* StrokeBuilderBuild(void* builder)
        {
            if (Environment.Is64BitProcess)
            {
                return StrokeBuilderBuild64(builder);
            }
            else
            {
                return StrokeBuilderBuild86(builder);
            }
        }

            
        /*
         * StrokeSampleBuilderNew
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "stroke_sample_builder_new")]
        private static extern void* StrokeSampleBuilderNew86(uint capacity);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "stroke_sample_builder_new")]
        private static extern void* StrokeSampleBuilderNew64(uint capacity);
        internal static void* StrokeSampleBuilderNew(uint capacity)
        {
            if (Environment.Is64BitProcess)
            {
                return StrokeSampleBuilderNew64(capacity);
            }
            else
            {
                return StrokeSampleBuilderNew86(capacity);
            }
        }

            
        /*
         * StrokeSampleBuilderAddStroke
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "stroke_sample_builder_add_stroke")]
        private static extern void StrokeSampleBuilderAddStroke86(void* builder, void* stroke);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "stroke_sample_builder_add_stroke")]
        private static extern void StrokeSampleBuilderAddStroke64(void* builder, void* stroke);
        internal static void StrokeSampleBuilderAddStroke(void* builder, void* stroke)
        {
            if (Environment.Is64BitProcess)
            {
                StrokeSampleBuilderAddStroke64(builder, stroke);
            }
            else
            {
                StrokeSampleBuilderAddStroke86(builder, stroke);
            }
        }

            
        /*
         * StrokeSampleBuilderBuild
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "stroke_sample_builder_build")]
        private static extern void* StrokeSampleBuilderBuild86(void* builder);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "stroke_sample_builder_build")]
        private static extern void* StrokeSampleBuilderBuild64(void* builder);
        internal static void* StrokeSampleBuilderBuild(void* builder)
        {
            if (Environment.Is64BitProcess)
            {
                return StrokeSampleBuilderBuild64(builder);
            }
            else
            {
                return StrokeSampleBuilderBuild86(builder);
            }
        }

            
        /*
         * SymbolsCount
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "symbols_count")]
        private static extern uint SymbolsCount86();
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "symbols_count")]
        private static extern uint SymbolsCount64();
        internal static uint SymbolsCount()
        {
            if (Environment.Is64BitProcess)
            {
                return SymbolsCount64();
            }
            else
            {
                return SymbolsCount86();
            }
        }

            
        /*
         * SymbolsGet
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "symbols_get")]
        private static extern void* SymbolsGet86(uint i);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "symbols_get")]
        private static extern void* SymbolsGet64(uint i);
        internal static void* SymbolsGet(uint i)
        {
            if (Environment.Is64BitProcess)
            {
                return SymbolsGet64(i);
            }
            else
            {
                return SymbolsGet86(i);
            }
        }

            
        /*
         * SymbolGetCommand
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "symbol_get_command")]
        private static extern void SymbolGetCommand86(void* symbol, StringBuilder buffer, uint len);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "symbol_get_command")]
        private static extern void SymbolGetCommand64(void* symbol, StringBuilder buffer, uint len);
        internal static void SymbolGetCommand(void* symbol, StringBuilder buffer, uint len)
        {
            if (Environment.Is64BitProcess)
            {
                SymbolGetCommand64(symbol, buffer, len);
            }
            else
            {
                SymbolGetCommand86(symbol, buffer, len);
            }
        }

            
        /*
         * SymbolGetFontEncoding
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "symbol_get_font_encoding")]
        private static extern void SymbolGetFontEncoding86(void* symbol, StringBuilder buffer, uint len);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "symbol_get_font_encoding")]
        private static extern void SymbolGetFontEncoding64(void* symbol, StringBuilder buffer, uint len);
        internal static void SymbolGetFontEncoding(void* symbol, StringBuilder buffer, uint len)
        {
            if (Environment.Is64BitProcess)
            {
                SymbolGetFontEncoding64(symbol, buffer, len);
            }
            else
            {
                SymbolGetFontEncoding86(symbol, buffer, len);
            }
        }

            
        /*
         * SymbolGetMathMode
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "symbol_get_math_mode")]
        private static extern bool SymbolGetMathMode86(void* symbol);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "symbol_get_math_mode")]
        private static extern bool SymbolGetMathMode64(void* symbol);
        internal static bool SymbolGetMathMode(void* symbol)
        {
            if (Environment.Is64BitProcess)
            {
                return SymbolGetMathMode64(symbol);
            }
            else
            {
                return SymbolGetMathMode86(symbol);
            }
        }

            
        /*
         * SymbolGetTextMode
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "symbol_get_text_mode")]
        private static extern bool SymbolGetTextMode86(void* symbol);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "symbol_get_text_mode")]
        private static extern bool SymbolGetTextMode64(void* symbol);
        internal static bool SymbolGetTextMode(void* symbol)
        {
            if (Environment.Is64BitProcess)
            {
                return SymbolGetTextMode64(symbol);
            }
            else
            {
                return SymbolGetTextMode86(symbol);
            }
        }

            
        /*
         * SymbolGetPackage
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "symbol_get_package")]
        private static extern void SymbolGetPackage86(void* symbol, StringBuilder buffer, uint len);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "symbol_get_package")]
        private static extern void SymbolGetPackage64(void* symbol, StringBuilder buffer, uint len);
        internal static void SymbolGetPackage(void* symbol, StringBuilder buffer, uint len)
        {
            if (Environment.Is64BitProcess)
            {
                SymbolGetPackage64(symbol, buffer, len);
            }
            else
            {
                SymbolGetPackage86(symbol, buffer, len);
            }
        }

            
        /*
         * SymbolFree
         */

        [DllImport(@"dll/x86/detexify.dll", EntryPoint = "symbol_free")]
        private static extern void SymbolFree86(void* symbol);
        [DllImport(@"dll/x64/detexify.dll", EntryPoint = "symbol_free")]
        private static extern void SymbolFree64(void* symbol);
        internal static void SymbolFree(void* symbol)
        {
            if (Environment.Is64BitProcess)
            {
                SymbolFree64(symbol);
            }
            else
            {
                SymbolFree86(symbol);
            }
        }

    }
}
