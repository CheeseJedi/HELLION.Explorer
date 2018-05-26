namespace HELLION.DataStructures.Blueprints
{
    public class HEBlueprintHierarchyViewTreeNode : HETreeNode
    {
        public HEBlueprintHierarchyViewTreeNode(object passedOwner = null, string nodeName = null,
            HETreeNodeType newNodeType = HETreeNodeType.BlueprintHierarchyView, string nodeText = null, string nodeToolTipText = null)
            : base(passedOwner, nodeName, newNodeType, nodeText, nodeToolTipText)
        {

        }
    }



}
