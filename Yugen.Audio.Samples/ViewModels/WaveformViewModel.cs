using ManagedBass;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Shapes;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Audio.Waveform.Services;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.Audio.Samples.ViewModels
{
    public class WaveformViewModel : ViewModelBase
    {
        private readonly IWaveformService _waveformService;
        private List<(float min, float max)> _peakList;
        private List<(float min, float max)> _peakList2;
        private bool _swap;

        public WaveformViewModel(IWaveformService waveformRendererService)
        {
            _waveformService = waveformRendererService;

            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);

            Bass.Init(-1);
        }

        public ICommand OpenCommand { get; }

        public List<(float min, float max)> PeakList
        {
            get => _peakList;
            set => SetProperty(ref _peakList, value);
        }

        public List<(float min, float max)> PeakList2
        {
            get => _peakList2;
            set => SetProperty(ref _peakList2, value);
        }

        private async Task OpenCommandBehavior()
        {
            var audioFile = await FilePickerHelper.OpenFile(
                     new List<string> { ".mp3" },
                     PickerLocationId.MusicLibrary
                 );

            if (audioFile != null)
            {
                _swap = !_swap;
                if (_swap)
                {
                    var audioStream = await audioFile.OpenStreamForReadAsync();
                    await NaudioGenerateAudioData(audioStream);
                }
                else
                {
                    var audioBytes = await audioFile.ReadBytesAsync();
                    BassGenerateAudioData(audioBytes);
                }
            }
        }

        private async Task NaudioGenerateAudioData(Stream audioStream)
        {
            List<(float min, float max)> peakList = null;

            await Task.Run(() =>
            {
                peakList = _waveformService.Render(audioStream);
            });

            PeakList = peakList;
        }

        private void BassGenerateAudioData(byte[] audioBytes)
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

            int stream = Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Decode | BassFlags.Float | BassFlags.Prescan);
            int frameLength = (int)Bass.ChannelSeconds2Bytes(stream, 0.02);
            long streamLength = Bass.ChannelGetLength(stream, 0);
            int frameCount = (int)((double)streamLength / (double)frameLength);
            int waveformLength = frameCount * 2;
            float[] waveformData = new float[waveformLength];
            float[] levels;

            int actualPoints = Math.Min(waveformCompressedPointCount, frameCount);

            int compressedPointCount = actualPoints * 2;
            //float[] waveformCompressedPoints = new float[compressedPointCount];
            List<int> waveMaxPointIndexes = new List<int>();
            for (int i = 1; i <= actualPoints; i++)
            {
                waveMaxPointIndexes.Add((int)Math.Round(waveformLength * ((double)i / (double)actualPoints), 0));
            }

            float maxLeftPointLevel = float.MinValue;
            float maxRightPointLevel = float.MinValue;
            int currentPointIndex = 0;
            for (int i = 0; i < waveformLength; i += 2)
            {
                levels = Bass.ChannelGetLevel(stream, 0.02f, LevelRetrievalFlags.Stereo);

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

            Bass.StreamFree(stream);

            ////


            PeakList2 = peakList;
        }
    }
}
