using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Yugen.DJ.WaveForm.Models
{
    public class WaveFormRendererSettings
    {
        public int Width { get; set; } = 800;
        public int TopHeight { get; set; } = 50;
        public int BottomHeight { get; set; } = 50;
        public int PixelsPerPeak { get; set; } = 1;
        public int SpacerPixels { get; set; } = 0;
        public bool DecibelScale { get; set; }
    }
}