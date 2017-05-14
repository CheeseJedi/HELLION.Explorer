/* HEOrbitalObjTreeNode.cs
 * CheeseJedi 2017
 * Defines a custom TreeNode class to hold some additional parameters used to speed up searches
 */


namespace HELLION.DataStructures
{
    public class HEOrbitalData
    {
        public long ParentGUID { get; set; }
        public long VesselID { get; set; }
        public long VesselType { get; set; }
        public float Eccentricity { get; set; }
        public float SemiMajorAxis { get; set; }
        public float LongitudeOfAscendingNode { get; set; }
        public float ArgumentOfPeriapsis { get; set; }
        public float Inclination { get; set; }
        public float TimeSincePeriapsis { get; set; }
        public float SolarSystemPeriapsisTime { get; set; }

        public HEOrbitalData()
        {
            // Constructor
            ParentGUID = 0;
            VesselID = 0;
            VesselType = 0;
            Eccentricity = 0;
            SemiMajorAxis = 0;
            LongitudeOfAscendingNode = 0;
            ArgumentOfPeriapsis = 0;
            Inclination = 0;
            TimeSincePeriapsis = 0;
            SolarSystemPeriapsisTime = 0;
        }

        protected HEOrbitalData(HEOrbitalData another)
        {
            // Constructor that takes parameters from another object, used to 'clone' the obect
            ParentGUID = another.ParentGUID;
            VesselID = another.VesselID;
            VesselType = another.VesselType;
            Eccentricity = another.Eccentricity;
            SemiMajorAxis = another.SemiMajorAxis;
            LongitudeOfAscendingNode = another.LongitudeOfAscendingNode;
            ArgumentOfPeriapsis = another.ArgumentOfPeriapsis;
            Inclination = another.Inclination;
            TimeSincePeriapsis = another.TimeSincePeriapsis;
            SolarSystemPeriapsisTime = another.SolarSystemPeriapsisTime;
        }

        public HEOrbitalData Clone()
        {
            return new HEOrbitalData(this);
        }
    } // End of class HEOrbitalData
} // End of namespace HELLION.DataStructures
