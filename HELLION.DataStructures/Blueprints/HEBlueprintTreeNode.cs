namespace HELLION.DataStructures
{
    public class Blueprint_TreeNode : HETreeNode
    {
        public Blueprint_TreeNode(object passedOwner = null, string nodeName = null,
            HETreeNodeType newNodeType = HETreeNodeType.Blueprint, string nodeText = "", string nodeToolTipText = "")
            : base(passedOwner, nodeName, newNodeType, nodeText, nodeToolTipText)
        {

        }
    }
}
