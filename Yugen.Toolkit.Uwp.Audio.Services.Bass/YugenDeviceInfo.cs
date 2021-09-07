using ManagedBass;
using System;
using System.Runtime.InteropServices;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
{
    [StructLayout(LayoutKind.Sequential)]
    public struct YugenDeviceInfo
    {
        private IntPtr name;
        private IntPtr driver;
        private DeviceInfoFlags flags;

        private static string PtrToString(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;

            var unicodeWin = ManagedBass.Bass.GetConfig(Configuration.UnicodeDeviceInformation);

            switch (unicodeWin)
            {
                case -1:
                    return Marshal.PtrToStringAuto(ptr);

                case 0:
                    return Marshal.PtrToStringAnsi(ptr);

                default:
                    return ManagedBass.Extensions.PtrToStringUtf8(ptr);
            }
        }

        /// <summary>
        /// The description of the device.
        /// </summary>
        public string Name => PtrToString(name);

        /// <summary>
        /// The filename of the driver being used... <see langword="null" /> = no driver (ie. <see cref="Bass.NoSoundDevice"/> device).
        /// <para>On systems that can use both VxD and WDM drivers (Windows Me/98SE), this will reveal which Type of driver is being used.</para>
        /// <para>Further information can be obtained from the file using the GetFileVersionInfo Win32 API function.</para>
        /// </summary>
        public string Driver => PtrToString(driver);

        /// <summary>
        /// The device is the system default device.
        /// </summary>
        public bool IsDefault => flags.HasFlag(DeviceInfoFlags.Default);

        /// <summary>
        /// The device is enabled and can be used.
        /// </summary>
        public bool IsEnabled => flags.HasFlag(DeviceInfoFlags.Enabled);

        /// <summary>
        /// The device is already initialized.
        /// </summary>
        public bool IsInitialized => flags.HasFlag(DeviceInfoFlags.Initialized);

        /// <summary>
        /// The device is a Loopback device.
        /// </summary>
        public bool IsLoopback => flags.HasFlag(DeviceInfoFlags.Loopback);

        /// <summary>
        /// The device's Type.
        /// </summary>
        public DeviceType Type => (DeviceType)(flags & DeviceInfoFlags.TypeMask);
    }
}