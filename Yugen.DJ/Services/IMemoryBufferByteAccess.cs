using System;
using System.Runtime.InteropServices;

namespace Yugen.DJ.Services
{
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    //private Stream fileStream;
    //AudioFrameInputNode audioFrameInputNode;

    //private void AddAudioFrameInputNode()
    //{
    //  var ras = await file.OpenReadAsync();
    //  fileStream = ras.AsStreamForRead();
    //
    //    AudioEncodingProperties audioEncodingProperties = new AudioEncodingProperties();
    //    audioEncodingProperties.BitsPerSample = 32;
    //    audioEncodingProperties.ChannelCount = 2;
    //    audioEncodingProperties.SampleRate = 44100;
    //    audioEncodingProperties.Subtype = MediaEncodingSubtypes.Float;

    //    audioFrameInputNode = masterAudioGraph.CreateFrameInputNode(audioEncodingProperties);
    //    audioFrameInputNode.QuantumStarted += FrameInputNode_QuantumStarted;

    //    audioFrameInputNode.AddOutgoingConnection(deviceOutputNode);
    //    audioGraph.Start();
    //}

    //private unsafe void FrameInputNode_QuantumStarted(AudioFrameInputNode sender, FrameInputNodeQuantumStartedEventArgs args)
    //{
    //    var bufferSize = args.RequiredSamples * sizeof(float) * 2;
    //    AudioFrame audioFrame = new AudioFrame((uint)bufferSize);

    //    if (fileStream == null)
    //        return;

    //    using (var audioBuffer = audioFrame.LockBuffer(AudioBufferAccessMode.Write))
    //    {
    //        using (var bufferReference = audioBuffer.CreateReference())
    //        {
    //            byte* dataInBytes;
    //            uint capacityInBytes;
    //            float* dataInFloat;

    //            // Get the buffer from the AudioFrame
    //            ((IMemoryBufferByteAccess)bufferReference).GetBuffer(out dataInBytes, out capacityInBytes);
    //            dataInFloat = (float*)dataInBytes;

    //            var managedBuffer = new byte[capacityInBytes];

    //            var lastLength = fileStream.Length - fileStream.Position;
    //            int readLength = (int)(lastLength < capacityInBytes ? lastLength : capacityInBytes);

    //            if (readLength <= 0)
    //            {
    //                fileStream.Close();
    //                fileStream = null;
    //                return;
    //            }

    //            fileStream.Read(managedBuffer, 0, readLength);

    //            for (int i = 0; i < readLength; i += 8)
    //            {
    //                dataInBytes[i + 4] = managedBuffer[i + 0];
    //                dataInBytes[i + 5] = managedBuffer[i + 1];
    //                dataInBytes[i + 6] = managedBuffer[i + 2];
    //                dataInBytes[i + 7] = managedBuffer[i + 3];
    //                dataInBytes[i + 0] = managedBuffer[i + 4];
    //                dataInBytes[i + 1] = managedBuffer[i + 5];
    //                dataInBytes[i + 2] = managedBuffer[i + 6];
    //                dataInBytes[i + 3] = managedBuffer[i + 7];
    //            }
    //        }
    //    }

    //    audioFrameInputNode.AddFrame(audioFrame);
    //}
}