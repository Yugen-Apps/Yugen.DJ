using AudioVisualizer;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.ViewModels
{
    public class DeckViewModel : ObservableObject
    {
        private readonly IDockService _dockService;

        private Side _side;

        private bool _isSongLoaded;
        private bool _isPaused = true;
        private string _playPauseButton = "\uE768";
        private double _pitch = 0;

        private string _artist;
        private string _title;
        private int _bpm;

        private DispatcherQueue _dispatcherQueue;
        private TimeSpan _naturalDuration = new TimeSpan();
        private TimeSpan _position = new TimeSpan();
        private List<(float min, float max)> _peakList;

        public DeckViewModel(IDockService dockService)
        {
            _dockService = dockService;

            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
        }

        public VUBarVieModel VUBarVieModel { get; set; } = new VUBarVieModel();

        public Side Side
        {
            get { return _side; }
            set
            {
                SetProperty(ref _side, value);

                _dockService.Init(_side);

                _dockService.AudioPropertiesLoaded += AudioServiceOnAudioPropertiesLoaded;
                _dockService.PositionChanged += AudioServiceOnPositionChanged;
                _dockService.BpmGenerated += OnBpmGenerated;
                _dockService.WaveformGenerated += OnWaveformGenerated;
            }
        }

        public bool IsSongLoaded
        {
            get { return _isSongLoaded; }
            set { SetProperty(ref _isSongLoaded, value); }
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                SetProperty(ref _isPaused, value);

                _dockService.TogglePlay(_isPaused);

                PlayPauseButton = _isPaused ? "\uE768" : "\uE769";
            }
        }

        public string PlayPauseButton
        {
            get { return _playPauseButton; }
            set { SetProperty(ref _playPauseButton, value); }
        }

        public double Pitch
        {
            get { return _pitch; }
            set
            {
                SetProperty(ref _pitch, value);

                _dockService.ChangePitch(_pitch);
            }
        }

        public string Artist
        {
            get { return _artist; }
            set { SetProperty(ref _artist, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public int BPM
        {
            get { return _bpm; }
            set { SetProperty(ref _bpm, value); }
        }

        public List<(float min, float max)> PeakList
        {
            get => _peakList;
            set => SetProperty(ref _peakList, value);
        }

        public TimeSpan NaturalDuration
        {
            get { return _naturalDuration; }
            set { SetProperty(ref _naturalDuration, value); }
        }

        public TimeSpan Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        public ICommand OpenCommand { get; }

        private async Task OpenCommandBehavior()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            IsPaused = true;

            await _dockService.LoadSong();

            SetPlaybackSource();

            _dockService.TogglePlay(IsPaused);
        }

        private void SetPlaybackSource()
        {
            if (_dockService.MasterFileInput != null)
            {
                VUBarVieModel.Source = PlaybackSource.CreateFromAudioNode(_dockService.MasterFileInput)?.Source;
            }
        }

        private void AudioServiceOnAudioPropertiesLoaded(object sender, EventArgs e)
        {
            IsSongLoaded = true;

            NaturalDuration = _dockService.NaturalDuration;

            Artist = _dockService.MusicProperties?.Artist;
            Title = _dockService.MusicProperties?.Title;
        }

        private void AudioServiceOnPositionChanged(object sender, TimeSpan e)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                Position = e;
            });
        }

        private void OnBpmGenerated(object sender, double e)
        {
            _ = _dispatcherQueue.EnqueueAsync(() =>
              {
                  BPM = (int)e;
              });
        }

        private void OnWaveformGenerated(object sender, EventArgs e)
        {
            _ = _dispatcherQueue.EnqueueAsync(() =>
            {
                PeakList = _dockService.PeakList;
            });
        }
    }
}