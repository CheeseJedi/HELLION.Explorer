using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.HEStationBlueprint;

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
        /// 
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
        /// Not yet implemented.
        /// </summary>
        public void SerialiseFromBlueprintObject()
        {
            JToken newData = JToken.FromObject(BlueprintObject); // .GetSerialisationTemplate()
            //Validity check?

            ApplyNewJData(newData);

            //SaveFile(CreateBackup: true);

        }


        public void GenerateAndSaveNewStructureDefinitionsFile(FileInfo passedFileInfo, HEBaseJsonFile structuresJsonFile)
        {

            //HEBlueprintStructureDefinitionsFile newSDFile = new HEBlueprintStructureDefinitionsFile(null);

            BlueprintObject.__ObjectType = BlueprintObjectType.BlueprintStructureDefinitions;
            BlueprintObject.Version = StationBlueprintFormatVersion;
            BlueprintObject.Name = String.Format("Hellion Station Blueprint Format - Structure Definitions Template Version {0} Generated {1}",
                StationBlueprintFormatVersion, DateTime.Now);
            BlueprintObject.LinkURI = new Uri(@"https://github.com/CheeseJedi/Hellion-Station-Blueprint-Format");

            //BlueprintObject.AuxData = new HEBlueprintStructureAuxData(null);

            // Loop through all the structures in the Structures.Json file
            foreach (JToken jtStructure in structuresJsonFile.JData)
            {
                // Define a new Structure
                HEBlueprintStructure nsd = new HEBlueprintStructure               
                {
                    SceneID = (HEBlueprintStructureSceneID)Enum
                        .Parse(typeof(HEBlueprintStructureSceneID), (string)jtStructure["ItemID"]),

                    // SceneName = (string)jtStructure["SceneName"],
                    // DisplayName = (string)jtStructure["SceneName"], // might benefit from renaming to DisplayName
                    // ItemID = (int)jtStructure["ItemID"], // possibly defunct.

                };

                nsd.AuxData = new HEBlueprintStructureAuxData(null);

                // Calculate the total (nominal) air volume.
                nsd.NominalAirVolume = (float)jtStructure["Rooms"].Sum(v => (float)v.SelectToken("Volume"));

                // Look up the Power requirement for this module.
                // Select subsystem type 13 with RoomID of -1
                nsd.StandbyPowerRequirement = (float)jtStructure.SelectToken("$.SubSystems.[?(@.Type == 13)].ResourceRequirements[0].Standby");
                // nsd.NominalPowerRequirement = (float)jtStructure.SelectToken("$.SubSystems.[?(@.Type == 13)].ResourceRequirements[0].Nominal");

                // Need to locate the info probably from the generators system.
                // Not currently set.
                //nsd.NominalPowerContribution = null;







                // Loop through the jtStructure's DockingPort collection.
                foreach (JToken jtDockingPort in jtStructure["DockingPorts"])
                {
                    HEBlueprintDockingPort newDockingPortDefinition = new HEBlueprintDockingPort
                    {
                        // We don't know the port names in advance so this is set to unspecified.
                        // This could be fed from a lookup dictionary or other similar structure
                        // in future to reduce the manual work updating the StructureDefinitions.
                        PortName = HEDockingPortType.Unspecified,

                        // OrderID is critical as this is what the game uses as the key to match
                        // the ports in-game.
                        OrderID = (int)jtDockingPort["OrderID"],

                        // Default locked/unlocked status is preserved.
                        Locked = (bool)jtDockingPort["Locked"],
                        // PortID is irrelevant.
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
