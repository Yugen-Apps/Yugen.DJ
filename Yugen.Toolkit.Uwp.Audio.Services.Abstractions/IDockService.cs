using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Storage.FileProperties;

namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions
{
    public interface IDockService
    {
        event EventHandler<float> BpmGenerated;

        event EventHandler<TimeSpan> PositionChanged;

        event EventHandler<MusicProperties> AudioPropertiesLoaded;

        event EventHandler<List<(float min, float max)>> WaveformGenerated;

        TimeSpan NaturalDuration { get; }

        AudioFileInputNode MasterFileInput { get; }

        void Init(Side side);

        Task LoadSong();

        void TogglePlay(bool isPaused);

        void ChangePitch(double pitch);

        Task Scratch(bool isTouched, bool isClockwise, float crossProduct);
    }
}