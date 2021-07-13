using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Yugen.Audio.Samples.Helpers;

namespace Yugen.Audio.Samples.Renderers
{
    public class VinylRenderer
    {
        private const float _size = 400;
        private const float _center = _size / 2;
        private const float _lineLength = _size / 10;
        private const float _radius = _size / 2 - _lineLength;

        private float _angle;
        private float _radians;
        private bool _isTouched;

        private Transform2DEffect _canvasImage;
        private Vector2 _currentCanvasImageSize;

        public static async Task<VinylRenderer> Create(CanvasAnimatedControl sender)
        {
            var bitmapTiger = await CanvasBitmap.LoadAsync(sender, "Assets/Images/Circle.png");

            return new VinylRenderer(sender, bitmapTiger);
        }

        private VinylRenderer(CanvasAnimatedControl sender, CanvasBitmap canvasBitmap)
        {
            CreateBrushes(sender, canvasBitmap);
        }

        private void CreateBrushes(CanvasAnimatedControl sender, CanvasBitmap canvasBitmap)
        {
            var bitmapSize = canvasBitmap.Size;
            var scale = _radius * 2 / (float)bitmapSize.Height;

            _currentCanvasImageSize = canvasBitmap.Size.ToVector2();
            _canvasImage = new Transform2DEffect()
            {
                Source = canvasBitmap,
                TransformMatrix = Matrix3x2.CreateScale(scale, scale) * Matrix3x2.CreateTranslation(_center - _radius, _center - _radius)
            };
        }

        public void Draw(ICanvasAnimatedControl sender, CanvasTimingInformation timingInformation, CanvasDrawingSession ds)
        {
            if (!_isTouched)
            {
                double updatesPerSecond = 1000.0 / sender.TargetElapsedTime.TotalMilliseconds;
                int seconds = (int)(timingInformation.UpdateCount / updatesPerSecond % 10);

                double updates = timingInformation.UpdateCount;
                double fractionSecond = updates / updatesPerSecond % 1.0;

                double fractionSecondAngle = 2 * Math.PI * fractionSecond;
                _radians = (float)(fractionSecondAngle % (2 * Math.PI));
            }

            _canvasImage.TransformMatrix = Matrix3x2.CreateRotation(_radians, _currentCanvasImageSize / 2);

            ds.DrawImage(_canvasImage);
        }

        public void PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _isTouched = true;
        }

        public void PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (sender is CanvasAnimatedControl canvasAnimatedControl &&
                _isTouched)
            {
                PointerPoint currentLocation = e.GetCurrentPoint(canvasAnimatedControl);

                var dialCenter = new Point(canvasAnimatedControl.ActualHeight / 2, canvasAnimatedControl.ActualWidth / 2);

                // Calculate an angle
                var radians = Math.Atan((currentLocation.Position.Y - dialCenter.Y) /
                                        (currentLocation.Position.X - dialCenter.X));

                // in order to get these figures to work, I actually had to *add* 90 degrees to it,
                // and *subtract* 180 from it if the X coord is negative.
                var x = radians * 180 / Math.PI + 90;
                if (currentLocation.Position.X - dialCenter.X < 0)
                {
                    x -= 180;
                }

                _angle = (float)x / 100;
            }
        }

        public void PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _isTouched = false;
        }

        public void StepClicked()
        {
            _angle += 90;
            _radians = (float)MathHelper.ConvertAngleToRadians(_angle);
        }
    }
}