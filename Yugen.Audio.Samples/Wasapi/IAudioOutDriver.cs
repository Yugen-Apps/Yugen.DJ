﻿using System;
using System.Runtime.InteropServices;

namespace WASAPI.NET
{
    [Guid("63237456-5CBF-4E50-BB80-285B51759E95")]
    public interface IAudioOutDriver : IDisposable
    {
        bool IsStarted { get; }

        double SampleRate { get; }

        int ChannelCount { get; }

        int BufferSize { get; }

        void Setup(Func<IAudioOutDriver, float[][], int> renderCallback, int bufferSize);

        void Start();

        void Stop();
    }
}