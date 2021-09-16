using Microsoft.Toolkit.Uwp;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Yugen.DJ.Uwp.ViewModels;
using Yugen.Toolkit.Uwp.Audio.Controls;

namespace Yugen.DJ.Uwp.Views.Controls
{
    public partial class Deck : UserControl
    {
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public Deck()
        {
            this.InitializeComponent();
        }

        private DeckViewModel ViewModel => (DeckViewModel)DataContext;

        private void OnVinylUpdate(VinylEventArgs e)
        {
            _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.ScratchCommand.Execute(e);
            });
        }

        private void OnVinylPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _dispatcherQueue.EnqueueAsync(() =>
            {
                ViewModel.ScratchCommand.Execute(new VinylEventArgs(false, false, 0));
            });
        }
    }
}