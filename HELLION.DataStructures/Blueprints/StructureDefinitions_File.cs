using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HELLION.DataStructures.StaticData;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.Blueprints.StationBlueprint;
using static HELLION.DataStructures.StaticData.DockingPortHelper;

namespace HELLION.DataStructures.Blueprints
{
    /// <summary>
    /// A derived class for handling the StructureDefinitions.json file, a special-case blueprint file.
    /// </summary>
    public class StructureDefinitions_File : StationBlueprint_File, IParent_Json_File
    {
        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="passedFileInfo">The FileInfo representing the file to be loaded.</param>
        public StructureDefinitions_File(object passedParent, FileInfo passedFileInfo) : base(null)
        {
            File = passedFileInfo ?? throw new NullReferenceException("passedFileInfo was null.");
            // RootNode = new Blueprint_TN(this, nodeName: File.Name, newNodeType: Base_TN_NodeType.DataFile, nodeToolTipText: File.FullName);
            if (File.Exists) LoadFile();
            else Debug.Print("Structure definitions initialised but file {0} doesn't exist.", passedFileInfo.FullName);
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

            BlueprintObject.AuxData = new BlueprintStructure_AuxData(null);

            // Loop through all the structures in the Structures.Json file
            foreach (JToken jtStructure in structuresJsonFile.JData)
            {
                // Create a new Structure definition
                BlueprintStructure nsd = new BlueprintStructure
                {
                    SceneID = (StructureSceneID)Enum
                        .Parse(typeof(StructureSceneID), (string)jtStructure["ItemID"]),

                    AuxData = new BlueprintStructure_AuxData(null),

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
                    BlueprintDockingPort newDockingPortDefinition = new BlueprintDockingPort
                    {
                        // OrderID is critical as this is what the game uses as the key to match
                        // the ports in-game.
                        OrderID = (int)jtDockingPort["OrderID"],

                        // Look up the correct port name for this structure and orderID
                        PortName = GetDockingPortType((StructureSceneID)nsd.SceneID,
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
