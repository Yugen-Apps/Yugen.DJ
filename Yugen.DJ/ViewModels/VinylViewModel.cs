using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Devices;
using Windows.Media.Render;
using Windows.Storage;
using Yugen.Toolkit.Standard.Mvvm.ComponentModel;
using Yugen.Toolkit.Standard.Mvvm.Input;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.DJ.ViewModels
{
    public class VinylViewModel : ViewModelBase
    {
        public int[] TargetElapsedTimeList = {
            10000,
            25000,
            50000,
            75000,
            100000,
            250000,
            500000,
            750000
        };

        private bool _isLeft;
        private bool _isHeadPhones;
        private bool _isPaused = true;
        private double _volume = 100;
        private double _fader = 0;
        private double _pitch = 0;
        private TimeSpan _targetElapsedTime = new TimeSpan(10000);
        private TimeSpan _naturalDuration = new TimeSpan();
        private TimeSpan _position = new TimeSpan();
        private ICommand _openButtonCommand;
        private DeviceInformation _masterAudioDeviceInformation;
        private DeviceInformation _headphonesAudioDeviceInformation;

        private AudioGraph graph;
        private AudioGraph graph2;
        private AudioFileInputNode fileInput;
        private AudioDeviceOutputNode deviceOutput;
        private AudioDeviceOutputNode deviceOutput2;

        public VinylViewModel(bool isLeft)
        {
            _isLeft = isLeft;
        }

        public void Init(DeviceInformation masterAudioDeviceInformation, DeviceInformation headphonesAudioDeviceInformation)
        {
            _masterAudioDeviceInformation = masterAudioDeviceInformation;
            _headphonesAudioDeviceInformation = headphonesAudioDeviceInformation;

            UpdateSettings();
        }

        private async void UpdateSettings()
        {
            //if (fileInput != null)
            //{
            //    // Release the file and dispose the contents of the node
            //    fileInput.Dispose();
            //    // Stop playback since a new file is being loaded.
            //    if (_isPaused)
            //    {
            //        graph.Stop();
            //    }
            //}

            // Create an AudioGraph with default settings
            AudioGraphSettings settings = new AudioGraphSettings(AudioRenderCategory.Media);
            settings.PrimaryRenderDevice = _headphonesAudioDeviceInformation;

            //if (_isHeadPhones)
            //{
            //    settings.PrimaryRenderDevice = _headphonesAudioDeviceInformation;
            //}
            //else
            //{
            //    settings.PrimaryRenderDevice = _masterAudioDeviceInformation;
            //}

            CreateAudioGraphResult result = await AudioGraph.CreateAsync(settings);
            if (result.Status != AudioGraphCreationStatus.Success)
            {
                return;
            }

            graph = result.Graph;
            graph.QuantumProcessed += Graph_QuantumProcessed;

            // Create a device output node
            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult = await graph.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return;
            }
            deviceOutput = deviceOutputNodeResult.DeviceOutputNode;

            AudioGraphSettings settings2 = new AudioGraphSettings(AudioRenderCategory.Media);
            settings2.PrimaryRenderDevice = _masterAudioDeviceInformation;
            CreateAudioGraphResult result2 = await AudioGraph.CreateAsync(settings2);
            if (result2.Status != AudioGraphCreationStatus.Success)
            {
                return;
            }
            graph2 = result2.Graph;
            CreateAudioDeviceOutputNodeResult deviceOutputNodeResult2 = await graph2.CreateDeviceOutputNodeAsync();
            if (deviceOutputNodeResult2.Status != AudioDeviceNodeCreationStatus.Success)
            {
                return;
            }
            deviceOutput2 = deviceOutputNodeResult2.DeviceOutputNode;

            //if (fileInput != null && file != null)
            //{
            //    CreateAudioFileInputNodeResult fileInputResult = await graph.CreateFileInputNodeAsync(file);
            //    if (AudioFileNodeCreationStatus.Success != fileInputResult.Status)
            //    {
            //        return;
            //    }

            //    fileInput = fileInputResult.FileInputNode;

            //    NaturalDuration = fileInput.Duration;

            //    fileInput.AddOutgoingConnection(deviceOutput);
            //}
        }

        private void Graph_QuantumProcessed(AudioGraph sender, object args)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                Position = fileInput.Position;
            });
        }

        public bool IsPaused
        {
            get { return _isPaused; }
            set
            {
                Set(ref _isPaused, value);

                if (_isPaused)
                {
                    graph.Stop();
                    graph2.Stop();
                }
                else
                {
                    graph.Start();
                    graph2.Start();
                }
            }
        }

        public bool IsHeadPhones
        {
            get { return _isHeadPhones; }
            set
            {
                Set(ref _isHeadPhones, value);

                UpdateSettings();
            }
        }

        public double Volume
        {
            get { return _volume; }
            set
            {
                Set(ref _volume, value);

                SetVolume();
            }
        }

        public double Fader
        {
            get { return _fader; }
            set
            {
                Set(ref _fader, value);

                SetVolume();
            }
        }

        public double Pitch
        {
            get { return _pitch; }
            set
            {
                Set(ref _pitch, value);

                var ratio = _pitch == 0 ? 0 : _pitch / 100;

                fileInput.PlaybackSpeedFactor = 1 + ratio;
            }
        }

        public TimeSpan TargetElapsedTime
        {
            get { return _targetElapsedTime; }
            set { Set(ref _targetElapsedTime, value); }
        }

        public int SelectedTargetElapsedTime
        {
            get => (int)TargetElapsedTime.Ticks;
            set => TargetElapsedTime = new TimeSpan(value);
        }

        public TimeSpan NaturalDuration
        {
            get { return _naturalDuration; }
            set { Set(ref _naturalDuration, value); }
        }    
        
        public TimeSpan Position
        {
            get { return _position; }
            set { Set(ref _position, value); }
        }

        public ICommand OpenButtonCommand => _openButtonCommand
            ?? (_openButtonCommand = new AsyncRelayCommand(OpenButtonCommandBehavior));

        private StorageFile file;

        private async Task OpenButtonCommandBehavior()
        {
            // If another file is already loaded into the FileInput node
            if (fileInput != null)
            {
                // Release the file and dispose the contents of the node
                fileInput.Dispose();
                // Stop playback since a new file is being loaded.
                if (_isPaused)
                {
                    graph.Stop();
                    graph2.Stop();
                }
            }

            file = await FilePickerHelper.OpenFile(
                new List<string> { ".mp3" },
                Windows.Storage.Pickers.PickerLocationId.MusicLibrary);

            if (file == null)
            {
                return;
            }

            CreateAudioFileInputNodeResult fileInputResult = await graph.CreateFileInputNodeAsync(file);
            if (AudioFileNodeCreationStatus.Success != fileInputResult.Status)
            {
                return;
            }
            fileInput = fileInputResult.FileInputNode;

            NaturalDuration = fileInput.Duration;
            
            fileInput.AddOutgoingConnection(deviceOutput);

            CreateAudioFileInputNodeResult fileInputResult2 = await graph2.CreateFileInputNodeAsync(file);
            if (AudioFileNodeCreationStatus.Success != fileInputResult2.Status)
            {
                return;
            }
            var fileInput2 = fileInputResult2.FileInputNode;
            fileInput2.AddOutgoingConnection(deviceOutput2);
        }

        private void SetVolume()
        {
            var volume = _volume * _fader;

            deviceOutput.OutgoingGain = volume / 100;
        }
    }
}