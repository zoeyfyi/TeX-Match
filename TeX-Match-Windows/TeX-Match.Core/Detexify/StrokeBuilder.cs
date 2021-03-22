using TeX_Match.Core.Detexify;

public class StrokeBuilder
{
    unsafe void* strokeBuilder;

    public StrokeBuilder(uint capacity)
    {
        unsafe { strokeBuilder = Bindings.stroke_builder_new(capacity); }
    }

    public void AddPoint(double x, double y)
    {
        unsafe { Bindings.stroke_builder_add_point(strokeBuilder, x, y); }
    }

    public Stroke build()
    {
        unsafe { return new Stroke(Bindings.stroke_builder_build(strokeBuilder)); }
    }
}