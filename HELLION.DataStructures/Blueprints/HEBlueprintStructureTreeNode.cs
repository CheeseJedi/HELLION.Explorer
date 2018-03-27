namespace HELLION.DataStructures
{
    public class HEBlueprintStructureTreeNode : HEBlueprintTreeNode
    {
        public HEBlueprintStructureTreeNode(HEBlueprint.HEBlueprintStructure passedOwner = null, string nodeName = null,
            string nodeText = null, string nodeToolTipText = null)
            : base(passedOwner, nodeName, HETreeNodeType.BlueprintStructure, nodeText, nodeToolTipText)
        {

        }

        public bool DisplayRootStructureIcon
        {
            get { return displayRootStructureIcon; }
            set
            {
                // Only make the change if it's actually a new value.
                if (value != displayRootStructureIcon)
                {
                    NodeType = value ? HETreeNodeType.BlueprintRootStructure : HETreeNodeType.BlueprintStructure;
                }
            }
        }

        protected bool displayRootStructureIcon = false;
    }



}
