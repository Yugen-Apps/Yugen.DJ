using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;
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
            {
                return;
            }

            DisposeFileInputs();

            await AddFileToMasterDevice();
            await AddFileToHeadsetDevice();
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

        private async Task InitMasterDevice()
        {
            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media)
            {
                PrimaryRenderDevice = _audioDeviceService.MasterAudioDeviceInformation
            };

            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            if (result.Status != AudioGraphCreationStatus.Success)
            {
                return;
            }

            masterAudioGraph = result.Graph;
            masterAudioGraph.QuantumProcessed += Graph_QuantumProcessed;

            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await masterAudioGraph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return;
            }

            masterDeviceOutput = deviceOutputNodeResult.DeviceOutputNode;
        }

        private async Task InitHeadphonesDevice()
        {
            if (_audioDeviceService.HeadphonesAudioDeviceInformation == null)
            {
                return;
            }

            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media)
            {
                PrimaryRenderDevice = _audioDeviceService.HeadphonesAudioDeviceInformation
            };

            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            if (result.Status != AudioGraphCreationStatus.Success)
            {
                return;
            }

            headphonesAudioGraph = result.Graph;

            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await headphonesAudioGraph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return;
            }

            headphonesDeviceOutput = deviceOutputNodeResult.DeviceOutputNode;
        }

        private void Graph_QuantumProcessed(AudioGraph sender, object args)
        {
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
            {
                return;
            }

            masterFileInput = masterFileInputResult.FileInputNode;
            masterFileInput.AddOutgoingConnection(masterDeviceOutput);
        }

        private async Task AddFileToHeadsetDevice()
        {
            if (headphonesAudioGraph == null)
            {
                return;
            }

            CreateAudioFileInputNodeResult headphonesFileInputResult = await headphonesAudioGraph.CreateFileInputNodeAsync(file);
            if (AudioFileNodeCreationStatus.Success != headphonesFileInputResult.Status)
            {
                return;
            }

            headphonesFileInput = headphonesFileInputResult.FileInputNode;
            headphonesFileInput.AddOutgoingConnection(headphonesDeviceOutput);
        }
    }
}