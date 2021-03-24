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

    internal unsafe Symbol(uint i) : this(Bindings.SymbolsGet(i)) { }

    static uint Count()
    {
        unsafe { return Bindings.SymbolsCount(); }
    }

    ~Symbol()
    {
        unsafe { Bindings.SymbolFree(Ptr); }
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
            unsafe { Bindings.SymbolGetCommand(Ptr, builder, MAX_STRING_BYTES); }
            return builder.ToString();
        }
    }

    public string Package {
        get {
            StringBuilder builder = new StringBuilder((int)MAX_STRING_BYTES);
            unsafe { Bindings.SymbolGetPackage(Ptr, builder, MAX_STRING_BYTES); }
            return builder.ToString();
        }
    }

    public string FontEncoding {
        get {
            StringBuilder builder = new StringBuilder((int)MAX_STRING_BYTES);
            unsafe { Bindings.SymbolGetFontEncoding(Ptr, builder, MAX_STRING_BYTES); }
            return builder.ToString();
        }
    }

    public bool TextMode {
        get {
            unsafe { return Bindings.SymbolGetTextMode(Ptr); }
        }
    }

    public bool MathMode {
        get {
            unsafe { return Bindings.SymbolGetMathMode(Ptr); }
        }
    }
}
