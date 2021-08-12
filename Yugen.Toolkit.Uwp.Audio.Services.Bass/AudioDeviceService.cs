using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
{
    public class AudioDeviceService : IAudioDeviceService
    {
        public DeviceInformationCollection DeviceInfoCollection => throw new NotImplementedException();

        public DeviceInformation MasterAudioDeviceInformation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public DeviceInformation HeadphonesAudioDeviceInformation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int PrimaryDeviceId { get; private set; } = 3;

        public int SecondaryDeviceId { get; private set; } = 2;

        public double GetMasterVolume()
        {
            throw new NotImplementedException();
        }

        public Task Init()
        {
            var isPrimaryInitialized = ManagedBass.Bass.Init(PrimaryDeviceId);
            var isSecondaryInitialized = ManagedBass.Bass.Init(SecondaryDeviceId);
            return Task.CompletedTask;
        }

        public void SetVolume(double volume)
        {
            throw new NotImplementedException();
        }
    }
}