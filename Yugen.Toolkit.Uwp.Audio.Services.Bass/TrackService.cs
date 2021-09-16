using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
{
    public class TrackService : ITrackService
    {
        public StorageFile AudioFile { get; private set; }

        public async Task<byte[]> GetAudioBytes()
        {
            if (AudioFile == null)
            {
                return null;
            }

            byte[] audioBytes;
            using (Stream stream = await AudioFile.OpenStreamForReadAsync())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    audioBytes = ms.ToArray();
                }
            }

            return audioBytes;
        }

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