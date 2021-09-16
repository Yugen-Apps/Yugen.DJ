using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage.FileProperties;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
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

        public AudioFileInputNode MasterFileInput => null;

        public void Init(Side side)
        {
            _audioPlaybackService = _audioPlaybackServiceProvider.GetAudioPlaybackService(side);
            _audioPlaybackService.PositionChanged += (sender, e) => PositionChanged?.Invoke(sender, e);
        }

        public async Task<bool> LoadSong()
        {
            bool isLoaded = false;

            if (await _trackService.LoadFile())
            {
                var audioBytes = await _trackService.GetAudioBytes();

                if (audioBytes != null)
                {
                    await _audioPlaybackService.LoadSong(audioBytes);

                    AudioPropertiesLoaded?.Invoke(this, _trackService.MusicProperties);

                    _ = Task.Run(() =>
                    {
                        var bpm = _bpmService.Decoding(audioBytes);
                        BpmGenerated?.Invoke(this, bpm);

                        var peakList = _waveformService.GenerateAudioData(audioBytes);
                        WaveformGenerated?.Invoke(this, peakList);
                    });

                    isLoaded = true;
                }
            }

            return isLoaded;
        }

        public void TogglePlay(bool isPaused) => _audioPlaybackService.TogglePlay(isPaused);

        public void ChangePitch(double pitch) => _audioPlaybackService.ChangePitch(pitch);

        public Task Scratch(bool isTouched, bool isClockwise, float crossProduct)
        {
            return _audioPlaybackService.Scratch(isTouched, isClockwise, crossProduct);
        }
    }
}