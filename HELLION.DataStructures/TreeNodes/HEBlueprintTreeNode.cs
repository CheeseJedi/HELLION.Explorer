using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HELLION.DataStructures
{
    public class HEBlueprintTreeNode : HETreeNode
    {
        public HEBlueprintTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.Unknown, string nodeText = "", string nodeToolTipText = "")
            : base(nodeName, newNodeType, nodeText, nodeToolTipText)
        {

        }



    }
}
