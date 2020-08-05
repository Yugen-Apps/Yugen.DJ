using NAudio.MediaFoundation;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Yugen.DJ.Audio.WaveForm
{
    /// <summary>
    /// Class for reading any file that Media Foundation can play
    /// Will only work in Windows Vista and above
    /// Automatically converts to PCM
    /// If it is a video file with multiple audio streams, it will pick out the first audio stream
    /// </summary>
    public class MyMediaFoundationReader : MediaFoundationReader
    {
        private readonly Stream stream;

        /// <summary>
        /// Creates a new MediaFoundationReader based on the supplied file
        /// </summary>
        /// <param name="stream">Stream</param>
        public MyMediaFoundationReader(Stream stream)
            : this(stream, null)
        {
        }

        /// <summary>
        /// Creates a new MediaFoundationReader based on the supplied file
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="settings">Advanced settings</param>
        public MyMediaFoundationReader(Stream stream, MediaFoundationReaderSettings settings)
        {
            this.stream = stream;
            Init(settings);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        protected MyMediaFoundationReader()
        {
        }

        /// <summary>
        /// Creates the reader (overridable by )
        /// </summary>
        protected override IMFSourceReader CreateReader(MediaFoundationReaderSettings settings)
        {
            MediaFoundationInterop.MFCreateMFByteStreamOnStreamEx(stream.AsRandomAccessStream(), out IMFByteStream byteStream);
            MediaFoundationInterop.MFCreateSourceReaderFromByteStream(byteStream, null, out IMFSourceReader reader);
            Marshal.ReleaseComObject(byteStream);

            reader.SetStreamSelection(MediaFoundationInterop.MF_SOURCE_READER_ALL_STREAMS, false);
            reader.SetStreamSelection(MediaFoundationInterop.MF_SOURCE_READER_FIRST_AUDIO_STREAM, true);

            // Create a partial media type indicating that we want uncompressed PCM audio
            var partialMediaType = new MediaType
            {
                MajorType = MediaTypes.MFMediaType_Audio,
                SubType = settings.RequestFloatOutput ? AudioSubtypes.MFAudioFormat_Float : AudioSubtypes.MFAudioFormat_PCM
            };

            var currentMediaType = GetCurrentMediaType(reader);

            // mono, low sample rate files can go wrong on Windows 10 unless we specify here
            partialMediaType.ChannelCount = currentMediaType.ChannelCount;
            partialMediaType.SampleRate = currentMediaType.SampleRate;

            try
            {
                // set the media type
                // can return MF_E_INVALIDMEDIATYPE if not supported
                reader.SetCurrentMediaType(MediaFoundationInterop.MF_SOURCE_READER_FIRST_AUDIO_STREAM, IntPtr.Zero, partialMediaType.MediaFoundationObject);
            }
            catch (COMException ex) when (ex.GetHResult() == MediaFoundationErrors.MF_E_INVALIDMEDIATYPE)
            {
                // HE-AAC (and v2) seems to halve the samplerate
                if (currentMediaType.SubType == AudioSubtypes.MFAudioFormat_AAC && currentMediaType.ChannelCount == 1)
                {
                    partialMediaType.SampleRate = currentMediaType.SampleRate *= 2;
                    partialMediaType.ChannelCount = currentMediaType.ChannelCount *= 2;
                    reader.SetCurrentMediaType(MediaFoundationInterop.MF_SOURCE_READER_FIRST_AUDIO_STREAM, IntPtr.Zero, partialMediaType.MediaFoundationObject);
                }
                else { throw; }
            }

            Marshal.ReleaseComObject(currentMediaType.MediaFoundationObject);
            return reader;
        }

        private static MediaType GetCurrentMediaType(IMFSourceReader reader)
        {
            reader.GetCurrentMediaType(MediaFoundationInterop.MF_SOURCE_READER_FIRST_AUDIO_STREAM, out IMFMediaType mediaType);
            return new MediaType(mediaType);
        }
    }
}