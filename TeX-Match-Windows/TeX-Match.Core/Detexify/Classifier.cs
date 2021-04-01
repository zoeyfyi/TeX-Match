namespace TeX_Match.Core.Detexify
{
    public class Classifier
    {
        internal unsafe void* classifier;

        public Classifier()
        {
            unsafe { classifier = Bindings.ClassifierNewDefault(); }
        }

        ~Classifier()
        {
            unsafe { Bindings.ClassifierFree(classifier); }
        }

        public Scores classify(StrokeSample sample)
        {
            unsafe { return new Scores(Bindings.Classify(classifier, sample.Ptr)); }
        }
    }
}