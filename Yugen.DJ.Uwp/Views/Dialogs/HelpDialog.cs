using Microsoft.Extensions.DependencyInjection;
using Yugen.DJ.Uwp.ViewModels;

namespace Yugen.DJ.Uwp.Views.Dialogs
{
    public sealed partial class HelpDialog
    {
        public HelpDialog()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<HelpViewModel>();
        }

        private HelpViewModel ViewModel => (HelpViewModel)DataContext;
    }
}