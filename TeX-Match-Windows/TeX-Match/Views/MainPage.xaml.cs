using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.Foundation;
using Windows.UI.Input.Inking;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace TeX_Match.Views
{

    public class SymbolListItem
    {
        private readonly string command;
        private readonly bool textMode;
        private readonly bool mathMode;
        private readonly string package;
        private readonly string fontEnc;
        private readonly double score;

        internal SymbolListItem(Symbol symbol) : this(symbol, 0.0) { }

        internal SymbolListItem(Symbol symbol, double score)
        {
            this.command = symbol.Command;
            this.textMode = symbol.TextMode;
            this.mathMode = symbol.MathMode;
            this.package = symbol.Package;
            this.fontEnc = symbol.FontEncoding;
            this.score = score;
        }

        public string Command => command;
        public string Mode
        {
            get
            {
                if (textMode && !mathMode)
                {
                    return "textmode";
                }
                else if (!textMode && mathMode)
                {
                    return "mathmode";
                }
                else if (textMode && mathMode)
                {
                    return "textmode & mathmode";
                }
                else
                {
                    return "";
                }
            }
        }
        public string Package => string.Format("\\usepackage{{ {0} }}", package);
        public double Score => score;
        public string ModeAndScore => string.Format("{0} (score: {1:F4})", Mode, Score);

        //private string id => Base32.ToBase32String(Encoding.ASCII.GetBytes(string.Format("{0}-{1}-{2}", package, fontEnc, command.Replace('\\', '_')))).ToLower();
        private string id => Base32.ToBase32String(Encoding.UTF8.GetBytes(string.Format("{0}-{1}-{2}", package, fontEnc, command.Replace('\\', '_')))).ToLower();
        public string SourceURI => string.Format("ms-appx:///Assets/symbols/{0}.svg", id);
    }

    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        Classifier classifier;

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

            classifier = new Classifier();

            //Debug.WriteLine("exgq6ybkf5pjukum64pnyxv1edwprvvucnq6et8 => " + Encoding.ASCII.GetString(Base32.FromBase32String("exgq6ybkf5pjukum64pnyxv1edwprvvucnq6et8")));
        }

        private void DrawingCanvas_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            System.Collections.Generic.IReadOnlyList<InkStroke> strokes = sender.StrokeContainer.GetStrokes();
            StrokeSampleBuilder sampleBuilder = new StrokeSampleBuilder((uint)strokes.Count);

            foreach (InkStroke stroke in strokes)
            {
                System.Collections.Generic.IReadOnlyList<InkPoint> points = stroke.GetInkPoints();
                StrokeBuilder strokeBuilder = new StrokeBuilder((uint)points.Count);
                
                foreach (InkPoint point in points)
                {
                    strokeBuilder.AddPoint(point.Position.X, point.Position.Y);
                }

                sampleBuilder.AddStroke(strokeBuilder.build());
            }

            StrokeSample sample = sampleBuilder.build();
            Scores scores = classifier.classify(sample);
                
            ResultsList.Items.Clear();

            foreach(Score score in scores)
            {
                ResultsList.Items.Add(new SymbolListItem(score.Symbol, score.Value));
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
