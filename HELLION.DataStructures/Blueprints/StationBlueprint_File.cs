using System;
using System.Diagnostics;
using System.IO;
using HELLION.DataStructures.Document;
using HELLION.DataStructures.UI;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.Blueprints.StationBlueprint;

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
            // Re-assign the OwnerObject (the base class stores this as an object,
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

            if (File.Exists) LoadFile(populateNodeTreeDepth);
            else Debug.Print("File {0} doesn't exist", File.FullName);
        }

        /*
        /// <summary>
        /// Constructor used to generate a StructuresDefinition.json file.
        /// </summary>
        /// <param name="passedParent"></param>
        /// <param name="passedFileInfo"></param>
        /// <param name="structuresJsonFile"></param>
        public StationBlueprint_File(IParent_Json_File passedParent, FileInfo passedFileInfo, Json_File_UI structuresJsonFile, bool generateSDfile = false) : base(passedParent)
        {
            File = passedFileInfo; // ?? throw new NullReferenceException("passedFileInfo was null.");
            // Check the reference to the Static Data's Structures.json file.
            if (structuresJsonFile == null) throw new NullReferenceException("structuresJsonFile was null.");

            BlueprintObject = new StationBlueprint();
            GenerateAndSaveNewStructureDefinitionsFile(passedFileInfo, structuresJsonFile);

        }
        */

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
        public void LoadFile(int populateNodeTreeDepth)
        {
            if (!File.Exists) throw new FileNotFoundException();
            base.LoadFile();
            PostLoadOperations(populateNodeTreeDepth);
        }

        /// <summary>
        /// Called after a file is loaded.
        /// </summary>
        /// <param name="populateNodeTreeDepth"></param>
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
                DataViewRootNode.JData = _jData;
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
