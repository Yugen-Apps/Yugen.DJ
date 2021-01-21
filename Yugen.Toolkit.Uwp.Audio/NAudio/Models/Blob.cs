using System;

namespace Yugen.Toolkit.Uwp.Audio.NAudio.Models
{
    /// <summary>
    /// Representation of binary large object container.
    /// </summary>
    public struct Blob
    {
        /// <summary>
        /// Length of binary object.
        /// </summary>
        public int Length;
        /// <summary>
        /// Pointer to buffer storing data.
        /// </summary>
        public IntPtr Data;
    }

}
