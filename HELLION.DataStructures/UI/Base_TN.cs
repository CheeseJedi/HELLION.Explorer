using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using static HELLION.DataStructures.EmbeddedImages.EmbeddedImages_ImageList;

namespace HELLION.DataStructures.UI
{
    public class Base_TN : TreeNode, Iparent_Base_TN
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
        /// <param name="nodeType"></param>
        public Base_TN(Iparent_Base_TN ownerObject, string nodeName = null,
            Base_TN_NodeType nodeType = Base_TN_NodeType.Unknown) : this(ownerObject)
        {
            NodeType = nodeType;

            if (nodeName != null && nodeName != String.Empty)
            {
                // A value for the name was supplied - no auto-generation.
                Name = nodeName;
            }
            else
            {
                AutoGenerateName = true;
                RefreshName();
            }


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
                    //if (value == null) RefreshName();
                    RefreshText();
                    //RefreshToolTipText();
                }
            }
        }

        /// <summary>
        /// Modifies the base's Text property so it's inaccessible for setting externally.
        /// </summary>
        public new string Text
        {
            get => base.Text;
            private set => base.Text = value;
        }

        /// <summary>
        /// Modifies the base's ToolTipText property so it's inaccessible for setting externally.
        /// </summary>
        public new string ToolTipText
        {
            get => base.ToolTipText;
            private set => base.ToolTipText = value;
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
        /// The separator character(s) placed between the prefix, name and suffix.
        /// </summary>
        public string Text_Seperator
        {
            get => _text_Seperator;
            set
            {
                if (_text_Seperator != value)
                {
                    _text_Seperator = value;

                    // Trigger updates here.
                    //RefreshText();
                    //RefreshToolTipText();
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
        /// The Base_TN_NodeType type of this node.
        /// </summary>
        public Base_TN_NodeType NodeType
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
        /// Is used to determine whether the node should auto-generate it's name.
        /// </summary>
        public bool AutoGenerateName { get; set; } = false;

        /// <summary>
        /// Define a default name for nodes that don't auto-generate and haven't had one specified.
        /// </summary>
        protected virtual string DefaultObjectName => "Unnamed node";

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
            RefreshToolTipText(includeSubTrees);
        }

        /// <summary>
        /// Refreshes the node's name.
        /// </summary>
        /// <param name="includeSubTrees"></param>
        protected virtual void RefreshName(bool includeSubTrees = false)
        {
            if (AutoGenerateName) Name = GenerateName();
            if (includeSubTrees)
            {
                foreach (Base_TN node in Nodes)
                {
                    RefreshName(includeSubTrees);
                }
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
            // Return the default name for an unnamed node.
            return DefaultObjectName;
        }

        /// <summary>
        /// Generates the Node's Text property based on the prefix and suffix
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateText()
        {
            // Alterations to the name formatting can be applied here.

            //return String.Format("{0}{1}{2}",
            //    (Text_Prefix.Length > 0 ? Text_Prefix + _text_Seperator : String.Empty), Name,
            //    (Text_Suffix.Length > 0 ? _text_Seperator + Text_Suffix : String.Empty) );

            return String.Format("{1}{0}{2}{0}{3}", _text_Seperator, Text_Prefix, Name, Text_Suffix);
            //return Text_Prefix + Name + Text_Suffix;
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
        protected Base_TN_NodeType _nodeType = Base_TN_NodeType.Unknown;
        protected string _text_Seperator = " ";

        #endregion

    }

    /// <summary>
    /// Defines an interface for the parent object of the Base_TN.
    /// </summary>
    public interface Iparent_Base_TN
    { }

}
