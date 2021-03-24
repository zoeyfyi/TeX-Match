using TeX_Match.Core.Detexify;

public class StrokeBuilder
{
    unsafe void* strokeBuilder;

    public StrokeBuilder(uint capacity)
    {
        unsafe { strokeBuilder = Bindings.StrokeBuilderNew(capacity); }
    }

    public void AddPoint(double x, double y)
    {
        unsafe { Bindings.StrokeBuilderAddPoint(strokeBuilder, x, y); }
    }

    public Stroke build()
    {
        unsafe { return new Stroke(Bindings.StrokeBuilderBuild(strokeBuilder)); }
    }
}