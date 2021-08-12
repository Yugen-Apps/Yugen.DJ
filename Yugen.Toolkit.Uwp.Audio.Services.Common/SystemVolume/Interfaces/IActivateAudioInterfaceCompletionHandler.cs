﻿using System;
using System.Runtime.InteropServices;

namespace Yugen.Toolkit.Uwp.Audio.Services.Common.SystemVolume.Interfaces
{
    [ComImport]
    [Guid("41D949AB-9862-444A-80F6-C261334DA5EB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IActivateAudioInterfaceCompletionHandler
    {
        void ActivateCompleted(IActivateAudioInterfaceAsyncOperation activateOperation);
    }
}