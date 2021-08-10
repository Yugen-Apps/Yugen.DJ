using NAudio.Wave;
using Yugen.Toolkit.Uwp.Audio.Services.NAudio.Models;

namespace Yugen.Toolkit.Uwp.Audio.Services.NAudio.Interfaces
{
    public interface IPeakProvider
    {
        void Init(ISampleProvider reader, int samplesPerPixel);

        PeakInfo GetNextPeak();
    }
}