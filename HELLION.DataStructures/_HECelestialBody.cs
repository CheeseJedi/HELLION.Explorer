

namespace HELLION.DataStructures
{
    public class HECelestialBody
    {
        public long GUID { get; set; }
        public string Name { get; set; }
        public long ParentGUID { get; set; }
        public double Mass { get; set; }
        public double Radius { get; set; }
        public double RotationPeriod { get; set; }
        public double Eccentricity { get; set; }
        public double SemiMajorAxis { get; set; }
        public double Inclination { get; set; }
        public double ArgumentOfPeriapsis { get; set; }
        public double LongitudeOfAscendingNode { get; set; }
        public string PlanetsPrefabPath { get; set; }
        public string MainCameraPrefabPath { get; set; }
        public string NavigationPrefabPath { get; set; }
        public double AtmosphereLevel1 { get; set; }
        public double AtmosphereLevel2 { get; set; }
        public double AtmosphereLevel3 { get; set; }
        public double ResourceResolution { get; set; }
    }
} // End of namespace HELLION.DataStructures
