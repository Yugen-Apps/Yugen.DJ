using AudioVisualizer;
using System;
using System.Threading.Tasks;

namespace Yugen.DJ.Interfaces
{
    public interface IAudioService
    {
        event EventHandler<TimeSpan> PositionChanged;

        TimeSpan NaturalDuration { get; }

        Task Init();
        Task OpenFile();

        void TogglePlay(bool isPaused);
        void ChangePitch(double pitch);
        void ChangeVolume(double volume, double fader);
        void IsHeadphones(bool isHeadphone);
        void AddAudioVisualizer(SpectrumVisualizer spectrumVisualizer);
        void AddAudioVisualizer(DiscreteVUBar leftVUBarChanel0, DiscreteVUBar leftVUBarChanel1);
    }
}
