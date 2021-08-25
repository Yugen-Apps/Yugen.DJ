using System.Collections.Generic;
using System.IO;

namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IWaveformService
    {
        List<(float min, float max)> GenerateAudioData(Stream stream);

        List<(float min, float max)> GenerateAudioData(byte[] audioBytes);
    }
}