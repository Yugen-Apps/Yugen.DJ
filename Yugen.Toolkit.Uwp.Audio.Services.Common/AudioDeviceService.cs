using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Devices;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;
using Yugen.Toolkit.Uwp.Audio.Services.Common.SystemVolume;

namespace Yugen.Toolkit.Uwp.Audio.Services.Common
{
    public class AudioDeviceService : IAudioDeviceService
    {
        public DeviceInformationCollection DeviceInfoCollection { get; private set; }

        public DeviceInformation MasterAudioDeviceInformation { get; set; }

        public DeviceInformation HeadphonesAudioDeviceInformation { get; set; }

        public int PrimaryDeviceId => throw new NotImplementedException();

        public int SecondaryDeviceId => throw new NotImplementedException();

        public double GetMasterVolume() => SystemVolumeHelper.GetVolume();

        public void SetVolume(double volume) => SystemVolumeHelper.SetVolume(volume / 100);

        public async Task Init()
        {
            var defaultAudioDeviceId = MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Default);
            DeviceInfoCollection = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender);

            MasterAudioDeviceInformation = DeviceInfoCollection.FirstOrDefault(
                x => x.Id.Equals(defaultAudioDeviceId));

            HeadphonesAudioDeviceInformation = DeviceInfoCollection.FirstOrDefault(
                x => !x.Id.Equals(defaultAudioDeviceId));
        }
    }
}