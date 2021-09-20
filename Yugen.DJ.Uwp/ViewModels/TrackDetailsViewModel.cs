using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using Windows.Storage.FileProperties;
using Windows.System;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class TrackDetailsViewModel : ObservableObject
    {
        private readonly IDockServiceProvider _dockServiceProvider;
        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private IDockService _dockService;

        private Side _side;
        private float _bpm;
        private string _artist;
        private string _title;
        private TimeSpan _naturalDuration = new TimeSpan();
        private TimeSpan _position = new TimeSpan();
        private List<(float min, float max)> _peakList;

        public TrackDetailsViewModel(IDockServiceProvider dockServiceProvider)
        {
            _dockServiceProvider = dockServiceProvider;
        }

        public Side Side
        {
            get => _side;
            set
            {
                _side = value;
                _dockService = _dockServiceProvider.Get(_side);

                _dockService.PositionChanged += OnDockServicePositionChanged;
                _dockService.AudioPropertiesLoaded += OnDockServiceAudioPropertiesLoaded;
                _dockService.WaveformGenerated += OnDockServiceWaveformGenerated;
                _dockService.BpmGenerated += OnDockServiceBpmGenerated;
            }
        }

        public string Artist
        {
            get => _artist;
            set => SetProperty(ref _artist, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public float Bpm
        {
            get => _bpm;
            set => SetProperty(ref _bpm, value);
        }

        public List<(float min, float max)> PeakList
        {
            get => _peakList;
            set => SetProperty(ref _peakList, value);
        }

        public TimeSpan NaturalDuration
        {
            get => _naturalDuration;
            set => SetProperty(ref _naturalDuration, value);
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
            });
        }

        private void OnDockServiceAudioPropertiesLoaded(object sender, MusicProperties e)
        {
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
    }
}