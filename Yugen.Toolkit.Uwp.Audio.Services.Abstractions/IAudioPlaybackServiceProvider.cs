namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IAudioPlaybackServiceProvider
    {
        IAudioPlaybackService LeftAudioPlaybackService { get; }

        IAudioPlaybackService RightAudioPlaybackService { get; }

        void Init();

        IAudioPlaybackService GetAudioPlaybackService(Side side);
    }
}