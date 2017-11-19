/* HEOrbitalObjTreeNode.cs
 * CheeseJedi 2017
 * Defines a custom TreeNode class to hold some additional parameters used to speed up searches
 */


namespace HELLION.DataStructures
{
    public class HEOrbitalObjTreeNode : HETreeNode
    {
        public string RealName { get; set; }
        public long GUID { get; set; }
        public long ParentGUID { get; set; }
        public int SceneID { get; set; }
        public int Type { get; set; }
        public double SemiMajorAxis { get; set; }
        public double Inclination { get; set; }
        public HEOrbitalData OrbitData { get; set; }
        public long DockedToShipGUID { get; set; }
        public int DockedPortID { get; set; }
        public int DockedToPortID { get; set; }

        public HEOrbitalObjTreeNode()
        {
            // Default constructor
            NodeType = HETreeNodeType.Unknown;
            RealName = "";
            GUID = 0;
            ParentGUID = 0;
            SceneID = 0;
            Type = 0;
            SemiMajorAxis = 0;
            Inclination = 0;
            OrbitData = new HEOrbitalData();
            DockedToShipGUID = 0;
            DockedPortID = 0;
            DockedToPortID = 0;
        }
    } // End of class HETreeNode
} // End of namespace HELLION.DataStructures
