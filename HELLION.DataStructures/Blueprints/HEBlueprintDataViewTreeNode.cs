namespace HELLION.DataStructures
{
    public class HEBlueprintDataViewTreeNode : HEGameDataTreeNode
    {
        public HEBlueprintDataViewTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.BlueprintDataView,
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {

        }
    }



}
