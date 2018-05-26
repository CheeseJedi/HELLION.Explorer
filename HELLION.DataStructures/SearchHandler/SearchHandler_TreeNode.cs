using System;

namespace HELLION.DataStructures
{
    public class SearchHandler_TreeNode : HETreeNode
    {
        public SearchHandler.HESearchOperator ParentSearchOperator { get => _parentSearchOperator; }
        
        private SearchHandler.HESearchOperator _parentSearchOperator = null;

        
        public SearchHandler_TreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.Unknown, 
            string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(passedOwner, nodeName, newNodeType, nodeText, nodeToolTipText)
        {

        }
        
        /// <summary>
        /// Constructor that takes a minimum of a parent SearchHandler and a name, 
        /// but also optionally a type and text (display name).
        /// </summary>
        /// <param name="passedParentSearchHandler"></param>
        /// <param name="nodeName"></param>
        /// <param name="newNodeType"></param>
        /// <param name="nodeText"></param>
        /// <param name="nodeToolTipText"></param>
        public SearchHandler_TreeNode(SearchHandler.HESearchOperator passedParentSearchOperator, string nodeName, 
            HETreeNodeType newNodeType = HETreeNodeType.Unknown, string nodeText = "", string nodeToolTipText = "", object passedOwner = null)
            : base(passedOwner, nodeName, newNodeType, nodeText, nodeToolTipText)
        {
            _parentSearchOperator = passedParentSearchOperator ?? throw new NullReferenceException("passedParentSearchOperator was null.");

        }


        public void AddResult()
        {
            // test option
            if (_parentSearchOperator == null) throw new NullReferenceException("_parentSearchOperator was null.");
            else
            {
                _parentSearchOperator.Execute();
            }


        }

    }
}
