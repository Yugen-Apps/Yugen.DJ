using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Yugen.DJ.ViewModels;

namespace Yugen.DJ.Renderer
{
    public class VinylRenderer
    {
        private const float width = 1000;
        private const float height = 1000;

        private CanvasBitmap _vinylBitmap;
        //private readonly TouchPointsRenderer _touchPointsRenderer = new TouchPointsRenderer();

        public VinylViewModel ViewModel { get; set; }

        public void OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }

        public async Task CreateResourcesAsync(CanvasAnimatedControl sender)
        {
            _vinylBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Images/Vinyl.png");
        }

        public void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            ds.Transform = CalculateLayout(sender.Size, width, height);
            //var time = args.Timing.ElapsedTime;
            
            if (isTouched || ViewModel == null)
            {
                Draw(sender, ds, angle);
            }
            else
            {
                Draw(sender, ds, ViewModel.Position);
            }

            //ds.Transform = Matrix3x2.Identity;
            //lock (_touchPointsRenderer)
            //{
            //    _touchPointsRenderer.Draw(ds);
            //}
        }

        public void Draw(ICanvasAnimatedControl sender, CanvasDrawingSession ds, TimeSpan timingInformation)
        {
            double fractionSecond = (double)timingInformation.Milliseconds / 1000;
            var fractionSecondAngle = (float)(2 * Math.PI * fractionSecond);
            var angle = (float)(fractionSecondAngle % (2 * Math.PI));

            var pitch = ((float)ViewModel.Pitch + 51) / 10;
            angle += pitch;

            Draw(sender, ds, angle);
        }

        public void Draw(ICanvasAnimatedControl sender, CanvasDrawingSession ds, float angle)
        {
            try
            {
                var originalImageRect = _vinylBitmap.GetBounds(ds);
                var endpoint = new Vector2((float)originalImageRect.Width, (float)originalImageRect.Height) / 2;

                ds.Clear(Colors.Transparent);

                ICanvasImage image = new Transform2DEffect
                {
                    Source = _vinylBitmap,
                    TransformMatrix = Matrix3x2.CreateRotation(angle, endpoint)
                };

                var offset = (sender.Size.ToVector2() * ds.Transform.M11) / 2;
                ds.DrawImage(image, offset);
            }
            catch { }
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

        float angle;
        bool isTouched;

        public void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            isTouched = true;

            //lock (_touchPointsRenderer)
            //{
            //    _touchPointsRenderer.OnPointerPressed();
            //}
        }

        public void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is CanvasAnimatedControl canvasAnimatedControl 
                && isTouched)
            {
                PointerPoint currentLocation = e.GetCurrentPoint(canvasAnimatedControl);

                Point dialCenter = new Point(canvasAnimatedControl.ActualHeight / 2, canvasAnimatedControl.ActualWidth / 2);

                // Calculate an angle
                double radians = Math.Atan((currentLocation.Position.Y - dialCenter.Y) /
                                           (currentLocation.Position.X - dialCenter.X));

                // in order to get these figures to work, I actually had to *add* 90 degrees to it,     
                // and *subtract* 180 from it if the X coord is negative. 
                double x = (radians * 180 / Math.PI) + 90;
                if ((currentLocation.Position.X - dialCenter.X) < 0)
                {
                    x -= 180;
                }

                angle = (float)x / 100;
            }

            //lock (_touchPointsRenderer)
            //{
            //    _touchPointsRenderer.OnPointerMoved(e.GetIntermediatePoints(canvasAnimatedControl));
            //}
        }

        public void OnPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            isTouched = false;
        }


        private static Matrix3x2 CalculateLayout(Size size, float width, float height)
        {
            float targetWidth = (float)size.Width;
            float targetHeight = (float)size.Height;
            float scaleFactor = targetWidth / width;

            if ((height * scaleFactor) > targetHeight)
            {
                scaleFactor = targetHeight / height;
            }

            return Matrix3x2.CreateScale(scaleFactor, scaleFactor);
        }
    }
}