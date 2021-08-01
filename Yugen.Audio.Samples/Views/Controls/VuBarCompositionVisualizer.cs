using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

//https://github.com/robmikh/audiovisualization
namespace Yugen.Audio.Samples.Views.Controls
{
    public class VuBarCompositionVisualizer : Control
    {
        private Compositor _compositor;
        private ContainerVisual _rootVisual;

        private SpriteVisual _backgroundVisual;
        private CompositionBrush _backgroundBrush;
        private SpriteVisual _barVisual;

        private Compositor _windowCompositor;
        private CompositionPropertySet _compositionPropertySet;

        public VuBarCompositionVisualizer()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _rootVisual = _compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(this, _rootVisual);

            _backgroundVisual = _compositor.CreateSpriteVisual();
            _rootVisual.Children.InsertAtTop(_backgroundVisual);
            _backgroundBrush = _compositor.CreateColorBrush(Colors.LightGray);
            _backgroundVisual.Brush = _backgroundBrush;

            _windowCompositor = ElementCompositionPreview.GetElementVisual(Window.Current.Content).Compositor;
            _compositionPropertySet = _windowCompositor.CreatePropertySet();
            _compositionPropertySet.InsertScalar("InputData", 0);

            this.SizeChanged += OnSizeChanged;
            this.Unloaded += OnUnloaded;

            SetupVisualizer();
        }

        public CompositionPropertySet CompositionPropertySet => _compositionPropertySet;

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_backgroundVisual != null)
            {
                _backgroundVisual.Size = new Vector2((float)this.ActualWidth, (float)this.ActualHeight);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e) => this.SizeChanged -= OnSizeChanged;

        private void SetupVisualizer()
        {
            _barVisual = _compositor.CreateSpriteVisual();
            _barVisual.Size = new Vector2(50, 0);
            _barVisual.AnchorPoint = new Vector2(0.5f, 1);
            _barVisual.Brush = _compositor.CreateColorBrush(Colors.Red);

            var sizeExpression = _compositor.CreateExpressionAnimation();
            sizeExpression.Expression = "propertySet.InputData";
            sizeExpression.SetReferenceParameter("propertySet", CompositionPropertySet);
            _barVisual.StartAnimation(nameof(Visual.Size) + ".Y", sizeExpression);

            var offsetExpression = _compositor.CreateExpressionAnimation();
            offsetExpression.Expression = "Vector3(visual.Size.X / 2, visual.Size.Y, 0)";
            offsetExpression.SetReferenceParameter("visual", _backgroundVisual);
            _barVisual.StartAnimation(nameof(Visual.Offset), offsetExpression);

            _backgroundVisual.Children.InsertAtTop(_barVisual);
        }
    }
}