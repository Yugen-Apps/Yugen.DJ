using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;
using Yugen.Toolkit.Standard.Helpers;

namespace Yugen.Toolkit.Uwp.Audio.Controls.Renderers
{
    public class VinylRenderer
    {
        public const float Size = 400;

        private const float _center = Size / 2;
        private const float _lineLength = Size / 10;
        private const float _radius = Size / 2 - _lineLength;

        private float _angle;
        private float _radians;
        private bool _isTouched;
        private bool _isPaused = true;

        private Transform2DEffect _canvasImage;
        private Vector2 _currentCanvasImageSize;
        private bool _isClockwise;
        private float _crossProduct;
        private Vector2 _previousPosition;

        public static async Task<VinylRenderer> Create(CanvasAnimatedControl sender)
        {
#if DEBUG
            var vinylBitmap = await CanvasBitmap.LoadAsync(sender, "Yugen.Toolkit.Uwp.Audio.Controls/Images/VinylDebug.png");
#else
            var vinylBitmap = await CanvasBitmap.LoadAsync(sender, "Yugen.Toolkit.Uwp.Audio.Controls/Images/Vinyl.png");
#endif

            return new VinylRenderer(sender, vinylBitmap);
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
            if (!_isTouched &&
                !_isPaused)
            {
                //double updatesPerSecond = 1000.0 / sender.TargetElapsedTime.TotalMilliseconds;
                //int seconds = (int)(timingInformation.UpdateCount / updatesPerSecond % 10);
                //double updates = timingInformation.UpdateCount;
                //double fractionSecond = updates / updatesPerSecond % 1.0;
                //double fractionSecondAngle = 2 * Math.PI * fractionSecond;
                //_radians = (float)(fractionSecondAngle % (2 * Math.PI));

                float updatesPerSecond = (float)(sender.TargetElapsedTime.TotalSeconds * 5);
                _radians += updatesPerSecond;
            }

            _canvasImage.TransformMatrix = Matrix3x2.CreateRotation(_radians, _currentCanvasImageSize / 2);

            ds.DrawImage(_canvasImage);
        }

        public void PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _isTouched = true;

            if (sender is CanvasAnimatedControl canvasAnimatedControl)
            {
                _previousPosition = e.GetCurrentPoint(canvasAnimatedControl).Position.ToVector2();
            }
        }

        public void PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (sender is CanvasAnimatedControl canvasAnimatedControl &&
                _isTouched)
            {
                Vector2 currentPosition = e.GetCurrentPoint(canvasAnimatedControl).Position.ToVector2();
                Vector2 centerPosition = canvasAnimatedControl.ActualSize / 2;

                double previousRadians = Math.Atan2(_previousPosition.Y - centerPosition.Y,
                                                    _previousPosition.X - centerPosition.X);

                double currentRadians = Math.Atan2(currentPosition.Y - centerPosition.Y,
                                                   currentPosition.X - centerPosition.X);

                _radians += (float)(currentRadians - previousRadians);

                _isClockwise = IsClockwise(_previousPosition, centerPosition, currentPosition);
                _crossProduct = CrossProduct(currentPosition, _previousPosition);

                _previousPosition = currentPosition;
            }
        }

        public void PointerReleased(object sender, PointerRoutedEventArgs e) => _isTouched = false;

        public VinylEventArgs Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            return new VinylEventArgs(_isTouched, _isClockwise, _crossProduct);
        }

        public void PauseToggled(bool isChecked) => _isPaused = isChecked;

        public void StepClicked()
        {
            _angle += 90;
            _radians = (float)MathHelper.ConvertAngleToRadians(_angle);
        }

        private bool IsClockwise(Vector2 previousPosition, Vector2 centerPosition, Vector2 currentPosition)
        {
            Vector2 se = new Vector2(previousPosition.X - centerPosition.X, previousPosition.Y - centerPosition.Y);
            Vector2 sm = new Vector2(currentPosition.X - centerPosition.X, currentPosition.Y - centerPosition.Y);
            double cp = se.X * sm.Y - se.Y * sm.X;

            return cp > 0;
        }

        /// <summary>
        /// Returns the z-value of the cross product of two vectors.
        /// Since the Vector2 is in the x-y plane, a 3D cross product
        /// only produces the z-value
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private float CrossProduct(Vector2 v1, Vector2 v2) => (v1.X * v2.Y) - (v1.Y * v2.X);
    }
}