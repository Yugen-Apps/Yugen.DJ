using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.System;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Services;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Audio.Samples.ViewModels
{
    public class DeckViewModel : ViewModelBase
    {
        private readonly IAudioPlayer _audioPlayer = new BassPlayer();

        private IBPMService _bpmService;
        private readonly IWaveformService _waveformService;
        private double _bpm;
        private List<(float min, float max)> _peakList;

        public DeckViewModel(
            IBPMService bpmService,
            IWaveformService waveformRendererService)
        {
            _bpmService = bpmService;
            _waveformService = waveformRendererService;

            _audioPlayer.Initialize("");

            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
            PlayCommand = new RelayCommand(PlayCommandBehavior);
        }

        public ICommand OpenCommand { get; }

        public ICommand PlayCommand { get; }

        public double Bpm
        {
            get => _bpm;
            set => SetProperty(ref _bpm, value);
        }

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
                await _audioPlayer.Load(bytes);


                var stream = await audioFile.OpenStreamForReadAsync();

                //MemoryStream fileStream = new MemoryStream();
                //await stream.CopyToAsync(fileStream);
                //await _audioPlayer.LoadStream(fileStream);
                //stream.Position = 0;

                MemoryStream waveformStream = new MemoryStream();
                await stream.CopyToAsync(waveformStream);
                await GenerateAudioData(waveformStream);
                stream.Position = 0;

                MemoryStream bpmStream = new MemoryStream();
                await stream.CopyToAsync(bpmStream);
                var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
                await Task.Run(() =>
                {
                    dispatcherQueue.EnqueueAsync(() =>
                    {
                        Bpm = _bpmService.Decoding(bpmStream);
                    });
                });
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

        private void PlayCommandBehavior()
        {
            _audioPlayer.Play();
        }
    }
}
