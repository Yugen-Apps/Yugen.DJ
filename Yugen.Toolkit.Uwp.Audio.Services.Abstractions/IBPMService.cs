using System.IO;

namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IBPMService
    {
        double BPM { get; }

        double Decoding(Stream stream);

        double Decoding(byte[] audioBytes);
    }
}