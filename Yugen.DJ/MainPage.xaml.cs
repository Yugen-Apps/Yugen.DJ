using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Yugen.DJ.Renderer;
using Yugen.DJ.ViewModels;

namespace Yugen.DJ
{
    public sealed partial class MainPage : Page
    {
        private const float width = 1000;
        private const float height = 1000;

        private VinylRenderer leftVinylRenderer;
        private VinylRenderer rightVinylRenderer;
        private TouchPointsRenderer leftTouchPointsRenderer = new TouchPointsRenderer();
        private TouchPointsRenderer rightTouchPointsRenderer = new TouchPointsRenderer();


        public MainPage()
        {
            this.InitializeComponent();
        }

        private void OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }

        private async Task CreateResourcesAsync(CanvasAnimatedControl sender)
        {
            var vinylBitmap = await CanvasBitmap.LoadAsync(sender, "Assets/Vinyl.png", 60);
            if (sender.Name == nameof(LeftCanvasAnimatedControl))
            {
                leftVinylRenderer = new VinylRenderer(sender, vinylBitmap);
            }
            else
            {
                rightVinylRenderer = new VinylRenderer(sender, vinylBitmap);
            }
        }


        private void OnLeftDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            ds.Transform = CalculateLayout(sender.Size, width, height);

            if (ViewModel.VinylLeft.IsPaused)
            {
                leftVinylRenderer.Draw(ds);
            }
            else
            {
                leftVinylRenderer.Draw(sender, ViewModel.VinylLeft.Position, ds);
            }

            ds.Transform = Matrix3x2.Identity;
            lock (leftTouchPointsRenderer)
            {
                leftTouchPointsRenderer.Draw(ds);
            }
        }

        private void OnRightDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            ds.Transform = CalculateLayout(sender.Size, width, height);

            if (ViewModel.VinylRight.IsPaused)
            {
                rightVinylRenderer.Draw(ds);
            }
            else
            {
                rightVinylRenderer.Draw(sender, ViewModel.VinylRight.Position, ds);
            }

            ds.Transform = Matrix3x2.Identity;
            lock (rightTouchPointsRenderer)
            {
                rightTouchPointsRenderer.Draw(ds);
            }
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


        private void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var vinylViewModel = GetVinylViedModel(sender);
            if (vinylViewModel != null)
            {
                vinylViewModel.IsTouched = true;

                if (vinylViewModel.IsLeft)
                {
                    lock (leftTouchPointsRenderer)
                    {
                        leftTouchPointsRenderer.OnPointerPressed();
                    }
                }
                else
                {
                    lock (rightTouchPointsRenderer)
                    {
                        rightTouchPointsRenderer.OnPointerPressed();
                    }
                }
            }
        }

        private void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var canvasAnimatedControl = GetCanvasAnimatedControl(sender);
            if (canvasAnimatedControl == null)
                return;

            if (canvasAnimatedControl.Name == nameof(LeftCanvasAnimatedControl))
            {
                lock (leftTouchPointsRenderer)
                {
                    leftTouchPointsRenderer.OnPointerMoved(e.GetIntermediatePoints(canvasAnimatedControl));
                }
            }
            else
            {
                lock (rightTouchPointsRenderer)
                {
                    rightTouchPointsRenderer.OnPointerMoved(e.GetIntermediatePoints(canvasAnimatedControl));
                }
            }
        }

        private void OnPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var vinylViewModel = GetVinylViedModel(sender);
            if (vinylViewModel != null)
            {
                vinylViewModel.IsTouched = false;
            }
        }


        private CanvasAnimatedControl GetCanvasAnimatedControl(object sender) =>
            sender is CanvasAnimatedControl canvasAnimatedControl
                ? canvasAnimatedControl : null;

        private VinylViewModel GetVinylViedModel(object sender)
        {
            var canvasAnimatedControl = GetCanvasAnimatedControl(sender);
            if (canvasAnimatedControl == null)
                return null;

            return canvasAnimatedControl.Name == nameof(LeftCanvasAnimatedControl)
                ? ViewModel.VinylLeft
                : ViewModel.VinylRight;
        }


        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadAudioDevces();

            ViewModel.VinylLeft.AddAudioVisualizer(LeftSpectrumVisualizer);
            ViewModel.VinylLeft.AddAudioVisualizer(LeftVUBarChanel0, LeftVUBarChanel1);

            ViewModel.VinylRight.AddAudioVisualizer(RightSpectrumVisualizer);
            ViewModel.VinylRight.AddAudioVisualizer(RightVUBarChanel0, RightVUBarChanel1);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Explicitly remove references to allow the Win2D controls to get garbage collected
            LeftCanvasAnimatedControl.RemoveFromVisualTree();
            LeftCanvasAnimatedControl = null;
            RightCanvasAnimatedControl.RemoveFromVisualTree();
            RightCanvasAnimatedControl = null;
        }
    }
}