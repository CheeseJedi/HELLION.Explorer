﻿namespace HELLION.DataStructures
{
    public class HEBlueprintTreeNode : HETreeNode
    {
        public HEBlueprintTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.Blueprint, 
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {

        }
    }
}
