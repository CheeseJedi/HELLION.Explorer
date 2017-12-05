/* HETreeNode.cs
 * CheeseJedi 2017
 * Defines a custom TreeNode class to hold some additional parameters used to speed up searches
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms; // required for the base TreeNode class

namespace HELLION.DataStructures
{
    // Derive a custom TreeNode class to hold some additional parameters
    public class HETreeNode : TreeNode
    {
        public HETreeNodeType NodeType { get; set; } = HETreeNodeType.Unknown;
        public bool IsReadOnly { get; set; } = false;
        public int CountOfChildNodes { get; set; } = -1; // Set to -1 to signify no count has been made yet
        public int CountOfAllChildNodes { get; set; } = -1; // Set to -1 to signify no count has been made yet

        // Constructor that takes a minimum of a name, but also optionally a type and text (display name) 
        public HETreeNode(string nodeName, HETreeNodeType nodeType = HETreeNodeType.Unknown, string nodeText = "", string nodeToolTipText = "")
        {
            if (nodeName != null && nodeName !="")
            {
                Name = nodeName;
            }
            else
            {
                Name = "node " + DateTime.Now.ToString();
            }
            if (nodeText == "")
            {
                Text = nodeName;
            }
            else
            {
                Text = nodeText;
            }
            if (nodeToolTipText == "")
            {
                ToolTipText = nodeName + "(" + nodeType.ToString() + ")";
            }
            else
            {
                ToolTipText = nodeToolTipText;
            }

            NodeType = nodeType;

            ImageIndex = SelectedImageIndex = HEUtilities.GetImageIndexByNodeType(NodeType);

            //IsReadOnly = false;
            //CountOfChildNodes = -1; // Set to -1 to signify no count has been made yet
            //CountOfAllChildNodes = -1; // Set to -1 to signify no count has been made yet
        }

        public void UpdateImageIndeces()
        {
            ImageIndex = SelectedImageIndex = HEUtilities.GetImageIndexByNodeType(NodeType);

        }



        public List<HETreeNode> GetAllNodes()
        {
            List<HETreeNode> result = new List<HETreeNode>();
            result.Add(this);
            foreach (HETreeNode child in Nodes)
            {
                result.AddRange(child.GetAllNodes());
            }
            return result;
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
