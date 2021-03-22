namespace TeX_Match.Core.Detexify
{
    public class StrokeSample
    {
        internal unsafe StrokeSample(void* strokeSample)
        {
            Ptr = strokeSample;
        }

        internal unsafe void* Ptr { get; }
    }

}