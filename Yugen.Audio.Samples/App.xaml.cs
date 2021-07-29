﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Yugen.Audio.Samples.Helpers;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Services;
using Yugen.Audio.Samples.ViewModels;
using Yugen.Audio.Samples.ViewModels.Controls;
using Yugen.Audio.Samples.Views;
using Yugen.Toolkit.Uwp.Audio.Bpm;
using Yugen.Toolkit.Uwp.Audio.Waveform.Services;
using Yugen.Toolkit.Uwp.Services;

namespace Yugen.Audio.Samples
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // Register services
            Services = ConfigureServices();

            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is AppShell shell))
            {
                await InitializeServices();

                // Create a AppShell to act as the navigation context and navigate to the first page
                shell = new AppShell();
                shell.MainFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: UWP Load state from previously suspended application
                }
            }

            // adds callbacks for Back requests and changes
            NavigationService.Initialize(typeof(App), shell.MainFrame, typeof(HomePage));

            // Place our app shell in the current Window
            Window.Current.Content = shell;

            if (shell.MainFrame.Content == null)
            {
                // When the navigation stack isn't restored, navigate to the first page
                // suppressing the initial entrance animation.
                NavigationService.NavigateToPage(typeof(HomePage), e.Arguments, new SuppressNavigationTransitionInfo());
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<IAudioGraphAudioPlayer, AudioGraphAudioPlayer>()
                .AddTransient<IWaveformService, WaveformService>()
                .AddTransient<IBPMService, BPMService>()
                .AddSingleton<AppShellViewModel>()
                .AddSingleton<AudioFrameInputNodeViewModel>()
                .AddSingleton<AudioGraphViewModel>()
                .AddSingleton<BassViewModel>()
                .AddSingleton<CsCoreViewModel>()
                .AddSingleton<SharpDXViewModel>()
                .AddSingleton<VuBarsVieModel>()
                .AddSingleton<DeckViewModel>()
                .AddSingleton<WaveformViewModel>()
                .BuildServiceProvider();
        }

        private async Task InitializeServices()
        {
            await AudioDevicesHelper.Initialize();
        }
    }
}
