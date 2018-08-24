using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using static HELLION.DataStructures.EmbeddedImages.EmbeddedImages_ImageList;
using static HELLION.DataStructures.Utilities.TreeNodeExtensions;

namespace HELLION.DataStructures.UI
{
    public class Base_TN : TreeNode, IParent_Base_TN
    {
        #region Constructors

        /// <summary>
        /// Default constructor that attempts to set the owner object.
        /// </summary>
        /// <param name="ownerObject"></param>
        public Base_TN(IParent_Base_TN ownerObject = null)
        {
            OwnerObject = ownerObject;
            base.ToolTipText = null;
        }

        /// <summary>
        /// Normal constructor - requires the owner object.
        /// </summary>
        /// <param name="ownerObject"></param>
        /// <param name="nodeType"></param>
        /// <param name="nodeName"></param>
        public Base_TN(IParent_Base_TN ownerObject,
            Base_TN_NodeType nodeType = Base_TN_NodeType.Unknown,
            string nodeName = null) : this(ownerObject)
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
                if (ownerObject != null) RefreshName();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Stores a reference to the owning object.
        /// </summary>
        public IParent_Base_TN OwnerObject { get; set; } = null;

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

                    // Triggered updates.
                    //if (value == null) RefreshName();
                    RefreshText();
                }
            }
        }

        /// <summary>
        /// Modifies the base's Text property so it's inaccessible for setting externally.
        /// </summary>
        public new string Text
        {
            get => base.Text;
            private set
            {
                if (base.Text != value)
                {
                    base.Text = value;

                    // Triggered updates.
                    // This could be bad :/
                    RefreshToolTipText();
                }
                
            }
        }

        /// <summary>
        /// Modifies the base's ToolTipText property so it's inaccessible for setting externally.
        /// </summary>
        public new string ToolTipText
        {
            get
            {
                if (string.IsNullOrEmpty(base.ToolTipText))
                    base.ToolTipText = GenerateToolTipText();

                return base.ToolTipText;
            }
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
        /// Returns a list including this node and all child nodes.
        /// </summary>
        public List<Base_TN> AllNodes => this.GetChildNodes(true);

        /// <summary>
        /// Define a default name for nodes that don't auto-generate and haven't had one specified.
        /// </summary>
        protected virtual string DefaultObjectName => "Unnamed node";

        /// <summary>
        /// Tracks the lock status of this node. 
        /// </summary>
        /// <remarks>
        /// Used to implement branch locking, to allow safe editing.
        /// </remarks>
        public bool Locked { get; protected set; } = false;

        public string DefaultPathSeperator { get; } = ">";

        /// <summary>
        /// The Path to the node using node Names instead of Text.
        /// </summary>
        /// <remarks>
        /// Does not include the node itself.
        /// </remarks>
        //public string Path => Parent != null ? FullPathBasedOnName(Parent, DefaultPathSeperator) : null;

        //public string Path => Parent?.FullPath;

        /// <summary>
        /// The path to the node including the node using Names instead of Text.
        /// </summary>
        //public new string FullPath => FullPathBasedOnName(this, DefaultPathSeperator);

        #endregion

        #region Refresh and Generation Methods

        /// <summary>
        /// Triggers the node to rebuild it's name, text and ToolTipText.
        /// </summary>
        /// <param name="includeSubTrees"></param>
        public virtual void Refresh(bool includeSubTrees = false)
        {
            //Debug.Print("Base_TN.Refresh called: node Name [{0}], Text [{1}], (parent hash [{2}], type [{3}])", Name, Text, OwnerObject.GetHashCode(), OwnerObject.GetType());
            RefreshName();
            RefreshText();
            RefreshToolTipText();

            // Process child nodes if necessary.
            if (includeSubTrees)
            {
                foreach (Base_TN node in Nodes)
                {
                    Refresh(includeSubTrees);
                }
            }

        }

        /// <summary>
        /// Refreshes the node's name.
        /// </summary>
        /// <param name="includeSubTrees"></param>
        protected virtual void RefreshName()
        {
            if (AutoGenerateName)
            {
                string newName = GenerateName();

                if (newName != String.Empty)
                {
                    Name = newName;
                    // Name has been generated, deactivate.
                    AutoGenerateName = false;
                    //Debug.Print("Base_TN.RefreshName: Generated name [" + Name +"]");
                }
            }
        }

        /// <summary>
        /// Refreshes the node's Text.
        /// </summary>
        protected virtual void RefreshText(bool includeSubTrees = false)
        {
            Text = GenerateText();
        }

        /// <summary>
        /// Refreshes the node's ToolTipText.
        /// </summary>
        /// <param name="includeSubTrees"></param>
        protected virtual void RefreshToolTipText(bool includeSubTrees = false)
        {
            ToolTipText = GenerateToolTipText();
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
            // Alterations to the name formatting can be applied by overriding this method.
            return String.Format("{0}{1}{2}",
                (Text_Prefix?.Length > 0 ? Text_Prefix + _text_Seperator : String.Empty), Name,
                (Text_Suffix?.Length > 0 ? _text_Seperator + Text_Suffix : String.Empty));
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
            //sb.Append("RealPath: " + this.RealPath() + Environment.NewLine);

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
        /// Returns the path of the node generated from node the Name field (rather than the
        /// default of using the Text field. 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        //private string FullPathBasedOnName(TreeNode node, string separator)
        //{
        //    // if (string.IsNullOrEmpty(separator)) separator = DefaultPathSeperator;

        //    if (node != null) throw new Exception();
        //    {
        //        Debug.Print("FullPathBasedOnName Name " + node.Name);

        //        if (Parent != null)
        //        {
        //            string parentPath = FullPathBasedOnName(node.Parent, separator);
        //            return !string.IsNullOrEmpty(parentPath) ? parentPath + separator + Name : Name;

        //        }






        //    }
        //    Debug.Print("FullPathBasedOnName null - hit a root");
        //    return string.Empty;

        //}


        /// <summary>
        /// Returns a list of this plus all first generation child nodes, 
        /// or all descendants via recursion.
        /// </summary>
        /// <param name="includeSubtrees"></param>
        /// <returns>Returns null if this node has no children.</returns>
        public List<Base_TN> GetChildNodes(bool includeSubtrees = false)
        {
            // Create a new list and add this node to it.
            List<Base_TN> _listOfChildNodes = new List<Base_TN> { this };

            foreach (Base_TN child in Nodes)
            {
                // If it's not sub-trees, then just add each child node.
                if (!includeSubtrees) _listOfChildNodes.Add(child);
                // Otherwise add the child node and it's nodes via recursion.
                else _listOfChildNodes.AddRange(child.GetChildNodes(includeSubtrees: true));
            }
            return _listOfChildNodes.Count > 0 ? _listOfChildNodes : null;
        }

        /// <summary>
        /// Attempts to lock this node.
        /// </summary>
        /// <returns>Returns false if the node was not able to be locked. </returns>
        public bool AttemptLock()
        {
            // Redundant check?
            if (Locked) return false;

            if (SelfOrAncestorLocked()) return false;
            if (SelfOrDescendantLocked()) return false;

            // Got this far, so lock the node.
            Locked = true;

            return true;
        }

        /// <summary>
        /// Unlocks the node.
        /// </summary>
        public void Unlock() => Locked = false;

        /// <summary>
        /// Determines whether this node, or an ancestor, is locked.
        /// </summary>
        /// <returns>Returns true if this or an ancestor node is locked.</returns>
        public bool SelfOrAncestorLocked()
        {
            if (Locked) return true;

            // If the Parent is null we've reached the top of the tree (a tree root node has no parent)
            if (Parent == null) return false;

            // Attempt to cast the parent to a Base_TN.
            Base_TN parent = (Base_TN)Parent;

            // Sanity check.
            if (parent == null) throw new NullReferenceException("SelfOrAncestorLocked: Cast Parent to Base_TN resulted in null.");

            // Ask the parent the same question via recursion.
            return parent.SelfOrAncestorLocked();

        }

        /// <summary>
        /// Determines whether this node, or a descendant, is locked.
        /// </summary>
        /// <returns>Returns true if this or an descendant node is locked.</returns>
        public bool SelfOrDescendantLocked()
        {
            if (Locked) return true;

            foreach (Base_TN node in Nodes)
            {
                if (node.SelfOrDescendantLocked()) return true;

            }

            return false;
        }

        /// <summary>
        /// The number of descendants that are locked. Not currently used.
        /// </summary>
        /// <returns></returns>
        public int CountOfLockedDescendants()
        {
            int count = 0;
            foreach (Base_TN node in Nodes)
            {
                if (node.Locked) count++;
                count += node.CountOfLockedDescendants();
            }

            return count;
        }

        /// <summary>
        /// Returns the Path of this node, with reference to the TreeView control.
        /// </summary>
        /// <remarks>
        /// Returns FullPath but strips off the node name and last path separator.
        /// </remarks>
        //public string Path
        //{
        //    get
        //    {
        //        if (TreeView != null)
        //        {
        //            string fullPath = FullPath;
        //            int lastIndex = fullPath.LastIndexOf(TreeView.PathSeparator);
        //            // Ensure the lastIndex is not -1, present zero instead.
        //            lastIndex = lastIndex != -1 ? lastIndex : 0;
        //            return fullPath.Substring(0, lastIndex);
        //        }
        //        else return "Path Unavailable (no TreeView)";
        //    }
        //}

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
    public interface IParent_Base_TN
    { }

}
