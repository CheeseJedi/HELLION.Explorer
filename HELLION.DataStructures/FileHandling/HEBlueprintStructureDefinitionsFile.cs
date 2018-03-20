using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using System.Runtime.CompilerServices;

namespace HELLION.DataStructures
{
    /// <summary>
    /// 
    /// </summary>
    public class HEBlueprintStructureDefinitionsFile : HEJsonBaseFile
    {
        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="passedFileInfo">The FileInfo representing the file to be loaded.</param>
        public HEBlueprintStructureDefinitionsFile(FileInfo passedFileInfo, object passedParentObject, int populateNodeTreeDepth)
        {
            // Blueprint files

            if (passedParentObject == null) throw new NullReferenceException();
            else parent = (IHENotificationReceiver)passedParentObject;

            if (passedFileInfo == null) throw new NullReferenceException();
            else
            {
                fileInfo = passedFileInfo;

                rootNode = new HEBlueprintTreeNode(File.Name, HETreeNodeType.DataFile, nodeToolTipText: File.FullName);

                dataViewRootNode = new HEGameDataTreeNode("Data View", HETreeNodeType.DataView,
                    nodeToolTipText: "Shows a representation of the Json data that makes up this blueprint.", passedOwner: this);

                definitionViewRootNode = new HESolarSystemTreeNode("Definition View", HETreeNodeType.BlueprintStructureDefinitionView,
                    nodeToolTipText: "Shows a representation of each structure definition and its docking ports.", passedOwner: this);

                rootNode.Nodes.Add(dataViewRootNode);
                rootNode.Nodes.Add(definitionViewRootNode);

                if (!File.Exists) throw new FileNotFoundException();
                else
                {
                    LoadFile();
                    // Populate the BlueprintStructureDefinitions object.
                    DeserialiseToBlueprintStructureDefinitionsObject();
                    // Populate the data view.
                    dataViewRootNode.Tag = jData;
                    dataViewRootNode.CreateChildNodesFromjData(populateNodeTreeDepth);
                }
                // Populate the hierarchy view.
                BuildHierarchyView();
            }
        }

        public new HETreeNode RootNode => rootNode;

        /// <summary>
        /// This class overrides the type of root node to represent a blueprint.
        /// </summary>
        protected new HETreeNode rootNode;

        public HEGameDataTreeNode DataViewRootNode => dataViewRootNode;
        protected HEGameDataTreeNode dataViewRootNode = null;
        protected HESolarSystemTreeNode definitionViewRootNode = null;

        /// <summary>
        /// 
        /// </summary>
        public HEBlueprintStructureDefinitions BlueprintStructureDefinitionsObject => blueprintStructureDefinitionsObject;

        /// <summary>
        /// This is the actual BlueprintStructureDefinition object - serialised and de-serialised from here.
        /// </summary>
        protected HEBlueprintStructureDefinitions blueprintStructureDefinitionsObject = null;

        /// <summary>
        /// Builds tree nodes from the GameData nodes, with cross-references
        /// </summary>
        public void BuildHierarchyView()
        {
            foreach (HEBlueprintStructureDefinitions.HEBlueprintStructureDefinition structDefn 
                in blueprintStructureDefinitionsObject.StructureDefinitions
                .Reverse<HEBlueprintStructureDefinitions.HEBlueprintStructureDefinition>())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("StructureType: " + structDefn.SanitisedName + Environment.NewLine);
                sb.Append("ItemID: " + structDefn.ItemID + Environment.NewLine);
                sb.Append("SceneName: " + structDefn.SceneName + Environment.NewLine);

                HETreeNode newStructNode = new HETreeNode(structDefn.SanitisedName, HETreeNodeType.BlueprintStructureDefinition, 
                    structDefn.SanitisedName, sb.ToString());

                definitionViewRootNode.Nodes.Add(newStructNode);

                foreach (HEBlueprintStructureDefinitions.HEBlueprintStructureDefinitionDockingPort portDefn
                    in structDefn.DockingPorts.Reverse<HEBlueprintStructureDefinitions.HEBlueprintStructureDefinitionDockingPort>())
                {
                    sb.Clear();
                    sb.Append("PortName: " + portDefn.PortName + Environment.NewLine);
                    sb.Append("OrderID: " + portDefn.OrderID + Environment.NewLine);
                    sb.Append("PortID: " + portDefn.PortID + Environment.NewLine);


                    HETreeNode newPortNode = new HETreeNode(portDefn.PortName.ToString(), HETreeNodeType.BlueprintDockingPortDefinition, 
                        portDefn.PortName.ToString(), sb.ToString());

                    newStructNode.Nodes.Add(newPortNode);
                }
            }
        }

        public void DeserialiseToBlueprintStructureDefinitionsObject()
        {
            blueprintStructureDefinitionsObject = jData.ToObject<HEBlueprintStructureDefinitions>();
            //blueprintStructureDefinitionsObject.ReconnectChildParentStructure();
        }

        public void SerialiseFromBlueprintObject()
        {
            throw new NotImplementedException("Not implemented yet.");
        }
    }


}
