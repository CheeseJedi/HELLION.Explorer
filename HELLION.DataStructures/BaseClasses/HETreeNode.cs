using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static HELLION.DataStructures.HEImageList;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Derives a custom TreeNode class to hold some additional parameters used to speed up searches
    /// </summary>
    public class HETreeNode : TreeNode
    {
        #region Constructors
        /// <summary>
        /// Default constructor, used by GameData and SolarSystem tree node derived types.
        /// </summary>
        public HETreeNode(object ownerObject)
        {
            OwnerObject = ownerObject ?? throw new NullReferenceException("passedOwner was null.");
        }

        /// <summary>
        /// Constructor that takes a minimum of a name, but also optionally a type, Text (display name)
        /// and ToolTipText and an Owner object.
        /// </summary>
        /// <param name="ownerObject"></param>
        /// <param name="nodeName"></param>
        /// <param name="newNodeType"></param>
        /// <param name="nodeText"></param>
        /// <param name="nodeToolTipText"></param>
        public HETreeNode(object ownerObject, string nodeName = null, HETreeNodeType newNodeType = HETreeNodeType.Unknown,
             string nodeText = "", string nodeToolTipText = "") : this(ownerObject)
        {
            Name = (nodeName == null || nodeName == "") ? "Unnamed node " + DateTime.Now.ToString() : nodeName;
            Text = (nodeText == null || nodeText == "") ? nodeName : nodeText;
            ToolTipText = (nodeToolTipText == null || nodeToolTipText == "") ? nodeName + " (" + newNodeType.ToString() + ")" : nodeToolTipText;

            NodeType = newNodeType;
        }
        #endregion

        /// <summary>
        /// Stores a reference to the owning object.
        /// </summary>
        public object OwnerObject { get; protected set; } = null;

        /// <summary>
        /// Redefines the Name accessor to track changes.
        /// </summary>
        public new string Name
        {
            get { return base.Name; }
            set
            {
                if (base.Name != value)
                {
                    base.Name = value;

                    // Trigger updates here.
                }
            }
        }

        /// <summary>
        /// Redefines the ToolTipText accessor to track changes.
        /// </summary>
        public new string Text
        {
            get { return base.Text; }
            set
            {
                if (base.Text != value)
                {
                    base.Text = value;

                    // Trigger updates here.

                }
            }
        }

        /// <summary>
        /// Redefines the base's Text accessor to track changes.
        /// </summary>
        public new string ToolTipText
        {
            get { return base.ToolTipText; }
            set
            {
                if (base.ToolTipText != value)
                {
                    base.ToolTipText = value;

                    // Trigger updates here.

                }
            }
        }
        
        /// <summary>
        /// The node's HETreeNodeType type.
        /// </summary>
        /// <remarks>
        /// On a Set operation it triggers the ImageIndex and SelectedImageIndex
        /// </remarks>
        protected HETreeNodeType _nodeType = HETreeNodeType.Unknown;
        public HETreeNodeType NodeType
        {
            get { return _nodeType; }
            set
            {
                // Only update the nodeType if it is different.
                if (value != _nodeType)
                {
                    _nodeType = value;
                    ImageIndex = GetIconImageIndexByNodeType(_nodeType);
                    SelectedImageIndex = ImageIndex;
                }
            }
        }

        /// <summary>
        /// Builds and returns a count of direct descendent nodes.
        /// </summary>
        public int CountOfChildNodes
        {
            get { return GetNodeCount(includeSubTrees: false); }

        }

        /// <summary>
        /// Builds and returns a could of all descendent nodes.
        /// </summary>
        public int CountOfAllChildNodes
        {
            get { return GetNodeCount(includeSubTrees: true); }
        }

        /// <summary>
        /// Returns a list of HETreeNodes of direct descendant nodes.
        /// </summary>
        /// <returns>Also returns null if there are no child nodes</returns>
        public List<HETreeNode> ListOfChildNodes
        {
            get
            {
                List<HETreeNode> _listOfChildNodes = new List<HETreeNode> { this };
                foreach (HETreeNode child in Nodes)
                {
                    _listOfChildNodes.Add(child);
                }
                return _listOfChildNodes.Count > 0 ? _listOfChildNodes: null;
            }
        }

        /// <summary>
        /// Returns a list of all child nodes via recursion.
        /// </summary>
        /// <returns>Also returns null if there are no child nodes</returns>
        public List<HETreeNode> ListOfAllChildNodes
        {
            get
            {
                List<HETreeNode> _listOfAllChildNodes = new List<HETreeNode> { this };
                foreach (HETreeNode child in Nodes)
                {
                    _listOfAllChildNodes.AddRange(child.ListOfAllChildNodes);
                }
                return _listOfAllChildNodes.Count > 0 ? _listOfAllChildNodes: null;
            }
        }

        /// <summary>
        /// Gets the first node that matches the given key in the current nodes children.
        /// </summary>
        /// <param name="nCurrentNode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public HETreeNode GetFirstChildNodeByName(string key)
        {
            TreeNode[] nodes = Nodes.Find(key, searchAllChildren: true);
            return nodes.Length > 0 ? (HETreeNode)nodes[0] : null;
        }

        /// <summary>
        /// Returns the Path of this node, with reference to the TreeView control.
        /// </summary>
        /// <remarks>
        /// Returns FullPath but strips off the node name and last path separator.
        /// </remarks>
        public string Path
        {
            get
            {
                string fullPath = FullPath;
                int lastIndex = fullPath.LastIndexOf(TreeView.PathSeparator);
                // Ensure the lastIndex is not -1, present zero instead.
                lastIndex = lastIndex != -1 ? lastIndex : 0;
                return fullPath.Substring(0, lastIndex);
            }
        }

        /// <summary>
        /// Re-generates the text displayed in the label of the tree node. 
        /// </summary>
        protected void RefreshText()
        {

        }
        protected void RefreshToolTipText()
        {

        }



    }
}
