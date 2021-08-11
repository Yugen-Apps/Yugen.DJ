using AudioVisualizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;
using Yugen.DJ.Interfaces;
using Yugen.DJ.Models;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.DJ.Services
{
    public class DockService : IDockService
    {
        private IAudioPlaybackService _audioPlaybackService;
        private readonly IAppService _appService;
        private readonly IBPMService _bpmService;
        private readonly ITrackService _songService;
        private readonly IAudioVisualizerService _audioVisualizerService;
        private readonly IWaveformService _waveformService;

        private Side _side;

        public DockService(IAppService appService, IBPMService bpmService, ITrackService songService,
            IWaveformService waveformService, IAudioVisualizerService audioVisualizerService)
        {
            _appService = appService;
            _audioVisualizerService = audioVisualizerService;
            _bpmService = bpmService;
            _songService = songService;
            _waveformService = waveformService;
        }


        public event EventHandler<TimeSpan> PositionChanged;

        public event EventHandler AudioPropertiesLoaded;

        public event EventHandler<double> BpmGenerated;

        public event EventHandler WaveformGenerated;

        public TimeSpan NaturalDuration => _audioPlaybackService?.NaturalDuration ?? new TimeSpan();

        public MusicProperties MusicProperties => _songService?.MusicProperties;

        public IVisualizationSource PlaybackSource =>
            _audioVisualizerService.GetPlaybackSource(_audioPlaybackService?.MasterFileInput)?.Source;

        public List<(float min, float max)> PeakList { get; private set; }
        public double Bpm { get; private set; }

        public void Init(Side side)
        {
            _side = side;

            _audioPlaybackService = _appService.AudioPlaybackService(_side);
            _audioPlaybackService.Init();

            _audioPlaybackService.PositionChanged += (sender, e) => PositionChanged?.Invoke(sender, e);
        }

        public async Task LoadSong()
        {
            await _songService.LoadFile();
            await _audioPlaybackService.LoadSong(_songService.AudioFile);

            AudioPropertiesLoaded?.Invoke(this, EventArgs.Empty);

            _ = Task.Run(async () =>
            {
                var stream = await _songService.AudioFile.OpenStreamForReadAsync();

                MemoryStream waveformStream = new MemoryStream();
                await stream.CopyToAsync(waveformStream);
                await GenerateWaveForm(waveformStream);
                stream.Position = 0;

                MemoryStream bpmStream = new MemoryStream();
                await stream.CopyToAsync(bpmStream);
                DetectBpm(bpmStream);
            });
        }

        public void TogglePlay(bool v) => _audioPlaybackService.TogglePlay(v);

        private async Task GenerateWaveForm(Stream stream)
        {
            List<(float min, float max)> peakList = null;

            await Task.Run(() =>
            {
                peakList = _waveformService.Render(stream);
            });

            PeakList = peakList;
            WaveformGenerated?.Invoke(this, EventArgs.Empty);
        }

        private void DetectBpm(Stream stream)
        {
            var bmp = _bpmService.Decoding(stream);
            BpmGenerated?.Invoke(this, bmp);
        }

        public void ChangePitch(double pitch) => _audioPlaybackService.ChangePitch(pitch);
    }
}