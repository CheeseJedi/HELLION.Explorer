namespace HELLION.DataStructures
{
    public class HEBlueprintCollectionTreeNode : HEGameDataTreeNode
    {
        public HEBlueprintCollectionTreeNode(HEBlueprintCollection passedOwner = null, string nodeName = null,
             string nodeText = null, string nodeToolTipText = null)
            : base(passedOwner, nodeName, HETreeNodeType.BlueprintCollection, nodeText, nodeToolTipText)
        {

        }
    }


}
