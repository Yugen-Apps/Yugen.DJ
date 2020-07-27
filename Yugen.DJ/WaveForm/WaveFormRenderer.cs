using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using NAudio.Wave;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Yugen.DJ.WaveForm.Interfaces;
using Yugen.DJ.WaveForm.Models;
using Yugen.DJ.WaveForm.Providers;

namespace Yugen.DJ.WaveForm
{
    public partial class WaveFormRenderer
    {
        private bool _isFinished;
        private WaveFormRendererSettings _settings = new WaveFormRendererSettings();
        private IPeakProvider _peakProvider = new MaxPeakProvider();

        public async Task Render(IStorageFile file)
        {
            var tmp = await file.CopyAsync(ApplicationData.Current.TemporaryFolder, file.Name, NameCollisionOption.ReplaceExisting);

            using (var reader = new MediaFoundationReader(tmp.Path))
            {
                ISampleProvider isp = reader.ToSampleProvider();
                var buffer = new float[reader.Length / 2];
                isp.Read(buffer, 0, buffer.Length);

                var bytesPerSample = reader.WaveFormat.BitsPerSample / 8;
                var samples = reader.Length / bytesPerSample;
                var samplesPerPixel = (int)(samples / _settings.Width);
                var stepSize = _settings.PixelsPerPeak + _settings.SpacerPixels;
                _peakProvider.Init(isp, samplesPerPixel * stepSize);

                _isFinished = true;
            }
        }

        //private static Image Render(IPeakProvider peakProvider, WaveFormRendererSettings settings)
        //{
        //    if (settings.DecibelScale)
        //        peakProvider = new DecibelPeakProvider(peakProvider, 48);

        //    var b = new Bitmap(settings.Width, settings.TopHeight + settings.BottomHeight);
        //    if (settings.BackgroundColor == Color.Transparent)
        //    {
        //        b.MakeTransparent();
        //    }

        //    using (var g = Graphics.FromImage(b))
        //    {
        //        g.FillRectangle(settings.BackgroundBrush, 0, 0, b.Width, b.Height);
        //        var midPoint = settings.TopHeight;

        //        int x = 0;
        //        var currentPeak = peakProvider.GetNextPeak();
        //        while (x < settings.Width)
        //        {
        //            var nextPeak = peakProvider.GetNextPeak();

        //            for (int n = 0; n < settings.PixelsPerPeak; n++)
        //            {
        //                var lineHeight = settings.TopHeight * currentPeak.Max;
        //                g.DrawLine(settings.TopPeakPen, x, midPoint, x, midPoint - lineHeight);
        //                lineHeight = settings.BottomHeight * currentPeak.Min;
        //                g.DrawLine(settings.BottomPeakPen, x, midPoint, x, midPoint - lineHeight);
        //                x++;
        //            }

        //            for (int n = 0; n < settings.SpacerPixels; n++)
        //            {
        //                // spacer bars are always the lower of the 
        //                var max = Math.Min(currentPeak.Max, nextPeak.Max);
        //                var min = Math.Max(currentPeak.Min, nextPeak.Min);

        //                var lineHeight = settings.TopHeight * max;
        //                g.DrawLine(settings.TopSpacerPen, x, midPoint, x, midPoint - lineHeight);
        //                lineHeight = settings.BottomHeight * min;
        //                g.DrawLine(settings.BottomSpacerPen, x, midPoint, x, midPoint - lineHeight);
        //                x++;
        //            }
        //            currentPeak = nextPeak;
        //        }
        //    }
        //    return b;
        //}

        public void DrawLine(CanvasControl sender, CanvasDrawingSession ds)
        {
            if (_isFinished)
            {
                DrawRealLine(sender, ds);
            }
            else
            {
                DrawFakeLine(sender, ds);
            }
        }

        public void DrawRealLine(CanvasControl sender, CanvasDrawingSession ds)
        {
            var midPoint = _settings.TopHeight;

            var x = 0;
            var currentPeak = _peakProvider.GetNextPeak();

            var strokeWidth = 1;

            while (x < _settings.Width)
            {
                var mu = (float)x / _settings.Width;
                var color = GradientColor(mu);

                var nextPeak = _peakProvider.GetNextPeak();

                for (var n = 0; n < _settings.PixelsPerPeak; n++)
                {
                    var lineHeight = _settings.TopHeight * currentPeak.Max;
                    ds.DrawLine(x, midPoint, x, midPoint - lineHeight, color, strokeWidth);

                    lineHeight = _settings.BottomHeight * currentPeak.Min;
                    ds.DrawLine(x, midPoint, x, midPoint - lineHeight, color, 10 - strokeWidth);

                    x++;
                }

                for (var n = 0; n < _settings.SpacerPixels; n++)
                {
                    // spacer bars are always the lower of the 
                    var max = Math.Min(currentPeak.Max, nextPeak.Max);
                    var min = Math.Max(currentPeak.Min, nextPeak.Min);

                    var lineHeight = _settings.TopHeight * max;
                    ds.DrawLine(x, midPoint, x, midPoint - lineHeight, color, strokeWidth);

                    lineHeight = _settings.BottomHeight * min;
                    ds.DrawLine(x, midPoint, x, midPoint - lineHeight, color, 10 - strokeWidth);

                    x++;
                }

                currentPeak = nextPeak;
            }
        }

        public void DrawFakeLine(CanvasControl sender, CanvasDrawingSession ds)
        {
            var width = (float)sender.ActualWidth;
            var height = (float)sender.ActualHeight;

            var middle = height / 2;

            var steps = _isFinished ? 100 : 50; // Math.Min((int)(width / 10), 30);

            for (var i = 0; i < steps; ++i)
            {
                var mu = (float)i / steps;
                var a = (float)(mu * Math.PI * 2);

                var color = GradientColor(mu);

                var x = width * mu;
                var y = (float)(middle + Math.Sin(a) * (middle * 0.3));

                var strokeWidth = 1; // (float)(Math.Cos(a) + 1) * 5;

                ds.DrawLine(x, 0, x, y, color, strokeWidth);
                ds.DrawLine(x, height, x, y, color, 10 - strokeWidth);
            }
        }

        private static Color GradientColor(float mu)
        {
            var c = (byte)((Math.Sin(mu * Math.PI * 2) + 1) * 127.5);

            return Color.FromArgb(255, (byte)(255 - c), c, 220);
        }
    }
}