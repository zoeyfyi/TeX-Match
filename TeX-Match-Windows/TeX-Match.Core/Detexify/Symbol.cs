using System.Collections;
using System.Collections.Generic;
using System.Text;
using TeX_Match.Core.Detexify;

public class Symbol
{
    const uint MAX_STRING_BYTES = 1024;

    internal unsafe void* Ptr { get; }

    internal unsafe Symbol(void* symbol)
    {
        Ptr = symbol;
    }

    internal unsafe Symbol(uint i) : this(Bindings.symbols_get(i)) { }

    static uint Count()
    {
        unsafe { return Bindings.symbols_count(); }
    }

    ~Symbol()
    {
        unsafe { Bindings.symbol_free(Ptr); }
    }
  
    public static IEnumerable All()
    {
            for (uint i = 0; i < Symbol.Count(); i++)
            {
                yield return new Symbol(i);
            }
    }

    public string Command {
        get {
            StringBuilder builder = new StringBuilder((int)MAX_STRING_BYTES);
            unsafe { Bindings.symbol_get_command(Ptr, builder, MAX_STRING_BYTES); }
            return builder.ToString();
        }
    }

    public string Package {
        get {
            StringBuilder builder = new StringBuilder((int)MAX_STRING_BYTES);
            unsafe { Bindings.symbol_get_package(Ptr, builder, MAX_STRING_BYTES); }
            return builder.ToString();
        }
    }

    public string FontEncoding {
        get {
            StringBuilder builder = new StringBuilder((int)MAX_STRING_BYTES);
            unsafe { Bindings.symbol_get_font_encoding(Ptr, builder, MAX_STRING_BYTES); }
            return builder.ToString();
        }
    }

    public bool TextMode {
        get {
            unsafe { return Bindings.symbol_get_text_mode(Ptr); }
        }
    }

    public bool MathMode {
        get {
            unsafe { return Bindings.symbol_get_math_mode(Ptr); }
        }
    }
}
