using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Yugen.Toolkit.Uwp.Audio.Controls.Renderers;

namespace Yugen.Toolkit.Uwp.Audio.Controls
{
    public sealed partial class Waveform : UserControl
    {
        public static readonly DependencyProperty PeakListProperty =
            DependencyProperty.Register(nameof(PeakList),
                                        typeof(List<(float min, float max)>),
                                        typeof(Waveform),
                                        new PropertyMetadata(null, PeakListPropertyChanged));

        public static readonly DependencyProperty GenerateCommandProperty =
            DependencyProperty.Register(nameof(GenerateCommand),
                                        typeof(ICommand),
                                        typeof(Waveform),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty BarColorProperty =
            DependencyProperty.Register(
                nameof(BarColor),
                typeof(Color),
                typeof(Waveform),
                new PropertyMetadata(Colors.Gray, BarColorPropertyChanged));

        private WaveformRenderer _waveformRenderer;

        public Waveform()
        {
            this.InitializeComponent();

            //var accentColor = (Color)this.Resources["SystemAccentColor"];
            _waveformRenderer = new WaveformRenderer(BarColor);
        }

        public List<(float min, float max)> PeakList
        {
            get { return (List<(float min, float max)>)GetValue(PeakListProperty); }
            set { SetValue(PeakListProperty, value); }
        }

        public ICommand GenerateCommand
        {
            get { return (ICommand)GetValue(GenerateCommandProperty); }
            set { SetValue(GenerateCommandProperty, value); }
        }

        public Color BarColor
        {
            get { return (Color)GetValue(BarColorProperty); }
            set { SetValue(BarColorProperty, value); }
        }

        private static void BarColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Waveform waveform)
            {
                waveform._waveformRenderer.UpdateColor(waveform.BarColor);
            }
        }

        private static void PeakListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((Waveform)d).WaveformCanvas.Invalidate();
            }
        }

        private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (PeakList != null)
            {
                _waveformRenderer.DrawRealLine(sender, args.DrawingSession, PeakList);
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
    }
}