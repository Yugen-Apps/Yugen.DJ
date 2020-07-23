using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Yugen.DJ.ViewModels;

namespace Yugen.DJ.Renderer
{
    public class VinylRenderer
    {
        private const float width = 1000;
        private const float height = 1000;

        private CanvasBitmap _vinylBitmap;
        private TouchPointsRenderer _touchPointsRenderer = new TouchPointsRenderer();

        public VinylViewModel ViewModel { get; set; }

        public void OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }

        public async Task CreateResourcesAsync(CanvasAnimatedControl sender)
        {
            _vinylBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Vinyl.png", 60);
        }

        public void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            ds.Transform = CalculateLayout(sender.Size, width, height);

            //var time = args.Timing.ElapsedTime;

            if (ViewModel?.IsPaused ?? true)
            {
                Draw(ds);
            }
            else
            {
                Draw(sender, ViewModel.Position, ds);
            }

            ds.Transform = Matrix3x2.Identity;
            lock (_touchPointsRenderer)
            {
                _touchPointsRenderer.Draw(ds);
            }
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


        public void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //ViewModel.IsTouched = true;

            lock (_touchPointsRenderer)
            {
                _touchPointsRenderer.OnPointerPressed();
            }

        }

        public void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is CanvasAnimatedControl canvasAnimatedControl)
            {
                lock (_touchPointsRenderer)
                {
                    _touchPointsRenderer.OnPointerMoved(e.GetIntermediatePoints(canvasAnimatedControl));
                }
            }
        }

        public void OnPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //ViewModel.IsTouched = false;
        }


        private static Matrix3x2 CalculateLayout(Size size, float width, float height)
        {
            float targetWidth = (float)size.Width / 2;
            float targetHeight = (float)size.Height;
            float scaleFactor = targetWidth / width;

            if ((height * scaleFactor) > targetHeight)
            {
                scaleFactor = targetHeight / height;
            }

            float yoffset = (targetHeight / 2) - (height * scaleFactor) / 2;

            return Matrix3x2.CreateScale(scaleFactor, scaleFactor) * Matrix3x2.CreateTranslation(0, yoffset);
        }
    }
}