using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Toolkit.Uwp.Audio.Controls.Renderers
{
    public class WaveformRenderer
    {
        public void DrawRealLine(CanvasControl sender, CanvasDrawingSession ds, int height, int width, List<(float min, float max)> peakList)
        {
            int midPoint = height / 2;
            int strokeWidth = 1;

            for (int x = 0; x < peakList.Count; x++)
            {
                var currentPeak = peakList[x];
                float mu = (float)x / width;
                Windows.UI.Color color = ColorHelper.GradientColor(mu);

                float topLineHeight = midPoint * currentPeak.max;
                float bottomLineHeight = midPoint * currentPeak.min;

                ds.DrawLine(x, midPoint, x, midPoint - topLineHeight, color, strokeWidth);
                ds.DrawLine(x, midPoint, x, midPoint - bottomLineHeight, color, strokeWidth);
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