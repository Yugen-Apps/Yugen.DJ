using ManagedBass;
using ManagedBass.Fx;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using Windows.Storage;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Models;

namespace Yugen.Audio.Samples.Services
{
    public class BassPlayer : IAudioPlayer
    {
        private int _handle;
        private ChannelInfo _channelInfo;
        private long _length;
        private double _secondsDuration;

        public void Initialize(string deviceId, int inputChannels = 2, int inputSampleRate = 44100)
        {
            Bass.Init();
            //Bass.ChannelSetDevice(_handle, i);
        }

        public TimeSpan Duration { get; private set; }

        public TimeSpan Position
        {
            get => TimeSpan.FromSeconds(Bass.ChannelBytes2Seconds(_handle, Bass.ChannelGetPosition(_handle)));
            set => Bass.ChannelSetPosition(_handle, Bass.ChannelSeconds2Bytes(_handle, value.TotalSeconds));
        }

        public bool IsRepeating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AudioPlayerState State => throw new NotImplementedException();

        public float Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
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

        public Task Load(StorageFile tmpAudioFile)
        {
            throw new NotImplementedException();
        }

        public Task Load(Stream audioStream)
        {
            throw new NotImplementedException();
        }

        public Task Load(byte[] bytes)
        {
            _handle = Bass.CreateStream(bytes, 0, bytes.Length, BassFlags.Float);

            Bass.ChannelGetInfo(_handle, out _channelInfo);
            _length = Bass.ChannelGetLength(_handle);
            _secondsDuration = Bass.ChannelBytes2Seconds(_handle, _length);
            Duration = TimeSpan.FromSeconds(_secondsDuration);

            var handle = BassFx.TempoCreate(_handle, BassFlags.Loop | BassFlags.FxFreeSource);


            return Task.CompletedTask;
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            //Bass.ChannelSetAttribute(_handle, ChannelAttribute.Volume, 0f);

            Bass.ChannelPlay(_handle);
        }

        public void PlayWithoutStreaming()
        {
        }

        public void Pause() => Bass.ChannelPause(_handle);

        public void Stop() => Bass.ChannelStop(_handle);

        public void Wait()
        {
            throw new NotImplementedException();
        }

        public void Record(StorageFile audioFile)
        {
            throw new NotImplementedException();
        }

    }
}