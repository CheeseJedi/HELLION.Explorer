namespace HELLION.DataStructures
{
    public class HEBlueprintStructureTreeNode : HETreeNode
    {
        public HEBlueprintStructureTreeNode(object passedOwner = null, string nodeName = null,
            HETreeNodeType newNodeType = HETreeNodeType.BlueprintStructure, string nodeText = null, string nodeToolTipText = null)
            : base(passedOwner, nodeName, newNodeType, nodeText, nodeToolTipText)
        {

        }
    }



}
