using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Yugen.Toolkit.Standard.Mvvm.ComponentModel;
using Yugen.Toolkit.Standard.Mvvm.Input;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.DJ.ViewModels
{
    public class VinylViewModel : ViewModelBase
    {
        public int[] TargetElapsedTimeList = {
            10000,
            25000,
            50000,
            75000,
            100000,
            250000,
            500000,
            750000
        };

        private readonly MediaPlayer _mediaPlayer = new MediaPlayer();
        private bool _isPaused = true;
        private double _volume = 100;
        private double _pitch;
        private TimeSpan _targetElapsedTime = new TimeSpan(10000);
        private ICommand _openButtonCommand;

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                Set(ref _isPaused, value);

                if (_isPaused)
                {
                    _mediaPlayer.Pause();
                }
                else
                {
                    _mediaPlayer.Play();
                }
            }
        }

        public double Volume
        {
            get { return _volume; }
            set
            {
                Set(ref _volume, value);

                _mediaPlayer.Volume = _volume / 100;
            }
        }

        public double Pitch
        {
            get { return _pitch; }
            set
            {
                Set(ref _pitch, value);

                _mediaPlayer.PlaybackSession.PlaybackRate = 1 + _pitch / 100;
            }
        }

        public TimeSpan TargetElapsedTime
        {
            get { return _targetElapsedTime; }
            set { Set(ref _targetElapsedTime, value); }
        }

        public int SelectedTargetElapsedTime
        {
            get => (int)TargetElapsedTime.Ticks;
            set => TargetElapsedTime = new TimeSpan(value);
        }

        public ICommand OpenButtonCommand => _openButtonCommand ?? (_openButtonCommand = new AsyncRelayCommand(OpenButtonCommandBehavior));

        private async Task OpenButtonCommandBehavior()
        {
            StorageFile masterFile = await FilePickerHelper.OpenFile(
                new List<string> { ".mp3" },
                Windows.Storage.Pickers.PickerLocationId.MusicLibrary);

            using (var inputStream = await masterFile.OpenReadAsync())
            {
                _mediaPlayer.Source = MediaSource.CreateFromStream(inputStream, masterFile.ContentType);
            }
        }
    }
}