using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HELLION.DataStructures
{
    public class HESearchHandlerTreeNode : HETreeNode
    {
        public HESearchHandler.HESearchOperator ParentSearchOperator { get => parentSearchOperator; }
        
        private HESearchHandler.HESearchOperator parentSearchOperator = null;

        
        public HESearchHandlerTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.Unknown, 
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {

        }
        
        /// <summary>
        /// Constructor that takes a minimum of a parent HESearchHandler and a name, 
        /// but also optionally a type and text (display name).
        /// </summary>
        /// <param name="passedParentSearchHandler"></param>
        /// <param name="nodeName"></param>
        /// <param name="newNodeType"></param>
        /// <param name="nodeText"></param>
        /// <param name="nodeToolTipText"></param>
        public HESearchHandlerTreeNode(HESearchHandler.HESearchOperator passedParentSearchOperator, string nodeName, 
            HETreeNodeType newNodeType = HETreeNodeType.Unknown, string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(nodeName, newNodeType, nodeText, nodeToolTipText, passedOwner)
        {
            parentSearchOperator = passedParentSearchOperator ?? throw new NullReferenceException("passedParentSearchOperator was null.");

        }


        public void AddResult()
        {
            // test option
            if (parentSearchOperator == null) throw new NullReferenceException("parentSearchOperator was null.");
            else
            {
                parentSearchOperator.Execute();
            }


        }

    }
}
