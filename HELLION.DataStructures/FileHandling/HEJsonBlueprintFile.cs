using System;
using System.IO;
using System.Linq;
using System.Text;

namespace HELLION.DataStructures
{
    /// <summary>
    /// 
    /// </summary>
    public class HEJsonBlueprintFile : HEJsonBaseFile
    {
        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="passedFileInfo">The FileInfo representing the file to be loaded.</param>
        public HEJsonBlueprintFile(HEBlueprintCollection passedParent, /* HEBlueprints passedBlueprints,*/ FileInfo passedFileInfo, int populateNodeTreeDepth)
        {
            // Blueprint files

            parentBlueprintCollection = passedParent ?? throw new NullReferenceException("passedParent is null.");

            if (passedFileInfo == null) throw new NullReferenceException();
            else
            {
                fileInfo = passedFileInfo;

                rootNode = new HEBlueprintTreeNode(File.Name, HETreeNodeType.Blueprint, nodeToolTipText: File.FullName, passedOwner: this);

                dataViewRootNode = new HEGameDataTreeNode("Data View", HETreeNodeType.DataView, 
                    nodeToolTipText: "Shows a representation of the Json data that makes up this blueprint.", passedOwner: this);

                //hierarchyViewRootNode = new HESolarSystemTreeNode("Hierarchy View", HETreeNodeType.BlueprintHierarchyView, 
                //nodeToolTipText: "Shows a tree-based view of the modules and their docking hierarchy.", passedOwner: this);

                rootNode.Nodes.Add(dataViewRootNode);

                //rootNode.Nodes.Add(hierarchyViewRootNode);


                if (!File.Exists) throw new FileNotFoundException();
                else
                {
                    LoadFile();
                    // Populate the blueprint object.
                    DeserialiseToBlueprintObject();

                    BlueprintObject.ReassembleDockingStructure();
                    rootNode.Nodes.Add(BlueprintObject.RootNode);


                    // Populate the data view.
                    dataViewRootNode.Tag = jData;
                    dataViewRootNode.CreateChildNodesFromjData(populateNodeTreeDepth);
                }
                // Populate the hierarchy view.
                //BuildHierarchyView();
            }
        }

        /// <summary>
        /// Public property to access the parent object, if set through the constructor.
        /// </summary>
        public HEBlueprintCollection ParentBlueprintCollection
        {
            get { return parentBlueprintCollection; }
            set { parentBlueprintCollection = value; }
        }

        /// <summary>
        /// Stores a reference to the parent object, if set using the constructor.
        /// </summary>
        protected HEBlueprintCollection parentBlueprintCollection = null;

        public new HEBlueprintTreeNode RootNode => rootNode;

        /// <summary>
        /// This class overrides the type of root node to represent a blueprint.
        /// </summary>
        protected new HEBlueprintTreeNode rootNode;

        public HEGameDataTreeNode DataViewRootNode => dataViewRootNode;

        protected HEGameDataTreeNode dataViewRootNode = null;

        public HESolarSystemTreeNode HierarchyViewRootNode => hierarchyViewRootNode;

        protected HESolarSystemTreeNode hierarchyViewRootNode = null;

        public HEBlueprint BlueprintObject => blueprintObject;

        /// <summary>
        /// This is the actual blueprint - serialised and de-serialised from here.
        /// </summary>
        protected HEBlueprint blueprintObject = null;

        /// <summary>
        /// Builds tree nodes from the GameData nodes, with cross-references
        /// </summary>
        public void BuildHierarchyView()
        {
            foreach (HEBlueprint.HEBlueprintStructure structure in blueprintObject.Structures
                .Reverse<HEBlueprint.HEBlueprintStructure>())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("StructureType: " + structure.StructureType + Environment.NewLine);
                sb.Append("StructureID: " + structure.StructureID + Environment.NewLine);
                sb.Append("DockedStructures: " + Environment.NewLine);
                foreach (HEBlueprint.HEBlueprintStructure dockedStructure in structure.DockedStructures())
                {
                    sb.Append(">" + dockedStructure.StructureType.ToString());
                    sb.Append(" id: " + dockedStructure.StructureID);

                    // Find the docking port on the docked module that is docked to this structure
                    HEBlueprint.HEBlueprintDockingPort remotePort = dockedStructure.DockingPorts
                        .Where(f => f.DockedStructureID == structure.StructureID).Single();
                    sb.Append(" PortsInUse: " + remotePort.DockedPortName + "::" + remotePort.PortName 
                        + ":" + dockedStructure.StructureType.ToString());
                        
                    sb.Append(Environment.NewLine);
                }

                HETreeNode newStructNode = new HETreeNode(structure.StructureType.ToString(), HETreeNodeType.BlueprintStructureDefinition,
                    structure.StructureType.ToString(), sb.ToString());

                hierarchyViewRootNode.Nodes.Add(newStructNode);

                foreach (HEBlueprint.HEBlueprintDockingPort port in structure.DockingPorts
                    .Reverse<HEBlueprint.HEBlueprintDockingPort>())
                {
                    sb.Clear();
                    sb.Append("PortName: " + port.PortName + Environment.NewLine);
                    sb.Append("OrderID: " + port.OrderID + Environment.NewLine);
                    sb.Append("DockedStructureID: " + port.DockedStructureID + Environment.NewLine);


                    HEBlueprintStructureTypes? dockedStructureType = HEBlueprintStructureTypes.UNKNOWN;
                    try
                    {
                        dockedStructureType = blueprintObject.GetStructureByID(port.DockedStructureID).StructureType;
                    }
                    catch { }

                    sb.Append("DockedStructureType: " + dockedStructureType + Environment.NewLine);
                    sb.Append("DockedPortName: " + port.DockedPortName + Environment.NewLine);


                    HETreeNode newPortNode = new HETreeNode(port.PortName.ToString(), HETreeNodeType.BlueprintDockingPortDefinition,
                        port.PortName.ToString(), sb.ToString());

                    newStructNode.Nodes.Add(newPortNode);
                }
            }
        }

