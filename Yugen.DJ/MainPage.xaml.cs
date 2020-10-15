using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Yugen.DJ.Renderers;
using Yugen.DJ.ViewModels;

namespace Yugen.DJ
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<MainViewModel>();
        }

        private MainViewModel ViewModel => (MainViewModel)DataContext;

        public VinylRenderer LeftVinylRenderer { get; set; } = new VinylRenderer();
        public VinylRenderer RightVinylRenderer { get; set; } = new VinylRenderer();

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadAudioDevces();

            ViewModel.VinylLeft.AddWaveForm(LeftWaveFormCanvas);
            ViewModel.VinylLeft.AddAudioVisualizer(LeftVUBarChannel0, LeftVUBarChannel1);

            ViewModel.VinylRight.AddWaveForm(RightWaveFormCanvas);
            ViewModel.VinylRight.AddAudioVisualizer(RightVUBarChannel0, RightVUBarChannel1);

            LeftVinylRenderer.ViewModel = ViewModel.VinylLeft;
            RightVinylRenderer.ViewModel = ViewModel.VinylRight;
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