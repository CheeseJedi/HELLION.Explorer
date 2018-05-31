using HELLION.DataStructures.Document;

namespace HELLION.DataStructures.Blueprints
{
    public class Blueprint_FileCollection_TN : Json_TN
    {
        public Blueprint_FileCollection_TN(Blueprint_FileCollection passedOwner = null, string nodeName = null)
             //, string nodeText = null, string nodeToolTipText = null)
            : base(passedOwner, nodeName, HETreeNodeType.BlueprintCollection) //, nodeText, nodeToolTipText)
        {

        }
    }
}
