using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.System;
using Yugen.Audio.Samples.Services;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Audio.Services.Common.Helpers;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Audio.Samples.ViewModels
{
    public class BassViewModel : ViewModelBase
    {
        private BassPlayer _audioPlayer = new BassPlayer();
        private Timer _progressBarTimer;
        private double _position;

        private double _volume = 1;
        private double _pitch;
        private double _tempo;

        private bool _isPlaying;
        private string _fileName1;
        private string _fileName2;
        private bool _isHeadphones;

        public BassViewModel()
        {
            OnLoadCommand = new RelayCommand(OnLoadCommandBehavior);
            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
            Open2Command = new AsyncRelayCommand(Open2CommandBehavior);
            Play2Command = new RelayCommand(Play2CommandBehavior);
            StopCommand = new RelayCommand(StopCommandBehavior);

            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _progressBarTimer = new Timer(100);
            _progressBarTimer.Elapsed += (s, e) =>
            {
                //if (!IsDragging)
                dispatcherQueue.EnqueueAsync(() =>
                {
                    OnPropertyChanged(nameof(Bpm));
                    OnPropertyChanged(nameof(Position));
                    OnPropertyChanged(nameof(Rms));
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

        public float Bpm => _audioPlayer.Bpm;

        public float Rms => _audioPlayer.Rms;

        /// <summary>
        /// Gets or Sets the Volume (0 ... 1.0).
        /// </summary>
        public double Volume
        {
            get => _volume;
            set
            {
                if (SetProperty(ref _volume, value))
                {
                    _audioPlayer.Volume = value;
                }
            }
        }

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

        public ICommand Open2Command { get; }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (SetProperty(ref _isPlaying, value))
                {
                    if (_isPlaying)
                    {
                        _audioPlayer.Play();
                        _progressBarTimer.Start();
                    }
                    else
                    {
                        _audioPlayer.Pause();
                        _progressBarTimer.Stop();
                    }
                }
            }
        }

        public bool IsHeadphones
        {
            get => _isHeadphones;
            set
            {
                if (SetProperty(ref _isHeadphones, value))
                {
                    _audioPlayer.Heaphones(_isHeadphones);
                }
            }
        }

        public string FileName1
        {
            get => _fileName1;
            set => SetProperty(ref _fileName1, value);
        }

        public string FileName2
        {
            get => _fileName2;
            set => SetProperty(ref _fileName2, value);
        }

        public ICommand Play2Command { get; }

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
                FileName1 = audioFile.Name;
            }
        }

        private async Task Open2CommandBehavior()
        {
            var audioFile = await FilePickerHelper.OpenFile(
                    new List<string> { ".mp3" },
                    PickerLocationId.MusicLibrary
                );

            if (audioFile != null)
            {
                var bytes = await audioFile.ReadBytesAsync();
                await _audioPlayer.Load2(bytes);
                FileName2 = audioFile.Name;
            }
        }

        private void Play2CommandBehavior()
        {
            _audioPlayer.Play2();
        }

        private void StopCommandBehavior() => _audioPlayer.Stop();
    }
}