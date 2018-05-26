using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.Blueprints.StationBlueprint;
using static HELLION.DataStructures.StaticDataHelper;

namespace HELLION.DataStructures.Blueprints
{
    /// <summary>
    /// 
    /// </summary>
    public class StructureDefinitions_File : StationBlueprint_File, Json_File_Parent
    {
        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="passedFileInfo">The FileInfo representing the file to be loaded.</param>
        public StructureDefinitions_File(object passedParent, FileInfo passedFileInfo) : base(null)
        {
            File = passedFileInfo ?? throw new NullReferenceException("passedFileInfo was null.");
            // RootNode = new Blueprint_TreeNode(this, nodeName: File.Name, newNodeType: HETreeNodeType.DataFile, nodeToolTipText: File.FullName);
            if (File.Exists) LoadFile();
        }

        /// <summary>
        /// Constructor used to build a new StructureDefinitions.json file.
        /// </summary>
        /// <param name="passedParent"></param>
        /// <param name="outputFileInfo"></param>
        /// <param name="structuresJsonFile"></param>
        public StructureDefinitions_File(FileInfo outputFileInfo, Json_File_UI structuresJsonFile) : base(null)
        {
            File = outputFileInfo ?? throw new NullReferenceException("passedFileInfo was null.");
            // Check the reference to the Static Data's Structures.json file.
            if (structuresJsonFile == null) throw new NullReferenceException("structuresJsonFile was null.");

            // BlueprintStructureDefinitionsObject = new HEBlueprintStructureDefinitions();
            GenerateAndSaveNewStructureDefinitionsFile(outputFileInfo, structuresJsonFile);

        }

        /// <summary>
        /// Generates a new StructureDefinitions.json file.
        /// </summary>
        /// <param name="passedFileInfo"></param>
        /// <param name="structuresJsonFile"></param>
        public void GenerateAndSaveNewStructureDefinitionsFile(FileInfo passedFileInfo, Json_File_UI structuresJsonFile)
        {
            BlueprintObject.__ObjectType = BlueprintObjectType.BlueprintStructureDefinitions;
            BlueprintObject.Version = StationBlueprintFormatVersion;
            BlueprintObject.Name = String.Format("Hellion Station Blueprint Format - Structure Definitions Template Version {0} Generated {1}",
                StationBlueprintFormatVersion, DateTime.Now);
            BlueprintObject.LinkURI = new Uri(@"https://github.com/CheeseJedi/Hellion-Station-Blueprint-Format");

            BlueprintObject.AuxData = new HEBlueprintStructureAuxData(null);

            // Loop through all the structures in the Structures.Json file
            foreach (JToken jtStructure in structuresJsonFile.JData)
            {
                // Create a new Structure definition
                BlueprintStructure nsd = new BlueprintStructure
                {
                    SceneID = (HEStructureSceneID)Enum
                        .Parse(typeof(HEStructureSceneID), (string)jtStructure["ItemID"]),

                    AuxData = new HEBlueprintStructureAuxData(null),

                    // Calculate the total (nominal) air volume.
                    NominalAirVolume = (float)jtStructure["Rooms"].Sum(v => (float)v.SelectToken("Volume")),

                    // Look up the Power requirement for this module.
                    // Select subsystem type 13 (VesselBasePowerConsumer) usually with RoomID of -1
                    StandbyPowerRequirement = (float)jtStructure.SelectToken(
                        "$.SubSystems.[?(@.Type == 13)].ResourceRequirements[0].Standby"),

                    // NominalPowerRequirement = (float)jtStructure.SelectToken(
                    //  "$.SubSystems.[?(@.Type == 13)].ResourceRequirements[0].Nominal"),

                    // Need to locate the info probably from the generators system.
                    // Not currently set.
                    //NominalPowerContribution = null;


                };

                // Loop through the jtStructure's DockingPort collection.
                foreach (JToken jtDockingPort in jtStructure["DockingPorts"])
                {
                    HEBlueprintDockingPort newDockingPortDefinition = new HEBlueprintDockingPort
                    {
                        // OrderID is critical as this is what the game uses as the key to match
                        // the ports in-game.
                        OrderID = (int)jtDockingPort["OrderID"],

                        // Look up the correct port name for this structure and orderID
                        PortName = GetDockingPortType((HEStructureSceneID)nsd.SceneID,
                            orderID: (int)jtDockingPort["OrderID"]),

                        // Default locked/unlocked status is preserved.
                        Locked = (bool)jtDockingPort["Locked"],
                    };

                    nsd.DockingPorts.Add(newDockingPortDefinition);
                }

                BlueprintObject.Structures.Add(nsd);

            }

            SerialiseFromBlueprintObject();

            SaveFile(CreateBackup: true);

        }

        /*

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

        public Json_TreeNode DataViewRootNode { get; protected set; } = null;
        public SolarSystem_TreeNode DefinitionViewRootNode { get; protected set; } = null;

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

        */
    }
}
