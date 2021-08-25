using System.Numerics;
using Windows.Foundation;

namespace Yugen.Toolkit.Uwp.Audio.Controls.Helpers
{
    public static class LayoutHelper
    {
        public static void CalculateLayout(Size size, double width, double height, out Matrix3x2 counterTransform)
        {
            float targetWidth = (float)size.Width;
            float targetHeight = (float)size.Height;

            float xScaleFactor = (float)(targetWidth / width);
            float yScaleFactor = (float)(targetHeight / height);

            float xoffset = targetWidth - (targetWidth * xScaleFactor);
            float yoffset = targetHeight - (targetHeight * yScaleFactor);

            counterTransform = Matrix3x2.CreateScale(new Vector2(xScaleFactor, yScaleFactor), new Vector2(xoffset, yoffset));
        }
    }
}