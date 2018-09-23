using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using HELLION.DataStructures.StaticData;
using HELLION.DataStructures.UI;
using HELLION.DataStructures.Utilities;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.Blueprints.StationBlueprint;

namespace HELLION.DataStructures.Blueprints
{
    /// <summary>
    /// Defines a class to load HSBF blueprint files.
    /// </summary>
    public class StationBlueprint_File : Json_File //_UI
    {
        #region Constructors

        /// <summary>
        /// General use constructor - takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="passedFileInfo">The FileInfo representing the file to be loaded.</param>
        public StationBlueprint_File(IParent_Json_File passedParent, FileInfo stationBlueprintFileInfo)
            : base(passedParent, stationBlueprintFileInfo, autoDeserialise: true)
        {
            Debug.Print("StationBlueprint_File.ctor(FileInfo) called {0}", stationBlueprintFileInfo.FullName);
            
            // Re-assign the OwnerStructure (the base class stores this as an object,
            // we ideally need it in its native type to work with its methods.
            OwnerObject = passedParent; // ?? throw new NullReferenceException();

            // Create a new Blueprint object - possibly not required when opening from file.
            if (BlueprintObject == null) BlueprintObject = new StationBlueprint(this, null);

        }

        /// <summary>
        /// A constructor for creating a new file in memory using a JToken.
        /// </summary>
        /// <param name="passedParent"></param>
        /// <param name="jdata"></param>
        public StationBlueprint_File(IParent_Json_File passedParent, JToken jdata)
            : base(passedParent, jdata, autoDeserialise: true) 
        {
            Debug.Print("StationBlueprint_File.ctor(JToken) called - HasValues [{0}]",
                jdata != null ? jdata.HasValues.ToString() : "null");

            // Re-assign the OwnerStructure (the base class stores this as an object,
            // we ideally need it in its native type to work with its methods.
            OwnerObject = passedParent; // ?? throw new NullReferenceException();

            // Create a new Blueprint object - possibly not required when creating from JData.
            if (BlueprintObject == null) BlueprintObject = new StationBlueprint(this, null);

        }

        #endregion

        #region Properties

        /// <summary>
        /// Stores a reference to the parent object, if set using the constructor.
        /// </summary>
        public new IParent_Json_File OwnerObject { get; protected set; }

        /// <summary>
        /// This class overrides the type of root node to represent a blueprint.
        /// </summary>
        public Blueprint_TN RootNode { get; protected set; } = null;

        /// <summary>
        /// A reference to the DataView's root node.
        /// </summary>
        public Json_TN DataViewRootNode { get; protected set; } = null;

        /// <summary>
        /// A reference to the hierarchy view root node.
        /// </summary>
        public SolarSystem_TN HierarchyViewRootNode { get; protected set; } = null;

        /// <summary>
        /// This is the actual blueprint - serialised and de-serialised from here.
        /// </summary>
        public StationBlueprint BlueprintObject { get; protected set; } = null;

        #endregion

        #region Methods

        /// <summary>
        /// Implements auto de-serialisation from the base class call.
        /// </summary>
        public override void Deserialise()
        {
            Debug.Print("StationBlueprint_File.Deserialise() called.");

            if (JData == null) Debug.Print("StationBlueprint_File.Deserialise() skipping - JData was null.");
            else
            {
                //BlueprintObject = JData.ToObject<StationBlueprint>();
                BuildStationBlueprintFromJData();

                //BlueprintObject.OwnerObject = this;
                BlueprintObject.PostDeserialisationInit();
            }
        }

