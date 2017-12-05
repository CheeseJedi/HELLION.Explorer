/* HEOrbitalObjTreeNode.cs
 * CheeseJedi 2017
 * Defines a custom TreeNode class to hold some additional parameters used to speed up searches
 */


namespace HELLION.DataStructures
{
    public class HEOrbitalObjTreeNode : HETreeNode
    {
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


        // Constructor that takes a minimum of a name, but also optionally a type and text (display name) - calls the base constructor
        public HEOrbitalObjTreeNode(string nodeName, HETreeNodeType nodeType = HETreeNodeType.Unknown, string nodeText = "", string nodeToolTipText = "")
            : base(nodeName, nodeType, nodeText, nodeToolTipText)
        {
            OrbitData = new HEOrbitalData();
        }


    } // End of class HETreeNode
} // End of namespace HELLION.DataStructures
