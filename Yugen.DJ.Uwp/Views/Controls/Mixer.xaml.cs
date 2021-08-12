using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.DJ.Uwp.ViewModels;

namespace Yugen.DJ.Uwp.Views.Controls
{
    public sealed partial class Mixer : UserControl
    {
        public Mixer()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<MixerViewModel>();
        }

        private MixerViewModel ViewModel => (MixerViewModel)DataContext;
    }
}