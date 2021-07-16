using NAudio.Wave;
using Yugen.Toolkit.Uwp.Audio.Waveform.Interfaces;
using Yugen.Toolkit.Uwp.Audio.Waveform.Models;

namespace Yugen.Toolkit.Uwp.Audio.Waveform.Services
{
    public interface IWaveformService
    {
        WaveformRendererSettings Settings { get; }

        IPeakProvider PeakProvider { get; }

        void Render(ISampleProvider isp, long samples);
    }
}