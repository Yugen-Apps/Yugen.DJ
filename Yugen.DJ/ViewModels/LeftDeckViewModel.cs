using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.ViewModels
{
    public class LeftDeckViewModel : DeckViewModel
    {
        public LeftDeckViewModel(IDockService dockService) : base(dockService)
        {
        }

        protected override Side _side => Side.Left;
    }
}