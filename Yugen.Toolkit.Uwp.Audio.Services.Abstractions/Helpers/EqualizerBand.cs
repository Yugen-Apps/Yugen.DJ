namespace Yugen.Toolkit.Uwp.Audio.Services.Abstractions.Helpers
{
    public class EqualizerBand
    {
        public EqualizerBand()
        {
        }

        public EqualizerBand(int bandNo, float centerFrequency)
        {
            BandNo = bandNo;
            CenterFrequency = centerFrequency;

            Label = centerFrequency < 1000 ? $"{centerFrequency}Hz"
                                           : $"{centerFrequency / 1000}KHz";
        }

        public int BandNo { get; set; }

        public float CenterFrequency { get; set; }

        public string Label { get; set; }
    }
}