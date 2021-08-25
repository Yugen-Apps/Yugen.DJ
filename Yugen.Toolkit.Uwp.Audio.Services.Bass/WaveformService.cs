using ManagedBass;
using System;
using System.Collections.Generic;
using System.IO;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
{
    public class WaveformService : IWaveformService
    {
        public List<(float min, float max)> GenerateAudioData(byte[] audioBytes)
        {
            List<(float min, float max)> peakList = new List<(float min, float max)>();

            ///

            //var handle = Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Decode);

            // TODO: FFT / Waveform
            // Perform a 1024 sample FFT on a channel and list the result.
            //var fft = new float[512]; // fft data buffer
            //Bass.ChannelGetData(handle, fft, (int)DataFlags.FFT1024);
            //for (int a = 0; a < 512; a++)
            //{
            //    var peak = fft[a] * 100000;
            //    peakList.Add((peak, peak));
            //    System.Diagnostics.Debug.WriteLine("{0}: {1}", a, fft[a]);
            //}

            //Perform a 1024 sample FFT on a channel and list the complex result.
            //var fft2 = new float[2048]; // fft data buffer
            //Bass.ChannelGetData(handle, fft2, (int)(DataFlags.FFT1024 | DataFlags.FFTComplex));
            //for (int a = 0; a < 1024; a++)
            //{
            //    var min = fft2[a * 2] * 100000;
            //    var max = fft2[a * 2 + 1] * 100000;
            //    peakList.Add((min, max));
            //    //System.Diagnostics.Debug.WriteLine("{0}: ({1}, {2})", a, fft2[a * 2], fft2[a * 2 + 1]);
            //}

            ////

            int waveformCompressedPointCount = 500;

            int stream = ManagedBass.Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Decode | BassFlags.Float | BassFlags.Prescan);
            int frameLength = (int)ManagedBass.Bass.ChannelSeconds2Bytes(stream, 0.02);
            long streamLength = ManagedBass.Bass.ChannelGetLength(stream, 0);
            int frameCount = (int)(streamLength / (double)frameLength);
            int waveformLength = frameCount * 2;
            float[] waveformData = new float[waveformLength];
            float[] levels;

            int actualPoints = Math.Min(waveformCompressedPointCount, frameCount);

            int compressedPointCount = actualPoints * 2;
            //float[] waveformCompressedPoints = new float[compressedPointCount];
            List<int> waveMaxPointIndexes = new List<int>();
            for (int i = 1; i <= actualPoints; i++)
            {
                waveMaxPointIndexes.Add((int)Math.Round(waveformLength * (i / (double)actualPoints), 0));
            }

            float maxLeftPointLevel = float.MinValue;
            float maxRightPointLevel = float.MinValue;
            int currentPointIndex = 0;
            for (int i = 0; i < waveformLength; i += 2)
            {
                levels = ManagedBass.Bass.ChannelGetLevel(stream, 0.02f, LevelRetrievalFlags.Stereo);

                waveformData[i] = levels[0];
                waveformData[i + 1] = levels[1];

                if (levels[0] > maxLeftPointLevel)
                {
                    maxLeftPointLevel = levels[0];
                }
                if (levels[1] > maxRightPointLevel)
                {
                    maxRightPointLevel = levels[1];
                }

                if (i > waveMaxPointIndexes[currentPointIndex])
                {
                    //waveformCompressedPoints[(currentPointIndex * 2)] = maxLeftPointLevel;
                    //waveformCompressedPoints[(currentPointIndex * 2) + 1] = maxRightPointLevel;
                    peakList.Add((-maxLeftPointLevel, maxRightPointLevel));
                    maxLeftPointLevel = float.MinValue;
                    maxRightPointLevel = float.MinValue;
                    currentPointIndex++;
                }
            }

            ManagedBass.Bass.StreamFree(stream);

            return peakList;
        }

        public List<(float min, float max)> GenerateAudioData(Stream stream) =>
            throw new NotImplementedException();
    }
}