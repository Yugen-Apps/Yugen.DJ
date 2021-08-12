using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Yugen.Audio.Samples.Views
{
    public sealed partial class VinylPage : Page
    {
        public VinylPage()
        {
            this.InitializeComponent();
        }

        private void OnStepClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Vinyl.StepClicked();
        }

        private async void OnTaskClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Task.Delay(1000);
            System.Diagnostics.Debug.WriteLine("Bello!");
        }

        private void OnBackgroundTaskClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                System.Diagnostics.Debug.WriteLine("Bello!");
            });
        }
    }
}
