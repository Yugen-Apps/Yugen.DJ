namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IAudioPlaybackFactory
    {
        IAudioPlaybackService Create(Side type);
    }
}