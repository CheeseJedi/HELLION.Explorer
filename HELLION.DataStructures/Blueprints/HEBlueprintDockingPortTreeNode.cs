namespace HELLION.DataStructures.Blueprints
{
    public class HEBlueprintDockingPortTreeNode : Blueprint_TreeNode
    {
        public HEBlueprintDockingPortTreeNode(StationBlueprint.HEBlueprintDockingPort passedOwner = null, string nodeName = null,
            string nodeText = null, string nodeToolTipText = null)
            : base(passedOwner, nodeName, HETreeNodeType.BlueprintDockingPort, nodeText, nodeToolTipText)
        {

        }
    }



}
