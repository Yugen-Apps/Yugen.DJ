﻿namespace Yugen.Toolkit.Uwp.Audio.Services.NAudio.Models
{
    public class PeakInfo
    {
        public PeakInfo(float min, float max)
        {
            Max = max;
            Min = min;
        }

        public float Min { get; private set; }

        public float Max { get; private set; }
    }
}