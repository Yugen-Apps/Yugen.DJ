using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Yugen.DJ
{
    public sealed partial class MainPage : Page
    {
        private const float width = 1000;

        private const float height = 1000;

        private DeckRenderer sweepRenderer;
        private DeckRenderer sweepRenderer2;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private static void CalculateLayout(Size size, float width, float height, out Matrix3x2 counterTransform, out Matrix3x2 graphTransform)
        {
            bool verticalLayout = true;
            if (size.Width > size.Height)
                verticalLayout = false;

            if (verticalLayout)
            {
                float targetWidth = (float)size.Width;
                float targetHeight = (float)size.Height / 2;

                float scaleFactor = targetHeight / height;

                if ((width * scaleFactor) > targetWidth)
                {
                    scaleFactor = targetWidth / width;
                }

                float xoffset = (targetWidth / 2) - (height * scaleFactor) / 2;
                counterTransform = Matrix3x2.CreateScale(scaleFactor, scaleFactor) * Matrix3x2.CreateTranslation(xoffset, 0);
                graphTransform = Matrix3x2.CreateScale(scaleFactor, scaleFactor) * Matrix3x2.CreateTranslation(xoffset, targetHeight);
            }
            else
            {
                float targetWidth = (float)size.Width / 2;
                float targetHeight = (float)size.Height;

                float scaleFactor = targetWidth / width;

                if ((height * scaleFactor) > targetHeight)
                {
                    scaleFactor = targetHeight / height;
                }

                float yoffset = (targetHeight / 2) - (height * scaleFactor) / 2;
                counterTransform = Matrix3x2.CreateScale(scaleFactor, scaleFactor) * Matrix3x2.CreateTranslation(0, yoffset);
                graphTransform = Matrix3x2.CreateScale(scaleFactor, scaleFactor) * Matrix3x2.CreateTranslation(targetWidth, yoffset);
            }
        }

        private void OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(Canvas_CreateResourcesAsync(sender).AsAsyncAction());
        }

        private async Task Canvas_CreateResourcesAsync(CanvasAnimatedControl sender)
        {
            var vinylBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Vinyl.png");
            sweepRenderer = new DeckRenderer(sender, vinylBitmap);
        }

        private void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            int updateCount = (int)(args.Timing.UpdateCount);

            var ds = args.DrawingSession;

            // Pick layout
            var size = sender.Size;

            CalculateLayout(size, width, height, out Matrix3x2 counterTransform, out Matrix3x2 graphTransform);

            // Draw
            ds.Transform = counterTransform;
            sweepRenderer.Draw(sender, args.Timing, ds);
        }

        private void OnCreateResources2(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(Canvas_CreateResourcesAsync2(sender).AsAsyncAction());
        }

        private async Task Canvas_CreateResourcesAsync2(CanvasAnimatedControl sender)
        {
            var vinylBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Vinyl.png");
            sweepRenderer2 = new DeckRenderer(sender, vinylBitmap);
        }

        private void OnDraw2(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            int updateCount = (int)(args.Timing.UpdateCount);

            var ds = args.DrawingSession;

            // Pick layout
            var size = sender.Size;

            CalculateLayout(size, width, height, out Matrix3x2 counterTransform, out Matrix3x2 graphTransform);

            // Draw
            ds.Transform = counterTransform;
            sweepRenderer2.Draw(sender, args.Timing, ds);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Explicitly remove references to allow the Win2D controls to get garbage collected
            AnimatedControl.RemoveFromVisualTree();
            AnimatedControl = null;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadAudioDevces();
        }
    }
}