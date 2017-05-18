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
        public double Eccentricity { get; set; }
        public double SemiMajorAxis { get; set; }
        public double LongitudeOfAscendingNode { get; set; }
        public double ArgumentOfPeriapsis { get; set; }
        public double Inclination { get; set; }
        public double TimeSincePeriapsis { get; set; }
        public double SolarSystemPeriapsisTime { get; set; }

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
            // Constructor that copies its parameters from another object, used to 'shallow clone' the obect
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
