using System;
using System.Runtime.InteropServices;

namespace WASAPI.NET.Com
{
    [ComImport, Guid("F294ACFC-3146-4483-A7BF-ADDCA7C260E2"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioRenderClient
    {
        int GetBuffer(int numFramesRequested, out IntPtr ptr);

        int ReleaseBuffer(int numFramesWritten, AudioClientBufferFlags flags);
    }
}