using HELLION.DataStructures.UI;

namespace HELLION.DataStructures.Blueprints
{
    public class HierarchyView_TN : Base_TN
    {
        public HierarchyView_TN(IParent_Base_TN passedOwner = null, string nodeName = null,
            Base_TN_NodeType newNodeType = Base_TN_NodeType.BlueprintHierarchyView)
            : base(passedOwner, newNodeType, nodeName)
        {

        }
    }

}
