using HELLION.DataStructures.UI;

namespace HELLION.DataStructures.Blueprints
{
    public class Blueprint_TN : Base_TN
    {
        public Blueprint_TN(IParent_Base_TN passedOwner = null,
            Base_TN_NodeType newNodeType = Base_TN_NodeType.Blueprint,
            string nodeName = null)
            : base(passedOwner, newNodeType, nodeName)
        {

        }
    }
}
