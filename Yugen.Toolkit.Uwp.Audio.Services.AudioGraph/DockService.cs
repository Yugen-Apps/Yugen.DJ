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

        public AudioFileInputNode MasterFileInput => _audioPlaybackService?.MasterFileInput;

        public double Bpm { get; private set; }

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

        public void TogglePlay(bool isPaused) => _audioPlaybackService.TogglePlay(isPaused);

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

        public void ChangePitch(double pitch) => _audioPlaybackService.ChangePitch(pitch);

        public float GetRms() => throw new NotImplementedException();
    }
}