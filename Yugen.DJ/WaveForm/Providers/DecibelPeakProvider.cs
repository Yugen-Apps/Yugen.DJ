using NAudio.Wave;
using System;
using Yugen.DJ.WaveForm.Interfaces;
using Yugen.DJ.WaveForm.Models;

namespace Yugen.DJ.WaveForm.Providers
{
    public partial class WaveFormRenderer
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
}