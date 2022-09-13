using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;
using Yugen.Toolkit.Standard.Extensions;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly IAudioDeviceService _audioDeviceService;
        private AudioDevice _masterAudioDeviceInformation;
        private AudioDevice _headphonesAudioDeviceInformation;

        public SettingsViewModel(IAudioDeviceService audioDeviceService)
        {
            _audioDeviceService = audioDeviceService;

            LoadAudioDevces();
        }

        public ObservableCollection<AudioDevice> AudioDeviceInformationCollection { get; set; } = 
            new ObservableCollection<AudioDevice>();

        public AudioDevice MasterAudioDeviceInformation
        {
            get { return _masterAudioDeviceInformation; }
            set
            {
                SetProperty(ref _masterAudioDeviceInformation, value);

                _audioDeviceService.PrimaryDevice = _masterAudioDeviceInformation;
            }
        }

        public AudioDevice HeadphonesAudioDeviceInformation
        {
            get { return _headphonesAudioDeviceInformation; }
            set
            {
                SetProperty(ref _headphonesAudioDeviceInformation, value);

                _audioDeviceService.SecondaryDevice = _headphonesAudioDeviceInformation;
            }
        }

        public void LoadAudioDevces()
        {
            AudioDeviceInformationCollection.AddRange(_audioDeviceService.AudioDeviceList);

            MasterAudioDeviceInformation = _audioDeviceService.PrimaryDevice;
            HeadphonesAudioDeviceInformation = _audioDeviceService.SecondaryDevice;
        }
    }
}