using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using Windows.System;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class VuBarViewModel : ObservableObject
    {
        private readonly IMixerService _mixerService;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private Side _side;
        private float _rms;

        public VuBarViewModel(IMixerService mixerService)
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
                    _mixerService.LeftRmsChanged += OnMixerServiceRmsChanged;
                }
                else
                {
                    _mixerService.RightRmsChanged += OnMixerServiceRmsChanged;
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