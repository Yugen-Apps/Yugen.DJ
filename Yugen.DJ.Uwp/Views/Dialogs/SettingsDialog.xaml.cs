using Microsoft.Extensions.DependencyInjection;
using Yugen.DJ.Uwp.ViewModels;

namespace Yugen.DJ.Uwp.Views.Dialogs
{
    public sealed partial class SettingsDialog
    {
        public SettingsDialog()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<SettingsViewModel>();
        }

        private SettingsViewModel ViewModel => (SettingsViewModel)DataContext;
    }
}