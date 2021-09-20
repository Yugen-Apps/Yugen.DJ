using Microsoft.Toolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Input;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Audio.Controls;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class DeckViewModel : ViewModelBase
    {
        private readonly IDockServiceProvider _dockServiceProvider;
        private IDockService _dockService;

        private Side _side;
        private bool _isSongLoaded;
        private bool _isPaused = true;
        private string _playPauseButton = "\uE768";
        private double _tempo;

        public DeckViewModel(IDockServiceProvider dockServiceProvider)
        {
            _dockServiceProvider = dockServiceProvider;

            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
            ScratchCommand = new AsyncRelayCommand<VinylEventArgs>(ScratchCommandBehavior);
        }

        public Side Side
        {
            get => _side;
            set
            {
                _side = value;
                _dockService = _dockServiceProvider.Get(_side);
            }
        }

        public ICommand OpenCommand { get; }

        public ICommand ScratchCommand { get; }

        public bool IsSongLoaded
        {
            get => _isSongLoaded;
            set => SetProperty(ref _isSongLoaded, value);
        }

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (SetProperty(ref _isPaused, value))
                {
                    _dockService.TogglePlay(_isPaused);

                    PlayPauseButton = _isPaused ? "\uE768" : "\uE769";
                }
            }
        }

        public string PlayPauseButton
        {
            get => _playPauseButton;
            set => SetProperty(ref _playPauseButton, value);
        }

        public double Tempo
        {
            get => _tempo;
            set
            {
                if (SetProperty(ref _tempo, value))
                {
                    _dockService.ChangePitch(_tempo);
                }
            }
        }

        private async Task OpenCommandBehavior()
        {
            IsPaused = true;

            if (await _dockService.LoadSong())
            {
                IsSongLoaded = true;
                Tempo = 0;
            }
        }

        private Task ScratchCommandBehavior(VinylEventArgs e)
        {
            return _dockService.Scratch(e.IsTouched, e.IsClockwise, e.CrossProduct);
        }
    }
}