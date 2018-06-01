using System;
using HELLION.DataStructures.UI;

namespace HELLION.DataStructures.Search
{
    public class SearchHandler_TN : Base_TN
    {
        public SearchHandler.HESearchOperator ParentSearchOperator { get; } = null;

        public SearchHandler_TN(string nodeName, Iparent_Base_TN passedOwner = null,
            Base_TN_NodeType newNodeType = Base_TN_NodeType.Unknown)
            : base(passedOwner, nodeName, newNodeType)
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
        public SearchHandler_TN(SearchHandler.HESearchOperator passedParentSearchOperator, string nodeName,
            Iparent_Base_TN passedOwner = null, Base_TN_NodeType newNodeType = Base_TN_NodeType.Unknown)
            : base(passedOwner, nodeName, newNodeType)
        {
            ParentSearchOperator = passedParentSearchOperator ?? throw new NullReferenceException("passedParentSearchOperator was null.");

        }


        public void AddResult()
        {
            // test option
            if (ParentSearchOperator == null) throw new NullReferenceException("_parentSearchOperator was null.");
            else
            {
                ParentSearchOperator.Execute();
            }


        }

    }
}
