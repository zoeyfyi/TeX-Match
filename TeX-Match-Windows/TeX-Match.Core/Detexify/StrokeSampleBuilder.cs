using TeX_Match.Core.Detexify;

public class StrokeSampleBuilder
{
    unsafe void* builder;

    public StrokeSampleBuilder(uint capacity)
    {
        unsafe
        {
            builder = Bindings.stroke_sample_builder_new(capacity);
        }
    }

    public void AddStroke(Stroke stroke)
    {
        unsafe { Bindings.stroke_sample_builder_add_stroke(builder, stroke.Ptr); }
    }

    public StrokeSample build()
    {
        unsafe { return new StrokeSample(Bindings.stroke_sample_builder_build(builder)); }
    }
}