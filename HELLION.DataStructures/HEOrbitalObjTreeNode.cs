using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Defines a derived HETreeNode to handle objects in the Solar System view.
/// Also defines a node sorter that sorts by Semi-Major axis instead of by name which is
/// the default otherwise, and a class to hold the orbital data.
/// </summary>
namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines a sub-derived TreeNode class to hold some additional parameters used to speed up working
    /// with the nodes by soring frequently used values.
    /// </summary>
    public class HEOrbitalObjTreeNode : HETreeNode
    {
        // Additional properties
        public string RealName { get; set; } = "";
        public long GUID { get; set; } = 0;
        public long ParentGUID { get; set; } = 0;
        public int SceneID { get; set; } = 0;
        public int Type { get; set; } = 0;
        public double SemiMajorAxis { get; set; } = 0;
        public double Inclination { get; set; } = 0;
        public HEOrbitalData OrbitData { get; set; } = null;
        public long DockedToShipGUID { get; set; } = 0;
        public int DockedPortID { get; set; } = 0;
        public int DockedToPortID { get; set; } = 0;


        /// <summary>
        /// Constructor that takes a minimum of a name, but also optionally a type and text (display name) - calls the base constructor
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="nodeType"></param>
        /// <param name="nodeText"></param>
        /// <param name="nodeToolTipText"></param>
        public HEOrbitalObjTreeNode(string nodeName, HETreeNodeType nodeType = HETreeNodeType.Unknown, string nodeText = "", string nodeToolTipText = "")
            : base(nodeName, nodeType, nodeText, nodeToolTipText)
        {
            OrbitData = new HEOrbitalData();
        }
    } // End of class HETreeNode

    /// <summary>
    /// Create a node sorter that implements the IComparer interface to sort HEOrbitalObjTreeNodes
    /// by Semi-Major axis.
    /// </summary>
    public class HETNSorterSemiMajorAxis : IComparer
    {
        /// <summary>
        /// Compare the values of SemiMajorAxis.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            HEOrbitalObjTreeNode nodeX = x as HEOrbitalObjTreeNode;
            HEOrbitalObjTreeNode nodeY = y as HEOrbitalObjTreeNode;

            // int iResult = Comparer<double>.Default.Compare(nodeX.SemiMajorAxis, nodeY.SemiMajorAxis);

            if (nodeX != null && nodeY != null)
            {
                // Compare the values of SemiMajorAxis, returning the result.
                return Comparer<double>.Default.Compare(nodeX.SemiMajorAxis, nodeY.SemiMajorAxis);
            }
            else
            {
                // One of the two nodes was null, cannot compare so resturn 0 indicating equivalency
                return 0;
            }
        }
    } // End of HETNSorterSemiMajorAxis

    /// <summary>
    /// Defines a class to hold orbital data as used by the game.
    /// </summary>
    public class HEOrbitalData
    {
        public long ParentGUID { get; set; } = 0;
        public long VesselID { get; set; } = 0;
        public long VesselType { get; set; } = 0;
        public double Eccentricity { get; set; } = 0;
        public double SemiMajorAxis { get; set; } = 0;
        public double LongitudeOfAscendingNode { get; set; } = 0;
        public double ArgumentOfPeriapsis { get; set; } = 0;
        public double Inclination { get; set; } = 0;
        public double TimeSincePeriapsis { get; set; } = 0;
        public double SolarSystemPeriapsisTime { get; set; } = 0;

        public HEOrbitalData()
        { }

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
