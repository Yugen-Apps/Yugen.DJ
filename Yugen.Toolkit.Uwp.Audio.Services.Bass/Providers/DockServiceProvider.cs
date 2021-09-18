using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass.Providers
{
    public class DockServiceProvider : IDockServiceProvider
    {
        private readonly IDockService _leftDockService;
        private readonly IDockService _rightDockService;

        public DockServiceProvider(
            IDockService leftDockService,
            IDockService rightDockService)
        {
            _leftDockService = leftDockService;
            _rightDockService = rightDockService;
        }

        public void Initialize()
        {
            _leftDockService.Initialize(Side.Left);
            _rightDockService.Initialize(Side.Right);
        }

        public IDockService Get(Side side)
        {
            return side == Side.Left
                ? _leftDockService
                : _rightDockService;
        }
    }
}