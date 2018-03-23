namespace HELLION.DataStructures
{
    public class HEBlueprintHierarchyViewTreeNode : HETreeNode
    {
        public HEBlueprintHierarchyViewTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.BlueprintHierarchyView,
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {

        }
    }



}
