using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Yugen.Toolkit.Standard.Mvvm.ComponentModel;

namespace Yugen.DJ.ViewModels
{
    public class DeckViewModel : ViewModelBase
    {
        private double _fader = 0;
        private DeviceInformation _masterAudioDeviceInformation;
        private DeviceInformation _headphonesAudioDeviceInformation;

        public ObservableCollection<DeviceInformation> AudioDeviceInformationCollection { get; set; } = new ObservableCollection<DeviceInformation>();

        public double Fader
        {
            get { return _fader; }
            set
            {
                Set(ref _fader, value);
            }
        }

        public DeviceInformation MasterAudioDeviceInformation
        {
            get { return _masterAudioDeviceInformation; }
            set { Set(ref _masterAudioDeviceInformation, value); }
        }  
        
        public DeviceInformation HeadphonesAudioDeviceInformation
        {
            get { return _headphonesAudioDeviceInformation; }
            set { Set(ref _headphonesAudioDeviceInformation, value); }
        }

        public VinylViewModel VinylLeft { get; set; } = new VinylViewModel(true);

        public VinylViewModel VinylRight { get; set; } = new VinylViewModel(false);

        public async Task LoadAudioDevces()
        {
            DeviceInformationCollection deviceInfoCollection = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender);

            foreach (var deviceInfo in deviceInfoCollection)
            {
                AudioDeviceInformationCollection.Add(deviceInfo);
            }

            MasterAudioDeviceInformation = deviceInfoCollection.FirstOrDefault();
            HeadphonesAudioDeviceInformation = deviceInfoCollection.ElementAtOrDefault(1) ?? MasterAudioDeviceInformation;

            VinylLeft.Init(MasterAudioDeviceInformation, HeadphonesAudioDeviceInformation);
            VinylRight.Init(MasterAudioDeviceInformation, HeadphonesAudioDeviceInformation);
        }
    }
}