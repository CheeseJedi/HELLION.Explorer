using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using static HELLION.DataStructures.EmbeddedImages.EmbeddedImages_ImageList;


namespace HELLION.DataStructures.UI
{
    public class Base_TN : TreeNode
    {
        #region Constructors

        /// <summary>
        /// Default constructor that attempts to set the owner object.
        /// </summary>
        /// <param name="ownerObject"></param>
        public Base_TN(Iparent_Base_TN ownerObject = null)
        {
            OwnerObject = ownerObject;
        }

        /// <summary>
        /// Normal constructor - requires the owner object.
        /// </summary>
        /// <param name="ownerObject"></param>
        /// <param name="nodeName"></param>
        /// <param name="newNodeType"></param>
        /// <param name="nodeText"></param>
        /// <param name="nodeToolTipText"></param>
        public Base_TN(Iparent_Base_TN ownerObject, string nodeName = null,
            HETreeNodeType newNodeType = HETreeNodeType.Unknown) : this(ownerObject)
        {
            NodeType = newNodeType;
            Name = nodeName;


            // Trigger name and other info generation based on what was passed.
            //     RefreshName();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Stores a reference to the owning object.
        /// </summary>
        public Iparent_Base_TN OwnerObject { get; protected set; } = null;

        /// <summary>
        /// Redefines the Name accessor to track changes.
        /// </summary>
        public new string Name
        {
            get => base.Name;
            set
            {
                if (base.Name != value)
                {
                    base.Name = value;

                    // Trigger updates here.
                    if (value == null) RefreshName();
                    RefreshText();
                    RefreshToolTipText();
                }
            }
        }

        /// <summary>
        /// the string Text prefix.
        /// </summary>
        public string Text_Prefix
        {
            get => _text_Prefix;
            set
            {
                if (_text_Prefix != value)
                {
                    _text_Prefix = value;

                    // Trigger updates here.
                    RefreshText();
                    RefreshToolTipText();
                }
            }
        }

        /// <summary>
        /// The string Text suffix.
        /// </summary>
        public string Text_Suffix
        {
            get => _text_Suffix;
            set
            {
                if (_text_Suffix != value)
                {
                    _text_Suffix = value;

                    // Trigger updates here.
                    RefreshText();
                    RefreshToolTipText();
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



        #endregion

        #region Refresh and Generation Methods

        /// <summary>
        /// Triggers the node to rebuild it's name, text and ToolTipText.
        /// </summary>
        /// <param name="includeSubTrees"></param>
        public virtual void Refresh(bool includeSubTrees = false)
        {
            Debug.Print("Base_TN.Refresh called on {0} - {1}", Name, OwnerObject.ToString());
            RefreshName(includeSubTrees);
            RefreshText(includeSubTrees);
            // RefreshToolTipText(includeSubTrees);
        }

        /// <summary>
        /// Refreshes the node's name.
        /// </summary>
        /// <param name="includeSubTrees"></param>
        protected virtual void RefreshName(bool includeSubTrees = false)
        {
            Name = GenerateName();
            if (includeSubTrees)
            {
                foreach (Base_TN node in Nodes) RefreshName(includeSubTrees);
            }
        }

        /// <summary>
        /// Refreshes the node's Text.
        /// </summary>
        protected virtual void RefreshText(bool includeSubTrees = false)
        {
            Text = GenerateText();
            if (includeSubTrees)
            {
                foreach (Base_TN node in Nodes) RefreshText(includeSubTrees);
            }
        }

        /// <summary>
        /// Refreshes the node's ToolTipText.
        /// </summary>
        /// <param name="includeSubTrees"></param>
        protected virtual void RefreshToolTipText(bool includeSubTrees = false)
        {
            ToolTipText = GenerateToolTipText();
            if (includeSubTrees)
            {
                foreach (Base_TN node in Nodes) RefreshToolTipText(includeSubTrees);
            }
        }

        /// <summary>
        /// Generates a name for the TreeNode.
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateName()
        {
            // Generate a name based on the current date and time.
            return "Unnamed node " + DateTime.Now.ToString();
        }

        /// <summary>
        /// Generates the Node's Text property based on the prefix and suffix
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateText()
        {
            // Alterations to the name formatting can be applied here.
            return Text_Prefix + Name + Text_Suffix;
        }

        /// <summary>
        /// Generates a fresh ToolTipText.
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateToolTipText()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Name: " + Name + Environment.NewLine);
            sb.Append("Text: " + Text + Environment.NewLine);
            sb.Append("NodeType: " + NodeType + Environment.NewLine);
            sb.Append("FullPath: ");
            if (TreeView == null) sb.Append("Not available");
            else sb.Append(FullPath);
            sb.Append(Environment.NewLine);

            return sb.ToString();
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

        #region Methods

        /// <summary>
        /// Returns a list of all first generation child nodes, or all descendants via recursion.
        /// </summary>
        /// <param name="includeSubtrees"></param>
        /// <returns>Returns null if this node has no children.</returns>
        public List<Base_TN> GetChildNodes(bool includeSubtrees = false)
        {
            List<Base_TN> _listOfChildNodes = new List<Base_TN> { this };
            foreach (Base_TN child in Nodes)
            {
                if (!includeSubtrees) _listOfChildNodes.Add(child);
                else _listOfChildNodes.AddRange(child.GetChildNodes(includeSubtrees: true));
            }
            return _listOfChildNodes.Count > 0 ? _listOfChildNodes : null;
        }

        /*

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

        */

        #endregion

        #region Fields

        protected string _text_Prefix = null;
        protected string _text_Suffix = null;
        protected HETreeNodeType _nodeType = HETreeNodeType.Unknown;

        #endregion

        #region Enumerations

        #endregion






    }

    /// <summary>
    /// Defines an interface for the parent object of the Base_TN.
    /// </summary>
    public interface Iparent_Base_TN
    {
        /// <summary>
        /// Defines the RootNode Base_TN for the object.
        /// </summary>
        //Base_TN RootNode { get; set; }

    }

}
