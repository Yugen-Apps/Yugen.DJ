﻿using NAudio.Wave;
using System.Collections.Generic;
using System.IO;
using Yugen.Toolkit.Uwp.Audio.Waveform.Interfaces;
using Yugen.Toolkit.Uwp.Audio.Waveform.Models;
using Yugen.Toolkit.Uwp.Audio.Waveform.Providers;

namespace Yugen.Toolkit.Uwp.Audio.Waveform.Services
{
    /// <summary>
    /// https://github.com/naudio/NAudio.WaveformRenderer
    /// </summary>
    public class WaveformService : IWaveformService
    {
        public WaveformService()
        {
        }

        public WaveformService(WaveformRendererSettings settings, IPeakProvider peakProvider)
        {
            Settings = settings;
            _peakProvider = peakProvider;
        }

        public List<PeakInfo> PeakList { get; } = new List<PeakInfo>();

        public WaveformRendererSettings Settings { get; } = new WaveformRendererSettings();

        private IPeakProvider _peakProvider = new MaxPeakProvider();

        public void Render(Stream stream)
        {
            ISampleProvider isp;
            long samples;

            using (var reader = new StreamMediaFoundationReader(stream))
            {
                isp = reader.ToSampleProvider();
                float[] Buffer = new float[reader.Length / 2];
                isp.Read(Buffer, 0, Buffer.Length);

                int bytesPerSample = reader.WaveFormat.BitsPerSample / 8;
                samples = reader.Length / bytesPerSample;

                //int sampleRate = isp.WaveFormat.SampleRate;
                //double totalMinutes = reader.TotalTime.TotalMinutes;
            }

            Render(isp, samples);
        }

        public void Render(ISampleProvider isp, long samples)
        {
            var samplesPerPixel = (int)(samples / Settings.Width);
            var stepSize = Settings.PixelsPerPeak + Settings.SpacerPixels;
            _peakProvider.Init(isp, samplesPerPixel * stepSize);

            // DecibelScale - if true, convert values to decibels for a logarithmic waveform
            if (Settings.DecibelScale)
            {
                _peakProvider = new DecibelPeakProvider(_peakProvider, 48);
            }

            PeakList.Clear();
            for (int i = 0; i < Settings.Width; i++)
            {
                PeakList.Add(_peakProvider.GetNextPeak());
            }
        }
    }
}