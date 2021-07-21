﻿using ManagedBass;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Models;

namespace Yugen.Audio.Samples.Services
{
    public class BassPlayer : IAudioPlayer
    {
        private int _handle;

        public TimeSpan Duration => throw new NotImplementedException();
        public bool IsRepeating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AudioPlayerState State => throw new NotImplementedException();
        public float Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Initialize(string deviceId, int inputChannels = 2, int inputSampleRate = 44100)
        {
            Bass.Init();
            //Bass.ChannelSetDevice(_handle, i);
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