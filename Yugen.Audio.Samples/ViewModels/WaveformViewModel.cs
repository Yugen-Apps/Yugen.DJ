using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.IO;
using System.Threading.Tasks;
using Yugen.Audio.Samples.Renderers;
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
                _waveformRenderer.DrawRealLine(sender, ds, _waveformRendererService.Settings, _waveformRendererService.PeakList);
            }
            else
            {
                _waveformRenderer.DrawFakeLine(sender, ds);
            }
        }

        public async Task GenerateAudioData(Stream stream)
        {
            await Task.Run(() =>
            {
                _waveformRendererService.Render(stream);
                _isGenerated = true;
                _sender.Invalidate();
            });
        }
    }
}