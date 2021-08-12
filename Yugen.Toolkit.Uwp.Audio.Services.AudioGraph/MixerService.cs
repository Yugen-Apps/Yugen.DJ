using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.AudioGraph
{
    public class MixerService : IMixerService
    {
        //private double _fader = 0.5;
        private double _leftFader = 0.5;

        private double _rightFader = 0.5;
        private double _leftVolume = 100;
        private double _rightVolume = 100;

        public IAudioPlaybackService LeftAudioPlaybackService { get; set; }

        public IAudioPlaybackService RightAudioPlaybackService { get; set; }

        public void IsHeadphones(bool isHeadPhones, Side side) 
        {
            if (side == Side.Left)
            {
                LeftAudioPlaybackService?.IsHeadphones(isHeadPhones);
            }
            else
            {
                RightAudioPlaybackService?.IsHeadphones(isHeadPhones);
            }
        }

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
            LeftAudioPlaybackService?.ChangeVolume(_leftVolume, _leftFader);
            RightAudioPlaybackService?.ChangeVolume(_rightVolume, _rightFader);
        }
    }
}