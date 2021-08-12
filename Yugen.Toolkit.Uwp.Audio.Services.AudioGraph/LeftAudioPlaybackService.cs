using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.AudioGraph
{
    public class LeftAudioPlaybackService : AudioPlaybackService
    {
        public LeftAudioPlaybackService(
            IAudioDeviceService audioDeviceService,
            IAudioGraphService masterAudioGraphService,
            IAudioGraphService headphonesAudioGraphService)
            : base(audioDeviceService, masterAudioGraphService, headphonesAudioGraphService)
        {
        }
    }
}