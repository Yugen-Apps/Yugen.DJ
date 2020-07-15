using Yugen.Toolkit.Standard.Mvvm.ComponentModel;

namespace Yugen.DJ.ViewModels
{
    public class DeckViewModel : ViewModelBase
    {
        public VinylViewModel VinylLeft { get; set; } = new VinylViewModel(true);
        public VinylViewModel VinylRight { get; set; } = new VinylViewModel(false);
    }
 }