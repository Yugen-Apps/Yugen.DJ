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
using Yugen.Toolkit.Uwp.Audio.Bpm;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Audio.Samples.ViewModels
{
    public class DeckViewModel : ViewModelBase
    {
        private readonly IAudioPlayer _audioPlayer = new BassPlayer();

        private IBPMService _bpmService;
        private WaveformViewModel _waveformViewModel;
        private double _bpm;

        public DeckViewModel(
            IBPMService bpmService,
            WaveformViewModel waveformViewModel)
        {
            _waveformViewModel = waveformViewModel;
            _bpmService = bpmService;

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
                await _waveformViewModel.GenerateAudioData(waveformStream);
                stream.Position = 0;

                MemoryStream bpmStream = new MemoryStream();
                await stream.CopyToAsync(bpmStream);
                var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
                await Task.Run(() =>
                {
                    dispatcherQueue.EnqueueAsync(() =>
                    {
                        Bpm = _bpmService.Detect(bpmStream);
                    }); 
                });
            }
        }

        private void PlayCommandBehavior()
        {
            _audioPlayer.Play();
        }
    }
}
