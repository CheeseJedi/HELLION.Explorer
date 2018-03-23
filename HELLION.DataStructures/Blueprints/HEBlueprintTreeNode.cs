namespace HELLION.DataStructures
{
    public class HEBlueprintTreeNode : HETreeNode
    {
        public HEBlueprintTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.Blueprint, 
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {

        }
    }

    public class HEBlueprintStructureTreeNode : HETreeNode
    {
        public HEBlueprintStructureTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.BlueprintStructure,
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {

        }
    }

    public class HEBlueprintDockingPortTreeNode : HETreeNode
    {
        public HEBlueprintDockingPortTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.BlueprintDockingPort,
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {

        }
    }
    
    public class HEBlueprintsViewTreeNode : HETreeNode
    {
        public HEBlueprintsViewTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.BlueprintDockingPort,
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {

        }
    }

    public class HEBlueprintHierarchyViewTreeNode : HETreeNode
    {
        public HEBlueprintHierarchyViewTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.BlueprintHierarchyView,
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {

        }
    }

    public class HEBlueprintDataViewTreeNode : HEGameDataTreeNode
    {
        public HEBlueprintDataViewTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.BlueprintDataView,
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {

        }
    }



}
