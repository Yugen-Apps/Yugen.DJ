using NAudio.Wave;
using System.Collections.Generic;
using System.IO;
using Yugen.Toolkit.Uwp.Audio.Waveform.Models;

namespace Yugen.Toolkit.Uwp.Audio.Waveform.Services
{
    public interface IWaveformService
    {
        WaveformRendererSettings Settings { get; }

        List<PeakInfo> PeakList { get; }

        void Render(ISampleProvider isp, long samples);
        void Render(Stream stream);
    }
}