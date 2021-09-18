using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Yugen.DJ.Uwp.ViewModels;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.Uwp.Views.Controls
{
    public sealed partial class TrackDetails : UserControl
    {
        public TrackDetails()
        {
            this.InitializeComponent();

            DataContext = App.Current.Services.GetService<TrackDetailsViewModel>();
        }

        public Side Side
        {
            set => ViewModel.Side = value;
        }

        private TrackDetailsViewModel ViewModel => (TrackDetailsViewModel)DataContext;
    }
}