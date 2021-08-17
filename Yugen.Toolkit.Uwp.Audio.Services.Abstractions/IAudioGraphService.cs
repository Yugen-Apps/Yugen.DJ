using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Storage;

namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IAudioGraphService
    {
        event EventHandler<TimeSpan> PositionChanged;

        AudioFileInputNode AudioFileInput { get; }

        Task AddFileToDevice(StorageFile audioFile);

        void ChangePitch(double ratio);

        void ChangeVolume(double volume);

        void DisposeFileInputs();

        Task InitDevice(DeviceInformation audioDeviceInformation, bool isMaster);

        void IsHeadphones(bool isHeadphone);

        void TogglePlay(bool isPaused);
    }
}