        /*
        /// <summary>
        /// Adds Solar System nodes of the specified type to the RootNode, generated from the 
        /// Game Data nodes.
        /// </summary>
        public void AddSolarSystemObjects()
        {
            // Set up the find key
            string findKey = "Structures";

            Debug.Print("dataViewRootNode.CountOfChildNodes " + dataViewRootNode.CountOfChildNodes);

            TreeNode[] tmpMatches = dataViewRootNode.Nodes.Find(findKey, searchAllChildren: true);

            HEGameDataTreeNode sectionRootNode = null;
            HEGameDataTreeNode arrayRootNode = null;

            foreach (var match in tmpMatches)
            {
                sectionRootNode = (HEGameDataTreeNode)match;
                break;
            }
            if (sectionRootNode == null) throw new NullReferenceException("sectionRootNode was null.");

            foreach (var match2 in sectionRootNode.Nodes)
            {
                arrayRootNode = (HEGameDataTreeNode)match2;
                break;
            }
            if (arrayRootNode == null) throw new NullReferenceException("subRootNode was null.");

            foreach (HEGameDataTreeNode node in arrayRootNode.Nodes)
            {
                //HETreeNodeType newNodeType = HETreeNodeType.Asteroid;

                JObject obj = (JObject)node.Tag;
                long newNodeParentGUID = 0;
                JToken testToken = obj["ParentGUID"];
                if (testToken != null)
                {
                    newNodeParentGUID = (long)obj["ParentGUID"];
                }

                HESolarSystemTreeNode newNode = node.CreateLinkedSolarSystemNode(HETreeNodeType.Ship);
                hierarchyViewRootNode.Nodes.Add(newNode);
            }
        }

        /// <summary>
        /// Re-arranges (rehydrates) existing ship nodes by their DockedToShipGUID forming a tree where the
        /// root node is the parent vessel of the docked ships (and is what shows up on radar in-game).
        /// </summary>
        /// <remarks>
        /// Although this particular function is non-recursive, recursive calls are made when calling
        /// the HETreeNode.GetAllNodes() to get sub-nodes.
        /// </remarks>
        public void RehydrateDockedShips()
        {
            IEnumerable<HESolarSystemTreeNode> shipsToBeReparented = RootNode.ListOfAllChildNodes
                .Cast<HESolarSystemTreeNode>()
                .Where(p => (p.NodeType == HETreeNodeType.Ship) && (p.DockedToShipGUID > 0));

            foreach (HESolarSystemTreeNode node in shipsToBeReparented)
            {
                // If this node has a non-zero value for DockedToShipGUID, process it.
                if (node.DockedToShipGUID != 0)
                {
                    // Find the node that has the GUID matching the DockedToShipGUID of this node.
                    // There can be only one!
                    HESolarSystemTreeNode newParentNode = RootNode.ListOfAllChildNodes
                        .Cast<HESolarSystemTreeNode>()
                        .Where(p => p.GUID == node.DockedToShipGUID)
                        .Single();
                    // If the .Single() causes an exception, there's more than one module docked to that port!

                    // Remove the ship to be re-parented from it's current parent's node collection.
                    node.Parent.Nodes.Remove(node);
                    // Add the ship being re-parented to the new parent's node collection.
                    newParentNode.Nodes.Add(node);
                    // As the new parent's node collection has changed, clear it's cache to force regeneration.
                    newParentNode.ClearCachedData();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }
        */


        public void DeserialiseToBlueprintObject()
        {
            blueprintObject = jData.ToObject<HEBlueprint>();
            blueprintObject.ParentJsonBlueprintFile = this;
            blueprintObject.ReconnectChildParentStructure();
        }

        public void SerialiseFromBlueprintObject()
        {
            throw new NotImplementedException("Not implemented yet.");
        }
    }


}
