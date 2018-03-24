namespace HELLION.DataStructures
{
    public class HEBlueprintTreeNode : HETreeNode
    {
        public HEBlueprintTreeNode(object passedOwner = null, string nodeName = null,
            HETreeNodeType newNodeType = HETreeNodeType.Blueprint, string nodeText = "", string nodeToolTipText = "")
            : base(passedOwner, nodeName, newNodeType, nodeText, nodeToolTipText)
        {

        }
    }
}
