using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Yugen.DJ.Interfaces;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.DJ.Services
{
    public class SongService : ISongService
    {
        public StorageFile AudioFile { get; private set; }

        public MusicProperties MusicProperties { get; private set; }

        public async Task LoadFile()
        {
            AudioFile = await FilePickerHelper.OpenFile(
                    new List<string> { ".mp3" },
                    PickerLocationId.MusicLibrary
                );

            if (AudioFile != null)
            {
                MusicProperties = await AudioFile.Properties.GetMusicPropertiesAsync();
            }
        }
    }
}