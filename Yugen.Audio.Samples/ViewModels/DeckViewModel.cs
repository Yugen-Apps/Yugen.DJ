using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.System;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Audio.Bpm;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Audio.Samples.ViewModels
{
    public class DeckViewModel : ViewModelBase
    {
        private IBPMService _bpmService;
        private WaveformViewModel _waveformViewModel;
        private double _bpm;

        public DeckViewModel(
            IBPMService bpmService,
            WaveformViewModel waveformViewModel)
        {
            _waveformViewModel = waveformViewModel;
            _bpmService = bpmService;

            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
        }

        public ICommand OpenCommand { get; }

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
                var stream = await audioFile.OpenStreamForReadAsync();

                MemoryStream bpmStream = new MemoryStream();
                await stream.CopyToAsync(bpmStream);

                stream.Position = 0;
                MemoryStream waveformStream = new MemoryStream();
                await stream.CopyToAsync(waveformStream);

                await _waveformViewModel.GenerateAudioData(waveformStream);

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
    }
}
