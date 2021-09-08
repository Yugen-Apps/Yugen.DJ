using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.DJ.Uwp.ViewModels;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.DJ.Uwp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            TitleBarHelper.SetTitleBar(AppTitleBar);

            DataContext = App.Current.Services.GetService<MainViewModel>();
        }

        private MainViewModel ViewModel => (MainViewModel)DataContext;
    }
}