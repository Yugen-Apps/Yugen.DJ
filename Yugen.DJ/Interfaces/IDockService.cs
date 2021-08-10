using AudioVisualizer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;
using Yugen.DJ.Models;

namespace Yugen.DJ.Interfaces
{
    public interface IDockService
    {
        TimeSpan NaturalDuration { get; }
        MusicProperties MusicProperties { get; }
        IVisualizationSource PlaybackSource { get; }
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