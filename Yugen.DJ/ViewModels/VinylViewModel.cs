using AudioVisualizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Yugen.DJ.DependencyInjection;
using Yugen.DJ.Interfaces;
using Yugen.DJ.WaveForm;
using Yugen.Toolkit.Standard.Mvvm.ComponentModel;
using Yugen.Toolkit.Standard.Mvvm.Input;

namespace Yugen.DJ.ViewModels
{
    public class VinylViewModel : ViewModelBase
    {
        private readonly IAudioService _audioService;

        private bool _isHeadPhones;
        private bool _isPaused = true;
        private string _playPauseButton = "\uE768";
        private double _volume = 100;
        private double _fader;
        private double _pitch = 0;
        private TimeSpan _targetElapsedTime = new TimeSpan(10000);
        private TimeSpan _naturalDuration = new TimeSpan();
        private TimeSpan _position = new TimeSpan();
        public CanvasControl WaveFormCanvas { get; private set; }
        private ICommand _openButtonCommand;

        public WaveFormRenderer WaveFormRenderer = new WaveFormRenderer();

        public VinylViewModel(bool isLeft)
        {
            IsLeft = isLeft;
            IsHeadPhones = isLeft;

            _audioService = Ioc.Default.GetService<IAudioService>();
            _audioService.PositionChanged += AudioServiceOnPositionChanged;
            _audioService.FileLoaded += AudioServiceOnFileLoaded;
        }

        private async void AudioServiceOnFileLoaded(object sender, Windows.Storage.StorageFile file)
        {
            await WaveFormRenderer.Render(file);

            WaveFormCanvas.Invalidate();
        }

        public bool IsLeft { get; }

        public bool IsHeadPhones
        {
            get { return _isHeadPhones; }
            set
            {
                Set(ref _isHeadPhones, value);

                _audioService?.IsHeadphones(_isHeadPhones);
            }
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                Set(ref _isPaused, value);

                _audioService.TogglePlay(_isPaused);

                PlayPauseButton = _isPaused ? "\uE768" : "\uE769";
            }
        }

        public string PlayPauseButton
        {
            get { return _playPauseButton; }
            set { Set(ref _playPauseButton, value); }
        }

        public double Volume
        {
            get { return _volume; }
            set
            {
                Set(ref _volume, value);

                _audioService.ChangeVolume(_volume, _fader);
            }
        }

        public double Fader
        {
            get { return _fader; }
            set
            {
                Set(ref _fader, value);

                _audioService.ChangeVolume(_volume, _fader);
            }
        }

        public double Pitch
        {
            get { return _pitch; }
            set
            {
                Set(ref _pitch, value);

                _audioService.ChangePitch(_pitch);
            }
        }

        public TimeSpan NaturalDuration
        {
            get { return _naturalDuration; }
            set { Set(ref _naturalDuration, value); }
        }

        public TimeSpan Position
        {
            get { return _position; }
            set { Set(ref _position, value); }
        }

        public ICommand OpenButtonCommand => _openButtonCommand
            ?? (_openButtonCommand = new AsyncRelayCommand(OpenButtonCommandBehavior));

        public async Task Init()
        {
            await _audioService.Init();
        }

        public void AddWaveForm(CanvasControl waveFormCanvas) => WaveFormCanvas = waveFormCanvas;

        //public void AddAudioVisualizer(SpectrumVisualizer spectrumVisualizer) =>
        //    _audioService.AddAudioVisualizer(spectrumVisualizer);

        internal void AddAudioVisualizer(DiscreteVUBar leftVUBarChanel0, DiscreteVUBar leftVUBarChanel1) =>
            _audioService.AddAudioVisualizer(leftVUBarChanel0, leftVUBarChanel1);

        private void AudioServiceOnPositionChanged(object sender, TimeSpan position)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                Position = position;
            });
        }

        private async Task OpenButtonCommandBehavior()
        {
            IsPaused = true;

            await _audioService.OpenFile();

            NaturalDuration = _audioService.NaturalDuration;
        }
    }
}