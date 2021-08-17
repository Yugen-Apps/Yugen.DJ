using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.FileProperties;
using Windows.System;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class DeckViewModel : ViewModelBase
    {
        private readonly IDockService _dockService;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public virtual Side Side { get; }

        private bool _isSongLoaded;
        private bool _isPaused = true;
        private string _playPauseButton = "\uE768";
        private float _bpm;
        private float _rms;
        private double _tempo;
        private string _artist;
        private string _title;
        private TimeSpan _naturalDuration = new TimeSpan();
        private TimeSpan _position = new TimeSpan();
        private List<(float min, float max)> _peakList;

        public DeckViewModel(IDockService dockService)
        {
            _dockService = dockService;
            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);

            _dockService.Init(Side);

            _dockService.PositionChanged += OnDockServicePositionChanged;
            _dockService.AudioPropertiesLoaded += OnDockServiceAudioPropertiesLoaded;
            _dockService.WaveformGenerated += OnDockServiceWaveformGenerated;
            _dockService.BpmGenerated += OnDockServiceBpmGenerated;
        }

        public ICommand OpenCommand { get; }

        public bool IsSongLoaded
        {
            get { return _isSongLoaded; }
            set { SetProperty(ref _isSongLoaded, value); }
        }

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (SetProperty(ref _isPaused, value))
                {
                    _dockService.TogglePlay(_isPaused);

                    PlayPauseButton = _isPaused ? "\uE768" : "\uE769";
                }
            }
        }
        public string PlayPauseButton
        {
            get { return _playPauseButton; }
            set { SetProperty(ref _playPauseButton, value); }
        }

        public double Tempo
        {
            get => _tempo;
            set
            {
                if (SetProperty(ref _tempo, value))
                {
                    _dockService.ChangePitch(_tempo);
                }
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

        public float Bpm
        {
            get => _bpm;
            set => SetProperty(ref _bpm, value);
        }

        public float Rms
        {
            get => _rms;
            set => SetProperty(ref _rms, value);
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
            get => _position;
            set => SetProperty(ref _position, value);
        }

        private void OnDockServicePositionChanged(object sender, TimeSpan e)
        {
            _dispatcherQueue.EnqueueAsync(() =>
            {
                Position = e;
                Rms = _dockService.GetRms();
            });
        }

        private void OnDockServiceAudioPropertiesLoaded(object sender, MusicProperties e)
        {
            IsSongLoaded = true;

            NaturalDuration = _dockService.NaturalDuration;

            Artist = e?.Artist;
            Title = e?.Title;
        }

        private void OnDockServiceWaveformGenerated(object sender, List<(float min, float max)> e)
        {
            _dispatcherQueue.EnqueueAsync(() =>
            {
                PeakList = e;
            });
        }

        private void OnDockServiceBpmGenerated(object sender, float e)
        {
            _dispatcherQueue.EnqueueAsync(() =>
            {
                Bpm = e;
            });
        }

        private Task OpenCommandBehavior() => _dockService.LoadSong();

    }
}