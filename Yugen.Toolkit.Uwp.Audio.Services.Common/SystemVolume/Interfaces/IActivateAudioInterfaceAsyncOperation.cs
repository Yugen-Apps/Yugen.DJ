using System;
using System.Runtime.InteropServices;

namespace Yugen.Toolkit.Uwp.Audio.Services.Common.SystemVolume.Interfaces
{
    [ComImport]
    [Guid("72A22D78-CDE4-431D-B8CC-843A71199B6D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IActivateAudioInterfaceAsyncOperation
    {
        void GetActivateResult(
            [MarshalAs(UnmanagedType.Error)] out uint activateResult,
            [MarshalAs(UnmanagedType.IUnknown)] out object activatedInterface);
    }
}