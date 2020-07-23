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
        public VinylRenderer LeftVinylRenderer { get; set; } = new VinylRenderer();
        public VinylRenderer RightVinylRenderer { get; set; } = new VinylRenderer();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadAudioDevces();

            ViewModel.VinylLeft.AddAudioVisualizer(LeftSpectrumVisualizer);
            ViewModel.VinylLeft.AddAudioVisualizer(LeftVUBarChanel0, LeftVUBarChanel1);

            ViewModel.VinylRight.AddAudioVisualizer(RightSpectrumVisualizer);
            ViewModel.VinylRight.AddAudioVisualizer(RightVUBarChanel0, RightVUBarChanel1);

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