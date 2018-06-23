using System;
using System.Diagnostics;
using System.IO;
using HELLION.DataStructures.UI;
using Newtonsoft.Json.Linq;

namespace HELLION.DataStructures.Blueprints
{
    /// <summary>
    /// Defines a class to load HSBF blueprint files.
    /// </summary>
    public class StationBlueprint_File : Json_File_UI
    {
        #region Constructors

        /// <summary>
        /// Basic constructor - attempts to set the parent.
        /// </summary>
        /// <param name="passedParent"></param>
        public StationBlueprint_File(IParent_Json_File passedParent) : base(passedParent)
        {
            // Re-assign the OwnerStructure (the base class stores this as an object,
            // we ideally need it in its native type to work with its methods.
            OwnerObject = passedParent; // ?? throw new NullReferenceException();

            RootNode = new Blueprint_TN(passedOwner: this, newNodeType: Base_TN_NodeType.Blueprint,
                nodeName: "Unsaved"); //, nodeToolTipText: "File not yet saved");

            DataViewRootNode = new Json_TN(ownerObject: this, nodeName: "Data View");
                //newNodeType: Base_TN_NodeType.BlueprintDataView, nodeToolTipText:
                //"Shows a representation of the Json data that makes up this blueprint.");

            RootNode.Nodes.Add(DataViewRootNode);
        }

        /// <summary>
        /// General use constructor - takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="passedFileInfo">The FileInfo representing the file to be loaded.</param>
        public StationBlueprint_File(IParent_Json_File passedParent, FileInfo structDefsFileInfo, int populateNodeTreeDepth = 0) : this(passedParent)
        {
            File = structDefsFileInfo ?? throw new NullReferenceException();
            RootNode = new Blueprint_TN(passedOwner: this, newNodeType: Base_TN_NodeType.Blueprint,
                nodeName: File.Name); //, nodeToolTipText: File.FullName);

            if (File.Exists) LoadFile(); //  populateNodeTreeDepth);
            else Debug.Print("File {0} doesn't exist", File.FullName);
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
        public new Blueprint_TN RootNode { get; protected set; } = null;

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
        /// Loads the file.
        /// </summary>
        /// <param name="populateNodeTreeDepth"></param>
        public override bool LoadFile() //int populateNodeTreeDepth)
        {
            if (!File.Exists) throw new FileNotFoundException();
            bool result = base.LoadFile();
            PostLoadOperations(); // populateNodeTreeDepth);
            return result;
        }

        /// <summary>
        /// Called after a file is loaded.
        /// </summary>
        /// <param name="populateNodeTreeDepth"></param>
        public void PostLoadOperations() // int populateNodeTreeDepth = 8)
        {
            // Populate the blueprint object.
            DeserialiseToBlueprintObject();
                 
        }

        /// <summary>
        /// Applies new JData and triggers the PostLoadOperations method.
        /// </summary>
        /// <param name="newData"></param>
        public void ApplyNewJData(JToken newData)
        {
            // Apply the new data.
            _jData = newData;

            // Clean up blueprint objects and tree nodes

            if (RootNode != null) RootNode.Nodes.Remove(BlueprintObject.RootNode);

            // Clean up DataView Tree Nodes.
            if (DataViewRootNode != null) DataViewRootNode.Nodes.Clear();

            PostLoadOperations();
        }

        /// <summary>
        /// De-serialises the JData to the blueprint object.
        /// </summary>
        public void DeserialiseToBlueprintObject()
        {
            BlueprintObject = _jData.ToObject<StationBlueprint>();
            BlueprintObject.OwnerObject = this;
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

        #endregion
    }

}
