using Windows.UI.Xaml.Controls;
using Yugen.DJ.ViewModels;

namespace Yugen.DJ.Views.Controls
{
    public sealed partial class Deck : UserControl
    {
        public Deck()
        {
            this.InitializeComponent();
        }

        private DeckViewModel ViewModel => (DeckViewModel)DataContext;
    }
}