using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.ViewModels
{
    public class RightDeckViewModel : DeckViewModel
    {
        public RightDeckViewModel(IDockService dockService) : base(dockService)
        {
        }

        public override Side Side => Side.Right;
    }
}