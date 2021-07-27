using ManagedBass;
using ManagedBass.DirectX8;
using ManagedBass.Fx;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Windows.Storage;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Models;
using static System.Net.WebRequestMethods;

namespace Yugen.Audio.Samples.Services
{
    public class BassPlayer : IAudioPlayer
    {
        private const int _bpmPeriod = 30;

        private byte[] _audioBytes;
        private int _handle;
        private ChannelInfo _channelInfo;
        private bool _isFirstPlay;

        private long _length;
        private double _secondsDuration;

        private int _bpmchan;
        private double _beatPosition;
        private int _bpmProgress;

        private DeviceInfo[] _deviceList;
        private int _handle2;

        public void Initialize(string deviceId, int inputChannels = 2, int inputSampleRate = 44100)
        {
            //var isInitialized = Bass.Init(-1); // default
            var isInitialized0 = Bass.Init(0); // no sound IsEnabled IsInitialized Name: N
            var isInitialized1 = Bass.Init(1); // speakers IsDefault Driver IsEnabled IsInitialized Name: D
            var isInitialized2 = Bass.Init(2); // headphones Driver IsEnabled IsInitialized Name: H
            var isInitialized3 = Bass.Init(3); // speakers IsDefault IsEnabled IsInitialized Name: D

            _deviceList = new DeviceInfo[Bass.DeviceCount];
            for (int i = 0; i < Bass.DeviceCount; i++)
            {
                var device = Bass.GetDeviceInfo(i);
                _deviceList[i] = device;
            }
        }

        public TimeSpan Duration { get; private set; }

        public TimeSpan Position
        {
            get => TimeSpan.FromSeconds(Bass.ChannelBytes2Seconds(_handle, Bass.ChannelGetPosition(_handle)));
            set => Bass.ChannelSetPosition(_handle, Bass.ChannelSeconds2Bytes(_handle, value.TotalSeconds));
        }

        public double Rms
        {
            get
            {
                if (_handle == 0)
                {
                    return 0;
                }

                var levels = new float[1];
                if (!Bass.ChannelGetLevel(_handle, levels, 0.05f, LevelRetrievalFlags.Mono | LevelRetrievalFlags.RMS))
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to get levels for channel {Enum.GetName(typeof(Errors), Bass.LastError)}");
                    return 0;
                }
                var dB = levels[0] > 0 ? 20 * Math.Log10(levels[0]) : -1000;

                //if (dB > -40)
                //{
                //    //TODO: Sometimes this value is less than zero so clamp it.
                //    //TODO: Some problem with BASS/ManagedBass, if you have exactly N bytes available call Bass.ChannelGetLevel with Length = Bass.ChannelBytesToSeconds(N) sometimes results in Errors.Ended.
                //    //TODO: Nuts.
                //    //var leadIn = Math.Max(stream.Position - length, 0);
                //    //return leadIn;
                //    return -40;
                //}

                return dB;
            }
        }

        public float Bpm { get; private set; }

        public bool IsRepeating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AudioPlayerState State => throw new NotImplementedException();

        /// <summary>
        /// Gets or Sets the Volume (0 ... 1.0).
        /// </summary>
        public double Volume
        {
            get => Bass.ChannelGetAttribute(_handle, ChannelAttribute.Volume);
            set => Bass.ChannelSetAttribute(_handle, ChannelAttribute.Volume, value);
        }

        /// <summary>
        /// Gets or Sets the Pitch in Semitones (-60 ... 0 ... 60).
        /// </summary>
        public double Pitch
        {
            get => Bass.ChannelGetAttribute(_handle, ChannelAttribute.Pitch);
            set => Bass.ChannelSetAttribute(_handle, ChannelAttribute.Pitch, value);
        }

        /// <summary>
        /// Gets or Sets the Tempo in Percentage (-95% ... 0 ... 5000%)
        /// </summary>
        public double Tempo
        {
            get => Bass.ChannelGetAttribute(_handle, ChannelAttribute.Tempo);
            set => Bass.ChannelSetAttribute(_handle, ChannelAttribute.Tempo, value);
        }

        public Task Load(StorageFile tmpAudioFile) => throw new NotImplementedException();

        public Task Load(Stream audioStream) => throw new NotImplementedException();

        public Task Load(byte[] audioBytes)
        {
            _audioBytes = audioBytes;

            // Create stream and get channel info
            //_handle = Bass.CreateStream(bytes, 0, bytes.Length, BassFlags.Float);
            _handle = Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Decode);

            Bass.ChannelGetInfo(_handle, out _channelInfo);
            //var sampleRate = _channelInfo.Frequency;


