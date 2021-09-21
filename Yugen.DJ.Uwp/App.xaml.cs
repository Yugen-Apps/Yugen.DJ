using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Yugen.DJ.Uwp.Interfaces;
using Yugen.DJ.Uwp.StateTriggers;
using Yugen.DJ.Uwp.ViewModels;
using Yugen.DJ.Uwp.Views;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;
using Yugen.Toolkit.Uwp.Audio.Services.Bass;
using Yugen.Toolkit.Uwp.Audio.Services.Bass.Providers;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.DJ.Uwp
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
            var size = new Windows.Foundation.Size(1280, 800);
            ApplicationView.GetForCurrentView().SetPreferredMinSize(size);
            ApplicationView.PreferredLaunchViewSize = size;
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                await InitializeServices();

                // Initial UI styling
                //var accentColor = (Color)this.Resources["SystemAccentColor"];
                TitleBarHelper.ExpandViewIntoTitleBar();
                TitleBarHelper.StyleTitleBar();

                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();

                await Services.GetService<IWhatsNewDisplayService>().ShowIfAppropriateAsync();
            }
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
                .AddSingleton<IAudioDeviceService, AudioDeviceService>()
                .AddSingleton<IBPMService, BPMService>()
                .AddSingleton<IDockServiceProvider, DockServiceProvider>()
                .AddTransient<IDockService, DockService>()
                .AddTransient<IAudioPlaybackService, AudioPlaybackService>()
                .AddSingleton<IAudioPlaybackServiceProvider, AudioPlaybackServiceProvider>()
                .AddSingleton<IMixerService, MixerService>()
                .AddSingleton<ITrackService, TrackService>()
                .AddSingleton<IWaveformService, WaveformService>()
                .AddSingleton<IWhatsNewDisplayService, WhatsNewDisplayService>()

                .AddTransient<DeckViewModel>()
                .AddSingleton<MainViewModel>()
                .AddSingleton<MixerViewModel>()
                .AddSingleton<SettingsViewModel>()
                .AddTransient<TrackDetailsViewModel>()
                .AddTransient<VolumeViewModel>()
                .AddTransient<VuBarViewModel>()

                .BuildServiceProvider();
        }

        private async Task InitializeServices()
        {
            var audioDeviceService = Services.GetService<IAudioDeviceService>();
            var audioPlaybackServiceProvider = Services.GetService<IAudioPlaybackServiceProvider>();
            var dockServiceProvider = Services.GetService<IDockServiceProvider>();

            await audioDeviceService.Initialize();
            audioPlaybackServiceProvider.Initialize();
            dockServiceProvider.Initialize();
        }
    }
}