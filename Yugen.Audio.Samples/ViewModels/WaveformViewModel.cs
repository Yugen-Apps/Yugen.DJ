using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Audio.Waveform.Services;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Audio.Samples.ViewModels
{
    public class WaveformViewModel : ViewModelBase
    {
        private readonly IWaveformService _waveformService;
        private List<(float min, float max)> _peakList;

        public WaveformViewModel(IWaveformService waveformRendererService)
        {
            _waveformService = waveformRendererService;

            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
        }

        public ICommand OpenCommand { get; }

        public List<(float min, float max)> PeakList
        {
            get => _peakList;
            set => SetProperty(ref _peakList, value);
        }

        private async Task OpenCommandBehavior()
        {
            var audioFile = await FilePickerHelper.OpenFile(
                     new List<string> { ".mp3" },
                     PickerLocationId.MusicLibrary
                 );

            if (audioFile != null)
            {
                var bytes = await audioFile.ReadBytesAsync();

                var stream = await audioFile.OpenStreamForReadAsync();

                MemoryStream waveformStream = new MemoryStream();
                await stream.CopyToAsync(waveformStream);
                await GenerateAudioData(waveformStream);
                stream.Position = 0;
            }
        }

        public async Task GenerateAudioData(Stream stream)
        {
            List<(float min, float max)> peakList = null;

            await Task.Run(() =>
            {
                peakList = _waveformService.Render(stream);
            });

            PeakList = peakList;
        }
    }
}
