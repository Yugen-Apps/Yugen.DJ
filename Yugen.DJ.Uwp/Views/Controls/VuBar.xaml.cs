using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.DJ.Uwp.ViewModels;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.Uwp.Views.Controls
{
    public sealed partial class VuBar : UserControl
    {
        public VuBar()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<VuBarViewModel>();
        }

        public Side Side
        {
            set => ViewModel.Side = value;
        }

        private VuBarViewModel ViewModel => (VuBarViewModel)DataContext;
    }
}