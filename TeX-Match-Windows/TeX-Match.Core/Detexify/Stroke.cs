namespace TeX_Match.Core.Detexify
{
    public class Stroke
    {
        internal unsafe Stroke(void* stroke)
        {
            Ptr = stroke;
        }

        internal unsafe void* Ptr { get; }
    }
}