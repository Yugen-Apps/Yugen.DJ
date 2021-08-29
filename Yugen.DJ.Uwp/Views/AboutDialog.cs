using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.DJ.Uwp.ViewModels;

namespace Yugen.DJ.Uwp.Views
{
    public sealed partial class AboutDialog : ContentDialog
    {
        public AboutDialog()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<AboutViewModel>();
        }

        private AboutViewModel ViewModel => (AboutViewModel)DataContext;
    }
}