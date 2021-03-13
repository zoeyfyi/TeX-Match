using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace TeX_Match.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public MainPage()
        {
            InitializeComponent();
            DrawingArea.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen;

            ApplicationView.PreferredLaunchViewSize = new Size(640, 400);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(640, 400));

            for (int i =0; i < 100; i++)
            {
                ResultsList.Items.Add("Foo" + i);
            }
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
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
