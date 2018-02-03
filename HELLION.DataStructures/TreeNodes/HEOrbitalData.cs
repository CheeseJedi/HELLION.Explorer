using System;
using Newtonsoft.Json.Linq;
/// <summary>
/// Defines a derived HETreeNode to handle objects in the Solar System view.
/// Also defines a node sorter that sorts by Semi-Major axis instead of by name which is
/// the default on a TreeView control, and a class to hold the orbital data.
/// </summary>
namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines a class to hold orbital data as used by the game.
    /// </summary>
    /// <remarks>
    /// May need to be expanded to handle additional functions related to new
    /// coordinate generation, for moving or spawning of ships/modules.
    /// </remarks>
    public class HEOrbitalData
    {
        public long ParentGUID { get; set; } = 0;
        public double Eccentricity { get; set; } = 0;
        public double SemiMajorAxis { get; set; } = 0;
        public double LongitudeOfAscendingNode { get; set; } = 0;
        public double ArgumentOfPeriapsis { get; set; } = 0;
        public double Inclination { get; set; } = 0;
        public double TimeSincePeriapsis { get; set; } = 0;
        public double SolarSystemPeriapsisTime { get; set; } = 0;

        /// <summary>
        /// Basic constructor used to initialise an empty object.
        /// </summary>
        public HEOrbitalData()
        { }

        /// <summary>
        /// Constructor that takes a JObject representing the OrbitData.
        /// </summary>
        /// <remarks>
        /// Not suitable for Celestial Bodies - they will need handling differently.
        /// </remarks>
        /// <param name=""></param>
        public HEOrbitalData(JObject orbitData)
        {
            if (orbitData == null) throw new NullReferenceException("Passed JObject was null.");
            else
            {
                ParentGUID = (long)orbitData["ParentGUID"];
                SemiMajorAxis = (double)orbitData["SemiMajorAxis"];
                Inclination = (double)orbitData["Inclination"];
                Eccentricity = (double)orbitData["Eccentricity"];
                ArgumentOfPeriapsis = (double)orbitData["ArgumentOfPeriapsis"];
                LongitudeOfAscendingNode = (double)orbitData["LongitudeOfAscendingNode"];

                // Celestial Bodies don't have the following two properties, so additional checking
                // will be required to prevent null reference exceptions.
                JToken testToken = orbitData["TimeSincePeriapsis"];
                if (testToken != null)
                {
                    TimeSincePeriapsis = (double)orbitData["TimeSincePeriapsis"];
                }
                else TimeSincePeriapsis = -1;

                testToken = orbitData["SolarSystemPeriapsisTime"];
                if (testToken != null)
                {
                    SolarSystemPeriapsisTime = (double)orbitData["SolarSystemPeriapsisTime"];
                }
                else SolarSystemPeriapsisTime = -1;

            }
        }

        /// <summary>
        /// Constructor that copies its parameters from another object, used by the
        /// Clone() routine to 'shallow clone' the object
        /// </summary>
        /// <param name="another">The HEOrbitalData object to be cloned.</param>
        private HEOrbitalData(HEOrbitalData another)
        {
            ParentGUID = another.ParentGUID;
            // VesselID = another.VesselID;
            // VesselType = another.VesselType;
            Eccentricity = another.Eccentricity;
            SemiMajorAxis = another.SemiMajorAxis;
            LongitudeOfAscendingNode = another.LongitudeOfAscendingNode;
            ArgumentOfPeriapsis = another.ArgumentOfPeriapsis;
            Inclination = another.Inclination;
            TimeSincePeriapsis = another.TimeSincePeriapsis;
            SolarSystemPeriapsisTime = another.SolarSystemPeriapsisTime;
        }

        /// <summary>
        /// Builds and returns a clone of the current HEOrbitalData object
        /// </summary>
        /// <returns></returns>
        public HEOrbitalData Clone()
        {
            return new HEOrbitalData(this);
        }

        /// <summary>
        /// Placeholder for a clone routine that generates an imperfect clone, used to generate similar
        /// coordinates to be used in spawning or moving of objects.
        /// </summary>
        /// <returns></returns>
        public HEOrbitalData FuzzyClone()
        {
            throw new NotImplementedException("Fuzzy clone is not yet implemented.");
        }

    }
}
