using NAudio.Wave;
using Yugen.DJ.Audio.Waveform.Models;

namespace Yugen.DJ.Audio.Waveform.Interfaces
{
    public interface IPeakProvider
    {
        void Init(ISampleProvider reader, int samplesPerPixel);

        PeakInfo GetNextPeak();
    }
}