using System;
using System.Text;
using HELLION.DataStructures.UI;

namespace HELLION.DataStructures.Blueprints
{
    public class BlueprintStructure_TN : Blueprint_TN
    {
        public BlueprintStructure_TN(Iparent_Base_TN passedOwner = null, string nodeName = null)
            : base(passedOwner, nodeName, HETreeNodeType.BlueprintStructure)
        {

        }
        
        /// <summary>
        /// Generates a name for the TreeNode.
        /// </summary>
        /// <returns></returns>
        protected override string GenerateName()
        {
            // Generate a name based on the current structure type.
            return "Unnamed node "; // + DateTime.Now.ToString();
        }

        /// <summary>
        /// Generates a fresh ToolTipText.
        /// </summary>
        /// <returns></returns>
        protected override string GenerateToolTipText()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Name: " + Name + Environment.NewLine);
            sb.Append("Text: " + Text + Environment.NewLine);



            // sb.Append("NodeType: " + NodeType + Environment.NewLine);
            // sb.Append("FullPath: " + FullPath + Environment.NewLine);

            return sb.ToString();
        }

    }
}
