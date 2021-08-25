using ManagedBass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
{
    public class AudioDeviceService : IAudioDeviceService
    {
        private readonly List<DeviceInfo> _deviceList = new List<DeviceInfo>();

        public DeviceInformationCollection DeviceInfoCollection => throw new NotImplementedException();

        public DeviceInformation MasterAudioDeviceInformation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public DeviceInformation HeadphonesAudioDeviceInformation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int PrimaryDeviceId { get; private set; } = -1;

        public int SecondaryDeviceId { get; private set; } = 0;

        public Task Init()
        {
            for (int i = 0; i < ManagedBass.Bass.DeviceCount; i++)
            {
                var device = ManagedBass.Bass.GetDeviceInfo(i);
                _deviceList.Add(device);
            }

            DeviceInfo? secondaryDevice = _deviceList.FirstOrDefault(x => !x.IsDefault && x.Driver != null);
            if (secondaryDevice != null)
            {
                SecondaryDeviceId = _deviceList.IndexOf(secondaryDevice.Value);
            }
            var isSecondaryInitialized = ManagedBass.Bass.Init(SecondaryDeviceId);

            DeviceInfo? primaryDevice = _deviceList.FirstOrDefault(x => x.IsDefault && x.Driver != null);
            if(primaryDevice != null)
            {
                PrimaryDeviceId = _deviceList.IndexOf(primaryDevice.Value);
            }
            var isPrimaryInitialized = ManagedBass.Bass.Init(PrimaryDeviceId);

            return Task.CompletedTask;
        }

        public double GetMasterVolume() => ManagedBass.Bass.Volume * 100;

        public void SetVolume(double volume) => ManagedBass.Bass.Volume = volume / 100;
    }
}