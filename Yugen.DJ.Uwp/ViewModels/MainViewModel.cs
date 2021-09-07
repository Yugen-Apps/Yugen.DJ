using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Yugen.DJ.Uwp.Views.Dialogs;
using Yugen.Toolkit.Standard.Mvvm;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            AboutCommand = new AsyncRelayCommand(AboutCommandBehavior);
            HelpCommand = new AsyncRelayCommand(HelpCommandBehavior);
            SettingsCommand = new AsyncRelayCommand(SettingsCommandBehavior);
            WhatsNewCommand = new AsyncRelayCommand(WhatsNewCommandBehavior);

            LeftDeckViewModel = App.Current.Services.GetService<LeftDeckViewModel>();
            RightDeckViewModel = App.Current.Services.GetService<RightDeckViewModel>();
        }

        public ICommand AboutCommand { get; }

        public ICommand HelpCommand { get; }

        public ICommand SettingsCommand { get; }

        public ICommand WhatsNewCommand { get; }

        public LeftDeckViewModel LeftDeckViewModel { get; }

        public RightDeckViewModel RightDeckViewModel { get; }

        private async Task AboutCommandBehavior()
        {
            var aboutDialog = new AboutDialog();
            await aboutDialog.ShowAsync();
        }

        private async Task HelpCommandBehavior()
        {
            var helpDialog = new HelpDialog();
            await helpDialog.ShowAsync();
        }
        
        private async Task SettingsCommandBehavior()
        {
            var settingsDialog = new SettingsDialog();
            await settingsDialog.ShowAsync();
        }

        private async Task WhatsNewCommandBehavior()
        {
            var whatsNewDialog = new WhatsNewDialog();
            await whatsNewDialog.ShowAsync();
        }
    }
}