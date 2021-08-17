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
    public class NAudioPlayerRT : IAudioPlayer
    {
        private WasapiOutRT player;
        private WaveStream reader;

        public TimeSpan Duration => throw new NotImplementedException();

        public bool IsRepeating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TimeSpan Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AudioPlayerState State => throw new NotImplementedException();

        public double Volume { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Initialize(string deviceId, int inputChannels = 2, int inputSampleRate = 44100)
        {
            if (player == null)
            {
                // Exclusive mode - fails with a weird buffer alignment error
                player = new WasapiOutRT(AudioClientShareMode.Shared, 200);

                //player.PlaybackStopped += PlayerOnPlaybackStopped;
            }
        }

        public Task Load(StorageFile tmpAudioFile) => throw new NotImplementedException();

        public Task Load(Stream audioStream)
        {
            if (reader is RawSourceWaveStream)
            {
                reader.Position = 0;
            }

            //reader = new MediaFoundationReaderUniversal(selectedStream);
            reader = new StreamMediaFoundationReader(audioStream);

            player.Init(() => reader);

            return Task.CompletedTask;
        }

        public Task Load(byte[] bytes) => throw new NotImplementedException();

        public void Close() => throw new NotImplementedException();

        public void Play()
        {
            player.Play();
        }

        public void PlayWithoutStreaming() => throw new NotImplementedException();

        public void Pause() => throw new NotImplementedException();

        public void Stop() => throw new NotImplementedException();

        public void Wait() => throw new NotImplementedException();

        public void Record(StorageFile audioFile) => throw new NotImplementedException();
    }
}