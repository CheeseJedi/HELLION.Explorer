using System;
using System.IO;
using System.Linq;
using System.Text;

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
        public HEBlueprintStructureDefinitionsFile(object passedParent, FileInfo passedFileInfo, int populateNodeTreeDepth) : base(passedParent)
        {
            // Blueprint files

            if (passedFileInfo == null) throw new NullReferenceException();
            else
            {
                File = passedFileInfo;

                RootNode = new HEBlueprintTreeNode(this, nodeName: File.Name, newNodeType: HETreeNodeType.DataFile, nodeToolTipText: File.FullName);

                DataViewRootNode = new HEGameDataTreeNode(ownerObject: this, nodeName: "Data View",
                    newNodeType: HETreeNodeType.DataView, nodeToolTipText: "Shows a representation of the Json data that makes up this blueprint.");

                DefinitionViewRootNode = new HESolarSystemTreeNode(passedOwner: this, nodeName: "Definition View",
                    nodeType: HETreeNodeType.BlueprintStructureDefinitionView, nodeToolTipText: "Shows a representation of each structure definition and its docking ports.");

                RootNode.Nodes.Add(DataViewRootNode);
                RootNode.Nodes.Add(DefinitionViewRootNode);

                if (!File.Exists) throw new FileNotFoundException();
                LoadFile();
                // Populate the BlueprintStructureDefinitions object.
                DeserialiseToBlueprintStructureDefinitionsObject();
                // Populate the data view.
                DataViewRootNode.JData = jData;
                DataViewRootNode.CreateChildNodesFromjData(populateNodeTreeDepth);
                // Populate the hierarchy view.
                BuildHierarchyView();
            }
        }

        /// <summary>
        /// This class overrides the type of root node to represent a blueprint.
        /// </summary>
        public new HETreeNode RootNode { get; protected set; } = null;

        public HEGameDataTreeNode DataViewRootNode { get; protected set; } = null;
        public HESolarSystemTreeNode DefinitionViewRootNode { get; protected set; } = null;

        /// <summary>
        /// This is the actual BlueprintStructureDefinition object - serialised and de-serialised from here.
        /// </summary>
        public HEBlueprintStructureDefinitions BlueprintStructureDefinitionsObject { get; protected set; } = null;
        
        /// <summary>
        /// Builds tree nodes from the GameData nodes, with cross-references
        /// </summary>
        public void BuildHierarchyView()
        {
            foreach (HEBlueprintStructureDefinitions.HEBlueprintStructureDefinition structDefn 
                in BlueprintStructureDefinitionsObject.StructureDefinitions
                .Reverse<HEBlueprintStructureDefinitions.HEBlueprintStructureDefinition>())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("StructureType: " + structDefn.SanitisedName + Environment.NewLine);
                sb.Append("ItemID: " + structDefn.ItemID + Environment.NewLine);
                sb.Append("SceneName: " + structDefn.SceneName + Environment.NewLine);

                HETreeNode newStructNode = new HETreeNode(this, nodeName: structDefn.SanitisedName, newNodeType: HETreeNodeType.BlueprintStructureDefinition,
                    nodeText: structDefn.SanitisedName, nodeToolTipText: sb.ToString());

                DefinitionViewRootNode.Nodes.Add(newStructNode);

                foreach (HEBlueprintStructureDefinitions.HEBlueprintStructureDefinitionDockingPort portDefn
                    in structDefn.DockingPorts.Reverse<HEBlueprintStructureDefinitions.HEBlueprintStructureDefinitionDockingPort>())
                {
                    sb.Clear();
                    sb.Append("PortName: " + portDefn.PortName + Environment.NewLine);
                    sb.Append("OrderID: " + portDefn.OrderID + Environment.NewLine);
                    sb.Append("PortID: " + portDefn.PortID + Environment.NewLine);


                    HETreeNode newPortNode = new HETreeNode(this, nodeName: portDefn.PortName.ToString(), newNodeType: HETreeNodeType.BlueprintDockingPortDefinition,
                        nodeText: portDefn.PortName.ToString(), nodeToolTipText: sb.ToString());

                    newStructNode.Nodes.Add(newPortNode);
                }
            }
        }

        public void DeserialiseToBlueprintStructureDefinitionsObject()
        {
            BlueprintStructureDefinitionsObject = jData.ToObject<HEBlueprintStructureDefinitions>();
            //blueprintStructureDefinitionsObject.ReconnectChildParentStructure();
        }

        public void SerialiseFromBlueprintObject()
        {
            throw new NotImplementedException("Not implemented yet.");
        }
    }


}
