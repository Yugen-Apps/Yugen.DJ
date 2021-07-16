using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading.Tasks;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Audio.Waveform.Services;

namespace Yugen.Audio.Samples.ViewModels
{
    public class WaveformViewModel : ViewModelBase
    {
        private readonly IWaveformService _waveformRendererService;
        private WaveformRenderer _waveformRenderer = new WaveformRenderer();
        private bool _isGenerated;
        private CanvasControl _sender;

        public WaveformViewModel(IWaveformService waveformRendererService)
        {
            _waveformRendererService = waveformRendererService;
        }

        public void OnDraw(CanvasControl sender, CanvasDrawingSession ds)
        {
            _sender = sender;
            if (_isGenerated)
            {
                _waveformRenderer.DrawRealLine(sender, ds, _waveformRendererService.Settings, _waveformRendererService.PeakProvider);
            }
            else
            {
                _waveformRenderer.DrawFakeLine(sender, ds);
            }
        }

        public async Task GenerateAudioData(Stream stream)
        {
            ISampleProvider isp;
            long samples;

            await Task.Run(() =>
            {
                using (var reader = new StreamMediaFoundationReader(stream))
                {
                    isp = reader.ToSampleProvider();
                    var Buffer = new float[reader.Length / 2];
                    isp.Read(Buffer, 0, Buffer.Length);

                    var bytesPerSample = reader.WaveFormat.BitsPerSample / 8;
                    samples = reader.Length / bytesPerSample;

                    var sampleRate = isp.WaveFormat.SampleRate;
                    var totalMinutes = reader.TotalTime.TotalMinutes;
                }

                _waveformRendererService.Render(isp, samples);
                _isGenerated = true;
                _sender.Invalidate();
            });
        }
    }
}