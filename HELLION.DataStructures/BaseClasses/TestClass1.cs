using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HELLION.DataStructures.BaseClasses
{
    class TestClass1
    {


        public class Rootobject
        {
            public Class1[] Property1 { get; set; }
        }

        public class Class1
        {
            public int GUID { get; set; }
            public string Name { get; set; }
            public int ParentGUID { get; set; }
            public float Mass { get; set; }
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
            public int ResourceResolution { get; set; }
        }

        


    }
}
