using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass.Providers
{
    public class AudioPlaybackServiceProvider : IAudioPlaybackServiceProvider
    {
        private readonly IAudioPlaybackService _leftAudioPlaybackService;
        private readonly IAudioPlaybackService _rightAudioPlaybackService;

        public AudioPlaybackServiceProvider(
            IAudioPlaybackService leftAudioPlaybackService,
            IAudioPlaybackService rightAudioPlaybackService)
        {
            _leftAudioPlaybackService = leftAudioPlaybackService;
            _rightAudioPlaybackService = rightAudioPlaybackService;
        }

        public void Initialize()
        {
            _leftAudioPlaybackService.Initialize();
            _rightAudioPlaybackService.Initialize();
        }

        public IAudioPlaybackService Get(Side side)
        {
            return side == Side.Left
                   ? _leftAudioPlaybackService
                   : _rightAudioPlaybackService;
        }
    }
}