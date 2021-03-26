
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

        [DllImport(@"detexify.dll", EntryPoint = "classifier_free")]
        internal static extern void ClassifierFree(void* classifier);

            
        /*
         * ClassifierNewDefault
         */

        [DllImport(@"detexify.dll", EntryPoint = "classifier_new_default")]
        internal static extern void* ClassifierNewDefault();

            
        /*
         * Classify
         */

        [DllImport(@"detexify.dll", EntryPoint = "classify")]
        internal static extern void* Classify(void* classifier, void* sample);

            
        /*
         * ScoresFree
         */

        [DllImport(@"detexify.dll", EntryPoint = "scores_free")]
        internal static extern void ScoresFree(string id);

            
        /*
         * ScoresGetScore
         */

        [DllImport(@"detexify.dll", EntryPoint = "scores_get_score")]
        internal static extern double ScoresGetScore(void* scores, uint i);

            
        /*
         * ScoresGetSymbol
         */

        [DllImport(@"detexify.dll", EntryPoint = "scores_get_symbol")]
        internal static extern void* ScoresGetSymbol(void* scores, uint i);

            
        /*
         * ScoresLength
         */

        [DllImport(@"detexify.dll", EntryPoint = "scores_length")]
        internal static extern uint ScoresLength(void* scores);

            
        /*
         * StrokeBuilderNew
         */

        [DllImport(@"detexify.dll", EntryPoint = "stroke_builder_new")]
        internal static extern void* StrokeBuilderNew(uint capacity);

            
        /*
         * StrokeBuilderAddPoint
         */

        [DllImport(@"detexify.dll", EntryPoint = "stroke_builder_add_point")]
        internal static extern void StrokeBuilderAddPoint(void* builder, double x, double y);

            
        /*
         * StrokeBuilderBuild
         */

        [DllImport(@"detexify.dll", EntryPoint = "stroke_builder_build")]
        internal static extern void* StrokeBuilderBuild(void* builder);

            
        /*
         * StrokeSampleBuilderNew
         */

        [DllImport(@"detexify.dll", EntryPoint = "stroke_sample_builder_new")]
        internal static extern void* StrokeSampleBuilderNew(uint capacity);

            
        /*
         * StrokeSampleBuilderAddStroke
         */

        [DllImport(@"detexify.dll", EntryPoint = "stroke_sample_builder_add_stroke")]
        internal static extern void StrokeSampleBuilderAddStroke(void* builder, void* stroke);

            
        /*
         * StrokeSampleBuilderBuild
         */

        [DllImport(@"detexify.dll", EntryPoint = "stroke_sample_builder_build")]
        internal static extern void* StrokeSampleBuilderBuild(void* builder);

            
        /*
         * SymbolsCount
         */

        [DllImport(@"detexify.dll", EntryPoint = "symbols_count")]
        internal static extern uint SymbolsCount();

            
        /*
         * SymbolsGet
         */

        [DllImport(@"detexify.dll", EntryPoint = "symbols_get")]
        internal static extern void* SymbolsGet(uint i);

            
        /*
         * SymbolGetCommand
         */

        [DllImport(@"detexify.dll", EntryPoint = "symbol_get_command")]
        internal static extern void SymbolGetCommand(void* symbol, StringBuilder buffer, uint len);

            
        /*
         * SymbolGetFontEncoding
         */

        [DllImport(@"detexify.dll", EntryPoint = "symbol_get_font_encoding")]
        internal static extern void SymbolGetFontEncoding(void* symbol, StringBuilder buffer, uint len);

            
        /*
         * SymbolGetMathMode
         */

        [DllImport(@"detexify.dll", EntryPoint = "symbol_get_math_mode")]
        internal static extern bool SymbolGetMathMode(void* symbol);

            
        /*
         * SymbolGetTextMode
         */

        [DllImport(@"detexify.dll", EntryPoint = "symbol_get_text_mode")]
        internal static extern bool SymbolGetTextMode(void* symbol);

            
        /*
         * SymbolGetPackage
         */

        [DllImport(@"detexify.dll", EntryPoint = "symbol_get_package")]
        internal static extern void SymbolGetPackage(void* symbol, StringBuilder buffer, uint len);

            
        /*
         * SymbolFree
         */

        [DllImport(@"detexify.dll", EntryPoint = "symbol_free")]
        internal static extern void SymbolFree(void* symbol);

    }
}
