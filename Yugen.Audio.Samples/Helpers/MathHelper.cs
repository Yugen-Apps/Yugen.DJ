using System;

namespace Yugen.Audio.Samples.Helpers
{
    public static class MathHelper
    {
        public static double ConvertAngleToRadians(double angle) =>
            Math.PI / 180 * angle;

        public static double ConvertRadiansToAngle(double radians) =>
            180 / Math.PI * radians;
    }
}