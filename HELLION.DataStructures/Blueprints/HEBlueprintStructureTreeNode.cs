namespace HELLION.DataStructures
{
    public class HEBlueprintStructureTreeNode : HETreeNode
    {
        public HEBlueprintStructureTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.BlueprintStructure,
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {

        }
    }



}
