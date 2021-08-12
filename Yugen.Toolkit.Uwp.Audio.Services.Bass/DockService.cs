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
        private readonly IAudioPlaybackService _audioPlaybackService;
        private readonly IBPMService _bpmService;
        private readonly IMixerService _mixerService;
        private readonly ITrackService _trackService;
        private readonly IWaveformService _waveformService;

        private Side _side;

        public DockService(
            IAudioPlaybackService audioPlaybackService,
            IBPMService bpmService,
            IMixerService mixerService,
            ITrackService trackService,
            IWaveformService waveformService)
        {
            _audioPlaybackService = audioPlaybackService;
            _bpmService = bpmService;
            _mixerService = mixerService;
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
            _side = side;

            _audioPlaybackService.Init();

            if (side == Side.Left)
            {
                _mixerService.LeftAudioPlaybackService = _audioPlaybackService;
            }
            else
            {
                _mixerService.RightAudioPlaybackService = _audioPlaybackService;
            }

            _audioPlaybackService.PositionChanged += (sender, e) => PositionChanged?.Invoke(sender, e);
        }

        public async Task LoadSong()
        {
            await _trackService.LoadFile();

            var audioBytes = await _trackService.AudioBytes;
            await _audioPlaybackService.LoadSong(audioBytes);

            AudioPropertiesLoaded?.Invoke(this, _trackService.MusicProperties);

            if (audioBytes != null)
            {
                _ = Task.Run(() =>
                {
                    var bpm = _bpmService.Decoding(audioBytes);
                    BpmGenerated?.Invoke(this, bpm);

                    var peakList = _waveformService.GenerateAudioData(audioBytes);
                    WaveformGenerated?.Invoke(this, peakList);
                });
            }
        }

        public void TogglePlay(bool isPaused) => _audioPlaybackService.TogglePlay(isPaused);

        public void ChangePitch(double pitch) => _audioPlaybackService.ChangePitch(pitch);

        public float GetRms() => _audioPlaybackService.GetRms();
    }
}