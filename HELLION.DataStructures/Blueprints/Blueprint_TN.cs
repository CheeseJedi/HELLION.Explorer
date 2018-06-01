using HELLION.DataStructures.UI;

namespace HELLION.DataStructures.Blueprints
{
    public class Blueprint_TN : Base_TN
    {
        public Blueprint_TN(Iparent_Base_TN passedOwner = null, string nodeName = null,
            Base_TN_NodeType newNodeType = Base_TN_NodeType.Blueprint)
            : base(passedOwner, nodeName, newNodeType)
        {

        }
    }
}
