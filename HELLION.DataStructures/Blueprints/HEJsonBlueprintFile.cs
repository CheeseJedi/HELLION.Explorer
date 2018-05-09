using System;
using System.IO;
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
            : base(passedParent)
        {
            // Re-assign the OwnerObject (the base class stores this as an object,
            // we ideally need it in its native type to work with its methods.
            OwnerObject = passedParent ?? throw new NullReferenceException();

            File = passedFileInfo ?? throw new NullReferenceException();

            RootNode = new HEBlueprintTreeNode(passedOwner: this, nodeName: File.Name, newNodeType: HETreeNodeType.Blueprint, nodeToolTipText: File.FullName);

            DataViewRootNode = new HEGameDataTreeNode(ownerObject: this, nodeName: "Data View",
                newNodeType: HETreeNodeType.BlueprintDataView, nodeToolTipText: "Shows a representation of the Json data that makes up this blueprint.");

            //hierarchyViewRootNode = new HESolarSystemTreeNode("Hierarchy View", HETreeNodeType.BlueprintHierarchyView, 
            //nodeToolTipText: "Shows a tree-based view of the modules and their docking hierarchy.", passedOwner: this);

            RootNode.Nodes.Add(DataViewRootNode);

            //rootNode.Nodes.Add(hierarchyViewRootNode);
            LoadFile(populateNodeTreeDepth);
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
            BlueprintObject.ReassembleTreeNodeDockingStructure(BlueprintObject.PrimaryStructureRoot, AttachToBlueprintTreeNode: true);
            RootNode.Nodes.Add(BlueprintObject.RootNode);

            // Populate the data view.
            DataViewRootNode.JData = jData;
            DataViewRootNode.CreateChildNodesFromjData(populateNodeTreeDepth);
            // Populate the hierarchy view.
            //BuildHierarchyView();
        }

        /// <summary>
        /// Handles 
        /// </summary>
        /// <param name="newData"></param>
        public void ApplyNewJData(JToken newData)
        {
            
            
            // Clean up blueprint objects and tree nodes



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
        public HEBlueprint BlueprintObject { get; protected set; } = null;

        /// <summary>
        /// De-serialises the JData to the blueprint object.
        /// </summary>
        public void DeserialiseToBlueprintObject()
        {
            BlueprintObject = jData.ToObject<HEBlueprint>();
            BlueprintObject.OwnerObject = this;
            //BlueprintObject.StructureDefinitions = ;
            BlueprintObject.PostDeserialisationInit();
        }

        /// <summary>
        /// Not yet implemented.
        /// </summary>
        public void SerialiseFromBlueprintObject()
        {
            JToken newData = JToken.FromObject(BlueprintObject.GetSerialisationTemplate());
            //Validity check?

            jData = newData;

            SaveFile(CreateBackup: true);

            // throw new NotImplementedException("Not implemented yet.");
        }
    }
}
