/// <summary>
/// Defines a derived HETreeNode to handle objects in the Solar System view.
/// Also defines a node sorter that sorts by Semi-Major axis instead of by name which is
/// the default on a TreeView control, and a class to hold the orbital data.
/// </summary>
namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines a sub-derived TreeNode class to hold some additional parameters used to speed up working
    /// with the nodes by storing frequently used values.
    /// </summary>
    public class HEOrbitalObjTreeNode : HETreeNode
    {
        /// <summary>
        /// The GUID for this object.
        /// </summary>
        public long GUID { get; set; } = 0;

        /// <summary>
        /// The GUID of the parent of this object.
        /// </summary>
        public long ParentGUID { get; set; } = 0;

        /// <summary>
        /// The Type of the object, as defined by the game.
        /// </summary>
        public int Type { get; set; } = 0;

        /// <summary>
        /// The Semi-Major Axis of the orbiting body.
        /// </summary>
        /// <remarks>
        /// Primary field for sorting objects that have the same ParentGUID.
        /// </remarks>
        public double SemiMajorAxis { get; set; } = 0;

        /// <summary>
        /// The Inclination of the orbiting body.
        /// </summary>
        public double Inclination { get; set; } = 0;

        /// <summary>
        /// The OrbitData for this node - a copy of the values directly from the Json data for
        /// Asteroids, Ships and Players.
        /// </summary>
        /// <remarks>
        /// Celestial Bodies do not populate this information as theirs is stored in a slightly
        /// different format, as they come from the Static Data rather than the .save file.
        /// </remarks>
        public HEOrbitalData OrbitData { get; set; } = null;

        /// <summary>
        /// If this ship/module is docked TO another, the other ship's 
        /// GUID will be populated here
        /// </summary>
        public long DockedToShipGUID { get; set; } = 0;

        /// <summary>
        /// Which local port is in use to dock to the other ship.
        /// </summary>
        public int DockedPortID { get; set; } = 0;

        /// <summary>
        /// Which port on the other ship is docked to this ship.
        /// </summary>
        public int DockedToPortID { get; set; } = 0;

        /// <summary>
        /// Constructor that takes a minimum of a name, but also optionally a type and text (display name) - inherits the base constructor
        /// </summary>
        /// <param name="nodeName">Name for the new node; required;</param>
        /// <param name="nodeType">HETreeNodeType of the new node; defaults to Unknown.</param>
        /// <param name="nodeText">The Text (DisplayName) of the node - uses the Name if omitted.</param>
        /// <param name="nodeToolTipText">The ToolTip text displayed; defaults to the nodeText if omitted.</param>
        public HEOrbitalObjTreeNode(string nodeName, HETreeNodeType nodeType = HETreeNodeType.Unknown, string nodeText = "", string nodeToolTipText = "")
            : base(nodeName, nodeType, nodeText, nodeToolTipText)
        {
            OrbitData = new HEOrbitalData();
        }
    }
}
