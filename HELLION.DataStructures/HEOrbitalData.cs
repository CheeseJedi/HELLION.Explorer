/* HEOrbitalObjTreeNode.cs
 * CheeseJedi 2017
 * Defines a custom TreeNode class to hold some additional parameters used to speed up searches
 */


namespace HELLION.DataStructures
{
    public class HEOrbitalData
    {
        long ParentGUID { get; set; }
        long VesselID { get; set; }
        long VesselType { get; set; }
        float Eccentricity { get; set; }
        float SemiMajorAxis { get; set; }
        float LongitudeOfAscendingNode { get; set; }
        float ArgumentOfPeriapsis { get; set; }
        float Inclination { get; set; }
        float TimeSincePeriapsis { get; set; }
        float SolarSystemPeriapsisTime { get; set; }


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
    } // End of class HEOrbitalData
} // End of namespace HELLION.DataStructures
