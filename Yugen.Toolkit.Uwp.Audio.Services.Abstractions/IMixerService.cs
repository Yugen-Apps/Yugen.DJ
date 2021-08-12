namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IMixerService
    {
        IAudioPlaybackService LeftAudioPlaybackService { get; set; }
        IAudioPlaybackService RightAudioPlaybackService { get; set; }
        void IsHeadphones(bool isHeadPhones, Side side);
        void ChangeVolume(double volume, Side side);
        void SetFader(double crossFader);
    }
}