namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IMixerService
    {
        void IsHeadphones(bool isHeadPhones, Side side);
        void ChangeVolume(double volume, Side side);
        void SetFader(double crossFader);
    }
}