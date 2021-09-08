﻿using Microsoft.Extensions.DependencyInjection;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Yugen.DJ.Uwp.ViewModels;

namespace Yugen.DJ.Uwp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            var accentColor = (Color)this.Resources["SystemAccentColor"];
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            // Active
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Colors.Transparent;
            // Hover
            titleBar.ButtonHoverBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverForegroundColor = Colors.BlueViolet;
            //Inactive
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveForegroundColor = Colors.Transparent;

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            DataContext = App.Current.Services.GetService<MainViewModel>();
        }

        private MainViewModel ViewModel => (MainViewModel)DataContext;
    }
}