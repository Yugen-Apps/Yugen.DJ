using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
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
        private bool _isEqualizerOpen;
        private string _playPauseButton = "\uE768";
        private double _tempo;
        private double _lowEQ;
        private double _midEQ;
        private double _highEQ;

        public DeckViewModel(IDockServiceProvider dockServiceProvider)
        {
            _dockServiceProvider = dockServiceProvider;

            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
            EqualizerCommand = new RelayCommand(EqualizerCommandBehavior);
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

        public IAsyncRelayCommand OpenCommand { get; }

        public IRelayCommand EqualizerCommand { get; }

        public IAsyncRelayCommand ScratchCommand { get; }

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

        public bool IsEqualizerOpen
        {
            get => _isEqualizerOpen;
            set => SetProperty(ref _isEqualizerOpen, value);
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

        public double LowEQ
        {
            get => _lowEQ;
            set
            {
                if (SetProperty(ref _lowEQ, value))
                {
                    _dockService.ChangeEQ(0, _lowEQ);
                }
            }
        }

        public double MidEQ
        {
            get => _midEQ;
            set
            {
                if (SetProperty(ref _midEQ, value))
                {
                    _dockService.ChangeEQ(1, _midEQ);
                }
            }
        }

        public double HighEQ
        {
            get => _highEQ;
            set
            {
                if (SetProperty(ref _highEQ, value))
                {
                    _dockService.ChangeEQ(2, _highEQ);
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

        private void EqualizerCommandBehavior()
        {
            IsEqualizerOpen = !IsEqualizerOpen;
        }

        private Task ScratchCommandBehavior(VinylEventArgs e)
        {
            return _dockService.Scratch(e.IsTouched, e.IsClockwise, e.CrossProduct);
        }
    }
}