using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Enumeration;
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
        private bool _isLeft;
        private bool _isHeadPhones;
        private bool _isPaused = true;
        private double _volume = 100;
        private double _fader = 0;
        private double _pitch = 0;
        private TimeSpan _targetElapsedTime = new TimeSpan(10000);
        private ICommand _openButtonCommand;
        private DeviceInformation _masterAudioDeviceInformation;
        private DeviceInformation _headphonesAudioDeviceInformation;

        public VinylViewModel(bool isLeft)
        {
            _isLeft = isLeft;
        }

        public void Init(DeviceInformation masterAudioDeviceInformation, DeviceInformation headphonesAudioDeviceInformation)
        {
            _masterAudioDeviceInformation = masterAudioDeviceInformation;
            _headphonesAudioDeviceInformation = headphonesAudioDeviceInformation;
        }

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

        public bool IsHeadPhones
        {
            get { return _isHeadPhones; }
            set
            {
                Set(ref _isHeadPhones, value);

                if (_isHeadPhones)
                {
                    _mediaPlayer.AudioDevice = _headphonesAudioDeviceInformation;
                }
                else
                {
                    _mediaPlayer.AudioDevice = _masterAudioDeviceInformation;
                }
            }
        }

        public double Volume
        {
            get { return _volume; }
            set
            {
                Set(ref _volume, value);

                SetVolume();
            }
        }

        public double Fader
        {
            get { return _fader; }
            set
            {
                Set(ref _fader, value);

                SetVolume();
            }
        }

        public double Pitch
        {
            get { return _pitch; }
            set
            {
                Set(ref _pitch, value);

                var ratio = _pitch == 0 ? 0 : _pitch / 100;
                _mediaPlayer.PlaybackSession.PlaybackRate = 1 + ratio;
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

        public ICommand OpenButtonCommand => _openButtonCommand
            ?? (_openButtonCommand = new AsyncRelayCommand(OpenButtonCommandBehavior));

        private async Task OpenButtonCommandBehavior()
        {
            StorageFile masterFile = await FilePickerHelper.OpenFile(
                new List<string> { ".mp3" },
                Windows.Storage.Pickers.PickerLocationId.MusicLibrary);

            _mediaPlayer.Source = MediaSource.CreateFromStorageFile(masterFile);
        }

        private void SetVolume()
        {
            var volume = _volume * _fader;

            _mediaPlayer.Volume = volume / 100;
        }
    }
}