using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.AudioGraph
{
    public class AudioGraphService : IAudioGraphService
    {
        private Windows.Media.Audio.AudioGraph _audioGraph;
        private AudioDeviceOutputNode _deviceOutput;

        public event EventHandler<TimeSpan> PositionChanged;

        public AudioFileInputNode AudioFileInput { get; private set; }

        public async Task InitDevice(DeviceInformation audioDeviceInformation, bool isMaster)
        {
            if (audioDeviceInformation == null)
                return;

            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media)
            {
                PrimaryRenderDevice = audioDeviceInformation
            };

            CreateAudioGraphResult result = await Windows.Media.Audio.AudioGraph.CreateAsync(settings);
            if (result.Status != AudioGraphCreationStatus.Success)
                return;

            _audioGraph = result.Graph;
            if (isMaster)
            {
                _audioGraph.QuantumProcessed += OnQuantumProcessed;
            }

            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await _audioGraph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
                return;

            _deviceOutput = deviceOutputNodeResult.DeviceOutputNode;
        }

        public async Task AddFileToDevice(StorageFile audioFile)
        {
            if (_audioGraph == null)
                return;

            CreateAudioFileInputNodeResult fileInputResult = await _audioGraph.CreateFileInputNodeAsync(audioFile);
            if (AudioFileNodeCreationStatus.Success != fileInputResult.Status)
                return;

            AudioFileInput = fileInputResult.FileInputNode;
            AudioFileInput.AddOutgoingConnection(_deviceOutput);
        }

        public void TogglePlay(bool isPaused)
        {
            if (isPaused)
            {
                _audioGraph?.Stop();
            }
            else
            {
                _audioGraph?.Start();
            }
        }

        public void ChangePitch(double ratio)
        {
            if (AudioFileInput != null)
            {
                AudioFileInput.PlaybackSpeedFactor = 1 + ratio;
            }
        }

        public void ChangeVolume(double volume)
        {
            if (_deviceOutput != null)
            {
                _deviceOutput.OutgoingGain = volume;
            }
        }

        public void IsHeadphones(bool isHeadphone)
        {
            if (_deviceOutput != null)
            {
                _deviceOutput.OutgoingGain = isHeadphone ? 1 : 0;
            }
        }

        /// <summary>
        /// If another file is already loaded into the FileInput node
        /// Stop playback since a new file is being loaded.
        /// Release the file and dispose the contents of the node
        /// </summary>
        /// <returns></returns>
        public void DisposeFileInputs()
        {
            _audioGraph?.Stop();
            AudioFileInput?.Dispose();
        }

        private void OnQuantumProcessed(Windows.Media.Audio.AudioGraph sender, object args) =>
                                                            PositionChanged?.Invoke(sender, AudioFileInput?.Position ?? new TimeSpan());
    }
}