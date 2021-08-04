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
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const int _bpmPeriod = 30;

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

        public MainViewModel()
        {
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
            var audioFile = await FilePickerHelper.OpenFile(".mp3", PickerLocationId.MusicLibrary);

            if (audioFile != null)
            {
                var audioBytes = await audioFile.ReadBytesAsync();

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

                DecodingBPM(audioBytes);

                BassGenerateAudioData(audioBytes);
            }
        }

        private void DecodingBPM(byte[] audioBytes)
        {
            var bpmchan = Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Decode);

            // create bpmChan stream and get bpm value for BpmPeriod seconds from current position
            var positon = Bass.ChannelGetPosition(bpmchan);
            var positionSeconds = Bass.ChannelBytes2Seconds(bpmchan, positon);
            var length = Bass.ChannelGetLength(bpmchan);
            var lengthSeconds = Bass.ChannelBytes2Seconds(bpmchan, length);

            BpmLeft = BassFx.BPMDecodeGet(bpmchan, 0, lengthSeconds, 0,
                                          BassFlags.FxBpmBackground | BassFlags.FXBpmMult2 | BassFlags.FxFreeSource,
                                          null);

            //double startSec = positionSeconds;
            //double endSec = positionSeconds + _bpmPeriod >= lengthSeconds
            //                ? lengthSeconds - 1 
            //                : positionSeconds + _bpmPeriod;

            //BassFx.BPMCallbackSet(bpmchan, BPMCallback, _bpmPeriod, 0, BassFlags.FXBpmMult2);

            //// detect bpm in background and return progress in GetBPM_ProgressCallback function
            //if (bpmchan != 0)
            //{
            //    BpmLeft = BassFx.BPMDecodeGet(bpmchan, startSec, endSec, 0,
            //                                  BassFlags.FxBpmBackground | BassFlags.FXBpmMult2 | BassFlags.FxFreeSource,
            //                                  null);
            //}
        }

        private void BPMCallback(int Channel, float BPM, IntPtr User)
        {
            // TODO: add dispatcher update the bpm view
            //BpmLeft = BPM;
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

        private void BassGenerateAudioData(byte[] audioBytes)
        {
            List<(float min, float max)> peakList = new List<(float min, float max)>();

            ///

            //var handle = Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Decode);

            // TODO: FFT / Waveform
            // Perform a 1024 sample FFT on a channel and list the result.
            //var fft = new float[512]; // fft data buffer
            //Bass.ChannelGetData(handle, fft, (int)DataFlags.FFT1024);
            //for (int a = 0; a < 512; a++)
            //{
            //    var peak = fft[a] * 100000;
            //    peakList.Add((peak, peak));
            //    System.Diagnostics.Debug.WriteLine("{0}: {1}", a, fft[a]);
            //}

            //Perform a 1024 sample FFT on a channel and list the complex result.
            //var fft2 = new float[2048]; // fft data buffer
            //Bass.ChannelGetData(handle, fft2, (int)(DataFlags.FFT1024 | DataFlags.FFTComplex));
            //for (int a = 0; a < 1024; a++)
            //{
            //    var min = fft2[a * 2] * 100000;
            //    var max = fft2[a * 2 + 1] * 100000;
            //    peakList.Add((min, max));
            //    //System.Diagnostics.Debug.WriteLine("{0}: ({1}, {2})", a, fft2[a * 2], fft2[a * 2 + 1]);
            //}

            ////

            int waveformCompressedPointCount = 500;

            int stream = Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Decode | BassFlags.Float | BassFlags.Prescan);
            int frameLength = (int)Bass.ChannelSeconds2Bytes(stream, 0.02);
            long streamLength = Bass.ChannelGetLength(stream, 0);
            int frameCount = (int)((double)streamLength / (double)frameLength);
            int waveformLength = frameCount * 2;
            float[] waveformData = new float[waveformLength];
            float[] levels;

            int actualPoints = Math.Min(waveformCompressedPointCount, frameCount);

            int compressedPointCount = actualPoints * 2;
            //float[] waveformCompressedPoints = new float[compressedPointCount];
            List<int> waveMaxPointIndexes = new List<int>();
            for (int i = 1; i <= actualPoints; i++)
            {
                waveMaxPointIndexes.Add((int)Math.Round(waveformLength * ((double)i / (double)actualPoints), 0));
            }

            float maxLeftPointLevel = float.MinValue;
            float maxRightPointLevel = float.MinValue;
            int currentPointIndex = 0;
            for (int i = 0; i < waveformLength; i += 2)
            {
                levels = Bass.ChannelGetLevel(stream, 0.02f, LevelRetrievalFlags.Stereo);

                waveformData[i] = levels[0];
                waveformData[i + 1] = levels[1];

                if (levels[0] > maxLeftPointLevel)
                {
                    maxLeftPointLevel = levels[0];
                }
                if (levels[1] > maxRightPointLevel)
                {
                    maxRightPointLevel = levels[1];
                }

                if (i > waveMaxPointIndexes[currentPointIndex])
                {
                    //waveformCompressedPoints[(currentPointIndex * 2)] = maxLeftPointLevel;
                    //waveformCompressedPoints[(currentPointIndex * 2) + 1] = maxRightPointLevel;
                    peakList.Add((-maxLeftPointLevel, maxRightPointLevel));
                    maxLeftPointLevel = float.MinValue;
                    maxRightPointLevel = float.MinValue;
                    currentPointIndex++;
                }
            }

            Bass.StreamFree(stream);

            ////
            PeakList = peakList;
        }

        private async Task OpenRightCommandBehavior()
        {
            var audioFile = await FilePickerHelper.OpenFile(".mp3", PickerLocationId.MusicLibrary);

            if (audioFile != null)
            {
                var audioBytes = await audioFile.ReadBytesAsync();

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