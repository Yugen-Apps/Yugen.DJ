using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;
using Yugen.Toolkit.Standard.Extensions;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private string _masterAudioDeviceInformation;
        private string _headphonesAudioDeviceInformation;

        public SettingsViewModel()
        {
            MasterAudioDeviceInformation = AudioDeviceInformationCollection[0];
            HeadphonesAudioDeviceInformation = AudioDeviceInformationCollection[1];
        }

        public ObservableCollection<string> AudioDeviceInformationCollection { get; set; } =
            new ObservableCollection<string>()
            {
                "Primary",
                "Secondary"
            };

        public string MasterAudioDeviceInformation
        {
            get => _masterAudioDeviceInformation;
            set => SetProperty(ref _masterAudioDeviceInformation, value);
        }

        public string HeadphonesAudioDeviceInformation
        {
            get => _headphonesAudioDeviceInformation;
            set => SetProperty(ref _headphonesAudioDeviceInformation, value);
        }
    }
}