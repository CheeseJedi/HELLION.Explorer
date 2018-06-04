using HELLION.DataStructures.Document;
using HELLION.DataStructures.UI;

namespace HELLION.DataStructures.Blueprints
{
    public class Blueprint_FileCollection_TN : Json_TN
    {
        public Blueprint_FileCollection_TN(Blueprint_FileCollection passedOwner = null, string nodeName = null)
             //, string nodeText = null, string nodeToolTipText = null)
            : base(passedOwner, Base_TN_NodeType.BlueprintCollection, nodeName) //, nodeText, nodeToolTipText)
        {

        }
    }
}
