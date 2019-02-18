using System;
using System.Collections.Generic;

namespace SolarCoordinates
{
    /// <summary>
    /// Calculate the geocentric solar coordinates for one or more given moments in time
    /// down to an accuracy of about 1 arcminute if date is near the current J2000.0 epoch.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            IList<Moment> moments = ParseDateArgs(args);
            
            foreach(Moment moment in moments)
            {
                Console.WriteLine("Calculate solar coordinates for: " + moment.ToString());

                Coordinates c = CalculateSolarCoordinates(moment);

                Console.WriteLine("R.A.: " + c.RightAscension.ToString());
                Console.WriteLine("Dec: " + c.Declination.ToString());
                Console.WriteLine();
            }
                     
            Console.ReadLine();
        }

        private static Coordinates CalculateSolarCoordinates(Moment moment)
        {
            // Julian centuries of 36525 ephemeris days from the epoch J2000.0
            double T = moment.TimeT;

            // geometric mean longitude of the Sun referred to the mean equinox of T (25.2)
            double L0 = 280.46646 + (36000.76983 * T) + (0.0003032 * Math.Pow(T, 2));
            L0 = L0.CorrectDegreeRange();

            // mean anomaly of the Sun (25.3)
            double M = 357.52911 + (35999.05029 * T) - (0.0001537 * Math.Pow(T, 2));
            M = M.CorrectDegreeRange();

            // eccentricity of Earth's orbit (25.4)
            double e = 0.016708634 - (0.000042037 * T) - (0.0000001267 * Math.Pow(T, 2));

            // Sun's equation of center
            double C =
                +(1.914602 - (0.004817 * T) - (0.000014 * Math.Pow(T, 2))) * Math.Sin(M.ToRadians())
                + (0.019993 - (0.000101 * T)) * Math.Sin(M.ToRadians() * 2)
                + (0.000289 * Math.Sin(M.ToRadians() * 3));

            // Sun's true geometric longitude
            double Ltrue = (L0 + C);
            Ltrue = Ltrue.CorrectDegreeRange();

            // Sun's true anomaly
            double n = (M + C);
            n = n.CorrectDegreeRange();

            // U.S. Naval Observatory function for radius vector. Compare to Meeus (25.5)
            double R = 1.00014 - 0.01671 * Math.Cos(M.ToRadians()) - 0.00014 * Math.Cos(2 * M.ToRadians());

            // correction "omega" for nutation and aberration
            double O = 125.04 - (1934.136 * T);

            // apparent longitude L (lambda) of the Sun
            double Lapp = Ltrue - 0.00569 - (0.00478 * Math.Sin(O.ToRadians()));

            // obliquity of the ecliptic (22.2)
            double U = T / 100;
            double e0 =
                new Angle(23, 26, 21.448).Degrees
                - new Angle(0, 0, 4680.93).Degrees * U
                - 1.55 * Math.Pow(U, 2)
                + 1999.25 * Math.Pow(U, 3)
                - 51.38 * Math.Pow(U, 4)
                - 249.67 * Math.Pow(U, 5)
                - 39.05 * Math.Pow(U, 6)
                + 7.12 * Math.Pow(U, 7)
                + 27.87 * Math.Pow(U, 8)
                + 5.79 * Math.Pow(U, 9)
                + 2.45 * Math.Pow(U, 10);

            // correction for parallax (25.8)
            double eCorrected = e0 + 0.00256 * Math.Cos(O.ToRadians());

            // Sun's right ascension a and declination d
            double a = Math.Atan2(Math.Cos(eCorrected.ToRadians()) * Math.Sin(Lapp.ToRadians()), Math.Cos(Lapp.ToRadians()));
            double d = Math.Asin(Math.Sin(eCorrected.ToRadians()) * Math.Sin(Lapp.ToRadians()));

            // solar coordinates
            RightAscension ra = new RightAscension(a.ToDegrees());
            Angle dec = new Angle(d.ToDegrees());

            return new Coordinates(ra, dec);
        }

        private static IList<Moment> ParseDateArgs(string[] args)
        {
            List<Moment> moments = new List<Moment>();
            string message = "Must pass in at least one arg in format 'yyyy-MM-ddTHH:mm:ssZ'";
            if (args == null || args.Length == 0)
            {
                throw new ArgumentException(message);
            }

            foreach (string arg in args)
            {
                if (string.IsNullOrEmpty(arg))
                {
                    continue;
                }

                if (!DateTimeOffset.TryParse(arg, out DateTimeOffset d))
                {
                    throw new ArgumentException("Could not parse arg into DateTimeOffset: " + arg);
                }

                moments.Add(new Moment(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second));
            }

            return moments;
        }
    }
}
