using Microsoft.Extensions.DependencyInjection;
using Yugen.DJ.Uwp.ViewModels;

namespace Yugen.DJ.Uwp.Views.Dialogs
{
    public sealed partial class AboutDialog
    {
        public AboutDialog()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<AboutViewModel>();
        }

        private AboutViewModel ViewModel => (AboutViewModel)DataContext;
    }
}