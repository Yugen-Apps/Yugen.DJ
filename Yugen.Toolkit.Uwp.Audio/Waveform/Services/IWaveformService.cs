using NAudio.Wave;
using System.Collections.Generic;
using System.IO;
using Yugen.Toolkit.Uwp.Audio.Waveform.Models;

namespace Yugen.Toolkit.Uwp.Audio.Waveform.Services
{
    public interface IWaveformService
    {
        WaveformRendererSettings Settings { get; }

        List<(float min, float max)> Render(ISampleProvider isp, long samples);

        List<(float min, float max)> Render(Stream stream);
    }
}