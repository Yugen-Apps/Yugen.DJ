using System;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
{
    public class MixerService : IMixerService
    {
        private readonly IAudioPlaybackServiceProvider _audioPlaybackServiceProvider;
        private readonly IAudioPlaybackService _leftAudioPlaybackService;
        private readonly IAudioPlaybackService _rightAudioPlaybackService;

        private double _leftFader = 0.5;
        private double _rightFader = 0.5;
        private double _leftVolume = 100;
        private double _rightVolume = 100;

        public MixerService(IAudioPlaybackServiceProvider audioPlaybackServiceProvider)
        {
            _audioPlaybackServiceProvider = audioPlaybackServiceProvider;
            _leftAudioPlaybackService = audioPlaybackServiceProvider.Get(Side.Left);
            _rightAudioPlaybackService = audioPlaybackServiceProvider.Get(Side.Right);

            _leftAudioPlaybackService.RmsChanged += (sender, e) => LeftRmsChanged?.Invoke(sender, e);
            _rightAudioPlaybackService.RmsChanged += (sender, e) => RightRmsChanged?.Invoke(sender, e);
        }

        public event EventHandler<float> LeftRmsChanged;
        public event EventHandler<float> RightRmsChanged;

        public void ChangeVolume(double volume, Side side)
        {
            if (side == Side.Left)
            {
                _leftVolume = volume;
            }
            else
            {
                _rightVolume = volume;
            }

            UpdateVolume();
        }

        public void IsHeadphones(bool isHeadPhones, Side side)
        {
            _audioPlaybackServiceProvider.Get(side)?.IsHeadphones(isHeadPhones);
        }

        public void SetFader(double crossFader)
        {
            var absoluteValue = 20 - (crossFader + 10);
            var percentace = 100 * absoluteValue / 20;
            _leftFader = percentace / 100;

            absoluteValue = crossFader + 10;
            percentace = 100 * absoluteValue / 20;
            _rightFader = percentace / 100;

            UpdateVolume();
        }

        private void UpdateVolume()
        {
            _leftAudioPlaybackService?.ChangeVolume(_leftVolume, _leftFader);
            _rightAudioPlaybackService?.ChangeVolume(_rightVolume, _rightFader);
        }
    }
}