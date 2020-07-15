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
        private bool _isPaused = true;
        private double _volume = 100;
        private double _pitch = 0;
        private TimeSpan _targetElapsedTime = new TimeSpan(10000);
        private ICommand _openButtonCommand;
        private DeviceInformation _selectedAudioDeviceInformation;

        public VinylViewModel(bool isLeft)
        {
            _isLeft = isLeft;
        }

        public ObservableCollection<DeviceInformation> AudioDeviceInformationCollection { get; set; } = new ObservableCollection<DeviceInformation>();

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

        public DeviceInformation SelectedAudioDeviceInformation
        {
            get { return _selectedAudioDeviceInformation; }
            set
            {
                Set(ref _selectedAudioDeviceInformation, value);

                if (_selectedAudioDeviceInformation != null)
                {
                    _mediaPlayer.AudioDevice = _selectedAudioDeviceInformation;
                }
            }
        }

        public ICommand OpenButtonCommand => _openButtonCommand ?? (_openButtonCommand = new AsyncRelayCommand(OpenButtonCommandBehavior));

        public async Task LoadAudioDevces()
        {
            DeviceInformationCollection deviceInfoCollection = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender);

            foreach (var deviceInfo in deviceInfoCollection)
            {
                AudioDeviceInformationCollection.Add(deviceInfo);
            }
        }

        private async Task OpenButtonCommandBehavior()
        {
            StorageFile masterFile = await FilePickerHelper.OpenFile(
                new List<string> { ".mp3" },
                Windows.Storage.Pickers.PickerLocationId.MusicLibrary);

            _mediaPlayer.Source = MediaSource.CreateFromStorageFile(masterFile);
        }
    }
}