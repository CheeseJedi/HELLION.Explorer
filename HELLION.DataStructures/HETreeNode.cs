/* HETreeNode.cs
 * CheeseJedi 2017
 * Defines a custom TreeNode class to hold some additional parameters used to speed up searches
 */

using System.Windows.Forms; // required for the base TreeNode class

namespace HELLION.DataStructures
{
    // Derive a custom TreeNode class to hold some additional parameters
    public class HETreeNode : TreeNode
    {
        public HETreeNodeType NodeType { get; set; }
        public bool IsReadOnly { get; set; }

        // Constructor that takes no arguments.
        public HETreeNode()
        {
            // Set the NodeType to Unknown
            NodeType = HETreeNodeType.Unknown;
            IsReadOnly = false;
        }
    } // End of class HETreeNode
} // End of namespace HELLION.DataStructures
