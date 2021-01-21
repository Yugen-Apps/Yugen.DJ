using System.Runtime.InteropServices;

namespace Yugen.Toolkit.Uwp.Audio.NAudio.Extensions
{
    /// <summary>
    /// HResult
    /// </summary>
    public static class COMExceptionExtension
    {
        /// <summary>
        /// Helper to deal with the fact that in Win Store apps,
        /// the HResult property name has changed
        /// </summary>
        /// <param name="exception">COM Exception</param>
        /// <returns>The HResult</returns>
        public static int GetHResult(this COMException exception)
        {
            return exception.HResult;
        }
    }

}
