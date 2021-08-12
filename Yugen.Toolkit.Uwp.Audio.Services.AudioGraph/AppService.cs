﻿using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.AudioGraph
{
    public class AppService : IAppService
    {
        private readonly IAudioPlaybackFactory _audioPlaybackFactory;
        private readonly IAudioPlaybackService _left;
        private readonly IAudioPlaybackService _right;

        public AppService(IAudioPlaybackFactory audioPlaybackFactory)
        {
            _audioPlaybackFactory = audioPlaybackFactory;

            _left = _audioPlaybackFactory.Create(Side.Left);
            _right = _audioPlaybackFactory.Create(Side.Right);
        }

        public IAudioPlaybackService AudioPlaybackService(Side side) => side == Side.Left ? _left : _right;
    }
}