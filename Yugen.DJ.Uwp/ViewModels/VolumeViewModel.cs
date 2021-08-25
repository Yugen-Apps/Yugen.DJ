using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using Windows.System;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class VolumeViewModel : ObservableObject
    {
        private readonly IMixerService _mixerService;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private Side _side;
        private double _volume = 100;
        private bool _isHeadPhones;
        private float _rms;

        public VolumeViewModel(IMixerService mixerService)
        {
            _mixerService = mixerService;

            if (_side == Side.Left)
            {
                _mixerService.LeftRmsChanged += OnMixerServiceRmsChanged;
            }
            else
            {
                _mixerService.RightRmsChanged += OnMixerServiceRmsChanged;
            }
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

        public float Rms
        {
            get => _rms;
            set => SetProperty(ref _rms, value);
        }

        private void OnMixerServiceRmsChanged(object sender, float e)
        {
            _dispatcherQueue.EnqueueAsync(() =>
            {
                Rms = e;
            });
        }
    }
}