using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using static HELLION.DataStructures.Utilities.EmbeddedImages_ImageList;

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
            BaseNodeName = nodeName;
            NodeType = newNodeType;
            BaseNodeText = nodeText;
            BaseNodeToolTipText = nodeToolTipText;

            // Trigger name and other info generation based on what was passed.
            RefreshName();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Stores a reference to the owning object.
        /// </summary>
        public object OwnerObject { get; protected set; } = null;


        /// <summary>
        /// Redefines the Name accessor to track changes.
        /// </summary>
        public new string Name
        {
            get => base.Name;
            protected set
            {
                if (base.Name != value)
                {
                    base.Name = value;

                    // Trigger updates here.
                    RefreshText();
                    RefreshToolTipText();
                }
            }
        }

        /// <summary>
        /// The 'prefix' applied prior to the node name.
        /// </summary>
        public string PrefixNodeName
        {
            get => _prefixNodeName;
            set
            {
                if (_prefixNodeName != value)
                {
                    _prefixNodeName = value;

                    RefreshName();
                }
            }
        }

        /// <summary>
        /// The 'base' of the node name - this may be displayed differently.
        /// </summary>
        public string BaseNodeName
        {
            get => _baseNodeName;
            set
            {
                _baseNodeName = value;
                RefreshName();
            }
        }

        /// <summary>
        /// The 'postfix' applied after to the node name.
        /// </summary>
        public string PostfixNodeName
        {
            get => _postfixNodeName;
            set
            {
                if (_postfixNodeName != value)
                {
                    _postfixNodeName = value;

                    RefreshName();
                }
            }
        }


        /// <summary>
        /// The HETreeNodeType type of this node.
        /// </summary>
        public HETreeNodeType NodeType
        {
            get => _nodeType;
            set
            {
                if (value != _nodeType)
                {
                    _nodeType = value;

                    RefreshImageIndex();
                }
            }
        }


        /// <summary>
        /// Redefines the ToolTipText accessor to track changes.
        /// </summary>
        public new string Text
        {
            get => base.Text;
            protected set
            {
                if (base.Text != value)
                {
                    base.Text = value;

                    // Trigger updates here.
                    RefreshToolTipText();
                }
            }
        }

        /// <summary>
        /// The 'prefix' applied prior to the node text.
        /// </summary>
        public string PrefixNodeText
        {
            get => _prefixNodeText;
            set
            {
                if (_prefixNodeText != value)
                {
                    _prefixNodeText = value;

                    RefreshText();
                }
            }
        }

        /// <summary>
        /// The 'base' of the node text aka what's displayed. 
        /// </summary>
        public string BaseNodeText
        {
            get => _baseNodeText;
            set
            {
                _baseNodeText = value;
                RefreshText();
            }
        }

        /// <summary>
        /// The 'postfix' applied after to the node text.
        /// </summary>
        public string PostfixNodeText
        {
            get => _postfixNodeText;
            set
            {
                if (_postfixNodeText != value)
                {
                    _postfixNodeText = value;

                    RefreshText();
                }
            }
        }


        /// <summary>
        /// Redefines the base's Text accessor to track changes.
        /// </summary>
        public new string ToolTipText
        {
            get => base.ToolTipText;
            protected set
            {
                if (base.ToolTipText != value)
                {
                    base.ToolTipText = value;

                    // Trigger updates here.
                }
            }
        }

        /// <summary>
        /// The 'base' of the ToolTipText - if set this is displayed instead
        /// of an auto-generated ToolTipText.
        /// </summary>
        public string BaseNodeToolTipText
        {
            get => _baseNodeToolTipText;
            set
            {
                _baseNodeToolTipText = value;
                RefreshToolTipText();
            }
        }




        #endregion

        #region Methods

        /// <summary>
        /// Returns a list of all first generation child nodes, or all descendants via recursion.
        /// </summary>
        /// <param name="includeSubtrees"></param>
        /// <returns>Returns null if this node has no children.</returns>
        public List<HETreeNode> GetChildNodes(bool includeSubtrees = false)
        {
            List<HETreeNode> _listOfChildNodes = new List<HETreeNode> { this };
            foreach (HETreeNode child in Nodes)
            {
                if (!includeSubtrees) _listOfChildNodes.Add(child);
                else _listOfChildNodes.AddRange(child.GetChildNodes(includeSubtrees: true));
            }
            return _listOfChildNodes.Count > 0 ? _listOfChildNodes : null;
        }

        /// <summary>
        /// Gets the first node that matches the given key in the current nodes children.
        /// </summary>
        /// <param name="nCurrentNode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public HETreeNode GetFirstChildNodeByName(string key)
        {
            TreeNode[] _nodes = Nodes.Find(key, searchAllChildren: true);
            return _nodes.Length > 0 ? (HETreeNode)_nodes[0] : null;
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
                if (TreeView != null)
                {
                    string fullPath = FullPath;
                    int lastIndex = fullPath.LastIndexOf(TreeView.PathSeparator);
                    // Ensure the lastIndex is not -1, present zero instead.
                    lastIndex = lastIndex != -1 ? lastIndex : 0;
                    return fullPath.Substring(0, lastIndex);
                }
                else return "Path Unavailable (no TreeView)";
            }
        }

        #region Refresh and Generation Methods

        /// <summary>
        /// Generates a name for the node.
        /// </summary>
        /// <returns></returns>
        protected string GenerateBaseNodeName()
        {
            return "Unnamed node " + DateTime.Now.ToString();
        }

        /// <summary>
        /// Refreshes the node name and triggers auto generation if necessary.
        /// </summary>
        public void RefreshName()
        {
            if (BaseNodeName == null || BaseNodeName == "")
            {
                BaseNodeName = GenerateBaseNodeName();
            }

            // Alterations to the base name can be applied here.
            Name = _prefixNodeName + BaseNodeName + _postfixNodeName;

            RefreshText();
        }

        /// <summary>
        /// Generates the text displayed in the label of the tree node.
        /// </summary>
        /// <returns></returns>
        protected string GenerateBaseNodeText()
        {
            return Name ?? "UNNAMED NODE";
        }

        /// <summary>
        /// Refreshes the text displayed in the label of the tree node. 
        /// </summary>
        public  void RefreshText()
        {
            if (BaseNodeText == null || BaseNodeText == "") Text = GenerateBaseNodeText();
            else
            {
                // Alterations to the base text can be applied here.
                Text = _prefixNodeText + BaseNodeText + _postfixNodeText;
                RefreshToolTipText();
            }
        }

        /// <summary>
        /// Regenerates the ToolTipText text used by the pop-up.
        /// </summary>
        /// <returns></returns>
        protected string GenerateBaseNodeToolTipText()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Name: " + Name + Environment.NewLine);
            sb.Append("Text: " + Text + Environment.NewLine);
            sb.Append("NodeType: " + NodeType + Environment.NewLine);
            //sb.Append("Path: " + Path + Environment.NewLine);

            return sb.ToString();
        }

        /// <summary>
        /// Refreshes the node's ToolTipText.
        /// </summary>
        public void RefreshToolTipText(bool includeSubtrees = false)
        {
            
            if (BaseNodeToolTipText == null || BaseNodeToolTipText == "") ToolTipText = GenerateBaseNodeToolTipText();
            else
            {
                // Alterations to the base text can be applied here.
                ToolTipText = BaseNodeText;
            }
            if (includeSubtrees && Nodes.Count > 0)
            {
                foreach (HETreeNode node in Nodes)
                {
                    node.RefreshToolTipText(includeSubtrees);
                }
            }
            
        }

        /// <summary>
        /// Refreshes the ImageIndex and SelectedImageIndex for this node.
        /// </summary>
        protected void RefreshImageIndex()
        {
            ImageIndex = GetIconImageIndexByNodeType(_nodeType);
            SelectedImageIndex = ImageIndex;
        }

        #endregion

        #endregion

        #region Fields

        protected HETreeNodeType _nodeType = HETreeNodeType.Unknown;
        protected string _baseNodeName = null;
        protected string _baseNodeText = null;
        protected string _baseNodeToolTipText = null;

        protected string _prefixNodeName = null;
        protected string _postfixNodeName = null;

        protected string _prefixNodeText = null;
        protected string _postfixNodeText = null;


        #endregion

    }
}
