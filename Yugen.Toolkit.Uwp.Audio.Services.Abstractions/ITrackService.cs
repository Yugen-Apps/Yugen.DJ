using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface ITrackService
    {
        StorageFile AudioFile { get; }

        MusicProperties MusicProperties { get; }

        Task<byte[]> GetAudioBytes();

        Task<bool> LoadFile();
    }
}