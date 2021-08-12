using System;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.AudioGraph
{
    public class AudioPlaybackFactory : IAudioPlaybackFactory
    {
        private readonly Func<LeftAudioPlaybackService> _left;
        private readonly Func<RightAudioPlaybackService> _right;

        public AudioPlaybackFactory(
            Func<LeftAudioPlaybackService> left,
            Func<RightAudioPlaybackService> right)
        {
            _left = left;
            _right = right;
        }

        public IAudioPlaybackService Create(Side type)
        {
            switch (type)
            {
                case Side.Left:
                    return _left();

                case Side.Right:
                    return _right();

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}