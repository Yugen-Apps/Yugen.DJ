using System;
using System.Runtime.InteropServices;

namespace WASAPI.NET.Com
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    internal struct WaveFormat
    {
        public WaveFormatEncodingEnum WaveFormatTag;
        public short Channels;
        public int SampleRate;
        public int AverageBytesPerSecond;
        public short BlockAlign;
        public short BitsPerSample;
        public short ExtraSize;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    internal struct WaveFormatEx
    {
        public const int WaveFormatExExtraSize = 22;

        public WaveFormatEncodingEnum WaveFormatTag;
        public short Channels;
        public int SampleRate;
        public int AverageBytesPerSecond;
        public short BlockAlign;
        public short BitsPerSample;
        public short ExtraSize;
        public short SamplesUnion;
        public ChannelMask ChannelMask;
        public Guid SubFormat;
    }
}