using System.Collections.Generic;
using System.Windows.Forms;

namespace HELLION.DataStructures.Utilities
{
    public static class TreeNodeExtensions
    {
        /// <summary>
        /// Gets the first node that matches the given key in the current nodes children.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TreeNode GetFirstChildNodeByName(this TreeNode node, string key)
        {
            // May be defunct?
            if (node == null) return null;
            TreeNode[] _nodes = node.Nodes.Find(key, searchAllChildren: true);
            return _nodes.Length > 0 ? (TreeNode)_nodes[0] : null;
        }

        /// <summary>
        /// Returns the Path of this node, with reference to the TreeView control.
        /// </summary>
        /// <remarks>
        /// Returns FullPath but strips off the node name and last path separator.
        /// </remarks>
        public static string Path(this TreeNode node)
        {
            if (node == null) return null;
            if (node.TreeView != null)
            {
                string fullPath = node.FullPath;
                int lastIndex = fullPath.LastIndexOf(node.TreeView.PathSeparator);
                // Ensure the lastIndex is not -1, present zero instead.
                lastIndex = lastIndex != -1 ? lastIndex : 0;
                return fullPath.Substring(0, lastIndex);
            }
            else return "Path Unavailable (no TreeView)";
        }

        /// <summary>
        /// Returns a list of all first generation child nodes, or all descendants via recursion.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="includeSubtrees"></param>
        /// <returns>Returns an empty list if the node has no children.</returns>
        public static List<TreeNode> GetChildNodes(this TreeNode node, bool includeSubtrees = false)
        {
            List<TreeNode> _listOfChildNodes = new List<TreeNode> { node };
            foreach (TreeNode child in node.Nodes)
            {
                if (!includeSubtrees) _listOfChildNodes.Add(child);
                else _listOfChildNodes.AddRange(child.GetChildNodes(includeSubtrees: true));
            }
            return _listOfChildNodes.Count > 0 ? _listOfChildNodes : null;
        }


    }
}
