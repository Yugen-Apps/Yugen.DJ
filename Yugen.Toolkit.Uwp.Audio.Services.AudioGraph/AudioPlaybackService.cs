using System;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.AudioGraph
{
    public class AudioPlaybackService : IAudioPlaybackService
    {
        private readonly IAudioDeviceService _audioDeviceService;
        private readonly IAudioGraphService _masterAudioGraphService;
        private readonly IAudioGraphService _headphonesAudioGraphService;

        public AudioPlaybackService(
            IAudioDeviceService audioDeviceService,
            IAudioGraphService masterAudioGraphService,
            IAudioGraphService headphonesAudioGraphService)
        {
            _audioDeviceService = audioDeviceService;
            _masterAudioGraphService = masterAudioGraphService;
            _headphonesAudioGraphService = headphonesAudioGraphService;

            _masterAudioGraphService.PositionChanged += OnPositionChanged;
        }

        public event EventHandler<TimeSpan> PositionChanged;

        public event EventHandler<float> RmsChanged;

        public AudioFileInputNode MasterFileInput => _masterAudioGraphService?.AudioFileInput;

        public TimeSpan NaturalDuration => MasterFileInput?.Duration ?? new TimeSpan();

        public async Task Init()
        {
            await _masterAudioGraphService.InitDevice(_audioDeviceService.PrimaryDevice.Driver, true);
            await _headphonesAudioGraphService.InitDevice(_audioDeviceService.SecondaryDevice.Driver, false);
        }

        public async Task LoadSong(StorageFile audioFile)
        {
            if (audioFile == null)
                return;

            _masterAudioGraphService.DisposeFileInputs();
            _headphonesAudioGraphService.DisposeFileInputs();

            await _masterAudioGraphService.AddFileToDevice(audioFile);
            await _headphonesAudioGraphService.AddFileToDevice(audioFile);
        }

        public void TogglePlay(bool isPaused)
        {
            _masterAudioGraphService.TogglePlay(isPaused);
            _headphonesAudioGraphService.TogglePlay(isPaused);
        }

        public void ChangePitch(double pitch)
        {
            var ratio = pitch == 0 ? 0 : pitch / 100;

            _masterAudioGraphService.ChangePitch(ratio);
            _headphonesAudioGraphService.ChangePitch(ratio);
        }

        public void ChangeVolume(double volume, double fader)
        {
            volume *= fader / 100;

            _masterAudioGraphService.ChangeVolume(volume);
            _headphonesAudioGraphService.ChangeVolume(volume);
        }

        public void IsHeadphones(bool isHeadphone) =>
            _headphonesAudioGraphService.IsHeadphones(isHeadphone);

        public Task LoadSong(byte[] audioBytes) => throw new NotImplementedException();

        private void OnPositionChanged(object sender, TimeSpan e) => PositionChanged?.Invoke(sender, e);
    }
}