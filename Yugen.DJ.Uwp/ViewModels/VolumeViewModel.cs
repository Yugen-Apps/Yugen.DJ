using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.System;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class VolumeViewModel : ObservableObject
    {
        private readonly IMixerService _mixerService;

        private Side _side;
        private double _volume = 100;
        private bool _isHeadPhones;

        public VolumeViewModel(IMixerService mixerService)
        {
            _mixerService = mixerService;
        }

        public Side Side
        {
            get => _side;
            set
            {
                _side = value;
                if (_side == Side.Left)
                {
                    IsHeadPhones = true;
                }
            }
        }

        public bool IsHeadPhones
        {
            get => _isHeadPhones;
            set
            {
                if (SetProperty(ref _isHeadPhones, value))
                {
                    _mixerService?.IsHeadphones(_isHeadPhones, _side);
                }
            }
        }

        public double Volume
        {
            get => _volume;
            set
            {
                if (SetProperty(ref _volume, value))
                {
                    _mixerService.ChangeVolume(_volume, _side);
                }
            }
        }
    }
}