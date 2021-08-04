using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Yugen.Toolkit.Uwp.Audio.Controls.Renderers;

namespace Yugen.Toolkit.Uwp.Audio.Controls
{
    public sealed partial class Waveform : UserControl
    {
        private WaveformRenderer _waveformRenderer = new WaveformRenderer();

        public Waveform()
        {
            this.InitializeComponent();
        }

        private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (PeakList != null)
            {
                _waveformRenderer.DrawRealLine(sender, args.DrawingSession, (int)sender.Height, (int)sender.Width, PeakList);
            }
            else
            {
                _waveformRenderer.DrawFakeLine(sender, args.DrawingSession);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Explicitly remove references to allow the Win2D controls to get garbage collected
            WaveformCanvas.RemoveFromVisualTree();
            WaveformCanvas = null;
        }

        private static void IsGeneratedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((Waveform)d).WaveformCanvas.Invalidate();
            }
        }

        public List<(float min, float max)> PeakList
        {
            get { return (List<(float min, float max)>)GetValue(PeakListProperty); }
            set { SetValue(PeakListProperty, value); }
        }

        public static readonly DependencyProperty PeakListProperty =
            DependencyProperty.Register(nameof(PeakList),
                                        typeof(List<(float min, float max)>),
                                        typeof(Waveform),
                                        new PropertyMetadata(null, IsGeneratedCallback));

        public ICommand GenerateCommand
        {
            get { return (ICommand)GetValue(GenerateCommandProperty); }
            set { SetValue(GenerateCommandProperty, value); }
        }

        public static readonly DependencyProperty GenerateCommandProperty =
            DependencyProperty.Register(nameof(GenerateCommand), 
                                        typeof(ICommand), 
                                        typeof(Waveform), 
                                        new PropertyMetadata(null));
    }
}