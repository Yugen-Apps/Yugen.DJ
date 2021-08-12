using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.ViewModels
{
    public class RightDeckViewModel : DeckViewModel
    {
        public RightDeckViewModel(IDockService dockService) : base(dockService)
        {
        }

        protected override Side _side => Side.Right;
    }
}