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
        public static readonly DependencyProperty RmsProperty =
            DependencyProperty.Register(
                nameof(Rms),
                typeof(float),
                typeof(VUBar),
                new PropertyMetadata(-100f));

        public static readonly DependencyProperty BarColorProperty =
            DependencyProperty.Register(
                nameof(BarColor),
                typeof(Color),
                typeof(VUBar),
                new PropertyMetadata(Colors.Gray, BarColorPropertyChanged));

        private const int _barCount = 23;

        private Compositor _compositor;
        private ContainerVisual _meterVisual;
        private CompositionBrush _unlitElementBrush;

        private Color _minColor = Colors.Gray;
        private Color _lowColor = Colors.Lime;
        private Color _mediumColor = Colors.Yellow;
        private Color _maxColor = Colors.Red;

        private SpriteVisual[] _elementVisuals = new SpriteVisual[_barCount];
        private (float Level, Color Color)[] _levels = new (float Level, Color Color)[_barCount];

        public VUBar()
        {
            Visual elementVisual = ElementCompositionPreview.GetElementVisual(this);
            _compositor = elementVisual.Compositor;

            //CanvasDevice device = CanvasDevice.GetSharedDevice();
            //var compositionDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, device);

            _meterVisual = _compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(this, _meterVisual);

            _unlitElementBrush = _compositor.CreateColorBrush(_minColor);

            InitializeDefaultLevels();

            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
        }

        public float Rms
        {
            get { return (float)GetValue(RmsProperty); }
            set { SetValue(RmsProperty, value); }
        }

        public Color BarColor
        {
            get { return (Color)GetValue(BarColorProperty); }
            set { SetValue(BarColorProperty, value); }
        }

        private static void BarColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VUBar vubar)
            {
                vubar._minColor = Color.FromArgb(50, vubar.BarColor.R, vubar.BarColor.G, vubar.BarColor.B);
                vubar._lowColor = Color.FromArgb(150, vubar.BarColor.R, vubar.BarColor.G, vubar.BarColor.B);
                vubar._mediumColor = Color.FromArgb(200, vubar.BarColor.R, vubar.BarColor.G, vubar.BarColor.B);
                vubar._maxColor = Color.FromArgb(255, vubar.BarColor.R, vubar.BarColor.G, vubar.BarColor.B);
                vubar.InitializeDefaultLevels();
            }
        }

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
            for (var i = 0; i < _barCount; i++, level += 3)
            {
                _levels[i].Level = level;
                if (level < -6)
                {
                    _levels[i].Color = _lowColor;
                }
                else if (level <= 0)
                {
                    _levels[i].Color = _mediumColor;
                }
                else
                {
                    _levels[i].Color = _maxColor;
                }
            }
        }

        private void LayoutVisuals(Size size)
        {
            var cellSize = new Vector2((float)size.Width, (float)(size.Height / (_barCount * 2)));
            var offset = new Vector3(0, (float)size.Height, 0);

            int level = -60;
            for (var i = 0; i < _barCount; i++, level += 3)
            {
                offset.Y -= (cellSize.Y * 2);

                var elementVisual = _compositor.CreateSpriteVisual();
                elementVisual.Size = cellSize;
                elementVisual.Brush = _unlitElementBrush;
                elementVisual.Offset = offset;
                _meterVisual.Children.InsertAtBottom(elementVisual);
                _elementVisuals[i] = elementVisual;
            }
        }

        private void UpdateBarValue(float rmsValue)
        {
            int valueIndex = GetBarElementIndex(rmsValue);

            for (int i = 0; i < _barCount; i++)
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