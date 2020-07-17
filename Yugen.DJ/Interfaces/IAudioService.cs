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
    }
}
