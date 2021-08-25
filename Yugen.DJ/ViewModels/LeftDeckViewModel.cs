using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.ViewModels
{
    public class LeftDeckViewModel : DeckViewModel
    {
        public LeftDeckViewModel(IDockService dockService) : base(dockService)
        {
        }

        public override Side Side => Side.Left;
    }
}