using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Yugen.Toolkit.Uwp.Audio.Controls
{
    public class VUBar : Control
    {
        private Compositor _compositor;
        private ContainerVisual _meterVisual;
        private CompositionBrush _unlitElementBrush;

        private SpriteVisual[] _elementVisuals = new SpriteVisual[23];
        private (float Level, Color Color)[] _levels = new (float Level, Color Color)[23];

        public VUBar()
        {
            Visual elementVisual = ElementCompositionPreview.GetElementVisual(this);
            _compositor = elementVisual.Compositor;

            //CanvasDevice device = CanvasDevice.GetSharedDevice();
            //var compositionDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, device);

            _meterVisual = _compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(this, _meterVisual);

            _unlitElementBrush = _compositor.CreateColorBrush(Colors.Gray);

            InitializeDefaultLevels();

            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
        }

        public float Rms
        {
            get { return (float)GetValue(RmsProperty); }
            set { SetValue(RmsProperty, value); }
        }

        public static readonly DependencyProperty RmsProperty =
            DependencyProperty.Register(
                nameof(Rms),
                typeof(float),
                typeof(VUBar),
                new PropertyMetadata(-100f));

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += OnDispatcherTimerTick; ;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
        }

        private void OnDispatcherTimerTick(object sender, object e)
        {
            UpdateBarValue(Rms);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) => LayoutVisuals(e.NewSize);

        private void InitializeDefaultLevels()
        {
            float level = -60;
            for (var i = 0; i < 23; i++, level += 3)
            {
                _levels[i].Level = level;
                if (level < -6)
                {
                    _levels[i].Color = Colors.Lime;
                }
                else if (level <= 0)
                {
                    _levels[i].Color = Colors.Yellow;
                }
                else
                {
                    _levels[i].Color = Colors.Red;
                }
            }
        }

        private void LayoutVisuals(Size size)
        {
            var offset = new Vector3(0, 230, 0);
            int level = -60;
            for (var i = 0; i < 23; i++, level += 3)
            {
                offset.Y -= 10;

                var elementVisual = _compositor.CreateSpriteVisual();
                elementVisual.Size = new Vector2(50, 10);
                elementVisual.Brush = _unlitElementBrush;
                elementVisual.Offset = offset;
                _meterVisual.Children.InsertAtBottom(elementVisual);
                _elementVisuals[i] = elementVisual;
            }
        }

        private void UpdateBarValue(float rmsValue)
        {
            int valueIndex = GetBarElementIndex(rmsValue);

            for (int i = 0; i < 23; i++)
            {
                if (i <= valueIndex)
                {
                    _elementVisuals[i].Brush = _compositor.CreateColorBrush(_levels[i].Color);
                }
                else
                {
                    _elementVisuals[i].Brush = _unlitElementBrush;
                }
            }
        }

        private int GetBarElementIndex(float value)
        {
            int valueIndex = -1;

            if (value < _levels[0].Level)
            {
                return -1;
            }

            for (int i = 0; i < _levels.Length; i++)
            {
                if (value >= _levels[i].Level)
                {
                    valueIndex = i;
                }
            }

            return valueIndex;
        }
    }
}