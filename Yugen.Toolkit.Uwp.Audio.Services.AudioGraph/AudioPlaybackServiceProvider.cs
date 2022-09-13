using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.AudioGraph
{
    public class AudioPlaybackServiceProvider : IAudioPlaybackServiceProvider
    {
        public AudioPlaybackServiceProvider(
            IAudioPlaybackService leftAudioPlaybackService,
            IAudioPlaybackService rightAudioPlaybackService)
        {
            LeftAudioPlaybackService = leftAudioPlaybackService;
            RightAudioPlaybackService = rightAudioPlaybackService;
        }
        public IAudioPlaybackService LeftAudioPlaybackService { get; private set; }

        public IAudioPlaybackService RightAudioPlaybackService { get; private set; }

        public void Initialize()
        {
            LeftAudioPlaybackService.Initialize();
            RightAudioPlaybackService.Initialize();
        }

        public IAudioPlaybackService Get(Side side)
        {
            if (side == Side.Left)
            {
                return LeftAudioPlaybackService;
            }
            else
            {
                return RightAudioPlaybackService;
            }
        }
    }
}