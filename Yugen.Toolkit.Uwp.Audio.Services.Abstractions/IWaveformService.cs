using System.Collections.Generic;
using System.IO;

namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IWaveformService
    {
        List<(float min, float max)> Render(Stream stream);
    }
}