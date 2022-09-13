using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Yugen.DJ.Views.Dialogs;

namespace Yugen.DJ.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            SettingsCommand = new AsyncRelayCommand(SettingsCommandBehavior);

            LeftDeckViewModel = App.Current.Services.GetService<LeftDeckViewModel>();
            RightDeckViewModel = App.Current.Services.GetService<RightDeckViewModel>();
        }

        public ICommand SettingsCommand { get; }

        public LeftDeckViewModel LeftDeckViewModel { get; }

        public RightDeckViewModel RightDeckViewModel { get; }

        private async Task SettingsCommandBehavior()
        {
            var settingsDialog = new SettingsDialog();
            await settingsDialog.ShowAsync();
        }
    }
}