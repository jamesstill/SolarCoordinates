namespace SolarCoordinates
{
    /// <summary>
    /// Geocentric astronomical coordinates in R.A. and Dec as projected
    /// onto the celestial sphere
    /// </summary>
    public struct Coordinates
    {
        public RightAscension RightAscension { get; }
        public Angle Declination { get; }

        public Coordinates(RightAscension a, Angle d)
        {
            RightAscension = a;
            Declination = d;
        }
    }
}
