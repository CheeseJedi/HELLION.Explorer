namespace HELLION.DataStructures
{
    public class BlueprintStructure_TreeNode : Blueprint_TreeNode
    {
        public BlueprintStructure_TreeNode(StationBlueprint.BlueprintStructure passedOwner = null, string nodeName = null,
            string nodeText = null, string nodeToolTipText = null)
            : base(passedOwner, nodeName, HETreeNodeType.BlueprintStructure, nodeText, nodeToolTipText)
        {

        }
    }
}
