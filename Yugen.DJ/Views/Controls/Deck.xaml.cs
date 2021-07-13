using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Yugen.DJ.Models;
using Yugen.DJ.Renderers;
using Yugen.DJ.ViewModels;

namespace Yugen.DJ.Views.Controls
{
    public sealed partial class Deck : UserControl
    {
        private readonly VinylRenderer _vinylRenderer;

        public Deck()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<DeckViewModel>();
            _vinylRenderer = App.Current.Services.GetService<VinylRenderer>();

            ViewModel.WaveformGenerated += OnWaveformCanvasGenerated;
        }

        public Side Side
        {
            set => ViewModel.Side = value;
        }

        private DeckViewModel ViewModel => (DeckViewModel)DataContext;

        //VinylRenderer
        private void OnVinylCanvasCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args) =>
                    args.TrackAsyncAction(_vinylRenderer.CreateResourcesAsync(sender).AsAsyncAction());

        private void OnVinylCanvasDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var isPaused = DispatcherHelper.ExecuteOnUIThreadAsync(() => ViewModel.IsPaused);
            var position = DispatcherHelper.ExecuteOnUIThreadAsync(() => ViewModel.Position);
            var pitch = DispatcherHelper.ExecuteOnUIThreadAsync(() => ViewModel.Pitch);
            _vinylRenderer.Draw(sender, args, isPaused.Result, position.Result, pitch.Result);
        }

        public void OnVinylPointerPressed(object sender, PointerRoutedEventArgs e) =>
            _vinylRenderer.PointerPressed(sender, e);

        public void OnVinylPointerMoved(object sender, PointerRoutedEventArgs e) =>
            _vinylRenderer.PointerMoved(sender, e);

        public void OnVinylPointerReleased(object sender, PointerRoutedEventArgs e) =>
            _vinylRenderer.PointerReleased(sender, e);

        // WaveformRenderer
        private void OnWaveformCanvasGenerated(object sender, EventArgs e) =>
            WaveformCanvas.Invalidate();

        private void OnWaveformCanvasDraw(CanvasControl sender, CanvasDrawEventArgs args) =>
            ViewModel.WaveformRendererService.DrawLine(sender, args.DrawingSession);

        private void OnUnloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Explicitly remove references to allow the Win2D controls to get garbage collected
            VinylCanvasAnimated.RemoveFromVisualTree();
            VinylCanvasAnimated = null;
            WaveformCanvas.RemoveFromVisualTree();
            WaveformCanvas = null;
        }
    }
}