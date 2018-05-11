using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.HEBlueprintStructureDefinitions;

namespace HELLION.DataStructures
{
    /// <summary>
    /// 
    /// </summary>
    public class HEBlueprintStructureDefinitionsFile : HEBaseJsonFile
    {
        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="passedFileInfo">The FileInfo representing the file to be loaded.</param>
        public HEBlueprintStructureDefinitionsFile(object passedParent, FileInfo passedFileInfo, int populateNodeTreeDepth) : this(passedParent)
        {
            File = passedFileInfo ?? throw new NullReferenceException("passedFileInfo was null.");
            RootNode = new HEBlueprintTreeNode(this, nodeName: File.Name, newNodeType: HETreeNodeType.DataFile, nodeToolTipText: File.FullName);
            if (File.Exists) LoadFile();
        }

        public HEBlueprintStructureDefinitionsFile(object passedParent) : base(passedParent)
        {
            OwnerObject = passedParent ?? throw new NullReferenceException("passedParent was null.");

            BlueprintStructureDefinitionsObject = new HEBlueprintStructureDefinitions();

            RootNode = new HEBlueprintTreeNode(this, nodeName: "Unsaved", newNodeType: HETreeNodeType.DataFile, nodeToolTipText: "File not yet saved");

            DataViewRootNode = new HEGameDataTreeNode(ownerObject: this, nodeName: "Data View",
                newNodeType: HETreeNodeType.DataView, nodeToolTipText: "Shows a representation of the Json data that makes up this blueprint.");

            DefinitionViewRootNode = new HESolarSystemTreeNode(passedOwner: this, nodeName: "Definition View",
                nodeType: HETreeNodeType.BlueprintStructureDefinitionView, nodeToolTipText: "Shows a representation of each structure definition and its docking ports.");

            RootNode.Nodes.Add(DataViewRootNode);
            RootNode.Nodes.Add(DefinitionViewRootNode);
        }


        public HEBlueprintStructureDefinitionsFile(object passedParent, FileInfo passedFileInfo, HEBaseJsonFile structuresJsonFile) : base(passedParent)
        {
            File = passedFileInfo ?? throw new NullReferenceException("passedFileInfo was null.");
            // Check the reference to the Static Data's Structures.json file.
            if (structuresJsonFile == null) throw new NullReferenceException("structuresJsonFile was null.");

            BlueprintStructureDefinitionsObject = new HEBlueprintStructureDefinitions();
            GenerateAndSaveNewStructureDefinitionsFile(passedFileInfo, structuresJsonFile);

        }


        public void GenerateAndSaveNewStructureDefinitionsFile(FileInfo passedFileInfo, HEBaseJsonFile structuresJsonFile)
        {

            //HEBlueprintStructureDefinitionsFile newSDFile = new HEBlueprintStructureDefinitionsFile(null);

            BlueprintStructureDefinitionsObject.__ObjectType = "BlueprintStructureDefinitions";
            BlueprintStructureDefinitionsObject.Version = 0.35m;

            // Loop through all the structures in the Structures.Json file
            foreach (JToken jtStructure in structuresJsonFile.JData)
            {
                // Define a new StructureDefinition
                HEBlueprintStructureDefinition newStructureDefinition = new HEBlueprintStructureDefinition
                {
                    SceneName = (string)jtStructure["SceneName"],
                    DisplayName = (string)jtStructure["SceneName"], // might benefit from renaming to DisplayName
                    ItemID = (int)jtStructure["ItemID"], // possibly defunct.
                };

                // Loop through the jtStructure's DockingPort collection.
                foreach (JToken jtDockingPort in jtStructure["DockingPorts"])
                {
                    HEBlueprintStructureDefinitionDockingPort newDockingPortDefinition = new HEBlueprintStructureDefinitionDockingPort
                    {
                        PortName = HEDockingPortType.Unspecified,
                        OrderID = (int)jtDockingPort["OrderID"],
                        // PortID is irrelevant.
                    };

                    newStructureDefinition.DockingPorts.Add(newDockingPortDefinition);
                }

                BlueprintStructureDefinitionsObject.StructureDefinitions.Add(newStructureDefinition);

            }

            SerialiseFromBlueprintStructureDefinitionsObject();



            SaveFile(CreateBackup: true);


        }


        public new void LoadFile()
        {
            if (!File.Exists) throw new FileNotFoundException();
            base.LoadFile();
            PostLoadOperations();
        }

        public void PostLoadOperations(int populateNodeTreeDepth = 8)
        {
            // Populate the BlueprintStructureDefinitions object.
            DeserialiseToBlueprintStructureDefinitionsObject();
            
            if (DataViewRootNode != null)
            {
                // Populate the data view.
                DataViewRootNode.JData = jData;
                DataViewRootNode.CreateChildNodesFromjData(populateNodeTreeDepth);

                // Populate the hierarchy view.
                BuildHierarchyView();
            }

        }

        /// <summary>
        /// Handles 
        /// </summary>
        /// <param name="newData"></param>
        public void ApplyNewJData(JToken newData)
        {
            jData = newData;
            //IsDirty = true;

            // Clean up blueprint objects and tree nodes

            //RootNode.Nodes.Remove(BlueprintStructureDefinitionsObject.RootNode);

            // Clean up DataView Tree Nodes.
            if (DataViewRootNode != null) DataViewRootNode.Nodes.Clear();

            PostLoadOperations();
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
                sb.Append("StructureType: " + structDefn.DisplayName + Environment.NewLine);
                sb.Append("ItemID: " + structDefn.ItemID + Environment.NewLine);
                sb.Append("SceneName: " + structDefn.SceneName + Environment.NewLine);

                HETreeNode newStructNode = new HETreeNode(this, nodeName: structDefn.DisplayName, newNodeType: HETreeNodeType.BlueprintStructureDefinition,
                    nodeText: structDefn.DisplayName, nodeToolTipText: sb.ToString());

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

        public void SerialiseFromBlueprintStructureDefinitionsObject()
        {
            JToken newData = JToken.FromObject(BlueprintStructureDefinitionsObject);
            //Validity check?

            ApplyNewJData(newData);

        }


    }
}
