using ManagedBass;
using ManagedBass.Fx;
using System;
using System.IO;
using Yugen.Toolkit.Uwp.Audio.Services.Abstractions;

namespace Yugen.Toolkit.Uwp.Audio.Services.Bass
{
    public class BPMService : IBPMService
    {
        public float BPM { get; private set; }

        public float Decoding(byte[] audioBytes)
        {
            var bpmchan = ManagedBass.Bass.CreateStream(audioBytes, 0, audioBytes.Length, BassFlags.Decode);

            // create bpmChan stream and get bpm value for BpmPeriod seconds from current position
            var positon = ManagedBass.Bass.ChannelGetPosition(bpmchan);
            var positionSeconds = ManagedBass.Bass.ChannelBytes2Seconds(bpmchan, positon);
            var length = ManagedBass.Bass.ChannelGetLength(bpmchan);
            var lengthSeconds = ManagedBass.Bass.ChannelBytes2Seconds(bpmchan, length);

            BPM = BassFx.BPMDecodeGet(bpmchan, 0, lengthSeconds, 0,
                                      BassFlags.FxBpmBackground | BassFlags.FXBpmMult2 | BassFlags.FxFreeSource,
                                      null);

            //double startSec = positionSeconds;
            //double endSec = positionSeconds + _bpmPeriod >= lengthSeconds
            //                ? lengthSeconds - 1
            //                : positionSeconds + _bpmPeriod;

            //BassFx.BPMCallbackSet(bpmchan, BPMCallback, _bpmPeriod, 0, BassFlags.FXBpmMult2);

            //// detect bpm in background and return progress in GetBPM_ProgressCallback function
            //if (bpmchan != 0)
            //{
            //    BpmLeft = BassFx.BPMDecodeGet(bpmchan, startSec, endSec, 0,
            //                                  BassFlags.FxBpmBackground | BassFlags.FXBpmMult2 | BassFlags.FxFreeSource,
            //                                  null);
            //}

            return BPM;
        }

        public float Decoding(Stream stream) => throw new NotImplementedException();

        private void BPMCallback(int Channel, float BPM, IntPtr User)
        {
            // TODO: add dispatcher update the bpm view
            //BpmLeft = BPM;
        }
    }
}