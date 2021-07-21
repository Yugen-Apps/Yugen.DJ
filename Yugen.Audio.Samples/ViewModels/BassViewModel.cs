using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Yugen.Audio.Samples.Helpers;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Services;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Audio.Samples.ViewModels
{
    public class BassViewModel : ViewModelBase
    {
        private BassPlayer _audioPlayer = new BassPlayer();
        private Timer _progressBarTimer;
        private double _position;

        public BassViewModel()
        {
            OnLoadCommand = new RelayCommand(OnLoadCommandBehavior);
            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
            PlayCommand = new RelayCommand(PlayCommandBehavior);
            StopCommand = new RelayCommand(StopCommandBehavior);

            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _progressBarTimer = new Timer(100);
            _progressBarTimer.Elapsed += (s, e) =>
            {
                //if (!IsDragging)
                dispatcherQueue.EnqueueAsync(() =>
                {
                    OnPropertyChanged(nameof(Position));
                });
            };
        }

        public double Position
        {
            get => _audioPlayer.Position.TotalSeconds;
            set
            {
                if (SetProperty(ref _position, value))
                {
                    _audioPlayer.Position = TimeSpan.FromSeconds(value);
                }
            }
        }
        double _pitch;

        /// <summary>
        /// Gets or Sets the Pitch in Semitones (-60 ... 0 ... 60).
        /// </summary>
        public double Pitch
        {
            get => _pitch;
            set
            {
                if (SetProperty(ref _pitch, value)) 
                {
                    _audioPlayer.Pitch = value;
                }
            }
        }
        double _tempo;

        /// <summary>
        /// Gets or Sets the Tempo in Percentage (-95% ... 0 ... 5000%)
        /// </summary>
        public double Tempo
        {
            get => _tempo;
            set
            {
                if (SetProperty(ref _tempo, value))
                {
                    _audioPlayer.Tempo = value;
                }
            }
        }

        public ICommand OnLoadCommand { get; }
        
        public ICommand OpenCommand { get; }
        
        public ICommand PlayCommand { get; }
        
        public ICommand StopCommand { get; }

        public void OnLoadCommandBehavior()
        {
            _audioPlayer.Initialize(AudioDevicesHelper.MasterAudioDeviceInformation.Id);
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
            }
        }

        private void PlayCommandBehavior()
        {
            _audioPlayer.Play();
            _progressBarTimer.Start();
        }

        private void StopCommandBehavior() => _audioPlayer.Stop();
    }
}