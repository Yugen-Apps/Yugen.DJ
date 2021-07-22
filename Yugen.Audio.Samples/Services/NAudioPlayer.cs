using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Yugen.Audio.Samples.Interfaces;
using Yugen.Audio.Samples.Models;

namespace Yugen.Audio.Samples.Services
{
    public class NAudioPlayer : IAudioPlayer
    {
        private WaveOutEvent outputDevice;

        public TimeSpan Duration => throw new NotImplementedException();
        public bool IsRepeating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AudioPlayerState State => throw new NotImplementedException();
        public double Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Initialize(string deviceId, int inputChannels = 2, int inputSampleRate = 44100)
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                //outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
        }

        public Task Load(StorageFile tmpAudioFile)
        {
            throw new NotImplementedException();
        }

        public Task Load(Stream audioStream)
        {
            using (var mf = new StreamMediaFoundationReader(audioStream))
            {
                outputDevice.Init(mf);
            }

            return Task.CompletedTask;
        }

        public Task Load(byte[] bytes)
        {
            throw new NotImplementedException();
        }


        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            outputDevice.Play();
        }

        public void PlayWithoutStreaming()
        {
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
        }

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