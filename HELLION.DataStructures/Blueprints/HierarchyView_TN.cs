using HELLION.DataStructures.UI;

namespace HELLION.DataStructures.Blueprints
{
    public class HierarchyView_TN : Base_TN
    {
        public HierarchyView_TN(Iparent_Base_TN passedOwner = null, string nodeName = null,
            HETreeNodeType newNodeType = HETreeNodeType.BlueprintHierarchyView)
            : base(passedOwner, nodeName, newNodeType)
        {

        }
    }

}
