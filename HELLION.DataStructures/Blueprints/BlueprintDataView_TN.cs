using HELLION.DataStructures.Document;
using HELLION.DataStructures.UI;

namespace HELLION.DataStructures.Blueprints
{
    public class BlueprintDataView_TN : Json_TN
    {
        public BlueprintDataView_TN(Iparent_Base_TN passedOwner = null, string nodeName = null,
            HETreeNodeType newNodeType = HETreeNodeType.BlueprintDataView, string nodeText = null, string nodeToolTipText = null)
            : base(passedOwner, nodeName, newNodeType) //, nodeText, nodeToolTipText)
        {

        }
    }
}
