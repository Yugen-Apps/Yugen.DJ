using AudioVisualizer;
using System;
using Windows.Foundation;

namespace Yugen.Audio.Samples.Views
{
    public class FakeDataSource : IVisualizationSource
    {
        public VisualizationDataFrame Frame;

        public VisualizationDataFrame GetData() => Frame;

        public uint? ActualChannelCount => 1;

        public uint? ActualFrequencyCount => 1024;

        public ScaleType? ActualFrequencyScale => ScaleType.Linear;

        public float? ActualMaxFrequency => 24000.0f;

        public float? ActualMinFrequency => 0.0f;

        public AnalyzerType AnalyzerTypes { get => AnalyzerType.All; set => throw new NotImplementedException(); }

        public float Fps { get => 60.0f; set => throw new NotImplementedException(); }

        public bool IsSuspended { get => false; set => throw new NotImplementedException(); }

        public SourcePlaybackState PlaybackState => SourcePlaybackState.Playing;

        public TimeSpan? PresentationTime => throw new NotImplementedException();

        public event TypedEventHandler<IVisualizationSource, string> ConfigurationChanged;
    }

}
