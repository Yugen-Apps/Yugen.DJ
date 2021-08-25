using Windows.UI.Xaml.Controls;
using Yugen.DJ.Uwp.ViewModels;

namespace Yugen.DJ.Uwp.Views.Controls
{
    public partial class Deck : UserControl
    {
        public Deck()
        {
            this.InitializeComponent();
        }

        private DeckViewModel ViewModel => (DeckViewModel)DataContext;
    }
}