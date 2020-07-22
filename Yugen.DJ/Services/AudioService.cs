using AudioVisualizer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Yugen.DJ.DependencyInjection;
using Yugen.DJ.Interfaces;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.DJ.Services
{
    public class AudioService : IAudioService
    {
        private readonly IAudioDeviceService _audioDeviceService;
        private StorageFile file;

        private AudioGraph masterAudioGraph;
        private AudioDeviceOutputNode masterDeviceOutput;
        private AudioFileInputNode masterFileInput;

        private AudioGraph headphonesAudioGraph;
        private AudioDeviceOutputNode headphonesDeviceOutput;
        private AudioFileInputNode headphonesFileInput;

        public AudioService()
        {
            _audioDeviceService = Ioc.Default.GetService<IAudioDeviceService>();
        }

        public event EventHandler<TimeSpan> PositionChanged;

        public TimeSpan NaturalDuration => masterFileInput?.Duration ?? new TimeSpan();

        public async Task Init()
        {
            await InitMasterDevice();
            await InitHeadphonesDevice();
        }

        public async Task OpenFile()
        {
            file = await FilePickerHelper.OpenFile(
                new List<string> { ".mp3" },
                Windows.Storage.Pickers.PickerLocationId.MusicLibrary);

            if (file == null)
                return;
            MusicProperties musicProps = await file.Properties.GetMusicPropertiesAsync();
            
            DisposeFileInputs();

            await AddFileToMasterDevice();
            await AddFileToHeadsetDevice();

            AddFileInputToAudioVisualizer();

            //AddAudioFrameInputNode();
        }

        public void TogglePlay(bool isPaused)
        {
            if (isPaused)
            {
                masterAudioGraph?.Stop();
                headphonesAudioGraph?.Stop();
            }
            else
            {
                masterAudioGraph?.Start();
                headphonesAudioGraph?.Start();
            }
        }

        public void ChangePitch(double pitch)
        {
            var ratio = pitch == 0 ? 0 : pitch / 100;

            if (masterFileInput != null)
            {
                masterFileInput.PlaybackSpeedFactor = 1 + ratio;
            }

            if (headphonesFileInput != null)
            {
                headphonesFileInput.PlaybackSpeedFactor = 1 + ratio;
            }
        }

        public void ChangeVolume(double volume, double fader)
        {
            volume *= fader / 100;

            masterDeviceOutput.OutgoingGain = volume;

            //if (headphonesDeviceOutput != null)
            //{
            //    headphonesDeviceOutput.OutgoingGain = volume;
            //}
        }

        public void IsHeadphones(bool isHeadphone)
        {
            if (headphonesDeviceOutput != null)
            {
                headphonesDeviceOutput.OutgoingGain = isHeadphone ? 1 : 0;
            }
        }

        private async Task InitMasterDevice()
        {
            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media)
            {
                PrimaryRenderDevice = _audioDeviceService.MasterAudioDeviceInformation
            };

            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            if (result.Status != AudioGraphCreationStatus.Success)
                return;

            masterAudioGraph = result.Graph;
            masterAudioGraph.QuantumProcessed += Graph_QuantumProcessed;

            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await masterAudioGraph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
                return;

            masterDeviceOutput = deviceOutputNodeResult.DeviceOutputNode;
        }

        private async Task InitHeadphonesDevice()
        {
            if (_audioDeviceService.HeadphonesAudioDeviceInformation == null)
                return;

            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media)
            {
                PrimaryRenderDevice = _audioDeviceService.HeadphonesAudioDeviceInformation
            };

            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            if (result.Status != AudioGraphCreationStatus.Success)
                return;

            headphonesAudioGraph = result.Graph;

            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await headphonesAudioGraph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
                return;

            headphonesDeviceOutput = deviceOutputNodeResult.DeviceOutputNode;
        }

        private void Graph_QuantumProcessed(AudioGraph sender, object args)
        {
            var a = _source?.Source?.GetData();
            PositionChanged?.Invoke(sender, masterFileInput?.Position ?? new TimeSpan());
        }

        /// <summary>
        /// If another file is already loaded into the FileInput node
        /// Stop playback since a new file is being loaded.
        /// Release the file and dispose the contents of the node
        /// </summary>
        /// <returns></returns>
        private void DisposeFileInputs()
        {
            masterAudioGraph?.Stop();
            headphonesAudioGraph?.Stop();

            masterFileInput?.Dispose();
            headphonesFileInput?.Dispose();
        }

        private async Task AddFileToMasterDevice()
        {
            CreateAudioFileInputNodeResult masterFileInputResult = await masterAudioGraph.CreateFileInputNodeAsync(file);
            if (AudioFileNodeCreationStatus.Success != masterFileInputResult.Status)
                return;

            masterFileInput = masterFileInputResult.FileInputNode;
            masterFileInput.AddOutgoingConnection(masterDeviceOutput);
        }

        private async Task AddFileToHeadsetDevice()
        {
            if (headphonesAudioGraph == null)
                return;

            CreateAudioFileInputNodeResult headphonesFileInputResult = await headphonesAudioGraph.CreateFileInputNodeAsync(file);
            if (AudioFileNodeCreationStatus.Success != headphonesFileInputResult.Status)
                return;

            headphonesFileInput = headphonesFileInputResult.FileInputNode;
            headphonesFileInput.AddOutgoingConnection(headphonesDeviceOutput);
        }


        private PlaybackSource _source;
        private SpectrumVisualizer _spectrumVisualizer;
        private DiscreteVUBar _leftVUBarChanel0;
        private DiscreteVUBar _leftVUBarChanel1;

        public void AddAudioVisualizer(SpectrumVisualizer spectrumVisualizer)
        {
            _spectrumVisualizer = spectrumVisualizer;
        }

        public void AddAudioVisualizer(DiscreteVUBar leftVUBarChanel0, DiscreteVUBar leftVUBarChanel1)
        {
            _leftVUBarChanel0 = leftVUBarChanel0;
            _leftVUBarChanel1 = leftVUBarChanel1;
        }

        private void AddFileInputToAudioVisualizer()
        {
            if (masterFileInput == null)
                return;

            _source = PlaybackSource.CreateFromAudioNode(masterFileInput);
            
            _spectrumVisualizer.Source = _source.Source;
            _leftVUBarChanel0.Source = _source.Source;
            _leftVUBarChanel1.Source = _source.Source;
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

    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }
}