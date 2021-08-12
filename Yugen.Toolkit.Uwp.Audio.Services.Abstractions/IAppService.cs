namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IAppService
    {
        IAudioPlaybackService AudioPlaybackService(Side side);
    }
}