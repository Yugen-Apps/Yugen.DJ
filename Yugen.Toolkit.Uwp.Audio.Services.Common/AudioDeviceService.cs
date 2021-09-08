using System;
using System.Collections.Generic;
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
        public List<AudioDevice> AudioDeviceList { get; } = new List<AudioDevice>();

        public AudioDevice PrimaryDevice { get; set; }

        public AudioDevice SecondaryDevice { get; set; }

        public double GetMasterVolume() => SystemVolumeHelper.GetVolume();

        public void SetVolume(double volume) => SystemVolumeHelper.SetVolume(volume / 100);

        public async Task Init()
        {
            var defaultAudioDeviceDriver = MediaDevice.GetDefaultAudioRenderId(AudioDeviceRole.Default);
            var deviceInfoList = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender);

            foreach (var deviceInfo in deviceInfoList)
            {
                AudioDeviceList.Add(new AudioDevice
                {
                    Driver = deviceInfo.Id,
                    IsDefault = deviceInfo.IsDefault,
                    Name = deviceInfo.Name
                });
            }

            PrimaryDevice = AudioDeviceList.FirstOrDefault(
                x => x.Driver.Equals(defaultAudioDeviceDriver));

            SecondaryDevice = AudioDeviceList.FirstOrDefault(
                x => !x.Driver.Equals(defaultAudioDeviceDriver));
        }
    }
}