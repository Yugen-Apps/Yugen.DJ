using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IAudioDeviceService
    {
        List<AudioDevice> AudioDeviceList { get; }

        AudioDevice PrimaryDevice { get; set; }

        AudioDevice SecondaryDevice { get; set; }

        double GetMasterVolume();

        void SetVolume(double volume);

        Task Init();
    }
}