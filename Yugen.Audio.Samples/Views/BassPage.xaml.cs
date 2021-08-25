using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.Audio.Samples.ViewModels;

namespace Yugen.Audio.Samples.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BassPage : Page
    {
        public BassPage()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<BassViewModel>();
        }

        private BassViewModel ViewModel => (BassViewModel)DataContext;
    }
}