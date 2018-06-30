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
    public class OrbitalData
    {
        #region Constructors

        /// <summary>
        /// Basic constructor used to initialise an empty object.
        /// </summary>
        public OrbitalData()
        { }

        /// <summary>
        /// Constructor that takes a JObject representing the OrbitData.
        /// </summary>
        public OrbitalData(JObject orbitData)
        {
            if (orbitData != null)
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
        /// <param name="another">The OrbitalData object to be cloned.</param>
        private OrbitalData(OrbitalData another)
        {
            ParentGUID = another.ParentGUID;
            Eccentricity = another.Eccentricity;
            SemiMajorAxis = another.SemiMajorAxis;
            LongitudeOfAscendingNode = another.LongitudeOfAscendingNode;
            ArgumentOfPeriapsis = another.ArgumentOfPeriapsis;
            Inclination = another.Inclination;
            TimeSincePeriapsis = another.TimeSincePeriapsis;
            SolarSystemPeriapsisTime = another.SolarSystemPeriapsisTime;
        }

        #endregion

        #region Core Properties

        public long? ParentGUID
        {
            get => _parentGUID;
            set
            {
                if (_parentGUID != value)
                {
                    _parentGUID = value;

                    // Trigger refresh here
                    ProcessParentGUIDChange();
                }
            }
        }

        public double? Eccentricity { get; set; } = null;
        public double? SemiMajorAxis { get; set; } = null;
        public double? LongitudeOfAscendingNode { get; set; } = null;
        public double? ArgumentOfPeriapsis { get; set; } = null;
        public double? Inclination { get; set; } = null;
        public double? TimeSincePeriapsis { get; set; } = null;
        public double? SolarSystemPeriapsisTime { get; set; } = null;

        #endregion

        #region Supplementary Properties

        /// <summary>
        /// Calculates the Apoapsis from the Semi-Major Axis and the Eccentricity.
        /// </summary>
        public double Apoapsis => (double)SemiMajorAxis * (1 + (double)Eccentricity);

        /// <summary>
        /// Calculates the Periapsis from the Semi-Major Axis and the Eccentricity.
        /// </summary>
        public double Periapsis => (double)SemiMajorAxis * (1 - (double)Eccentricity);


        #endregion

        #region Methods

        private void ProcessParentGUIDChange()
        {
            // TODO - to be implemented: look up parent body etc.
        }

        /// <summary>
        /// Builds and returns a clone of the current OrbitalData object
        /// </summary>
        /// <returns></returns>
        public OrbitalData Clone() => new OrbitalData(this);

        /// <summary>
        /// Placeholder for a clone routine that generates an imperfect clone, used to generate similar
        /// coordinates to be used in spawning or moving of objects.
        /// </summary>
        /// <returns></returns>
        public OrbitalData FuzzyClone()
        {
            throw new NotImplementedException("Fuzzy clone is not yet implemented.");
        }


        public double CalculateOrbitalPeriod()
        {

            // Newton's law of universal gravitation
            // Every point mass attracts every other point mass by a force acting along
            // the line intersecting both points. The force is proportional to the product
            // of the two masses, and inversely proportional to the square of the distance
            // between them.
            // Force F acting between two objects = G * ( (m1 * m2 ) / r^2)

            // Gravitational constant.
            const double G = 6.673E-11;



            // Kepler's 3rd law:
            // The square of the orbital period of a planet is proportional
            // to the cube of the semi-major axis of its orbit.






            double PlanetMass = 1.2500000414766919E+31; // Hellion's mass for testing - this will be read from the orbiting body.

            double theConstant = (G * PlanetMass) / (4 * Math.PI * Math.PI);

            double periodSquared = theConstant / (Math.Pow((double)SemiMajorAxis, 3));

            double period = Math.Sqrt(periodSquared);



            return period;

            
            
            // formula from http://www.physicsclassroom.com/class/circles/Lesson-4/Mathematics-of-Satellite-Motion
            // upside down :)
            // (Time t ^2) / (Radius r ^3)  ==  (4 * (Math.PI ^2)) / Gravitational Constant G * PlanetMass

        }

        #endregion


        #region Fields

        private long? _parentGUID = null;

        #endregion
    }
}
