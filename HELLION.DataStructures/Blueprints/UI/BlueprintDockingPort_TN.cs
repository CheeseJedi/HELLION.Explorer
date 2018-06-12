using System;
using System.Diagnostics;
using System.Text;
using HELLION.DataStructures.UI;
using static HELLION.DataStructures.Blueprints.StationBlueprint;

namespace HELLION.DataStructures.Blueprints
{
    public class BlueprintDockingPort_TN : Base_TN
    {
        public BlueprintDockingPort_TN(IParent_Base_TN passedOwner = null)
            : base(passedOwner)
        {
            // Set NodeType
            NodeType = Base_TN_NodeType.BlueprintDockingPort;

            // Enable name auto-generation.
            //AutoGenerateName = true;

            // Trigger name generation.
            //RefreshName();
        }

        /// <summary>
        /// Refreshes the node's name.
        /// </summary>
        /// <param name="includeSubTrees"></param>
        protected override void RefreshName(bool includeSubTrees = false)
        {
            BlueprintDockingPort dockingPort = (BlueprintDockingPort)OwnerObject;
            BlueprintStructure structure = dockingPort.OwnerStructure;

            //Debug.Print("structure {0}, ({1}), ID {2}", (int)structure?.SceneID, structure?.SceneName, structure?.StructureID);
            //Debug.Print("docking port {0}, ({1})", dockingPort?.OrderID, dockingPort?.PortName);

            if (structure == null || dockingPort == null || structure.SceneID == null
                || dockingPort.PortName == null || !AutoGenerateName)
            {
                Debug.Print("RefreshName() skipped.");
                if (structure == null) Debug.Print("structure == null");
                else if (structure.SceneID == null) Debug.Print("structure.SceneID == null");

                if (dockingPort == null) Debug.Print("dockingPort == null");
                else if (dockingPort.PortName == null) Debug.Print("dockingPort.PortName == null");

                if (!AutoGenerateName) Debug.Print("AutoGenerate == false");

            }
            else
            {

                Name = GenerateName();
                Debug.Print("RefreshName() generated [{0}].", Name);
            }

            if (includeSubTrees)
            {
                foreach (Base_TN node in Nodes)
                {
                    RefreshName(includeSubTrees);
                }
            }
        }

        /// <summary>
        /// Generates a name for the TreeNode.
        /// </summary>
        /// <returns></returns>
        protected override string GenerateName()
        {
            // Generate a name based on the current docking port names.
            BlueprintDockingPort ownerPort = (BlueprintDockingPort)OwnerObject;
            Debug.Print("### Docking Port TN GenerateName() called - {0}", ownerPort.PortName);

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
