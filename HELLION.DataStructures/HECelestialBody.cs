

namespace HELLION.DataStructures
{
    public class HECelestialBody
    {
        public long GUID { get; set; }
        public string Name { get; set; }
        public long ParentGUID { get; set; }
        public double Mass { get; set; }
        public float Radius { get; set; }
        public float RotationPeriod { get; set; }
        public float Eccentricity { get; set; }
        public float SemiMajorAxis { get; set; }
        public float Inclination { get; set; }
        public float ArgumentOfPeriapsis { get; set; }
        public float LongitudeOfAscendingNode { get; set; }
        public string PlanetsPrefabPath { get; set; }
        public string MainCameraPrefabPath { get; set; }
        public string NavigationPrefabPath { get; set; }
        public float AtmosphereLevel1 { get; set; }
        public float AtmosphereLevel2 { get; set; }
        public float AtmosphereLevel3 { get; set; }
        public float ResourceResolution { get; set; }
    }
} // End of namespace HELLION.DataStructures
