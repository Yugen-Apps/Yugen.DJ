using ManagedBass;
using ManagedBass.Fx;
using ManagedBass.Mix;
using System;
using System.Threading.Tasks;
using System.Timers;
using Windows.Media.Audio;
using Windows.Storage;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
{
    public class AudioPlaybackService : IAudioPlaybackService
    {
        private readonly IAudioDeviceService _audioDeviceService;
        private readonly Timer _progressBarTimer = new Timer(100);

        private int _streamHandle;
        private int _primarySplitStream;
        private int _secondarySplitStream;
        private bool _isPaused = true;

        public AudioPlaybackService(IAudioDeviceService audioDeviceService)
        {
            _audioDeviceService = audioDeviceService;
        }

        public event EventHandler<TimeSpan> PositionChanged;

        public event EventHandler<float> RmsChanged;

        public TimeSpan NaturalDuration { get; private set; }

        public AudioFileInputNode MasterFileInput => throw new NotImplementedException();

        public void ChangePitch(double pitch)
        {
            ManagedBass.Bass.ChannelSetAttribute(_streamHandle, ChannelAttribute.Tempo, pitch);
            // get => Bass.ChannelGetAttribute(_streamHandle, ChannelAttribute.Tempo);
        }

        public void ChangeVolume(double volume, double fader)
        {
            volume *= fader / 100;

            ManagedBass.Bass.ChannelSetAttribute(_primarySplitStream, ChannelAttribute.Volume, volume);
            //get => Bass.ChannelGetAttribute(_primarySplitStream, ChannelAttribute.Volume);
        }

        public Task Init()
        {
            _progressBarTimer.Elapsed += (s, e) =>
            {
                var position = TimeSpan.FromSeconds(ManagedBass.Bass.ChannelBytes2Seconds(_primarySplitStream, ManagedBass.Bass.ChannelGetPosition(_primarySplitStream)));
                PositionChanged?.Invoke(this, position);
                RmsChanged?.Invoke(this, GetRms());
            };
            _progressBarTimer.Start();
            return Task.CompletedTask;
        }

        public void IsHeadphones(bool isHeadphone)
        {
            //throw new NotImplementedException();
        }

        public Task LoadSong(StorageFile audioFile) => throw new NotImplementedException();

        public Task LoadSong(byte[] audioBytes)
        {
            if (audioBytes != null)
            {
                var streamHandle = ManagedBass.Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Decode); // create decoder for 1st file
                _streamHandle = BassFx.TempoCreate(streamHandle, BassFlags.Decode | BassFlags.FxFreeSource);

                var channelInfo = ManagedBass.Bass.ChannelGetInfo(_streamHandle);
                var length = ManagedBass.Bass.ChannelGetLength(_streamHandle);
                var lengthSeconds = ManagedBass.Bass.ChannelBytes2Seconds(_streamHandle, length);
                NaturalDuration = TimeSpan.FromSeconds(lengthSeconds);

                _primarySplitStream = BassMix.CreateSplitStream(_streamHandle, 0, null); // create splitter for mixer
                var isSet1 = ManagedBass.Bass.ChannelSetDevice(_primarySplitStream, _audioDeviceService.PrimaryDeviceId); // set device for separate playback splitter

                _secondarySplitStream = BassMix.CreateSplitStream(_streamHandle, 0, null); // create splitter for separate playback
                var isSet2 = ManagedBass.Bass.ChannelSetDevice(_secondarySplitStream, _audioDeviceService.SecondaryDeviceId); // set device for separate playback splitter

                ManagedBass.Bass.ChannelSetLink(_primarySplitStream, _secondarySplitStream);
            }

            return Task.CompletedTask;
        }

        public void TogglePlay(bool isPaused)
        {
            _isPaused = isPaused;
            if (isPaused)
            {
                ManagedBass.Bass.ChannelPause(_primarySplitStream);
            }
            else
            {
                ManagedBass.Bass.ChannelPlay(_primarySplitStream);
            }
        }

        private float GetRms()
        {
            if (_primarySplitStream == 0 ||
                _isPaused)
            {
                return 0;
            }

            var levels = new float[1];
            if (!ManagedBass.Bass.ChannelGetLevel(_primarySplitStream, levels, 0.05f, LevelRetrievalFlags.Mono | LevelRetrievalFlags.RMS))
            {
                System.Diagnostics.Debug.WriteLine($"Failed to get levels for channel {Enum.GetName(typeof(Errors), ManagedBass.Bass.LastError)}");
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
    }
}