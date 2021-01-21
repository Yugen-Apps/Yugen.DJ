using Yugen.Toolkit.Uwp.Audio.NAudio.Interfaces;
using Yugen.Toolkit.Uwp.Audio.NAudio.Providers;

namespace Yugen.Toolkit.Uwp.Audio.NAudio.Extensions
{
    /// <summary>
    /// Useful extension methods to make switching between WaveAndSampleProvider easier
    /// </summary>
    public static class WaveExtension
    {
        /// <summary>
        /// Converts a WaveProvider into a SampleProvider (only works for PCM)
        /// </summary>
        /// <param name="waveProvider">WaveProvider to convert</param>
        /// <returns></returns>
        public static ISampleProvider ToSampleProvider(this IWaveProvider waveProvider)
        {
            return SampleProviderConverters.ConvertWaveProviderIntoSampleProvider(waveProvider);
        }
    }
}
