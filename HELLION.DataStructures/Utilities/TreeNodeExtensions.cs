using System.Windows.Forms;

namespace HELLION.DataStructures.Utilities
{
    public static class TreeNodeExtensions
    {

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
        /// Returns the calculated path for the TreeNode based on the node's Name rather
        /// than it's Text and doesn't care whether it's in a TreeView or not.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string RealPath(this TreeNode node, string seperator = ">")
        {
            if (node.Parent == null) return node.Name;
            return RealPath(node.Parent, seperator) + seperator + node.Name;
        }

    }
}
