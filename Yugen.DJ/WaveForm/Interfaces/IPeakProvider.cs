using NAudio.Wave;
using Yugen.DJ.WaveForm.Models;

namespace Yugen.DJ.WaveForm.Interfaces
{
    public interface IPeakProvider
    {
        void Init(ISampleProvider reader, int samplesPerPixel);
        PeakInfo GetNextPeak();
    }
}