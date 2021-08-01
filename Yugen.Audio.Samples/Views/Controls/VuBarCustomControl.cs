using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://github.com/microsoft/Win2D-Samples/blob/master/ExampleGallery/GlowTextCustomControl.cs
namespace Yugen.Audio.Samples.Views.Controls
{
    public sealed class VuBarCustomControl : UserControl
    {
        public float Rms
        {
            get { return (float)GetValue(RmsProperty); }
            set { SetValue(RmsProperty, value); }
        }

        public static readonly DependencyProperty RmsProperty =
            DependencyProperty.Register(
                nameof(Rms),
                typeof(float),
                typeof(VuBarCustomControl),
                new PropertyMetadata(-100f, new PropertyChangedCallback(OnRmsPropertyChanged)));

        private CanvasControl canvas;

        public VuBarCustomControl()
        {
            Loaded += UserControl_Loaded;
            Unloaded += UserControl_Unloaded;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            canvas = new CanvasControl();
            canvas.Draw += OnDraw;
            Content = canvas;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Explicitly remove references to allow the Win2D controls to get garbage collected
            if (canvas != null)
            {
                canvas.RemoveFromVisualTree();
                canvas = null;
            }
        }

        private static void OnRmsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as VuBarCustomControl;
            if (d == null)
                return;

            if (instance.canvas != null)
            {
                instance.canvas.Invalidate();
                instance.InvalidateMeasure();
            }
        }

        private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            Draw(args.DrawingSession, sender.Size);
        }

        private void Draw(CanvasDrawingSession ds, Size size)
        {
            var size2 = size.ToVector2();
            var radius = (100 / 2.0f) - 4.0f;
            var center = size2 / 2;

            ds.DrawCircle(center, radius, Colors.LightGray);
            
            ds.DrawRectangle(0, 0, 100, 100, Colors.AliceBlue);
            
            ds.DrawLine(0, 0, 0, 100, Colors.DarkGreen, 10);
        }
    }
}