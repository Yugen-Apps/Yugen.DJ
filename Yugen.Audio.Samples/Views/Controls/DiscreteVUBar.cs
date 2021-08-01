using AudioVisualizer;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

//https://github.com/clarkezone/audiovisualizer/blob/master/AudioVisualizer/DiscreteVUBar.cpp
//https://github.com/clarkezone/audiovisualizer/blob/master/AudioVisualizer/BarVisualizerBase.hss
namespace Yugen.Audio.Samples.Views.Controls
{
    public class DiscreteVUBar : Control
    {
        int _barCount;
        private Orientation _orientation;
        private MeterBarLevel[] _levels;
        int _channelIndex;
        private Thickness _elementMargin;
        private Color _unlitElementColor;
        private Mutex _lock;
        private Size _elementSize;
        private Vector3 _elementShadowOffset;
        private float _elementShadowOpacity;
        private Color _elementShadowColor;
        private float _elementShadowBlurRadius;

        private Compositor _compositor;
        private CompositionGraphicsDevice _compositionDevice;
        private ContainerVisual _meterVisual;
        private SpriteVisual _meterBackgroundVisual;
        private ContainerVisual _meterElementVisuals;
        private CompositionBrush _backgroundBrush;
        private CompositionBrush _unlitElementBrush;
        private CompositionBrush _auxElementBrush;
        private CompositionBrush _shadowMask;
        private IBarElementFactory _elementFactory;

        private SpriteVisual[] _elementVisuals;
        private int[] _barStates;    // Keeps state of the bar elements lit
        private int[] _barAuxStates;    // Keeps state of the auxiliary bar elements lit

        private Dictionary<Color, CompositionBrush> _elementBrushes = new Dictionary<Color, CompositionBrush>();

        public DiscreteVUBar()
        {
            _barCount = 1;
            _channelIndex = 0;
            _orientation = Orientation.Vertical;
            _elementMargin = new Thickness(0, 0, 0, 0);
            _unlitElementColor = Colors.Gray;
            _elementShadowBlurRadius = 0.0f;
            _elementShadowOpacity = 1.0f;
            _elementShadowColor = Colors.Transparent;
            _elementShadowOffset = new Vector3(0, 0, 0);
            Visual elementVisual = ElementCompositionPreview.GetElementVisual(this);
            _compositor = elementVisual.Compositor;

            CanvasDevice device = CanvasDevice.GetSharedDevice();
            _compositionDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, device);

            _meterVisual = _compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(this, _meterVisual);

            //_auxElementBrush = _compositor.CreateColorBrush(Colors.BlueViolet);
            _backgroundBrush = _compositor.CreateColorBrush(Colors.BlueViolet);
            _meterBackgroundVisual = _compositor.CreateSpriteVisual();
            _meterBackgroundVisual.Brush = _backgroundBrush;
            _meterVisual.Children.InsertAtBottom(_meterBackgroundVisual);

            _meterElementVisuals = _compositor.CreateContainerVisual();
            _meterVisual.Children.InsertAtTop(_meterElementVisuals);

            _unlitElementBrush = _compositor.CreateColorBrush(_unlitElementColor);

            InitializeDefaultLevels();

            this.SizeChanged += OnSizeChanged;
            this.Loaded += OnLoaded;
            //this.Unloaded += OnUnloaded;
            //this.RegisterPropertyChangedCallback += OnBackgroundChanged;
        }

        public float RmsFake { get; set; }

        public float PeakFake { get; set; }

        //public int RmsFake
        //{
        //    get { return (int)GetValue(RmsFakeProperty); }
        //    set { SetValue(RmsFakeProperty, value); }
        //}

        //public static readonly DependencyProperty RmsFakeProperty =
        //    DependencyProperty.Register(nameof(RmsFake),
        //                                typeof(int),
        //                                typeof(BarVisualizerBase),
        //                                new PropertyMetadata(0));
        //public int PeakFake
        //{
        //    get { return (int)GetValue(PeakFakeProperty); }
        //    set { SetValue(PeakFakeProperty, value); }
        //}

        //public static readonly DependencyProperty PeakFakeProperty =
        //    DependencyProperty.Register(nameof(PeakFake),
        //                                typeof(int),
        //                                typeof(BarVisualizerBase),
        //                                new PropertyMetadata(0));

