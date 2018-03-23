namespace HELLION.DataStructures
{
    public class HEBlueprintsViewTreeNode : HETreeNode
    {
        public HEBlueprintsViewTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.BlueprintDockingPort,
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {

        }
    }



}
