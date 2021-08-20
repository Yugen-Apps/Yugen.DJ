using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage.FileProperties;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.AudioGraph
{
    public class DockService : IDockService
    {
        private IAudioPlaybackService _audioPlaybackService;
        private readonly IAudioPlaybackServiceProvider _audioPlaybackServiceProvider;
        private readonly IBPMService _bpmService;
        private readonly ITrackService _trackService;
        private readonly IWaveformService _waveformService;

        public DockService(
            IAudioPlaybackServiceProvider audioPlaybackServiceProvider,
            IBPMService bpmService,
            ITrackService trackService,
            IWaveformService waveformService)
        {
            _audioPlaybackServiceProvider = audioPlaybackServiceProvider;
            _bpmService = bpmService;
            _trackService = trackService;
            _waveformService = waveformService;
        }

        public event EventHandler<TimeSpan> PositionChanged;

        public event EventHandler<MusicProperties> AudioPropertiesLoaded;

        public event EventHandler<float> BpmGenerated;

        public event EventHandler<List<(float min, float max)>> WaveformGenerated;

        public TimeSpan NaturalDuration => _audioPlaybackService?.NaturalDuration ?? new TimeSpan();

        public AudioFileInputNode MasterFileInput => _audioPlaybackService?.MasterFileInput;

        public double Bpm { get; private set; }

        public void Init(Side side)
        {
            _audioPlaybackService = _audioPlaybackServiceProvider.GetAudioPlaybackService(side);
            _audioPlaybackService.PositionChanged += (sender, e) => PositionChanged?.Invoke(sender, e);
        }

        public async Task LoadSong()
        {
            if (await _trackService.LoadFile())
            {
                await _audioPlaybackService.LoadSong(_trackService.AudioFile);

                AudioPropertiesLoaded?.Invoke(this, _trackService.MusicProperties);

                _ = Task.Run(async () =>
                {
                    var stream = await _trackService.AudioFile.OpenStreamForReadAsync();

                    MemoryStream waveformStream = new MemoryStream();
                    await stream.CopyToAsync(waveformStream);
                    await GenerateWaveForm(waveformStream);
                    stream.Position = 0;

                    MemoryStream bpmStream = new MemoryStream();
                    await stream.CopyToAsync(bpmStream);
                    DetectBpm(bpmStream);
                });
            }
        }

        public void TogglePlay(bool isPaused) => _audioPlaybackService.TogglePlay(isPaused);

        public void ChangePitch(double pitch) => _audioPlaybackService.ChangePitch(pitch);

        private async Task GenerateWaveForm(Stream stream)
        {
            List<(float min, float max)> peakList = null;

            await Task.Run(() =>
            {
                peakList = _waveformService.GenerateAudioData(stream);
            });

            WaveformGenerated?.Invoke(this, peakList);
        }

        private void DetectBpm(Stream stream)
        {
            var bmp = _bpmService.Decoding(stream);
            BpmGenerated?.Invoke(this, bmp);
        }
    }
}