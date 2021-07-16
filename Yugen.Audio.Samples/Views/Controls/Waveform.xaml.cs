using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Yugen.Audio.Samples.ViewModels;

namespace Yugen.Audio.Samples.Views.Controls
{
    public sealed partial class Waveform : UserControl
    {
        public Waveform()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<WaveformViewModel>();
        }

        private WaveformViewModel ViewModel => (WaveformViewModel)DataContext;

        private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args) =>
            ViewModel.OnDraw(sender, args.DrawingSession);

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Explicitly remove references to allow the Win2D controls to get garbage collected
            WaveformCanvas.RemoveFromVisualTree();
            WaveformCanvas = null;
        }
    }
}