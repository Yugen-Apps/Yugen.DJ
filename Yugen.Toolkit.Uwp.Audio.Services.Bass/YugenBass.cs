using System.Runtime.InteropServices;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
{
    public static partial class YugenBass
    {
        [DllImport("bass", EntryPoint = "BASS_GetDeviceInfo")]
        public static extern bool GetDeviceInfo(int Device, out YugenDeviceInfo Info);
    }
}