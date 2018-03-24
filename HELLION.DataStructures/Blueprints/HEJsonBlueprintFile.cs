using System;
using System.IO;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines a class to load HSBF blueprint files.
    /// </summary>
    public class HEJsonBlueprintFile : HEJsonBaseFile
    {
        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="passedFileInfo">The FileInfo representing the file to be loaded.</param>
        public HEJsonBlueprintFile(HEBlueprintCollection passedParent, FileInfo passedFileInfo, int populateNodeTreeDepth)
            : base(passedParent)
        {
            File = passedFileInfo ?? throw new NullReferenceException();

            RootNode = new HEBlueprintTreeNode(passedOwner: this, nodeName: File.Name, newNodeType: HETreeNodeType.Blueprint, nodeToolTipText: File.FullName);

            DataViewRootNode = new HEGameDataTreeNode(ownerObject: this, nodeName: "Data View",
                newNodeType: HETreeNodeType.DataView, nodeToolTipText: "Shows a representation of the Json data that makes up this blueprint.");

            //hierarchyViewRootNode = new HESolarSystemTreeNode("Hierarchy View", HETreeNodeType.BlueprintHierarchyView, 
            //nodeToolTipText: "Shows a tree-based view of the modules and their docking hierarchy.", passedOwner: this);

            RootNode.Nodes.Add(DataViewRootNode);

            //rootNode.Nodes.Add(hierarchyViewRootNode);


            if (!File.Exists) throw new FileNotFoundException();
            LoadFile();
            // Populate the blueprint object.
            DeserialiseToBlueprintObject();

            BlueprintObject.ReassembleDockingStructure();
            RootNode.Nodes.Add(BlueprintObject.RootNode);

            // Populate the data view.
            //DataViewRootNode.JData = jData;
            DataViewRootNode.CreateChildNodesFromjData(populateNodeTreeDepth);
            // Populate the hierarchy view.
            //BuildHierarchyView();

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
            BlueprintObject.ReconnectChildParentStructure();
        }

        /// <summary>
        /// Not yet implemented.
        /// </summary>
        public void SerialiseFromBlueprintObject()
        {
            throw new NotImplementedException("Not implemented yet.");
        }
    }
}
