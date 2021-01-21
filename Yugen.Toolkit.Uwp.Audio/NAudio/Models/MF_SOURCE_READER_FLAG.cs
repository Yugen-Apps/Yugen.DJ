using System;

namespace Yugen.Toolkit.Uwp.Audio.NAudio.Models
{
    /// <summary>
    /// Contains flags that indicate the status of the IMFSourceReader::ReadSample method
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/dd375773(v=vs.85).aspx
    /// </summary>
    [Flags]
    public enum MF_SOURCE_READER_FLAG
    {
        /// <summary>
        /// No Error
        /// </summary>
        None = 0,
        /// <summary>
        /// An error occurred. If you receive this flag, do not make any further calls to IMFSourceReader methods.
        /// </summary>
        MF_SOURCE_READERF_ERROR = 0x00000001,
        /// <summary>
        /// The source reader reached the end of the stream.
        /// </summary>
        MF_SOURCE_READERF_ENDOFSTREAM = 0x00000002,
        /// <summary>
        /// One or more new streams were created
        /// </summary>
        MF_SOURCE_READERF_NEWSTREAM = 0x00000004,
        /// <summary>
        /// The native format has changed for one or more streams. The native format is the format delivered by the media source before any decoders are inserted.
        /// </summary>
        MF_SOURCE_READERF_NATIVEMEDIATYPECHANGED = 0x00000010,
        /// <summary>
        /// The current media has type changed for one or more streams. To get the current media type, call the IMFSourceReader::GetCurrentMediaType method.
        /// </summary>
        MF_SOURCE_READERF_CURRENTMEDIATYPECHANGED = 0x00000020,
        /// <summary>
        /// There is a gap in the stream. This flag corresponds to an MEStreamTick event from the media source.
        /// </summary>
        MF_SOURCE_READERF_STREAMTICK = 0x00000100,
        /// <summary>
        /// All transforms inserted by the application have been removed for a particular stream.
        /// </summary>
        MF_SOURCE_READERF_ALLEFFECTSREMOVED = 0x00000200
    }
}
