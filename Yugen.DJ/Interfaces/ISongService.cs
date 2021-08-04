using NAudio.Wave;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Yugen.DJ.Interfaces
{
    public interface ISongService
    {
        StorageFile AudioFile { get; }

        MusicProperties MusicProperties { get; }

        Task LoadFile();
    }
}