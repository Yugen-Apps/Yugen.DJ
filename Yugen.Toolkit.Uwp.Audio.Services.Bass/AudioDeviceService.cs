using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
{
    public class AudioDeviceService : IAudioDeviceService
    {
        public List<AudioDevice> AudioDeviceList { get; } = new List<AudioDevice>();

        public AudioDevice PrimaryDevice { get; set; } = new AudioDevice{ Id = -1 };

        public AudioDevice SecondaryDevice { get; set; } = new AudioDevice { Id = 0 };

        public Task Init()
        {
            for (var i = 0; YugenBass.GetDeviceInfo(i, out var deviceInfo); ++i)
            {
                if (!string.IsNullOrEmpty(deviceInfo.Driver))
                {
                    AudioDeviceList.Add(new AudioDevice
                    {
                        Driver = deviceInfo.Driver,
                        Id = i,
                        IsDefault = deviceInfo.IsDefault,
                        Name = deviceInfo.Name
                    });
                }
            }

            var secondaryDevice = AudioDeviceList.FirstOrDefault(x => !x.IsDefault);
            if (secondaryDevice != null && secondaryDevice.Id != 0)
            {
                SecondaryDevice = secondaryDevice;
            }
            var isSecondaryInitialized = ManagedBass.Bass.Init(SecondaryDevice.Id);

            var primaryDevice = AudioDeviceList.FirstOrDefault(x => x.IsDefault);
            if (primaryDevice != null && primaryDevice.Id != 0)
            {
                PrimaryDevice = primaryDevice;
            }
            var isPrimaryInitialized = ManagedBass.Bass.Init(PrimaryDevice.Id);

            return Task.CompletedTask;
        }

        public double GetMasterVolume() => ManagedBass.Bass.Volume * 100;

        public void SetVolume(double volume) => ManagedBass.Bass.Volume = volume / 100;
    }
}