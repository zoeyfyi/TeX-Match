using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.Input.Inking;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using TeX_Match.Core.Detexify;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.System;
using Windows.UI.Core;
using System.Collections.Generic;
using System.Xml;

namespace TeX_Match.Views
{
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

            foreach (Symbol symbol in Symbol.All())
            {
                ResultsList.Items.Add(new SymbolListItem(symbol));
            }
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

        private void ResultsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            SymbolListItem item = (SymbolListItem) e.ClickedItem;
            DataPackage dataPackage = new DataPackage();

            string content;

            if (Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down))
            {
                content = item.Package;
            } else
            {
                content = item.Command;
            }

            dataPackage.SetText(content);
            Clipboard.SetContent(dataPackage);

            Notification.Show("Copied " + content + " to clipboard", 3000);
        }
    }
}
