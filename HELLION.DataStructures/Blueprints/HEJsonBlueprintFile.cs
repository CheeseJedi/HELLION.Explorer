using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines a class to load HSBF blueprint files.
    /// </summary>
    public class HEJsonBlueprintFile : HEBaseJsonFile
    {
        
        
        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="passedFileInfo">The FileInfo representing the file to be loaded.</param>
        public HEJsonBlueprintFile(HEBlueprintCollection passedParent, FileInfo passedFileInfo, int populateNodeTreeDepth)
            : this(passedParent)
        {
            File = passedFileInfo ?? throw new NullReferenceException();
            RootNode = new HEBlueprintTreeNode(passedOwner: this, nodeName: File.Name,
                newNodeType: HETreeNodeType.Blueprint, nodeToolTipText: File.FullName);


            if (File.Exists) LoadFile(populateNodeTreeDepth);
        }

        public HEJsonBlueprintFile(HEBlueprintCollection passedParent) : base(passedParent)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newData"></param>
        public void ApplyNewJData(JToken newData)
        {
            jData = newData;


            // Clean up blueprint objects and tree nodes

            RootNode.Nodes.Remove(BlueprintObject.RootNode);

            // Clean up DataView Tree Nodes.
            DataViewRootNode.Nodes.Clear();

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







    }
}
