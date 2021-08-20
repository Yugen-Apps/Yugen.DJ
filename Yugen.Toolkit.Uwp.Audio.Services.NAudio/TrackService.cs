using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Toolkit.Uwp.Audio.Services.NAudio
{
    public class TrackService : ITrackService
    {
        public StorageFile AudioFile { get; private set; }

        public Task<byte[]> AudioBytes => AudioFile?.ReadBytesAsync() ?? null;

        public MusicProperties MusicProperties { get; private set; }

        public async Task<bool> LoadFile()
        {
            AudioFile = await FilePickerHelper.OpenFile(".mp3", PickerLocationId.MusicLibrary);

            if (AudioFile != null)
            {
                MusicProperties = await AudioFile.Properties.GetMusicPropertiesAsync();
                return true;
            }

            return false;
        }
    }
}