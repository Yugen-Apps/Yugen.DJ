﻿using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Yugen.Toolkit.Uwp.Audio.Controls.Renderers;

namespace Yugen.Toolkit.Uwp.Audio.Controls
{
    public sealed partial class Vinyl : UserControl
    {
        public static readonly DependencyProperty IsPausedProperty =
            DependencyProperty.Register(
                nameof(IsPaused),
                typeof(bool),
                typeof(Vinyl),
                new PropertyMetadata(true, IsPausedCallback));

        private const float _width = 400;
        private const float _height = 400;

        private VinylRenderer _vinylRenderer;
        private TouchPointsRenderer _touchPointsRenderer = new TouchPointsRenderer();
        private bool _debug = true;

        public Vinyl()
        {
            this.InitializeComponent();
        }

        public bool IsPaused
        {
            get { return (bool)GetValue(IsPausedProperty); }
            set { SetValue(IsPausedProperty, value); }
        }

        public void StepClicked() => _vinylRenderer.StepClicked();

        private static void IsPausedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                ((Vinyl)d)._vinylRenderer?.PauseToggled((bool)e.NewValue);
            }
        }

        private void OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(Canvas_CreateResourcesAsync(sender).AsAsyncAction());
        }

        private async Task Canvas_CreateResourcesAsync(CanvasAnimatedControl sender)
        {
            _vinylRenderer = await VinylRenderer.Create(sender);
        }

        private void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            // Pick layout
            //LayoutHelper.CalculateLayout(sender.Size, _width, _height, out Matrix3x2 counterTransform);

            // Draw
            //ds.Transform = counterTransform;
            _vinylRenderer.Draw(sender, args.Timing, ds);

            if (_debug)
            {
                ds.Transform = Matrix3x2.Identity;
                lock (_touchPointsRenderer)
                {
                    _touchPointsRenderer.Draw(ds);
                }
            }
        }

        private void OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _vinylRenderer.PointerPressed(sender, e);

            lock (_touchPointsRenderer)
            {
                _touchPointsRenderer.OnPointerPressed();
            }

            VinylCanvasAnimated.Invalidate();
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _vinylRenderer.PointerMoved(sender, e);

            lock (_touchPointsRenderer)
            {
                _touchPointsRenderer.OnPointerMoved(e.GetIntermediatePoints(VinylCanvasAnimated));
            }

            VinylCanvasAnimated.Invalidate();
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _vinylRenderer.PointerReleased(sender, e);

            //VinylCanvasAnimated.Invalidate();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Explicitly remove references to allow the Win2D controls to get garbage collected
            VinylCanvasAnimated.RemoveFromVisualTree();
            VinylCanvasAnimated = null;
        }
    }
}