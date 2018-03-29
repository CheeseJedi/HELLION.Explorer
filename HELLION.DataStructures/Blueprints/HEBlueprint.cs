using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HELLION.DataStructures
{
    #region Enums

    /// <summary>
    /// Structure Types Enum.
    /// </summary>
    /// <remarks>
    /// The numeric values of these correspond to the game's ItemID in the structures.json
    /// to allow for easier cross-referencing.
    /// </remarks>
    public enum HEBlueprintStructureTypes
    {
        Unspecified = 0,
        CIM = 3,
        CTM = 4,
        CLM = 5,
        ARG = 6,
        PSM = 7,
        LSM = 8,
        CBM = 9,
        CSM = 10,
        CM = 11,
        CRM = 12,
        OUTPOST = 13,
        AM = 14,
        IC = 19,
        CQM = 22,
        SPM = 26,
        FM = 28,
    }

    /// <summary>
    /// Docking Port Types Enum.
    /// </summary>
    public enum HEDockingPortTypes
    {
        Unspecified = 0,
        StandardDockingPortA,
        StandardDockingPortB,
        StandardDockingPortC,
        StandardDockingPortD,
        AirlockDockingPort,
        Grapple,
        IndustrialContainerPortA,
        IndustrialContainerPortB,
        IndustrialContainerPortC,
        IndustrialContainerPortD,
        CargoDockingPortA,
        CargoDockingPortB,
        CargoDock  // Dockable Cargo (IC) module
    }

    #endregion

    /// <summary>
    /// A class to handle blueprint data structures.
    /// </summary>
    public class HEBlueprint
    {
        #region Constructors

        /// <summary>
        /// Default Constructor, called directly by the JToken.ToObject<T>()
        /// </summary>
        public HEBlueprint()
        {
            Structures = new List<HEBlueprintStructure>();
            RootNode = new HEBlueprintTreeNode(passedOwner: this, nodeName: "Hierarchy View",
                newNodeType: HETreeNodeType.BlueprintHierarchyView, nodeToolTipText: "Shows a tree-based view of the modules and their docking hierarchy.");
        }

        /// <summary>
        /// Constructor that takes a HEJsonBlueprintFile Owner Object and reference to the 
        /// structure definitions object.
        /// </summary>
        /// <param name="ownerObject"></param>
        /// <param name="structureDefs"></param>
        public HEBlueprint(HEJsonBlueprintFile ownerObject, HEBlueprintStructureDefinitions structureDefs) : this()
        {
            OwnerObject = ownerObject ?? throw new NullReferenceException("passedParent was null.");
            StructureDefinitions = structureDefs ?? throw new NullReferenceException("structureDefs was null.");
            __ObjectType = "StationBlueprint";
            Version = (decimal)0.3;

        }

        #endregion

        #region Properties

        /// <summary>
        /// Parent object - not to be included in serialisation.
        /// </summary>
        public HEJsonBlueprintFile OwnerObject { get; set; } = null;

        /// <summary>
        /// Not to be serialised.
        /// </summary>
        public HEBlueprintTreeNode RootNode { get; set; } = null;

        /// <summary>
        /// Type of object (should be "StationBlueprint")
        /// </summary>
        /// <remarks>
        /// To be serialised.
        /// </remarks>
        public string __ObjectType { get; set; } = null;

        /// <summary>
        /// To be serialised.
        /// </summary>
        public decimal? Version { get; set; } = null;

        /// <summary>
        /// To be serialised.
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// To be serialised.
        /// </summary>
        public Uri LinkURI { get; set; } = null;

        /// <summary>
        /// To be serialised.
        /// </summary>
        public List<HEBlueprintStructure> Structures { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public HEBlueprintStructureDefinitions StructureDefinitions { get; set; } = null;

        #endregion

        #region Methods

        /// <summary>
        /// Links up all child object's parent reference to their parent.
        /// </summary>
        public void ReconnectChildParentStructure()
        {
            if (OwnerObject == null) throw new NullReferenceException("ParentJsonBlueprintFile was null.");

            // the following causes an exception.
            StructureDefinitions = OwnerObject.OwnerObject.OwnerObject
                .StructureDefinitionsFile.BlueprintStructureDefinitionsObject;

            foreach (HEBlueprintStructure structure in Structures)
            {
                // Set the structure's parent object (this, the blueprint object)
                structure.OwnerObject = this;
                foreach (HEBlueprintDockingPort port in structure.DockingPorts) port.OwnerObject = structure;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReassembleDockingStructure()
        {
            // Start with the root node, should be item zero in the list.
            HEBlueprintStructure dockingRoot = GetDockingRoot();

            // Debug.Print("dockingRoot {0} StructureID {1}", dockingRoot.StructureType, dockingRoot.StructureID);

            // Process the docking root's ports slightly differently - always child nodes.
            RootNode.Nodes.Add(dockingRoot.RootNode);
            foreach (HEBlueprintDockingPort port in dockingRoot.DockingPorts.ToArray().Reverse())
            {
                dockingRoot.RootNode.Nodes.Add(port.RootNode);
                if (port.IsDocked())
                {
                    Reassemble(GetStructureByID(port.DockedStructureID), dockingRoot);
                }
            }

            void Reassemble(HEBlueprintStructure structure, HEBlueprintStructure parent)
            {
                //Debug.Print("structure = " + structure.ToString());
                //Debug.Print("parent = " + parent.ToString());

                // Figure out which port is docking us to the parent and vice versa
                HEBlueprintDockingPort linkToParent = structure.GetDockingPortByDockedStructureID((int)parent.StructureID);
                HEBlueprintDockingPort linkFromParent = parent.GetDockingPortByDockedStructureID((int)structure.StructureID);

                // Add the node for the link to parent to the link from parent's node collection.
                linkFromParent.RootNode.Nodes.Add(linkToParent.RootNode);

                // Add the structures's node to the link to parent node collection.
                linkToParent.RootNode.Nodes.Add(structure.RootNode);

                foreach (HEBlueprintDockingPort port in structure.DockingPorts.ToArray().Reverse())
                {
                    if (port != linkToParent)
                    {
                        structure.RootNode.Nodes.Add(port.RootNode);
                        if (port.IsDocked()) Reassemble(GetStructureByID(port.DockedStructureID), structure);
                    }
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HEBlueprintTreeNode GetDockingRootNode()
        {
            return GetDockingRoot().RootNode;
        }

        /// <summary>
        /// Gets the 'zeroth' structure, the root of the docking tree when in-game.
        /// </summary>
        /// <returns>Returns null if Structures is empty.</returns>
        public HEBlueprintStructure GetDockingRoot()
        {
            return Structures.Count > 0 ? Structures[0] : null;
        }

        /// <summary>
        /// Gets a structure by its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HEBlueprintStructure GetStructureByID(int? id)
        {
            HEBlueprintStructure result = null;
            if (id != null)
            {
                List<HEBlueprintStructure> resultsList = Structures.Where(f => f.StructureID == id).ToList();
                result = (resultsList.Count() > 0) ? resultsList[0] : null;
            }
            return result;
        }

        /// <summary>
        /// Adds a new structure.
        /// </summary>
        /// <returns>Returns true if the added structure is in the Structures list once created.</returns>
        public HEBlueprintStructure AddStructure(HEBlueprintStructureTypes structureType)
        {
            HEBlueprintStructure newStructure = new HEBlueprintStructure
            {
                StructureType = structureType,
                StructureID = Structures.Count() + 1
            };
            newStructure.OwnerObject = this;
            newStructure.AddAppropriateDockingPorts();

            Structures.Add(newStructure);

            ReconnectChildParentStructure();
            return newStructure;
        }

        public bool RemoveStructure(int? id)
        {

            HEBlueprintStructure structureToDelete = GetStructureByID(id);

            if (structureToDelete != null)
                structureToDelete.RemoveStructure(RemoveOrphanedStructures: true);
            else
                return false;

            return Structures.Contains(GetStructureByID(id)) ? true : false;

        }

        #endregion


        /// <summary>
        /// A class to define structures (modules/ships) within the blueprint.
        /// </summary>
        public class HEBlueprintStructure
        {
            #region Constructors

            /// <summary>
            /// Basic constructor.
            /// </summary>
            public HEBlueprintStructure()
            {
                DockingPorts = new List<HEBlueprintDockingPort>();
                RootNode = new HEBlueprintStructureTreeNode(passedOwner: this, nodeName: StructureType.ToString());
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="ownerObject"></param>
            public HEBlueprintStructure(HEBlueprint ownerObject = null) : this()
            {
                OwnerObject = ownerObject;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Parent object - not to be included in serialisation.
            /// </summary>
            public HEBlueprint OwnerObject { get; set; } = null;

            /// <summary>
            /// Not to be serialised.
            /// </summary>
            public HEBlueprintStructureTreeNode RootNode { get; set; } = null;

            /// <summary>
            /// The ID of the structure.
            /// </summary>
            public int? StructureID
            {
                get => _structureID;
                set
                {
                    if (value != _structureID)
                    {
                        // Change detected, evaluate whether this is the root node (has ID of zero)
                        _structureID = value;
                        RootNode.DisplayRootStructureIcon = (value != null && value == 0) ? true : false;
                    }
                }
            }

            /// <summary>
            /// The structure type - a value from the HEBlueprintStructureTypes enum.
            /// </summary>
            public HEBlueprintStructureTypes? StructureType
            {
                get => _structureType;
                set
                {
                    _structureType = value;
                    RootNode.BaseNodeName = value.ToString();
                    //RootNode.BaseNodeText = RootNode.Name;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public List<HEBlueprintDockingPort> DockingPorts { get; set; } = null;

            #endregion

            #region Methods

            /// <summary>
            /// Returns a list of directly docked structures, or all connected structures.
            /// </summary>
            /// <returns></returns>
            public List<HEBlueprintStructure> DockedStructures(bool AllConnected = false, HEBlueprintStructure incomingLink = null)
            {
                List<HEBlueprintStructure> dockedStructures = new List<HEBlueprintStructure>();
                foreach (HEBlueprintDockingPort port in DockingPorts)
                {
                    if (port.DockedStructureID != null)
                    {
                        //This port is docked to a structure, retrieve it by ID
                        HEBlueprintStructure result = OwnerObject.GetStructureByID(port.DockedStructureID);
                        if (incomingLink != null && incomingLink != result)
                        {
                            dockedStructures.Add(result);

                            if (AllConnected) dockedStructures.AddRange(result.DockedStructures(AllConnected, this));
                        }
                    }
                }
                return dockedStructures;
            }

            /// <summary>
            /// Returns a list of available (un-docked) docking ports for this structure or null if
            /// there are no ports available.
            /// </summary>
            /// <returns></returns>
            public List<HEBlueprintDockingPort> AvailableDockingPorts()
            {
                List<HEBlueprintDockingPort> availableDockingPorts = new List<HEBlueprintDockingPort>();

                foreach (HEBlueprintDockingPort port in DockingPorts)
                {
                    if (!port.IsDocked())
                    {
                        // Docking port is free, add it to the list.
                        availableDockingPorts.Add(port);
                    }
                }
                return availableDockingPorts.Count > 0 ? availableDockingPorts : null;
            }

            /// <summary>
            /// Returns a list of available (un-docked) docking ports for this structure or null if
            /// there are no ports available.
            /// </summary>
            /// <returns></returns>
            public List<HEBlueprintStructure> AllConnectedDockableStructures()
            {
                // Define a list of structures.
                List<HEBlueprintStructure> results = new List<HEBlueprintStructure>();

                List<HEBlueprintStructure> visitedStructures = new List<HEBlueprintStructure>();

                // Add this structure if there are available ports.
                if (AvailableDockingPorts() != null && AvailableDockingPorts().Count() > 0 && !visitedStructures.Contains(this))
                {
                    results.Add(this);
                    visitedStructures.Add(this);
                }

                //Iterate through the list of docked structures
                foreach (HEBlueprintStructure structure in DockedStructures(AllConnected: true, incomingLink: this))
                {
                    if (structure.AvailableDockingPorts() != null
                        && structure.AvailableDockingPorts().Count() > 0
                        && !visitedStructures.Contains(structure))
                    {
                        // Add to results list.
                        results.Add(structure);

                        // Add to the visitedStructures list.

                    }
                }
                return results.Count > 0 ? results : null;
            }

            /// <summary>
            /// Gets a docking port by name.
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public HEBlueprintDockingPort GetDockingPortByName(string name)
            {
                if (name != null) return DockingPorts.Where(f => f.PortName.ToString() == name).Single();
                else return null;
            }

            /// <summary>
            /// Gets a docking port by the ID of the structure it's docked to.
            /// </summary>
            /// <param name="dockedStructureID"></param>
            /// <returns></returns>
            public HEBlueprintDockingPort GetDockingPortByDockedStructureID(int dockedStructureID)
            {
                return DockingPorts.Where(f => f.DockedStructureID == dockedStructureID).Single();
            }

            /// <summary>
            /// Removes this structure, and any docked to it that would be orphaned by
            /// the removal operation.
            /// </summary>
            /// <returns>Returns true on success.</returns>
            public bool RemoveStructure(bool RemoveOrphanedStructures = true)
            {
                // Not yet implemented.

                // Make list of directly docked structures
                // Find local docking ports that are in use
                // Loop through each and find the corresponding docking port and reset it's data with .Undock()
                //      check the previously docked structures and make a list of structures not



                return false;
            }

            public void AddAppropriateDockingPorts()
            {
                // Find the matching definition type for this structures type.

                if (OwnerObject.StructureDefinitions == null) throw new NullReferenceException();

                HEBlueprintStructureDefinitions.HEBlueprintStructureDefinition defn = OwnerObject.StructureDefinitions
                    .StructureDefinitions.Where(f => f.SanitisedName == _structureType.ToString()).Single();

                foreach (HEBlueprintStructureDefinitions.HEBlueprintStructureDefinitionDockingPort dockingPort in defn.DockingPorts)
                {
                    HEBlueprintDockingPort newPort = new HEBlueprintDockingPort()
                    {
                        OwnerObject = this,
                        PortName = dockingPort.PortName,
                        OrderID = dockingPort.OrderID,
                        DockedStructureID = null
                    };
                    DockingPorts.Add(newPort);
                    RootNode.Nodes.Add(newPort.RootNode);

                }



            }

            #endregion

            #region Fields

            protected HEBlueprintStructureTypes? _structureType = null;
            protected int? _structureID = null;

            #endregion

        }

        public class HEBlueprintDockingPort
        {
            #region Constructors

            /// <summary>
            /// Basic Constructor.
            /// </summary>
            public HEBlueprintDockingPort()
            {
                RootNode = new HEBlueprintDockingPortTreeNode(passedOwner: this, nodeName: PortName.ToString());
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="passedParent"></param>
            public HEBlueprintDockingPort(HEBlueprintStructure passedParent = null) : this()
            {
                OwnerObject = passedParent;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Parent object - not to be included in serialisation.
            /// </summary>
            public HEBlueprintStructure OwnerObject { get; set; } = null;

            /// <summary>
            /// Not to be serialised.
            /// </summary>
            public HEBlueprintDockingPortTreeNode RootNode { get; set; } = null;

            /// <summary>
            /// 
            /// </summary>
            public HEDockingPortTypes? PortName
            {
                get => _portName;
                set
                {
                    if (_portName != value)
                    {
                        _portName = value;
                        RootNode.BaseNodeName = _portName.ToString();
                        //RootNode.BaseNodeText = RootNode.Name;
                    }
                }
            }

            public int? OrderID { get; set; } = null;
            public int? DockedStructureID { get; set; } = null;
            public string DockedPortName { get; set; } = null;

            #endregion

            #region Methods

            /// <summary>
            /// Indicates whether this port is docked to a port on another structure.
            /// </summary>
            /// <returns></returns>
            public bool IsDocked()
            {
                return DockedStructureID == null ? false : true;
            }

            /// <summary>
            /// Gets a reference to the structure that is docked to this port.
            /// </summary>
            /// <returns></returns>
            public HEBlueprintStructure GetDockedStructure()
            {
                return OwnerObject.OwnerObject.GetStructureByID(DockedStructureID);
            }

            /// <summary>
            /// Gets a reference to the port that is docked to this port.
            /// </summary>
            /// <returns></returns>
            public HEBlueprintDockingPort GetDockedPort()
            {
                return OwnerObject.OwnerObject.GetStructureByID(DockedStructureID)
                    .GetDockingPortByName(DockedPortName);
            }

            public bool DockTo()
            {

                return false;
            }

            public bool Undock()
            {
                // Find the paired docking port



                // Reset it's settings

                // Reset local port settings
                //DockedStructureID = null;
                //DockedPortName = "";

                return false;
            }

            #endregion

            #region Fields

            private HEDockingPortTypes? _portName = null;

            #endregion

        }
    }
}
