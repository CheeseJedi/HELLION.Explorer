using System;
using System.Text;
using HELLION.DataStructures.UI;
using static HELLION.DataStructures.Blueprints.StationBlueprint;

namespace HELLION.DataStructures.Blueprints
{
    public class BlueprintStructure_TN : Blueprint_TN
    {
        public BlueprintStructure_TN(Iparent_Base_TN passedOwner = null, string nodeName = null)
            : base(passedOwner, nodeName, Base_TN_NodeType.BlueprintStructure)
        {

        }
        
        /// <summary>
        /// Generates a name for the TreeNode.
        /// </summary>
        /// <returns></returns>
        protected override string GenerateName()
        {
            // Generate a name based on the current structure type.
            BlueprintStructure ownerStructure = (BlueprintStructure)OwnerObject;
            return ownerStructure.SceneID.ToString() ??  "Unspecified Structure "; // + DateTime.Now.ToString();
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
