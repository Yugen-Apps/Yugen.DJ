using System;

namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IMixerService
    {
        event EventHandler<float> LeftRmsChanged;
        event EventHandler<float> RightRmsChanged;

        void IsHeadphones(bool isHeadPhones, Side side);

        void ChangeVolume(double volume, Side side);

        void SetFader(double crossFader);
    }
}