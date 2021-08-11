using ManagedBass;
using ManagedBass.Fx;
using ManagedBass.Mix;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.System;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const int _bpmPeriod = 30;

        private readonly IBPMService _bpmService;
        private readonly ITrackService _trackService;
        private readonly IWaveformService _waveformService;

        private readonly DispatcherQueue _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private readonly Timer _progressBarTimer = new Timer(100);

        private int _primaryDeviceId = 3;
        private int _secondaryDeviceId = 2;

        int _streamHandleLeft;
        int _streamHandleRight;

        private int _primarySplitStreamLeft;
        private int _secondarySplitStreamLeft;
        private bool _isPlayingLeft;
        private double _positionLeft;
        private float _bpmLeft;
        private float _rmsLeft;
        private List<(float min, float max)> _peakList;

        private int _primarySplitStreamRight;
        private int _secondarySplitStreamRight;
        private bool _isPlayingRight;

        public MainViewModel(
            IBPMService bpmService,
            ITrackService trackService, 
            IWaveformService waveformService)
        {
            _bpmService = bpmService;
            _trackService = trackService;
            _waveformService = waveformService;

            var isPrimaryInitialized = Bass.Init(_primaryDeviceId);
            var isSecondaryInitialized = Bass.Init(_secondaryDeviceId);

            OpenLeftCommand = new AsyncRelayCommand(OpenLeftCommandBehavior);
            OpenRightCommand = new AsyncRelayCommand(OpenRightCommandBehavior);

            _progressBarTimer.Elapsed += (s, e) =>
            {
                _dispatcherQueue.EnqueueAsync(() =>
                {
                    var position = TimeSpan.FromSeconds(Bass.ChannelBytes2Seconds(_primarySplitStreamLeft, Bass.ChannelGetPosition(_primarySplitStreamLeft)));
                    PositionLeft = position.TotalSeconds;
                    RmsLeft = GetRms();
                });
            };
            _progressBarTimer.Start();
        }


        public ICommand OpenLeftCommand { get; }

        public ICommand OpenRightCommand { get; }


        public bool IsPlayingLeft
        {
            get => _isPlayingLeft;
            set
            {
                if (SetProperty(ref _isPlayingLeft, value))
                {
                    if (_isPlayingLeft)
                    {
                        Bass.ChannelPlay(_primarySplitStreamLeft);
                    }
                    else
                    {
                        Bass.ChannelPause(_primarySplitStreamLeft);
                    }
                }
            }
        }

        public double PositionLeft
        {
            get => _positionLeft;
            set => SetProperty(ref _positionLeft, value);
        }

        public double VolumeLeft
        {
            get => Bass.ChannelGetAttribute(_primarySplitStreamLeft, ChannelAttribute.Volume);
            set => Bass.ChannelSetAttribute(_primarySplitStreamLeft, ChannelAttribute.Volume, value);
        }

        public double TempoLeft
        {
            get => Bass.ChannelGetAttribute(_streamHandleLeft, ChannelAttribute.Tempo);
            set => Bass.ChannelSetAttribute(_streamHandleLeft, ChannelAttribute.Tempo, value);
        }

        public float BpmLeft
        {
            get => _bpmLeft;
            set => SetProperty(ref _bpmLeft, value);
        }

        public float RmsLeft
        {
            get => _rmsLeft;
            set => SetProperty(ref _rmsLeft, value);
        }

        public List<(float min, float max)> PeakList
        {
            get => _peakList;
            set => SetProperty(ref _peakList, value);
        }

        public bool IsPlayingRight
        {
            get => _isPlayingRight;
            set
            {
                if (SetProperty(ref _isPlayingRight, value))
                {
                    if (_isPlayingRight)
                    {
                        Bass.ChannelPlay(_primarySplitStreamRight);
                    }
                    else
                    {
                        Bass.ChannelPause(_primarySplitStreamRight);
                    }
                }
            }
        }

        public double VolumeRight
        {
            get => Bass.ChannelGetAttribute(_primarySplitStreamRight, ChannelAttribute.Volume);
            set => Bass.ChannelSetAttribute(_primarySplitStreamRight, ChannelAttribute.Volume, value);
        }

        public double TempoRight
        {
            get => Bass.ChannelGetAttribute(_streamHandleRight, ChannelAttribute.Tempo);
            set => Bass.ChannelSetAttribute(_streamHandleRight, ChannelAttribute.Tempo, value);
        }


        private async Task OpenLeftCommandBehavior()
        {
            await _trackService.LoadFile();

            if (_trackService.AudioBytes != null)
            {
                var audioBytes = await _trackService.AudioBytes;

                var streamHandle = Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Decode); // create decoder for 1st file
                _streamHandleLeft = BassFx.TempoCreate(streamHandle, BassFlags.Decode | BassFlags.FxFreeSource);

                var channelInfo = Bass.ChannelGetInfo(_streamHandleLeft);
                var length = Bass.ChannelGetLength(_streamHandleLeft);
                var lengthSeconds = Bass.ChannelBytes2Seconds(_streamHandleLeft, length);
                var duration = TimeSpan.FromSeconds(lengthSeconds);

                _primarySplitStreamLeft = BassMix.CreateSplitStream(_streamHandleLeft, 0, null); // create splitter for mixer
                var isSet1 = Bass.ChannelSetDevice(_primarySplitStreamLeft, _primaryDeviceId); // set device for separate playback splitter

                _secondarySplitStreamLeft = BassMix.CreateSplitStream(_streamHandleLeft, 0, null); // create splitter for separate playback
                var isSet2 = Bass.ChannelSetDevice(_secondarySplitStreamLeft, _secondaryDeviceId); // set device for separate playback splitter

                Bass.ChannelSetLink(_primarySplitStreamLeft, _secondarySplitStreamLeft);

                BpmLeft = (float)_bpmService.Decoding(audioBytes);

                PeakList = _waveformService.GenerateAudioData(audioBytes);
            }
        }

        private float GetRms()
        {
            if (_primarySplitStreamLeft == 0)
            {
                return 0;
            }

            var levels = new float[1];
            if (!Bass.ChannelGetLevel(_primarySplitStreamLeft, levels, 0.05f, LevelRetrievalFlags.Mono | LevelRetrievalFlags.RMS))
            {
                System.Diagnostics.Debug.WriteLine($"Failed to get levels for channel {Enum.GetName(typeof(Errors), Bass.LastError)}");
                return 0;
            }

            var dB = levels[0] > 0
                     ? 20 * Math.Log10(levels[0])
                     : -1000;

            //if (dB > -40)
            //{
            //    //TODO: Sometimes this value is less than zero so clamp it.
            //    //TODO: Some problem with BASS/ManagedBass, if you have exactly N bytes available call Bass.ChannelGetLevel with Length = Bass.ChannelBytesToSeconds(N) sometimes results in Errors.Ended.
            //    //TODO: Nuts.
            //    //var leadIn = Math.Max(stream.Position - length, 0);
            //    //return leadIn;
            //    return -40;
            //}

            //var left = Bass.ChannelGetLevelLeft(_handle);
            //var right = Bass.ChannelGetLevelRight(_handle);
            //System.Diagnostics.Debug.WriteLine($"{left} - {right}");

            return (float)dB;
        }

        private async Task OpenRightCommandBehavior()
        {
            await _trackService.LoadFile();

            if (_trackService.AudioBytes != null)
            {
                var audioBytes = await _trackService.AudioBytes;

                var streamHandle = Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Decode); // create decoder for 1st file
                _streamHandleRight = BassFx.TempoCreate(streamHandle, BassFlags.Decode | BassFlags.Loop | BassFlags.FxFreeSource);

                var channelInfo = Bass.ChannelGetInfo(_streamHandleRight);
                var length = Bass.ChannelGetLength(_streamHandleRight);
                var secondsDuration = Bass.ChannelBytes2Seconds(_streamHandleRight, length);
                var duration = TimeSpan.FromSeconds(secondsDuration);

                _primarySplitStreamRight = BassMix.CreateSplitStream(_streamHandleRight, 0, null); // create splitter for mixer
                var isSet1 = Bass.ChannelSetDevice(_primarySplitStreamRight, _primaryDeviceId); // set device for separate playback splitter

                _secondarySplitStreamRight = BassMix.CreateSplitStream(_streamHandleRight, 0, null); // create splitter for separate playback
                var isSet2 = Bass.ChannelSetDevice(_secondarySplitStreamRight, _secondaryDeviceId); // set device for separate playback splitter

                Bass.ChannelSetLink(_primarySplitStreamRight, _secondarySplitStreamRight);
            }
        }
    }
}