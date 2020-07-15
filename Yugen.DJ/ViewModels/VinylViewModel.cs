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