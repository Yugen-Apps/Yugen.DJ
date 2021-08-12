using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage.FileProperties;

namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IDockService
    {
        TimeSpan NaturalDuration { get; }
        MusicProperties MusicProperties { get; }
        AudioFileInputNode MasterFileInput { get; }
        List<(float min, float max)> PeakList { get; }

        event EventHandler<double> BpmGenerated;
        event EventHandler<TimeSpan> PositionChanged;
        event EventHandler AudioPropertiesLoaded;
        event EventHandler WaveformGenerated;

        void Init(Side side);
        Task LoadSong();
        void TogglePlay(bool v);
        void ChangePitch(double pitch);
    }
}