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
        public int CountOfChildNodes { get; set; }
        public int CountOfAllChildNodes { get; set; } 

        // Constructor that takes no arguments.
        public HETreeNode()
        {
            // Set the NodeType to Unknown
            NodeType = HETreeNodeType.Unknown;
            IsReadOnly = false;
            CountOfChildNodes = -1;
            CountOfAllChildNodes = -1;
        }

        public void UpdateCounts()
        {
            // Updates the counts of nodes and subnodes for this child and triggers each child to update also
            CountOfChildNodes = GetNodeCount(includeSubTrees: false);
            CountOfAllChildNodes = GetNodeCount(includeSubTrees: true);

            if (CountOfChildNodes > 0)
            {
                // This node has child nodes, recursively process them
                foreach (HETreeNode nChildNode in Nodes)
                {
                    nChildNode.UpdateCounts();
                }
            }
        }
    } // End of class HETreeNode
} // End of namespace HELLION.DataStructures
