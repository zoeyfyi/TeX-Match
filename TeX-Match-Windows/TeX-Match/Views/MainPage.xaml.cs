using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI.Input.Inking;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace TeX_Match.Views
{

    public unsafe sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        void* classifier;

        public MainPage()
        {
            InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size(640, 400);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(640, 400));

            DrawingArea.InkPresenter.InputDeviceTypes =
                Windows.UI.Core.CoreInputDeviceTypes.Mouse
                | Windows.UI.Core.CoreInputDeviceTypes.Touch
                | Windows.UI.Core.CoreInputDeviceTypes.Pen;
            DrawingArea.InkPresenter.StrokesCollected += DrawingCanvas_StrokesCollected;
            

            classifier = Detextify.classifier_new_default();

            //void* sampleBuilder = Detextify.stroke_sample_new_builder(0);
            //void* sample = Detextify.stroke_sample_build(sampleBuilder);
            //void* scores = Detextify.classify(classifier, sample);
            //ResultsList.Items.Clear();
            //for (uint i = 0; i < Detextify.scores_length(scores); i++)
            //{
            //    string command = Detextify.scores_get_command(scores, i);
            //    double score = Detextify.scores_get_score(scores, i);
            //    ResultsList.Items.Add(String.Format("{0}: command: {1}, score: {2}", i, command, score));
            //}
        }

        private void DrawingCanvas_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            System.Collections.Generic.IReadOnlyList<InkStroke> strokes = sender.StrokeContainer.GetStrokes();
            void* sampleBuilder = Detextify.stroke_sample_new_builder((uint)strokes.Count);
            
            foreach (InkStroke stroke in strokes)
            {
                System.Collections.Generic.IReadOnlyList<InkPoint> points = stroke.GetInkPoints();
                void* sb = Detextify.stroke_builder_new((uint)points.Count);

                foreach (InkPoint point in points)
                {
                    Detextify.stroke_builder_add_point(sb, point.Position.X, point.Position.Y);
                }

                void* s = Detextify.stroke_builder_build(sb);
                Detextify.stroke_sample_add_stroke(sampleBuilder, s);
            }

            void* sample = Detextify.stroke_sample_build(sampleBuilder);
            void* scores = Detextify.classify(classifier, sample);

            ResultsList.Items.Clear();

            for (uint i =0; i < Detextify.scores_length(scores); i++)
            {
                string command = Detextify.scores_get_command(scores, i);
                double score = Detextify.scores_get_score(scores, i);
                ResultsList.Items.Add(String.Format("{0}: command: {1}, score: {2}", i, command, score));
                //Detextify.scores_free(command);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ClearButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DrawingArea.InkPresenter.StrokeContainer.Clear();
        }


    }
}
