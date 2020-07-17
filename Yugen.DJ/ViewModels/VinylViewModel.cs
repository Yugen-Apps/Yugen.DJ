using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Yugen.DJ.DependencyInjection;
using Yugen.DJ.Interfaces;
using Yugen.Toolkit.Standard.Mvvm.ComponentModel;
using Yugen.Toolkit.Standard.Mvvm.Input;

namespace Yugen.DJ.ViewModels
{
    public class VinylViewModel : ViewModelBase
    {
        public int[] TargetElapsedTimeList = {
            10000,
            25000,
            50000,
            75000,
            100000,
            250000,
            500000,
            750000
        };

        private readonly IAudioService _audioService;
        private readonly bool _isLeft;
        private bool _isHeadPhones;
        private bool _isPaused = true;
        private double _volume = 100;
        private double _fader;
        private double _pitch = 0;
        private TimeSpan _targetElapsedTime = new TimeSpan(10000);
        private TimeSpan _naturalDuration = new TimeSpan();
        private TimeSpan _position = new TimeSpan();
        private ICommand _openButtonCommand;

        public VinylViewModel(bool isLeft)
        {
            _isLeft = isLeft;
            IsHeadPhones = isLeft ? true : false;
            _audioService = Ioc.Default.GetService<IAudioService>();
            _audioService.PositionChanged += AudioServiceOnPositionChanged;
        }

        private void AudioServiceOnPositionChanged(object sender, TimeSpan position)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                Position = position;
            });
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                Set(ref _isPaused, value);

                _audioService.TogglePlay(_isPaused);
            }
        }

        public bool IsHeadPhones
        {
            get { return _isHeadPhones; }
            set
            {
                Set(ref _isHeadPhones, value);

                _audioService?.IsHeadphones(_isHeadPhones);
            }
        }

        public double Volume
        {
            get { return _volume; }
            set
            {
                Set(ref _volume, value);

                _audioService.ChangeVolume(_volume, _fader);
            }
        }

        public double Fader
        {
            get { return _fader; }
            set
            {
                Set(ref _fader, value);

                _audioService.ChangeVolume(_volume, _fader);
            }
        }

        public double Pitch
        {
            get { return _pitch; }
            set
            {
                Set(ref _pitch, value);

                _audioService.ChangePitch(_pitch);
            }
        }

        public TimeSpan TargetElapsedTime
        {
            get { return _targetElapsedTime; }
            set { Set(ref _targetElapsedTime, value); }
        }

        public int SelectedTargetElapsedTime
        {
            get => (int)TargetElapsedTime.Ticks;
            set => TargetElapsedTime = new TimeSpan(value);
        }

        public TimeSpan NaturalDuration
        {
            get { return _naturalDuration; }
            set { Set(ref _naturalDuration, value); }
        }

        public TimeSpan Position
        {
            get { return _position; }
            set { Set(ref _position, value); }
        }

        public ICommand OpenButtonCommand => _openButtonCommand
            ?? (_openButtonCommand = new AsyncRelayCommand(OpenButtonCommandBehavior));

        public async Task Init()
        {
            await _audioService.Init();
        }

        private async Task OpenButtonCommandBehavior()
        {
            IsPaused = true;

            await _audioService.OpenFile();

            NaturalDuration = _audioService.NaturalDuration;
        }
    }
}