        public void BuildStationBlueprintFromJData()
        {
            Debug.Print("------------------------------------------------------------");

            Debug.Print("BuildStationBlueprintFromJData: Starting.");

            // Check it's for the correct __ObjectType.
            JToken testToken = JData["__ObjectType"];
            if (testToken == null || (string)testToken != "StationBlueprint")
            {
                Debug.Print("BuildStationBlueprintFromJData: ObjectType not StationBlueprint");
                return;
            }

            // Create a new StationBlueprint.
            BlueprintObject = new StationBlueprint
            {
                OwnerObject = this,
                __ObjectType = BlueprintObjectType.StationBlueprint
            };


            // Set the blueprint name;
            testToken = JData["Name"];
            if (testToken != null) BlueprintObject.Name = (string)testToken;
            // Set the blueprint Link Uri;
            testToken = JData["LinkURI"];
            if (testToken != null) BlueprintObject.LinkURI = (Uri)testToken;

            // Set blueprint level properties.
            testToken = JData["Invulnerable"];
            if (testToken != null) BlueprintObject.Invulnerable = (bool)testToken;
            testToken = JData["SystemsOnline"];
            if (testToken != null) BlueprintObject.SystemsOnline = (bool)testToken;
            testToken = JData["DoorsLocked"];
            if (testToken != null) BlueprintObject.DoorsLocked = (bool)testToken;
            Debug.Print("------------------------------------------------------------");
            Debug.Print("Creating new blueprint structures (" + JData["Structures"].Count() + ")");
            foreach (JObject structure in JData["Structures"])
            {
                Debug.Print("------------------------------");
                StructureSceneID sceneID = StructureSceneID.Unspecified;

                int structureID = (int)structure["StructureID"];
                Debug.Print("structureID " + structureID);


                // Attempt to get a StructureType - this is a mix between the in-game three letter
                // names and the SceneNames.
                testToken = structure["StructureType"];
                if (testToken != null)
                {
                    string parsedStructureType = (string)structure["StructureType"];
                    Debug.Print("parsedStructureType " + parsedStructureType);
                    sceneID = parsedStructureType.ParseToEnumDescriptionOrEnumerator<StructureSceneID>();
                    Debug.Print("sceneID " + sceneID);
                }

                //// Attempt to get a SceneID - this is for newer format blueprints.
                //testToken = structure["SceneID"];
                //if (testToken != null)
                //{
                //    int parsedSceneID = (int)structure["SceneID"];
                //    sceneID = parsedSceneID.ParseToEnumDescriptionOrEnumerator<StructureSceneID>();
                //}

                // Call the BlueprintObject's AddStructure method with the sceneID and structureID.
                BlueprintStructure newStructure = BlueprintObject.AddStructure(sceneID, structureID);

                Debug.Print(" newStructure ports (" + newStructure.DockingPorts.Count + ")");
                foreach (BlueprintDockingPort tmpPort in newStructure.DockingPorts)
                {
                    Debug.Print("  PortName " + tmpPort.PortName);
                    Debug.Print("  OrderID " + tmpPort.OrderID);
                    Debug.Print("  Locked " + tmpPort.Locked);

                }

            }

            Debug.Print("------------------------------------------------------------");
            Debug.Print("Setting docking port data from loaded data.");
            foreach (JObject structure in JData["Structures"])
            {
                Debug.Print("------------------------------");
                int structureID = (int)structure["StructureID"];
                Debug.Print("structureID " + structureID);

                BlueprintStructure newStructure = BlueprintObject.GetStructure(structureID);
                if (newStructure == null) throw new Exception();


                foreach (JObject dockingPort in structure["DockingPorts"])
                {
                    Debug.Print("-------------------------");

                    Debug.Print(" DockingPort");
                    int orderID = (int)dockingPort["OrderID"];
                    Debug.Print(" orderID " + orderID);


                    int? dockedStructureID = (int?)dockingPort["DockedStructureID"];
                    Debug.Print(" dockedStructureID " + dockedStructureID);


                    bool locked = (bool)dockingPort["Locked"];
                    Debug.Print(" locked " + locked);

                    BlueprintDockingPort newPort = newStructure.GetDockingPortByOrderID(orderID);
                    if (newPort == null) throw new Exception();

                    // If we got here we should have the port in the new structure that corresponds
                    // to the port in this JObject.

                    newPort.DockedStructureID = dockedStructureID;

                    newPort.Locked = locked;






                }


            }









        }




        /// <summary>
        /// Implements auto serialisation from the base class call.
        /// </summary>
        public override void Serialise()
        {
            JToken newData = JToken.FromObject(BlueprintObject);

            // Make a note of the current setting.
            bool currentSetting = AutoDeserialiseOnJdataModification;

            // Prevent the serialisation and modification of the JData object
            // from triggering a de-serialisation.
            AutoDeserialiseOnJdataModification = false;

            // Apply the JData.
            JData = newData;

            // Restore the previous value.
            AutoDeserialiseOnJdataModification = currentSetting;
        }

        #endregion

    }

}
