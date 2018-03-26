namespace HELLION.DataStructures
{
    public class HEBlueprintDockingPortTreeNode : HEBlueprintTreeNode
    {
        public HEBlueprintDockingPortTreeNode(HEBlueprint.HEBlueprintDockingPort passedOwner = null, string nodeName = null,
            string nodeText = null, string nodeToolTipText = null)
            : base(passedOwner, nodeName, HETreeNodeType.BlueprintDockingPort, nodeText, nodeToolTipText)
        {

        }
    }



}
