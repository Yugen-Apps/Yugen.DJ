using NAudio.Wave;
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

        public WaveformRendererSettings Settings { get; } = new WaveformRendererSettings();

        public IPeakProvider PeakProvider { get; } = new MaxPeakProvider();

        public WaveformService(WaveformRendererSettings settings, IPeakProvider peakProvider)
        {
            Settings = settings;
            PeakProvider = peakProvider;
        }

        //public async Task Render(IStorageFile file)
        //{
        //    var stream = await file.OpenStreamForReadAsync();
        //    Render(stream);
        //}

        //public void Render(Stream stream)
        //{
        //    ISampleProvider isp;
        //    var samples = 0L;

        //    using (var reader = new StreamMediaFoundationReader(stream))
        //    {
        //        isp = reader.ToSampleProvider();
        //        var buffer = new float[reader.Length / 2];
        //        isp.Read(buffer, 0, buffer.Length);

        //        var bytesPerSample = reader.Waveformat.BitsPerSample / 8;
        //        samples = reader.Length / bytesPerSample;
        //    }

        //    Render(isp, samples);
        //}

        public void Render(ISampleProvider isp, long samples)
        {
            var samplesPerPixel = (int)(samples / Settings.Width);
            var stepSize = Settings.PixelsPerPeak + Settings.SpacerPixels;
            PeakProvider.Init(isp, samplesPerPixel * stepSize);
        }
    }
}