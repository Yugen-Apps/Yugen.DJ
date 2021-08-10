using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IBPMService
    {
        double BPM { get; }

        double Detect(float[] buffer, int sampleRate, double totalMinutes);

        Task<double> Detect(IStorageFile file);

        double Detect(Stream stream);
    }
}