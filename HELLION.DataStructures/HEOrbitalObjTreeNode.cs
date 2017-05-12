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
        public float SemiMajorAxis { get; set; }
        public float Inclination { get; set; }

        public HEOrbitalObjTreeNode()
        {
            // Default constructor
            NodeType = HETreeNodeType.Unknown;
            RealName = "";
            GUID = 0;
            ParentGUID = 0;
            SemiMajorAxis = 0;
            Inclination = 0;
        }

        /* Constructor
        public HEOrbitalObjTreeNode(int nodeId, string nodeType, string fp)
        {
            // Constructor that takes some arguments.
            //NodeId = nodeId;
            //NodeType = nodeType;
            //this.Text = fp.Substring(fp.LastIndexOf("\\"));
        }
        */

    } // End of class HETreeNode
} // End of namespace HELLION.DataStructures