        public CompositionBrush CreateElementBrush(object sender, Color elementColor, Size size, Compositor compositor, CompositionGraphicsDevice device)
        {
            if (size.Width == 0 && size.Height == 0)
                return null;
            var surface = device.CreateDrawingSurface(size, DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
            using (var drawingSession = CanvasComposition.CreateDrawingSession(surface))
            {
                drawingSession.Clear(Colors.Transparent);
                var center = size.ToVector2() / 2.0f;
                var radius = center.X > center.Y ? center.Y : center.X;
                drawingSession.FillCircle(center, radius, elementColor);
            }
            return compositor.CreateSurfaceBrush(surface);
        }

        public CompositionBrush CreateShadowMask(object sender, Size size, Compositor compositor, CompositionGraphicsDevice device)
        {
            if (size.Width == 0 && size.Height == 0)
                return null;

            var surface = device.CreateDrawingSurface(size, DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
            using (var drawingSession = CanvasComposition.CreateDrawingSession(surface))
            {
                drawingSession.Clear(Colors.Transparent);
                var center = size.ToVector2() / 2.0f;
                var radius = center.X > center.Y ? center.Y : center.X;
                drawingSession.FillCircle(center, radius, Color.FromArgb(255, 255, 255, 255));
            }
            return compositor.CreateSurfaceBrush(surface);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            LayoutVisuals(e.NewSize);
        }

        private void InitializeDefaultLevels()
        {
            MeterBarLevel[] defaultLevels = new MeterBarLevel[23];
            int level = -60;
            for (var i = 0; i < 23; i++, level += 3)
            {
                defaultLevels[i].Level = (float)level;
                if (level < -6)
                {
                    defaultLevels[i].Color = Colors.Lime;
                }
                else if (level <= 0)
                {
                    defaultLevels[i].Color = Colors.Yellow;
                }
                else
                {
                    defaultLevels[i].Color = Colors.Red;
                }
            }
            Levels(defaultLevels);
        }

        private void Levels(MeterBarLevel[] defaultLevels)
        {
            //lock (_lock)

            if (defaultLevels == null || defaultLevels.Length < 1)
            {
                throw new Exception("Value cannot be empty");
            }

            float lastValue = defaultLevels[0].Level;
            for (var index = 1; index < defaultLevels.Length; index++)
            {
                if (defaultLevels[index].Level <= lastValue)
                {
                    throw new Exception("Array elements need to be in ascending order");
                }

                lastValue = defaultLevels[index].Level;
            }

            _levels = defaultLevels;
            //std::copy(defaultLevels.begin(), defaultLevels.end(), _levels.begin());

            CreateElementVisuals();
            LayoutVisuals();
        }

        private void CreateElementVisuals()
        {
            _meterElementVisuals.Children.RemoveAll();
            _elementVisuals = null;
            var elementRowCount = _levels.Length;
            Array.Resize(ref _elementVisuals, _barCount * elementRowCount);
            Array.Resize(ref _barStates, _barCount);
            Array.Resize(ref _barAuxStates, _barCount);

            for (int columnIndex = 0, elementIndex = 0; columnIndex < _barCount; columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < elementRowCount; rowIndex++, elementIndex++)
                {
                    var elementVisual = _compositor.CreateSpriteVisual();
                    var dropShadow = _compositor.CreateDropShadow();
                    dropShadow.BlurRadius = _elementShadowBlurRadius;
                    // If unlit elements are transparent, so are their shadows
                    dropShadow.Color = _unlitElementColor == Colors.Transparent
                                       ? Colors.Transparent
                                       : _elementShadowColor;

                    dropShadow.Offset = _elementShadowOffset;
                    dropShadow.Opacity = _elementShadowOpacity;
                    elementVisual.Shadow = dropShadow;
                    elementVisual.Brush = _unlitElementBrush;
                    _meterElementVisuals.Children.InsertAtBottom(elementVisual);
                    _elementVisuals[elementIndex] = elementVisual;
                }
            }
        }

        private void LayoutVisuals()
        {
            LayoutVisuals(new Size((float)this.ActualWidth, (float)this.ActualHeight));
        }

        private void LayoutVisuals(Size size)
        {
            if (_elementVisuals.Length == 0)
                return;

            var elementRowCount = _levels.Length;

            Size newSize = size;

            var cellSize = _orientation == Orientation.Vertical
                           ? new Size(newSize.Width / _barCount, newSize.Height / _levels.Length)
                           : new Size(newSize.Width / _levels.Length, newSize.Height / _barCount);

            var elementOffset = new Size(_elementMargin.Left * cellSize.Width, _elementMargin.Top * cellSize.Height);
            var relativeElementSize = new Size((float)(1.0 - _elementMargin.Left - _elementMargin.Right), (float)(1.0 - _elementMargin.Top - _elementMargin.Bottom));
            _elementSize = new Size(relativeElementSize.Width * cellSize.Width,
                                    relativeElementSize.Height * cellSize.Height);
            CreateElementBrushes();

            if (_elementFactory != null)
            {
                _shadowMask = _elementFactory.CreateShadowMask(this, _elementSize, _compositor, _compositionDevice);
            }
            else
            {
                _shadowMask = null;
            }

            //UpdateShadows([=](Composition.DropShadow const &shadow) { shadow.Mask(_shadowMask); });

            for (int barIndex = 0, elementIndex = 0; barIndex < _barCount; barIndex++)
            {
                var elementCell = _orientation == Orientation.Vertical
                                  ? new Size(barIndex * cellSize.Width, newSize.Height - cellSize.Height) // If vertical layout, first cell is bottommost to the left
                                  : new Size(0, cellSize.Height * barIndex);  // else left top

                elementCell = new Size(elementCell.Width + elementOffset.Width, elementCell.Height + elementOffset.Height);

                for (var rowIndex = 0; rowIndex < elementRowCount; rowIndex++, elementIndex++)
                {
                    var elementVisual = _elementVisuals[elementIndex];
                    elementVisual.Offset = new Vector3(elementCell.ToVector2(), 0);
                    elementVisual.Size = _elementSize.ToVector2();
                    if (_orientation == Orientation.Vertical)
                    {
                        elementCell.Height += cellSize.Height; // -=
                    }
                    else
                    {
                        elementCell.Width += cellSize.Width;
                    }
                }
            }
        }

        private void UpdateShadows(Action<DropShadow> action)
        {
            foreach (var visual in _elementVisuals)
            {
                if (visual.Shadow is DropShadow shadow)
                {
                    action(shadow);
                }
            }
        }

        private void CreateElementBrushes()
        {
            _elementBrushes.Clear();
            foreach (var level in _levels)
            {
                var existingElement = _elementBrushes.TryGetValue(level.Color, out var compositionBrush);
                if (!existingElement)
                {
                    _elementBrushes.Add(level.Color, CreateBrushForColor(level.Color));
                }
            }
        }

        private CompositionBrush CreateBrushForColor(Color color)
        {
            //if (_elementFactory)
            //{
            //    auto size = winrt::Windows::Foundation::Size(_elementSize.x, _elementSize.y);
            //    return _elementFactory.CreateElementBrush(*derived_this(), color, size, _compositor, _compositionDevice);
            //}
            //else
            //{
            return _compositor.CreateColorBrush(color);
            //}
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += OnUpdateMeter;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();
        }

        private void OnUpdateMeter(object sender, object e)
        {
            float rmsValue = -100.0f;
            float peakValue = -100.0f;

            //if (frame && frame.RMS() && frame.RMS().Size() > _channelIndex)
            //{
            //    rmsValue = frame.RMS().AmplitudeScale() == ScaleType::Linear ? 20.0f * log10f(frame.RMS().GetAt(_channelIndex)) : frame.RMS().GetAt(_channelIndex);
            //}
            //if (_displayPeak && frame && frame.Peak() && frame.Peak().Size() > _channelIndex)
            //{
            //    peakValue = frame.Peak().AmplitudeScale() == ScaleType::Linear ? 20.0f * log10f(frame.Peak().GetAt(_channelIndex)) : frame.Peak().GetAt(_channelIndex);
            //}

            UpdateBarValue(0, rmsValue, peakValue);
        }

        private void UpdateBarValue(int barIndex, float mainValue, float auxValue)
        {
            int mainValueIndex = GetBarElementIndex(mainValue);
            int auxValueIndex = GetBarElementIndex(auxValue);

            //Random random = new Random();
            //mainValueIndex = random.Next(0, 22);
            //auxValueIndex = random.Next(0, 22);

            mainValueIndex = (int)RmsFake;
            auxValueIndex = (int)PeakFake;

            int updateFrom = Math.Min(mainValueIndex, _barStates[barIndex]);
            int updateTo = Math.Max(mainValueIndex, _barStates[barIndex]);

            for (int rowIndex = 0; rowIndex < (int)_levels.Length; rowIndex++)
            {
                if ((rowIndex >= updateFrom && rowIndex <= updateTo && updateFrom != updateTo) ||
                    (auxValueIndex != _barAuxStates[barIndex] &&
                    (rowIndex == auxValueIndex || rowIndex == _barAuxStates[barIndex])))
                {
                    var visual = _elementVisuals[barIndex * _levels.Length + rowIndex];
                    var shadow = visual.Shadow as DropShadow;
                    if (rowIndex <= mainValueIndex || rowIndex == auxValueIndex)
                    {
                        CompositionBrush brush = null;
                        if (rowIndex == auxValueIndex && auxValueIndex >= mainValueIndex && _auxElementBrush != null)
                        {
                            brush = _auxElementBrush;
                        }
                        else
                        {
                            _elementBrushes.TryGetValue(_levels[rowIndex].Color, out var brushEntry);
                            //Debug.Assert(brushEntry != _elementBrushes.Last);    // Should not happen
                            brush = brushEntry;
                        }
                        visual.Brush = brush;
                        shadow.Color = _elementShadowColor;
                    }
                    else
                    {
                        visual.Brush = _unlitElementBrush;
                        shadow.Color = _unlitElementColor == Colors.Transparent
                                        ? Colors.Transparent
                                        : _elementShadowColor;    // Unlit element shadows are transparent
                    }
                }
            }
            _barStates[barIndex] = mainValueIndex;
            _barAuxStates[barIndex] = auxValueIndex;
        }

        private int GetBarElementIndex(float value)
        {
            if (value > _levels[0].Level)
            {
                float firstLevelGTE = 0;

                for (int i = 0; i < _levels.Length; i++)
                {
                    if (_levels[i].Level >= value)
                    {
                        firstLevelGTE = _levels[i].Level;
                    }
                }

                if (firstLevelGTE != _levels[_levels.Length - 1].Level)
                {
                    if (firstLevelGTE == _levels[0].Level)
                        return -1;

                    return (int)((firstLevelGTE - _levels[0].Level) - 1);
                }
                else
                {
                    return (int)(_levels.Length - 1);
                }
            }
            return -1;
        }
    }
}