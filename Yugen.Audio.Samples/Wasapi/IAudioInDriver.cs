using System;
using System.Runtime.InteropServices;

namespace WASAPI.NET
{
    [Guid("63237456-5CBF-4E50-BB80-285B51759E96")]
    public interface IAudioInDriver : IDisposable
    {
        bool IsStarted { get; }

        double SampleRate { get; }

        int ChannelCount { get; }

        int BufferSize { get; }

        void Setup(Action<IAudioInDriver, float[][]> receiveCallback, int bufferSize);

        void Start();

        void Stop();
    }
}