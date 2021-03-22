namespace TeX_Match.Core.Detexify
{
    public class Classifier
    {
        internal unsafe void* classifier;

        public Classifier()
        {
            unsafe { classifier = Bindings.classifier_new_default(); }
        }

        ~Classifier()
        {
            unsafe { Bindings.classifier_free(classifier); }
        }

        public Scores classify(StrokeSample sample)
        {
            unsafe { return new Scores(Bindings.classify(classifier, sample.Ptr)); }
        }
    }
}