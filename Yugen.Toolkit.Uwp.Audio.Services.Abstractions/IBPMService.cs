using System.IO;

namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IBPMService
    {
        float BPM { get; }

        float Decoding(Stream stream);

        float Decoding(byte[] audioBytes);
    }
}