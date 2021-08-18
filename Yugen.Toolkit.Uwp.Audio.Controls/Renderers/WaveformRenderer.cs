using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using Windows.UI;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Toolkit.Uwp.Audio.Controls.Renderers
{
    public class WaveformRenderer
    {
        Color _topColor;
        Color _bottomColor;
        public WaveformRenderer(Color accentColor)
        {
            _topColor = accentColor;
            _bottomColor = Color.FromArgb(150, _topColor.R, _topColor.G, _topColor.B);
        }

        public void DrawRealLine(CanvasControl sender, CanvasDrawingSession ds, int height, int width, List<(float min, float max)> peakList)
        {
            int midPoint = height / 2;
            int strokeWidth = 1;

            for (int x = 0; x < peakList.Count; x+=10)
            {
                var currentPeak = peakList[x];
                float mu = (float)x / width;
                //Windows.UI.Color color = Uwp.Helpers.ColorHelper.GradientColor(mu);

                float topLineHeight = midPoint * currentPeak.max;
                float bottomLineHeight = midPoint * currentPeak.min;

                ds.DrawLine(x, midPoint, x, midPoint - topLineHeight, _topColor, strokeWidth);
                ds.DrawLine(x, midPoint, x, midPoint - bottomLineHeight, _bottomColor, strokeWidth);
            }
        }

        public void DrawFakeLine(CanvasControl sender, CanvasDrawingSession ds)
        {
            var width = (float)sender.ActualWidth;
            var height = (float)sender.ActualHeight;

            //var middle = height / 2;
            var steps = 50; // Math.Min((int)(width / 10), 30);

            for (var i = 0; i < steps; i++)
            {
                var mu = (float)i / steps;
                var a = (float)(mu * Math.PI * 2);

                //var color = Uwp.Helpers.ColorHelper.GradientColor(mu);

                var x = width * mu;
                var rnd = new Random();
                var y = rnd.Next(1, 100); //(float)(middle + Math.Sin(a) * (middle * 0.3));

                var strokeWidth = 1; // (float)(Math.Cos(a) + 1) * 5;

                ds.DrawLine(x, 0, x, y, _topColor, strokeWidth);
                ds.DrawLine(x, height, x, y, _bottomColor, 10 - strokeWidth);
            }
        }
    }
}