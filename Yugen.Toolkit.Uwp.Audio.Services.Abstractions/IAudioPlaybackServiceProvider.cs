namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IAudioPlaybackServiceProvider
    {
        IAudioPlaybackService LeftAudioPlaybackService { get; }

        IAudioPlaybackService RightAudioPlaybackService { get; }

        void Init(Side side, IAudioPlaybackService _audioPlaybackService);

        IAudioPlaybackService GetAudioPlaybackService(Side side);
    }
}