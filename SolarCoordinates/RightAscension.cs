using System;

namespace SolarCoordinates
{
    public struct RightAscension
    {
        public double Degrees { get; }

        /// <summary>
        /// Given a = 9h14m55.8s we convert to hours and decimals:
        ///     9 + 14/60 + 55.8 / 3600 = 9.248833333 hours
        /// Then:
        ///     Multiply by 15 so that a = 138.73250 degrees
        /// Convert to radians:
        ///     a * (pi / 180) = 0.174532925
        /// </summary>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        public RightAscension(int hours, int minutes, double seconds)
        {
            Degrees = (hours + (minutes / 60) + (seconds / 3600)) * 15;
        }

        public RightAscension(double degrees)
        {
            if (degrees < 0)
            {
                degrees += 360;
            }

            Degrees = degrees;
        }

        public double ToRadians()
        {
            return Degrees * (Math.PI / 180);
        }

        public override string ToString()
        {
            double d = (Degrees / 15);
            double h = Math.Floor(d);

            double i = d.GetIntegral();
            d = i * 60;
            double m = Math.Floor(d);

            i = d.GetIntegral();
            d = i * 60;
            double s = Math.Floor(d);

            return string.Format("{0}h {1}m {2}s", h, m, s);
        }
    }
}
