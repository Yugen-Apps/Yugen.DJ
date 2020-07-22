using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;

namespace Yugen.DJ.Renderer
{
    internal class VinylRenderer
    {
        private CanvasBitmap _vinylBitmap;

        public VinylRenderer(CanvasAnimatedControl sender, CanvasBitmap vinylBitmap)
        {
            _vinylBitmap = vinylBitmap;
        }

        //public void Draw(ICanvasAnimatedControl sender, CanvasTimingInformation timingInformation, CanvasDrawingSession ds)
        //{
        //    double fractionSecond;
        //    int seconds;

        //    var updatesPerSecond = 1000.0 / sender.TargetElapsedTime.TotalMilliseconds;
        //    seconds = (int)(timingInformation.UpdateCount / updatesPerSecond % 10);

        //    var updates = (double)timingInformation.UpdateCount;
        //    fractionSecond = updates / updatesPerSecond % 1.0;

        //    var Angle = (float)Math.PI * (seconds / 10.0f) * 2.0f;

        //    Rotate(ds, fractionSecond);
        //}

        public void Draw(ICanvasAnimatedControl sender, TimeSpan timingInformation, CanvasDrawingSession ds)
        {
            double fractionSecond;
            int seconds;

            seconds = timingInformation.Seconds;

            fractionSecond = timingInformation.Milliseconds / 100;
            fractionSecond /= 10;

            Rotate(ds, fractionSecond);
        }

        public void Rotate(CanvasDrawingSession ds, double fractionSecond)
        {
            var fractionSecondAngle = (float)(2 * Math.PI * fractionSecond);
            var angle = (float)(fractionSecondAngle % (2 * Math.PI));

            try
            {
                var originalImageRect = _vinylBitmap.GetBounds(ds);
                var endpoint = new Vector2((float)originalImageRect.Width / 2, (float)originalImageRect.Height / 2);

                ds.Clear(Colors.Transparent);

                ICanvasImage image = new Transform2DEffect
                {
                    Source = _vinylBitmap,
                    TransformMatrix = Matrix3x2.CreateRotation(angle, endpoint)
                };

                var sourceRect = image.GetBounds(ds);
                ds.DrawImage(image);
            }
            catch
            {
            }
        }

        public void Draw(CanvasDrawingSession ds)
        {
            try
            {
                var originalImageRect = _vinylBitmap.GetBounds(ds);
                var endpoint = new Vector2((float)originalImageRect.Width / 2, (float)originalImageRect.Height / 2);

                ds.Clear(Colors.Transparent);

                ICanvasImage image = new Transform2DEffect
                {
                    Source = _vinylBitmap,
                };

                ds.DrawImage(image);
            }
            catch
            {
            }
        }
    }
}