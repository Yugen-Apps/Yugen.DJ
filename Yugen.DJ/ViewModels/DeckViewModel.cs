﻿using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Yugen.DJ.DependencyInjection;
using Yugen.DJ.Interfaces;
using Yugen.Toolkit.Standard.Extensions;
using Yugen.Toolkit.Standard.Mvvm.ComponentModel;

namespace Yugen.DJ.ViewModels
{
    public class DeckViewModel : ViewModelBase
    {
        private readonly IAudioDeviceService _audioDeviceService;
        private double _crossFader = 0;
        private DeviceInformation _masterAudioDeviceInformation;
        private DeviceInformation _headphonesAudioDeviceInformation;

        public DeckViewModel()
        {
            _audioDeviceService = Ioc.Default.GetService<IAudioDeviceService>();
        }

        public ObservableCollection<DeviceInformation> AudioDeviceInformationCollection { get; set; } = new ObservableCollection<DeviceInformation>();

        public double CrossFader
        {
            get { return _crossFader; }
            set
            {
                Set(ref _crossFader, value);

                SetFader();
            }
        }

        public DeviceInformation MasterAudioDeviceInformation
        {
            get { return _masterAudioDeviceInformation; }
            set
            {
                Set(ref _masterAudioDeviceInformation, value);

                _audioDeviceService.MasterAudioDeviceInformation = _masterAudioDeviceInformation;
            }
        }

        public DeviceInformation HeadphonesAudioDeviceInformation
        {
            get { return _headphonesAudioDeviceInformation; }
            set
            {
                Set(ref _headphonesAudioDeviceInformation, value);

                _audioDeviceService.HeadphonesAudioDeviceInformation = _headphonesAudioDeviceInformation;
            }
        }

        public VinylViewModel VinylLeft { get; set; } = new VinylViewModel(true);

        public VinylViewModel VinylRight { get; set; } = new VinylViewModel(false);

        public async Task LoadAudioDevces()
        {
            await _audioDeviceService.Init();
            AudioDeviceInformationCollection.AddRange(_audioDeviceService.DeviceInfoCollection);

            MasterAudioDeviceInformation = _audioDeviceService.MasterAudioDeviceInformation;
            HeadphonesAudioDeviceInformation = _audioDeviceService.HeadphonesAudioDeviceInformation;

            await VinylLeft.Init();
            await VinylRight.Init();

            SetFader();
        }

        private void SetFader()
        {
            var absoluteValue = 20 - (_crossFader + 10);
            var percentace = 100 * absoluteValue / 20;
            VinylLeft.Fader = percentace / 100;

            absoluteValue = _crossFader + 10;
            percentace = 100 * absoluteValue / 20;
            VinylRight.Fader = percentace / 100;
        }
    }
}