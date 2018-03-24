namespace HELLION.DataStructures
{
    public class HEBlueprintDataViewTreeNode : HEGameDataTreeNode
    {
        public HEBlueprintDataViewTreeNode(object passedOwner = null, string nodeName = null,
            HETreeNodeType newNodeType = HETreeNodeType.BlueprintDataView, string nodeText = null, string nodeToolTipText = null)
            : base(passedOwner, nodeName, newNodeType, nodeText, nodeToolTipText)
        {

        }
    }


}