            // TODO: FFT / Waveform
            // Perform a 1024 sample FFT on a channel and list the result.
            var fft = new float[512]; // fft data buffer
            Bass.ChannelGetData(_handle, fft, (int)DataFlags.FFT1024);
            //for (int a = 0; a < 512; a++)
            //{ 
            //    Console.WriteLine("{0}: {1}", a, fft[a]);
            //}
            //Perform a 1024 sample FFT on a channel and list the complex result.
            //var fft = new float[2048]; // fft data buffer
            //Bass.ChannelGetData(_handle, fft, (int)(DataFlags.FFT1024 | DataFlags.FFTComplex));
            //for (int a = 0; a < 1024; a++)
            //{
            //    Console.WriteLine("{0}: ({1}, {2})", a, fft[a * 2], fft[a * 2 + 1]);
            //}

            // Get duration
            _length = Bass.ChannelGetLength(_handle);
            _secondsDuration = Bass.ChannelBytes2Seconds(_handle, _length);
            Duration = TimeSpan.FromSeconds(_secondsDuration);

            if (_handle == 0)
            {
                //  Loads a MOD music file - MO3 / IT / XM / S3M / MTM / MOD / UMX formats from memory.
                _handle = Bass.MusicLoad(audioBytes, 0, audioBytes.Length, BassFlags.MusicRamp | BassFlags.Prescan | BassFlags.Decode, 0);
            }

            if (_handle == 0)
            {
                System.Diagnostics.Debug.WriteLine("Selected file couldn't be loaded!");
            }

            // create a new stream - decoded & resampled
            _handle = BassFx.TempoCreate(_handle, BassFlags.Loop | BassFlags.FxFreeSource);
            if (_handle == 0)
            {
                System.Diagnostics.Debug.WriteLine("Couldn't create a resampled stream!");
                Bass.StreamFree(_handle);
                Bass.MusicFree(_handle);
                return Task.CompletedTask;
            }

            // set the callback bpm and beat
            BassFx.BPMCallbackSet(_handle, BPMCallback, _bpmPeriod, 0, BassFlags.FXBpmMult2);
            BassFx.BPMBeatCallbackSet(_handle, BeatPosCallback);

            _isFirstPlay = true;

            return Task.CompletedTask;
        }

        public void Heaphones(bool isHeadphones)
        {
            var isDeviceSet = isHeadphones
                ? Bass.ChannelSetDevice(_handle, 2)
                : Bass.ChannelSetDevice(_handle, 3);
        }

        public Task Load2(byte[] audioBytes)
        {
            _handle2 = Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Float);
            var isDeviceSet = Bass.ChannelSetDevice(_handle2, 3);

            return Task.CompletedTask;
        }

        public void Close() => throw new NotImplementedException();

        public void Play()
        {
            // play new created stream
            Bass.ChannelPlay(_handle);

            if (_isFirstPlay)
            {
                _isFirstPlay = false;
                // create bpmChan stream and get bpm value for BpmPeriod seconds from current position
                var pos = Bass.ChannelBytes2Seconds(_handle, Bass.ChannelGetPosition(_handle));
                var maxpos = Bass.ChannelBytes2Seconds(_handle, Bass.ChannelGetLength(_handle));
                DecodingBPM(true, pos, pos + _bpmPeriod >= maxpos ? maxpos - 1 : pos + _bpmPeriod, _audioBytes);
            }
        }

        public void Play2()
        {
            // play new created stream
            Bass.ChannelPlay(_handle2);
        }

        public void PlayWithoutStreaming() => throw new NotImplementedException();

        public void Pause() => Bass.ChannelPause(_handle);

        public void Stop() => Bass.ChannelStop(_handle);

        public void Wait() => throw new NotImplementedException();

        public void Record(StorageFile audioFile) => throw new NotImplementedException();

        private void DecodingBPM(bool newStream, double startSec, double endSec, byte[] bytes)
        {
            if (newStream)
            {
                // open the same file as played but for bpm decoding detection
                _bpmchan = Bass.CreateStream(bytes, 0, bytes.Length, BassFlags.Decode);

                if (_bpmchan == 0)
                {
                    _bpmchan = Bass.MusicLoad(bytes, 0, bytes.Length, BassFlags.Decode | BassFlags.Prescan, 0);
                }
            }

            // detect bpm in background and return progress in GetBPM_ProgressCallback function
            if (_bpmchan != 0)
            {
                Bpm = BassFx.BPMDecodeGet(_bpmchan, startSec, endSec, 0,
                                              BassFlags.FxBpmBackground | BassFlags.FXBpmMult2 | BassFlags.FxFreeSource,
                                              BPMProgressCallback);
            }
        }

        private void BPMProgressCallback(int Channel, float Percent, IntPtr User)
        {
            _bpmProgress = (int)Percent;
        }

        private void BPMCallback(int Channel, float BPM, IntPtr User)
        {
            // update the bpm view
            Bpm = BPM;
        }

        private async void BeatPosCallback(int Channel, double beatPosition, IntPtr User)
        {
            var curpos = Bass.ChannelBytes2Seconds(Channel, Bass.ChannelGetPosition(Channel));

            await Task.Delay(TimeSpan.FromSeconds(beatPosition - curpos));

            _beatPosition = Bass.ChannelBytes2Seconds(Channel, Bass.ChannelGetPosition(Channel)) / BassFx.TempoGetRateRatio(Channel);
        }
    }
}