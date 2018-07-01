using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using HELLION.DataStructures.UI;
using Newtonsoft.Json.Linq;

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
                BlueprintObject = JData.ToObject<StationBlueprint>();
                BlueprintObject.OwnerObject = this;
                BlueprintObject.PostDeserialisationInit();
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
