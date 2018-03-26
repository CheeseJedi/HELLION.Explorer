namespace HELLION.DataStructures
{
    public class HEBlueprintStructureTreeNode : HEBlueprintTreeNode
    {
        public HEBlueprintStructureTreeNode(HEBlueprint.HEBlueprintStructure passedOwner = null, string nodeName = null,
            string nodeText = null, string nodeToolTipText = null)
            : base(passedOwner, nodeName, HETreeNodeType.BlueprintStructure, nodeText, nodeToolTipText)
        {

        }
    }



}
