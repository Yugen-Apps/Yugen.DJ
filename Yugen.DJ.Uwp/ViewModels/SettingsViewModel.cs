using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private IAudioDeviceService _audioDeviceService;
        private AudioDevice _masterAudioDeviceInformation;
        private AudioDevice _headphonesAudioDeviceInformation;

        public SettingsViewModel(IAudioDeviceService audioDeviceService)
        {
            _audioDeviceService = audioDeviceService;

            AudioDeviceInformationCollection.Add(_audioDeviceService.PrimaryDevice);
            AudioDeviceInformationCollection.Add(_audioDeviceService.SecondaryDevice);

            MasterAudioDeviceInformation = AudioDeviceInformationCollection[0];
            HeadphonesAudioDeviceInformation = AudioDeviceInformationCollection[1];
        }

        public ObservableCollection<AudioDevice> AudioDeviceInformationCollection { get; set; } =
            new ObservableCollection<AudioDevice>();

        public AudioDevice MasterAudioDeviceInformation
        {
            get => _masterAudioDeviceInformation;
            set
            {
                if (SetProperty(ref _masterAudioDeviceInformation, value))
                {
                    _audioDeviceService.PrimaryDevice = _masterAudioDeviceInformation;
                }
            }
        }

        public AudioDevice HeadphonesAudioDeviceInformation
        {
            get => _headphonesAudioDeviceInformation;
            set
            {
                if (SetProperty(ref _headphonesAudioDeviceInformation, value))
                {
                    _audioDeviceService.SecondaryDevice = _headphonesAudioDeviceInformation;
                }
            }
        }
    }
}