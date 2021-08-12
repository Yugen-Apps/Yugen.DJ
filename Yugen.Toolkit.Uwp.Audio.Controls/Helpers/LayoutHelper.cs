using System.Numerics;
using Windows.Foundation;

namespace Yugen.Toolkit.Uwp.Audio.Controls.Helpers
{
    public static class LayoutHelper
    {
        public static void CalculateLayout(Size size, float width, float height, out Matrix3x2 counterTransform)
        {
            // Horizontal Layout
            if (size.Width > size.Height)
            {
                float targetWidth = (float)size.Width / 2;
                float targetHeight = (float)size.Height;

                float scaleFactor = targetWidth / width;

                if (height * scaleFactor > targetHeight)
                {
                    scaleFactor = targetHeight / height;
                }

                float yoffset = targetHeight / 2 - height * scaleFactor / 2;
                counterTransform = Matrix3x2.CreateScale(scaleFactor, scaleFactor) * Matrix3x2.CreateTranslation(0, yoffset);
            }
            else
            {
                float targetWidth = (float)size.Width;
                float targetHeight = (float)size.Height / 2;

                float scaleFactor = targetHeight / height;

                if (width * scaleFactor > targetWidth)
                {
                    scaleFactor = targetWidth / width;
                }

                float xoffset = targetWidth / 2 - height * scaleFactor / 2;
                counterTransform = Matrix3x2.CreateScale(scaleFactor, scaleFactor) * Matrix3x2.CreateTranslation(xoffset, 0);
            }
        }

        //    public static Matrix3x2 CalculateLayout(Size size, float width, float height)
        //    {
        //        var targetWidth = (float)size.Width;
        //        var targetHeight = (float)size.Height;
        //        var scaleFactor = targetWidth / width;

        //        if (height * scaleFactor > targetHeight)
        //        {
        //            scaleFactor = targetHeight / height;
        //        }

        //        return Matrix3x2.CreateScale(scaleFactor, scaleFactor);
        //    }
    }
}