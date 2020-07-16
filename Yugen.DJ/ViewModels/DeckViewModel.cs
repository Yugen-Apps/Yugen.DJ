using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Yugen.Toolkit.Standard.Extensions;
using Yugen.Toolkit.Standard.Mvvm.ComponentModel;

namespace Yugen.DJ.ViewModels
{
    public class DeckViewModel : ViewModelBase
    {
        private double _crossFader = 0;
        private DeviceInformation _masterAudioDeviceInformation;
        private DeviceInformation _headphonesAudioDeviceInformation;

        public ObservableCollection<DeviceInformation> AudioDeviceInformationCollection { get; set; } = new ObservableCollection<DeviceInformation>();

        public double CrossFader
        {
            get { return _crossFader; }
            set
            {
                Set(ref _crossFader, value);

                var absoluteValue = 20 - (_crossFader + 10);
                var percentace = 100 * absoluteValue / 20;
                VinylLeft.Fader = percentace / 100;

                absoluteValue = _crossFader + 10;
                percentace = 100 * absoluteValue / 20;
                VinylRight.Fader = percentace / 100;
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
            AudioDeviceInformationCollection.AddRange(deviceInfoCollection);

            MasterAudioDeviceInformation = deviceInfoCollection.FirstOrDefault();
            HeadphonesAudioDeviceInformation = deviceInfoCollection.ElementAtOrDefault(1) ?? MasterAudioDeviceInformation;

            VinylLeft.Init(MasterAudioDeviceInformation, HeadphonesAudioDeviceInformation);
            VinylRight.Init(MasterAudioDeviceInformation, HeadphonesAudioDeviceInformation);
        }
    }
}