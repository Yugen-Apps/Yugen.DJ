using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

//https://github.com/robmikh/audiovisualization
namespace Yugen.Audio.Samples.Views.Controls
{
    public class VuBarSwapChainVisualizer : Control, IDisposable
    {
        private Compositor _compositor;
        private ContainerVisual _rootVisual;

        private CanvasDevice _device;
        private CompositionGraphicsDevice _compositionGraphicsDevice;
        private CanvasSwapChain _swapChain;
        private SpriteVisual _swapChainVisual;
        private CancellationTokenSource _drawLoopCancellationTokenSource;

        public VuBarSwapChainVisualizer()
        {
            var elementVisual = ElementCompositionPreview.GetElementVisual(this as UIElement);
            _compositor = elementVisual.Compositor;
            _rootVisual = _compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(this as UIElement, _rootVisual);

            CreateDevice();
            _swapChainVisual = _compositor.CreateSpriteVisual();
            _rootVisual.Children.InsertAtTop(_swapChainVisual);

            this.SizeChanged += OnSizeChanged;
            this.Unloaded += OnUnloaded;
        }

        public void Dispose()
        {
            _drawLoopCancellationTokenSource?.Cancel();
            _swapChain?.Dispose();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e != null && e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                SetDevice(_device, e.NewSize);
                _swapChainVisual.Size = new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged -= OnSizeChanged;
            this.Dispose();
        }

        private void SetDevice(CanvasDevice device, Size windowSize)
        {
            _drawLoopCancellationTokenSource?.Cancel();

            _swapChain = new CanvasSwapChain(device, (float)this.ActualWidth, (float)this.ActualHeight, 96);
            _swapChainVisual.Brush = _compositor.CreateSurfaceBrush(CanvasComposition.CreateCompositionSurfaceForSwapChain(_compositor, _swapChain));

            _drawLoopCancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(
                DrawLoop,
                _drawLoopCancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        private void CreateDevice()
        {
            _device = CanvasDevice.GetSharedDevice();
            _device.DeviceLost += OnDeviceLost;

            if (_compositionGraphicsDevice == null)
            {
                _compositionGraphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, _device);
            }
            else
            {
                CanvasComposition.SetCanvasDevice(_compositionGraphicsDevice, _device);
            }
        }

        private void OnDeviceLost(CanvasDevice sender, object args)
        {
            _device.DeviceLost -= OnDeviceLost;

            var unwaitedTask = Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => CreateDevice());
        }

        private void DrawLoop()
        {
            var canceled = _drawLoopCancellationTokenSource.Token;

            try
            {
                while (!canceled.IsCancellationRequested)
                {
                    DrawSwapChain(_swapChain);
                    _swapChain.WaitForVerticalBlank();
                }

                _swapChain.Dispose();
            }
            catch (Exception e) when (_swapChain.Device.IsDeviceLost(e.HResult))
            {
                _swapChain.Device.RaiseDeviceLost();
            }
        }

        private void DrawSwapChain(CanvasSwapChain swapChain)
        {
            using (var ds = swapChain.CreateDrawingSession(Colors.Transparent))
            {
                var size2 = swapChain.Size.ToVector2();

                var radius = (100 / 2.0f) - 4.0f;
                var center = size2 / 2;

                ds.DrawCircle(center, radius, Colors.LightGray);

                ds.DrawRectangle(0, 0, 100, 100, Colors.AliceBlue);

                ds.DrawLine(0, 0, 0, 100, Colors.DarkGreen, 10);
            }

            swapChain.Present();
        }
    }
}