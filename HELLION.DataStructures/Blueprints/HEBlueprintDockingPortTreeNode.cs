namespace HELLION.DataStructures
{
    public class HEBlueprintDockingPortTreeNode : HETreeNode
    {
        public HEBlueprintDockingPortTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.BlueprintDockingPort,
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {

        }
    }



}
