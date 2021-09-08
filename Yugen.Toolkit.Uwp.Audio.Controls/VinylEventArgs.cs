using System;

namespace Yugen.Toolkit.Uwp.Audio.Controls
{
    public class VinylEventArgs : EventArgs
    {
        public VinylEventArgs(bool isTouched, bool isClockwise, float crossProduct)
        {
            IsTouched = isTouched;
            IsClockwise = isClockwise;
            CrossProduct = crossProduct;
        }

        public bool IsTouched { get; set; }

        public bool IsClockwise { get; set; }

        public float CrossProduct { get; set; }
    }
}