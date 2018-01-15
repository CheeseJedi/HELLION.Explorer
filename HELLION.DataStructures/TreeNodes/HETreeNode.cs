using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.HEImageList;

/// <summary>
/// Defines a derived HETreeNode to handle objects in the Game Data view.
/// Also defines an enum of node types applicable to the HETreeNode class.
/// </summary>

namespace HELLION.DataStructures
{
    /// <summary>
    /// Derives a custom TreeNode class to hold some additional parameters used to speed up searches
    /// </summary>
    public class HETreeNode : TreeNode
    {
        /// <summary>
        /// The type of the HETreeNode
        /// </summary>
        private HETreeNodeType nodeType = HETreeNodeType.Unknown;

        /// <summary>
        /// The cached count of direct child nodes. -1 means no data is cached.
        /// </summary>
        private int countOfChildNodes = -1;

        /// <summary>
        /// The cached count of all child nodes (including sub nodes). -1 means no data is cached.
        /// </summary>
        private int countOfAllChildNodes = -1;

        /// <summary>
        /// The cached list of direct child nodes. A value of null means no data is cached.
        /// </summary>
        private List<HETreeNode> listOfChildNodes = null;

        /// <summary>
        /// The cached list of all child nodes (including sub nodes). A value of null means no data is cached.
        /// </summary>
        private List<HETreeNode> listOfAllChildNodes = null;

        /*
        /// <summary>
        /// Determines whether the node has data for child nodes that weren't yet added to the
        /// tree. Once all direct child nodes are added this gets set to false.
        /// </summary>
        private bool lazyLoadAvailable = false;
        */

        /// <summary>
        /// Default constructor
        /// </summary>
        public HETreeNode()
        {

        }

        /// <summary>
        /// Constructor that takes a minimum of a name, but also optionally a type and text (display name).
        /// </summary>
        /// <param name="nodeName">Name of the new node.</param>
        /// <param name="nodeType">Type of the new node (HETreeNodeType enum)</param>
        /// <param name="nodeText">Text of the new node (Display Name). If not specified this defaults to the node's name.</param>
        /// <param name="nodeToolTipText">Tool tip text of the new node. If not specified this defaults to the node's text.</param>
        public HETreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.Unknown, string nodeText = "", string nodeToolTipText = "")
        {
            if (nodeName != null && nodeName != "") Name = nodeName;
            else Name = "node " + DateTime.Now.ToString();

            if (nodeText == "") Text = nodeName;
            else Text = nodeText;

            if (nodeToolTipText == "") ToolTipText = nodeName + " (" + newNodeType.ToString() + ")";
            else ToolTipText = nodeToolTipText;

            NodeType = newNodeType;
        }

        /// <summary>
        /// Gets/Sets the node type. On a Set operation it triggers the ImageIndex and SelectedImageIndex
        /// to refresh based 
        /// </summary>
        public HETreeNodeType NodeType
        {
            get { return nodeType; }
            set
            {
                // Only update the nodeType if it is different.
                if (value != nodeType)
                {
                    nodeType = value;
                    ImageIndex = GetImageIndexByNodeType(nodeType);
                    SelectedImageIndex = ImageIndex;
                }
            }
        }
        
        /// <summary>
        /// Builds and returns a count of direct descendent nodes.
        /// </summary>
        public int CountOfChildNodes
        {
            get
            {
                if (countOfChildNodes == -1) UpdateCounts();
                return countOfChildNodes;
            }
        }
        
        /// <summary>
        /// Builds and returns a could of all descendent nodes.
        /// </summary>
        public int CountOfAllChildNodes
        {
            get
            {
                if (countOfAllChildNodes == -1) UpdateCounts();
                return countOfAllChildNodes;
            }
        }

        /// <summary>
        /// Returns a list of direct descendants
        /// </summary>
        /// <returns>List<HETreeNode></HETreeNode></returns>
        public List<HETreeNode> ListOfChildNodes
        {
            get
            {
                if (listOfChildNodes == null)
                {
                    listOfChildNodes = new List<HETreeNode>
                    {
                        this
                    };
                    foreach (HETreeNode child in Nodes)
                    {
                        listOfChildNodes.Add(child);
                    }
                }
                return listOfChildNodes;
            }
        }

        /// <summary>
        /// Returns a list of all child nodes via a separate recursive function
        /// </summary>
        /// <returns></returns>
        public List<HETreeNode> ListOfAllChildNodes
        {
            get
            {
                if (listOfAllChildNodes == null)
                {
                    listOfAllChildNodes = GetAllNodes();
                }
                return listOfAllChildNodes;
            }
        }

        /*
        /// <summary>
        /// Public property to return the status of whether the node can lazy load it's data.
        /// </summary>
        /// <remarks>
        /// The Set function is only intended to be used 
        /// </remarks>
        public bool LazyLoadAvailable
        {
            get
            {
                return lazyLoadAvailable;
            }
            set
            {
                if (value && !lazyLoadAvailable)
                {
                    lazyLoadAvailable = value;
                }
            }
        }

        public void LazyLoad()
        {
            //HEJsonBaseFile.LazyPopulateNodeTree(this, 1);
        }
        */


        /// <summary>
        /// Used to reset the counts and lists back to uninitialised state.
        /// This is intended to be called by a newly-added child node upon adding to this
        /// node's Nodes TreeNodeCollection and will ensure the regeneration of this
        /// data next time it's needed.
        /// </summary>
        public void ClearCachedData()
        {
            countOfChildNodes = -1;
            countOfAllChildNodes = -1;
            listOfChildNodes = null;
            listOfAllChildNodes = null;
        }

        /// <summary>
        /// Updates the counts of sub nodes for this child (recursive).
        /// </summary>
        /// <remarks>
        /// As this is recursive it can take a while on large trees.
        /// </remarks>
        public void UpdateCounts()
        {
            countOfChildNodes = GetNodeCount(includeSubTrees: false);
            countOfAllChildNodes = GetNodeCount(includeSubTrees: true);

            if (CountOfChildNodes > 0)
            {
                // This node has child nodes, recursively process them
                foreach (HETreeNode nChildNode in Nodes)
                {
                    nChildNode.UpdateCounts();
                }
            }
        }

        /// <summary>
        /// Returns a List(HETreeNode) of nodes containing this node plus all descendants.
        /// </summary>
        /// <returns></returns>
        private List<HETreeNode> GetAllNodes()
        {
            List<HETreeNode> result = new List<HETreeNode>
            {
                this
            };
            foreach (HETreeNode child in Nodes)
            {
                result.AddRange(child.GetAllNodes());
            }
            return result;
        }
    }
}
