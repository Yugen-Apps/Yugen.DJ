using Microsoft.Toolkit.Mvvm.ComponentModel;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class MixerViewModel : ObservableObject
    {
        private readonly IMixerService _mixerService;
        private readonly IAudioDeviceService _audioDeviceService;

        private double _crossFader = 0;
        private double _masterVolume = 100;

        public MixerViewModel(
            IMixerService mixerService,
            IAudioDeviceService audioDeviceService)
        {
            _mixerService = mixerService;
            _audioDeviceService = audioDeviceService;
        }

        public double MasterVolume
        {
            get => _masterVolume;
            set
            {
                if (SetProperty(ref _masterVolume, value))
                {
                    _audioDeviceService?.SetVolume(_masterVolume);
                }
            }
        }

        public double CrossFader
        {
            get => _crossFader;
            set
            {
                if (SetProperty(ref _crossFader, value))
                {
                    _mixerService.SetFader(_crossFader);
                }
            }
        }
    }
}