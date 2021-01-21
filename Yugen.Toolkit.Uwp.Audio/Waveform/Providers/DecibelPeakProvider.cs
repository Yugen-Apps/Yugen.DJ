﻿using System;
using Yugen.Toolkit.Uwp.Audio.NAudio.Interfaces;
using Yugen.Toolkit.Uwp.Audio.Waveform.Interfaces;
using Yugen.Toolkit.Uwp.Audio.Waveform.Models;

namespace Yugen.Toolkit.Uwp.Audio.Waveform.Providers
{
    public class DecibelPeakProvider : IPeakProvider
    {
        private readonly IPeakProvider sourceProvider;
        private readonly double dynamicRange;

        public DecibelPeakProvider(IPeakProvider sourceProvider, double dynamicRange)
        {
            this.sourceProvider = sourceProvider;
            this.dynamicRange = dynamicRange;
        }

        public void Init(ISampleProvider reader, int samplesPerPixel)
        {
            throw new NotImplementedException();
        }

        public PeakInfo GetNextPeak()
        {
            var peak = sourceProvider.GetNextPeak();
            var decibelMax = 20 * Math.Log10(peak.Max);
            if (decibelMax < 0 - dynamicRange) decibelMax = 0 - dynamicRange;
            var linear = (float)((dynamicRange + decibelMax) / dynamicRange);
            return new PeakInfo(0 - linear, linear);
        }
    }
}