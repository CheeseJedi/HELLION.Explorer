using System;
using System.Text;
using HELLION.DataStructures.UI;
using static HELLION.DataStructures.Blueprints.StationBlueprint;

namespace HELLION.DataStructures.Blueprints
{
    public class BlueprintDockingPort_TN : Blueprint_TN
    {
        public BlueprintDockingPort_TN(Iparent_Base_TN passedOwner = null, string nodeName = null)
            : base(passedOwner, nodeName, Base_TN_NodeType.BlueprintDockingPort)
        {

        }

        /// <summary>
        /// Generates a name for the TreeNode.
        /// </summary>
        /// <returns></returns>
        protected override string GenerateName()
        {
            // Generate a name based on the current docking port names.
            HEBlueprintDockingPort ownerPort = (HEBlueprintDockingPort)OwnerObject;
            return ownerPort.PortName.ToString() ?? "Unspecified Docking Port";
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
