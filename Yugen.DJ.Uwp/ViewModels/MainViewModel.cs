﻿using Microsoft.Extensions.DependencyInjection;
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
            HelpCommand = new AsyncRelayCommand(HelpCommandBehavior);
            SettingsCommand = new AsyncRelayCommand(SettingsCommandBehavior);

            LeftDeckViewModel = App.Current.Services.GetService<LeftDeckViewModel>();
            RightDeckViewModel = App.Current.Services.GetService<RightDeckViewModel>();
        }

        public ICommand HelpCommand { get; }

        public ICommand SettingsCommand { get; }

        public LeftDeckViewModel LeftDeckViewModel { get; }

        public RightDeckViewModel RightDeckViewModel { get; }

        private async Task HelpCommandBehavior()
        {
            var aboutDialog = new AboutDialog();
            await aboutDialog.ShowAsync();
        }
        
        private async Task SettingsCommandBehavior()
        {
            var settingsDialog = new SettingsDialog();
            await settingsDialog.ShowAsync();
        }
    }
}