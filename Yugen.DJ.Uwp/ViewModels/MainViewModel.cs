using ManagedBass;
using ManagedBass.Fx;
using ManagedBass.Mix;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Yugen.Toolkit.Standard.Mvvm;
using Yugen.Toolkit.Uwp.Helpers;

namespace Yugen.DJ.Uwp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private int _mixer;
        private int _source1;
        private int _source2;

        private bool _isPlaying;
        private bool _isPlaying2;

        public MainViewModel()
        {
            Bass.Init();
            _mixer = BassMix.CreateMixerStream(44100, 2, 0);

            OpenCommand = new AsyncRelayCommand(OpenCommandBehavior);
            OpenCommand2 = new AsyncRelayCommand(OpenCommandBehavior2);
        }

        public ICommand OpenCommand { get; }

        public ICommand OpenCommand2 { get; }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (SetProperty(ref _isPlaying, value))
                {
                    if (_isPlaying)
                    {
                        BassMix.ChannelFlags(_source1, BassFlags.MixerChanPause, BassFlags.MixerChanPause);
                        //Bass.ChannelPlay(_source1);
                    }
                    else
                    {
                        BassMix.ChannelFlags(_source1, 0, BassFlags.MixerChanPause);
                        //Bass.ChannelPause(_source1);
                    }
                }
            }
        }

        public bool IsPlaying2
        {
            get => _isPlaying2;
            set
            {
                if (SetProperty(ref _isPlaying2, value))
                {
                    if (_isPlaying2)
                    {
                        BassMix.ChannelFlags(_source2, BassFlags.MixerChanPause, BassFlags.MixerChanPause);
                        //Bass.ChannelPlay(_source2);
                    }
                    else
                    {
                        BassMix.ChannelFlags(_source2, 0, BassFlags.MixerChanPause);
                        //Bass.ChannelPause(_source2);
                    }
                }
            }
        }

        private async Task OpenCommandBehavior()
        {
            var audioFile = await FilePickerHelper.OpenFile(
                    new List<string> { ".mp3" },
                    PickerLocationId.MusicLibrary
                );

            if (audioFile != null)
            {
                var audioBytes = await audioFile.ReadBytesAsync();

                _source1 = Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Decode); // create decoder for 1st file
                BassMix.MixerAddChannel(_mixer, _source1, 0); // add it to the mix
                Bass.ChannelPlay(_mixer);
            }
        }

        private async Task OpenCommandBehavior2()
        {
            var audioFile = await FilePickerHelper.OpenFile(
                    new List<string> { ".mp3" },
                    PickerLocationId.MusicLibrary
                );

            if (audioFile != null)
            {
                var audioBytes = await audioFile.ReadBytesAsync();

                _source2 = Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Decode); // create decoder for 1st file
                BassMix.MixerAddChannel(_mixer, _source2, 0); // add it to the mix
            }
        }
    }
}