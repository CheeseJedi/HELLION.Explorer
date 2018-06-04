using HELLION.DataStructures.UI;

namespace HELLION.DataStructures.Blueprints
{
    public class BlueprintDataView_TN : Json_TN
    {
        public BlueprintDataView_TN(IParent_Base_TN passedOwner = null, string nodeName = null,
            Base_TN_NodeType newNodeType = Base_TN_NodeType.BlueprintDataView)
            : base(passedOwner, newNodeType, nodeName)
        {

        }
    }
}
