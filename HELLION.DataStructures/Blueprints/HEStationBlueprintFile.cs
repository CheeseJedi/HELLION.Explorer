using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.HEStationBlueprint;
using static HELLION.DataStructures.StaticDataHelper;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines a class to load HSBF blueprint files.
    /// </summary>
    public class HEStationBlueprintFile : HEBaseJsonFile
    {
        #region Constructors

        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="passedFileInfo">The FileInfo representing the file to be loaded.</param>
        public HEStationBlueprintFile(HEBlueprintCollection passedParent, FileInfo passedFileInfo, int populateNodeTreeDepth)
            : this(passedParent)
        {
            File = passedFileInfo ?? throw new NullReferenceException();
            RootNode = new HEBlueprintTreeNode(passedOwner: this, nodeName: File.Name,
                newNodeType: HETreeNodeType.Blueprint, nodeToolTipText: File.FullName);


            if (File.Exists) LoadFile(populateNodeTreeDepth);
        }

        public HEStationBlueprintFile(HEBlueprintCollection passedParent) : base(passedParent)
        {
            // Re-assign the OwnerObject (the base class stores this as an object,
            // we ideally need it in its native type to work with its methods.
            OwnerObject = passedParent ?? throw new NullReferenceException();

            RootNode = new HEBlueprintTreeNode(passedOwner: this, nodeName: "Unsaved", 
                newNodeType: HETreeNodeType.Blueprint, nodeToolTipText: "File not yet saved");

            DataViewRootNode = new HEGameDataTreeNode(ownerObject: this, nodeName: "Data View",
                newNodeType: HETreeNodeType.BlueprintDataView, nodeToolTipText: 
                "Shows a representation of the Json data that makes up this blueprint.");

            RootNode.Nodes.Add(DataViewRootNode);

        }

        public HEStationBlueprintFile(object passedParent, FileInfo passedFileInfo, HEBaseJsonFile structuresJsonFile) : base(passedParent)
        {
            File = passedFileInfo ?? throw new NullReferenceException("passedFileInfo was null.");
            // Check the reference to the Static Data's Structures.json file.
            if (structuresJsonFile == null) throw new NullReferenceException("structuresJsonFile was null.");

            BlueprintObject = new HEStationBlueprint();
            GenerateAndSaveNewStructureDefinitionsFile(passedFileInfo, structuresJsonFile);

        }



        #endregion

        public void LoadFile(int populateNodeTreeDepth)
        {
            if (!File.Exists) throw new FileNotFoundException();
            base.LoadFile();
            PostLoadOperations(populateNodeTreeDepth);
        }

        public void PostLoadOperations(int populateNodeTreeDepth = 8)
        { 
            // Populate the blueprint object.
            DeserialiseToBlueprintObject();

            
            if (BlueprintObject.__ObjectType != null && BlueprintObject.__ObjectType == BlueprintObjectType.StationBlueprint)
            {
                // Assemble the primary tree hierarchy based on the DockingRoot.
                BlueprintObject.ReassembleTreeNodeDockingStructure
                    (BlueprintObject.PrimaryStructureRoot, AttachToBlueprintTreeNode: true);
                RootNode.Nodes.Add(BlueprintObject.RootNode);





                // Populate the data view.
                DataViewRootNode.JData = jData;
                DataViewRootNode.CreateChildNodesFromjData(populateNodeTreeDepth);
                ///RootNode.Nodes.Add(DataViewRootNode);

                // Populate the hierarchy view.
                //BuildHierarchyView();




            }


        }

        /// <summary>
        /// Applies new JData and triggers the PostLoadOperations method.
        /// </summary>
        /// <param name="newData"></param>
        public void ApplyNewJData(JToken newData)
        {
            jData = newData;


            // Clean up blueprint objects and tree nodes

            if (RootNode != null) RootNode.Nodes.Remove(BlueprintObject.RootNode);

            // Clean up DataView Tree Nodes.
            if (DataViewRootNode != null) DataViewRootNode.Nodes.Clear();

            PostLoadOperations();
        }


        /// <summary>
        /// Stores a reference to the parent object, if set using the constructor.
        /// </summary>
        public new HEBlueprintCollection OwnerObject{ get; protected set; }

        /// <summary>
        /// This class overrides the type of root node to represent a blueprint.
        /// </summary>
        public new HEBlueprintTreeNode RootNode { get; protected set; } = null;

        /// <summary>
        /// A reference to the DataView's root node.
        /// </summary>
        public HEGameDataTreeNode DataViewRootNode { get; protected set; } = null;

        /// <summary>
        /// A reference to the hierarchy view root node.
        /// </summary>
        public HESolarSystemTreeNode HierarchyViewRootNode { get; protected set; } = null;

        /// <summary>
        /// This is the actual blueprint - serialised and de-serialised from here.
        /// </summary>
        public HEStationBlueprint BlueprintObject { get; protected set; } = null;

        /// <summary>
        /// De-serialises the JData to the blueprint object.
        /// </summary>
        public void DeserialiseToBlueprintObject()
        {
            BlueprintObject = jData.ToObject<HEStationBlueprint>();
            BlueprintObject.OwnerObject = this;
            //BlueprintObject.StructureDefinitions = ;
            BlueprintObject.PostDeserialisationInit();
        }

        /// <summary>
        /// Serialises the JData to from blueprint object.
        /// </summary>
        public void SerialiseFromBlueprintObject()
        {
            JToken newData = JToken.FromObject(BlueprintObject); // .GetSerialisationTemplate()
            //Validity check?

            ApplyNewJData(newData);

        }

        /// <summary>
        /// Generates a new StructureDefinitions.json file.
        /// </summary>
        /// <param name="passedFileInfo"></param>
        /// <param name="structuresJsonFile"></param>
        public void GenerateAndSaveNewStructureDefinitionsFile(FileInfo passedFileInfo, HEBaseJsonFile structuresJsonFile)
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
                HEBlueprintStructure nsd = new HEBlueprintStructure               
                {
                    SceneID = (HEStructureSceneID)Enum
                        .Parse(typeof(HEStructureSceneID), (string)jtStructure["ItemID"]),

                    AuxData = new HEBlueprintStructureAuxData(null),

                                    // Calculate the total (nominal) air volume.
                    NominalAirVolume = (float)jtStructure["Rooms"].Sum(v => (float)v.SelectToken("Volume")),

                    // Look up the Power requirement for this module.
                    // Select subsystem type 13 (usually with RoomID of -1)
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

    }
}
