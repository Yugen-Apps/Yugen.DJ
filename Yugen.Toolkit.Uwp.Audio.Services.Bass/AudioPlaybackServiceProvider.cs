using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
{
    public class AudioPlaybackServiceProvider : IAudioPlaybackServiceProvider
    {
        public IAudioPlaybackService LeftAudioPlaybackService { get; private set; }

        public IAudioPlaybackService RightAudioPlaybackService { get; private set; }

        public void Init(Side side, IAudioPlaybackService _audioPlaybackService)
        {
            if (side == Side.Left)
            {
                LeftAudioPlaybackService = _audioPlaybackService;
            }
            else
            {
                RightAudioPlaybackService = _audioPlaybackService;
            }
        }

        public IAudioPlaybackService GetAudioPlaybackService(Side side)
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