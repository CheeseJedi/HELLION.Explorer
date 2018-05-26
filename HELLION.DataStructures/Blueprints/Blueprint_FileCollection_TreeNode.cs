using HELLION.DataStructures.Document;

namespace HELLION.DataStructures.Blueprints
{
    public class Blueprint_FileCollection_TreeNode : Json_TreeNode
    {
        public Blueprint_FileCollection_TreeNode(Blueprint_FileCollection passedOwner = null, string nodeName = null,
             string nodeText = null, string nodeToolTipText = null)
            : base(passedOwner, nodeName, HETreeNodeType.BlueprintCollection, nodeText, nodeToolTipText)
        {

        }
    }
}
