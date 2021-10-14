using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
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
        }

        public IAsyncRelayCommand AboutCommand { get; }

        public IAsyncRelayCommand HelpCommand { get; }

        public IAsyncRelayCommand SettingsCommand { get; }

        public IAsyncRelayCommand WhatsNewCommand { get; }

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