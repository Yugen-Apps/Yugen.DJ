using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using Yugen.Toolkit.Uwp.Audio.Helpers;
using Yugen.Toolkit.Uwp.Audio.Waveform.Interfaces;
using Yugen.Toolkit.Uwp.Audio.Waveform.Models;
using Yugen.Toolkit.Uwp.Audio.Waveform.Providers;

namespace Yugen.Audio.Samples.ViewModels
{
    public class WaveformRenderer
    {
        public void DrawRealLine(CanvasControl sender, CanvasDrawingSession ds, WaveformRendererSettings waveformRendererSettings, IPeakProvider peakProvider)
        {
            // DecibelScale - if true, convert values to decibels for a logarithmic waveform
            if (waveformRendererSettings.DecibelScale)
                peakProvider = new DecibelPeakProvider(peakProvider, 48);

            var midPoint = waveformRendererSettings.TopHeight;

            var x = 0;
            var currentPeak = peakProvider.GetNextPeak();

            var strokeWidth = 1;

            while (x < waveformRendererSettings.Width)
            {
                var mu = (float)x / waveformRendererSettings.Width;
                var color = ColorHelper.GradientColor(mu);

                var nextPeak = peakProvider.GetNextPeak();

                for (var n = 0; n < waveformRendererSettings.PixelsPerPeak; n++)
                {
                    var lineHeight = waveformRendererSettings.TopHeight * currentPeak.Max;
                    ds.DrawLine(x, midPoint, x, midPoint - lineHeight, color, strokeWidth);

                    lineHeight = waveformRendererSettings.BottomHeight * currentPeak.Min;
                    ds.DrawLine(x, midPoint, x, midPoint - lineHeight, color, 10 - strokeWidth);

                    x++;
                }

                for (var n = 0; n < waveformRendererSettings.SpacerPixels; n++)
                {
                    // spacer bars are always the lower of the
                    var max = Math.Min(currentPeak.Max, nextPeak.Max);
                    var min = Math.Max(currentPeak.Min, nextPeak.Min);

                    var lineHeight = waveformRendererSettings.TopHeight * max;
                    ds.DrawLine(x, midPoint, x, midPoint - lineHeight, color, strokeWidth);

                    lineHeight = waveformRendererSettings.BottomHeight * min;
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

            //var middle = height / 2;

            var steps = 50; // Math.Min((int)(width / 10), 30);

            for (var i = 0; i < steps; ++i)
            {
                var mu = (float)i / steps;
                var a = (float)(mu * Math.PI * 2);

                var color = ColorHelper.GradientColor(mu);

                var x = width * mu;
                var rnd = new Random();
                var y = rnd.Next(1, 100); //(float)(middle + Math.Sin(a) * (middle * 0.3));

                var strokeWidth = 1; // (float)(Math.Cos(a) + 1) * 5;

                ds.DrawLine(x, 0, x, y, color, strokeWidth);
                ds.DrawLine(x, height, x, y, color, 10 - strokeWidth);
            }
        }
    }
}