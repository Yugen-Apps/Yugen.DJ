using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using Windows.System;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class MixerViewModel : ObservableObject
    {
        private readonly IMixerService _mixerService;
        private readonly IAudioDeviceService _audioDeviceService;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private double _crossFader = 0;
        private double _masterVolume = 0;
        private float _leftRms;
        private float _rightRms;

        public MixerViewModel(
            IMixerService mixerService,
            IAudioDeviceService audioDeviceService)
        {
            _mixerService = mixerService;
            _audioDeviceService = audioDeviceService;

            _mixerService.LeftRmsChanged += OnMixerServiceLeftRmsChanged;
            _mixerService.RightRmsChanged += OnMixerServiceRightRmsChanged;
            //_masterVolume = _audioDeviceService?.GetMasterVolume() * 100 ?? MasterVolume;
        }

        public double MasterVolume
        {
            get { return _masterVolume; }
            set
            {
                SetProperty(ref _masterVolume, value);

                //_audioDeviceService?.SetVolume(_masterVolume);
            }
        }

        public double CrossFader
        {
            get { return _crossFader; }
            set
            {
                SetProperty(ref _crossFader, value);

                _mixerService.SetFader(_crossFader);
            }
        }

        public float LeftRms
        {
            get => _leftRms;
            set => SetProperty(ref _leftRms, value);
        }

        public float RightRms
        {
            get => _rightRms;
            set => SetProperty(ref _rightRms, value);
        }

        private void OnMixerServiceLeftRmsChanged(object sender, float e)
        {
            _dispatcherQueue.EnqueueAsync(() =>
            {
                LeftRms = e;
            });
        }

        private void OnMixerServiceRightRmsChanged(object sender, float e)
        {
            _dispatcherQueue.EnqueueAsync(() =>
            {
                RightRms = e;
            });
        }
    }
}