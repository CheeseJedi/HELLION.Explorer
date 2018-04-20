using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HELLION.DataStructures
{
    #region Enumerations

    /// <summary>
    /// Structure Types Enum.
    /// </summary>
    /// <remarks>
    /// The numeric values of these correspond to the game's ItemID in the structures.json
    /// to allow for easier cross-referencing.
    /// </remarks>
    public enum HEBlueprintStructureType
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
    public enum HEDockingPortType
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

    /// <summary>
    /// Enumeration for the results of a docking operation.
    /// </summary>
    public enum HEDockingResultStatus
    {
        Success = 0,
        InvalidPortA,
        InvalidPortB,
        InvalidStructurePortA,
        InvalidStructurePortB,
        AlreadyDockedPortA,
        AlreadyDockedPortB,
        PortsNotDocked,
        PortsOnSameStructure,
        IncompatiblePortTypes,
        WillCauseOrphanedStructure,
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
            SecondaryStructures = new List<HEBlueprintStructure>();
            RootNode = new HEBlueprintTreeNode(passedOwner: this, nodeName: "Hierarchy View",
                newNodeType: HETreeNodeType.BlueprintHierarchyView, 
                nodeToolTipText: "Shows a tree-based view of the modules and their docking hierarchy.");

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

        public HEBlueprintStructureDefinitions StructureDefinitions { get; set; } = null;

        /// <summary>
        /// The list of all Secondary Structures - these are not technically part of a finished
        /// blueprint and are linked here to allow processing separately to the primary structure.
        /// </summary>
        public List<HEBlueprintStructure> SecondaryStructures { get; set; } = null;

        public bool IsDirty { get; protected set; } = false;


        #endregion

        #region Serialised Properties

        /// <summary>
        /// Type of object (should be "StationBlueprint")
        /// </summary>
        /// <remarks>
        /// To be serialised.
        /// </remarks>
        public string __ObjectType { get; set; } = null;

        /// <summary>
        /// The Blueprint version.
        /// </summary>
        /// <remarks>
        /// Should reflect the version of the StructureDefinitions.json file.
        /// To be serialised.
        /// </remarks>
        public decimal? Version { get; set; } = null;

        /// <summary>
        /// The Station's name.
        /// </summary>
        /// <remarks>
        /// To be serialised.
        /// </remarks>
        public string Name { get; set; } = null;

        /// <summary>
        /// The link URI for the blueprint source.
        /// </summary>
        /// <remarks>
        /// If this is generates in Hellion Explorer, this will be a link to the GitHub Repo.
        /// If the blueprint originated from HSP, this will be a link to the station in HSP.
        /// To be serialised.
        /// </remarks>
        public Uri LinkURI { get; set; } = null;

        /// <summary>
        /// The list of all structures in the blueprint.
        /// </summary>
        /// <remarks>
        /// May contain secondary structures during the process of editing.
        /// To be serialised.
        /// </remarks>
        public List<HEBlueprintStructure> Structures { get; set; } = null;

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void PostDeserialisationInit()
        {
            ReconnectChildToParentObjectHierarchy();
            ReconnectDockedObjects();

        }

        /// <summary>
        /// Links up all child object's parent reference to their parent.
        /// </summary>
        private void ReconnectChildToParentObjectHierarchy()
        {
            if (OwnerObject == null) throw new NullReferenceException("ParentJsonBlueprintFile was null.");

            StructureDefinitions = OwnerObject.OwnerObject.OwnerObject
                .StructureDefinitionsFile.BlueprintStructureDefinitionsObject;

            foreach (HEBlueprintStructure structure in Structures)
            {
                // Set the structure's parent object (this, the blueprint object)
                structure.OwnerObject = this;
                foreach (HEBlueprintDockingPort port in structure.DockingPorts)
                {
                    port.OwnerObject = structure;

                }
            }
        }

        /// <summary>
        /// Links up all child object's parent reference to their parent.
        /// </summary>
        private void ReconnectDockedObjects()
        {
            foreach (HEBlueprintStructure structure in Structures)
            {
                foreach (HEBlueprintDockingPort port in structure.DockingPorts)
                {
                    port.AttemptUpdateDockedStructure();
                    port.AttemptUpdateDockedPort();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReassembleTreeNodeDockingStructure(HEBlueprintStructure hierarchyRoot, bool AttachToBlueprintTreeNode = false)
        {
            // Start with the root node, should be item zero in the list.
            

            // Debug.Print("dockingRoot {0} StructureID {1}", dockingRoot.StructureType, dockingRoot.StructureID);

            // Process the docking root's ports slightly differently - always child nodes.
            if (AttachToBlueprintTreeNode) RootNode.Nodes.Add(hierarchyRoot.RootNode);

            foreach (HEBlueprintDockingPort port in hierarchyRoot.DockingPorts.ToArray().Reverse())
            {
                hierarchyRoot.RootNode.Nodes.Add(port.RootNode);
                if (port.IsDocked)
                {
                    Reassemble(GetStructure(port.DockedStructureID), hierarchyRoot);
                }
            }

            void Reassemble(HEBlueprintStructure structure, HEBlueprintStructure parent)
            {
                // Figure out which port is docking us to the parent and vice versa.
                // TODO - This probably should use the object references now they are available instead of the simple de-serialised IDs.
                HEBlueprintDockingPort linkToParent = structure.GetDockingPort((int)parent.StructureID);
                HEBlueprintDockingPort linkFromParent = parent.GetDockingPort((int)structure.StructureID);

                // Add the node for the link to parent to the link from parent's node collection.
                linkFromParent.RootNode.Nodes.Add(linkToParent.RootNode);

                // Add the structures's node to the link to parent node collection.
                linkToParent.RootNode.Nodes.Add(structure.RootNode);

                foreach (HEBlueprintDockingPort port in structure.DockingPorts.ToArray().Reverse())
                {
                    if (port != linkToParent)
                    {
                        structure.RootNode.Nodes.Add(port.RootNode);
                        if (port.IsDocked) Reassemble(GetStructure(port.DockedStructureID), structure);
                    }
                }

            }
        }

        /// <summary>
        /// Refreshes all tree node structures for the Primary and Secondary structures.
        /// </summary>
        public void RefreshAllTreeNodes()
        {
            // Break any existing parent/child relationships.
            foreach (HEBlueprintStructure structure in Structures)
            {
                structure.OwnerObject.RootNode.Nodes.Remove(structure.RootNode);
                foreach (HEBlueprintDockingPort port in structure.DockingPorts)
                {
                    port.OwnerObject.RootNode.Nodes.Remove(port.RootNode);
                }
            }

            // Assemble the primary blueprint structure's docked node tree.
            ReassembleTreeNodeDockingStructure(GetDockingRoot());

            // Assemble the secondary structures node trees.
            foreach (HEBlueprintStructure _secondaryStructure in SecondaryStructures)
            {
                ReassembleTreeNodeDockingStructure(_secondaryStructure);
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
        /// <param name="structureID"></param>
        /// <returns></returns>
        public HEBlueprintStructure GetStructure(int? structureID)
        {
            if (structureID == null) return null;
            IEnumerable<HEBlueprintStructure> results = Structures.Where(f => f.StructureID == structureID);
            if (results.Count() < 1 || results.Count() > 1) return null;
            return results.Single();
        }

        /// <summary>
        /// Adds a new structure to the Secondary Structures list.
        /// </summary>
        /// <returns>Returns true if the added structure is in the Structures list once created.</returns>
        public HEBlueprintStructure AddStructure(HEBlueprintStructureType structureType)
        {
            HEBlueprintStructure newStructure = new HEBlueprintStructure
            {
                StructureType = structureType,

                StructureID = Structures.Count() // + 2000,
            };
            // Set this blueprint as the owner.
            newStructure.OwnerObject = this;
            // Populate the DockingPorts collection with ports appropriate for this structure type.
            newStructure.AddAppropriateDockingPorts();

            // Add the new structure to the main Structures list.
            Structures.Add(newStructure);

            // Add the new structure to the Secondary Structures list.
            SecondaryStructures.Add(newStructure);


            ReconnectChildToParentObjectHierarchy();

            IsDirty = true;

            return newStructure;
        }

        public bool RemoveStructure(int? id)
        {
            return RemoveStructure(GetStructure(id));
        }

        public bool RemoveStructure(HEBlueprintStructure structure)
        {
            if (structure != null)
                structure.Remove(RemoveOrphanedStructures: true);
            else
                return false;

            IsDirty = true;



            return Structures.Contains(structure) ? true : false;

        }



        /// <summary>
        /// Docks the structures that the two specified ports belong to.
        /// </summary>
        /// <param name="portA"></param>
        /// <param name="portB"></param>
        /// <returns></returns>
        public HEDockingResultStatus DockPorts(HEBlueprintDockingPort portA, HEBlueprintDockingPort portB)
        {
            // portA should be the Primary Structure side, portB the Secondary Structure.
            
            // Check the passed ports are valid.
            if (portA == null) return HEDockingResultStatus.InvalidPortA;
            if (portB == null) return HEDockingResultStatus.InvalidPortB;

            // check the ports parent structures are valid.
            if (portA.OwnerObject == null) return HEDockingResultStatus.InvalidStructurePortA;
            if (portB.OwnerObject == null) return HEDockingResultStatus.InvalidStructurePortB;

            // Ensure that the two ports aren't on the same structure.
            if (portA.OwnerObject == portB.OwnerObject) return HEDockingResultStatus.PortsOnSameStructure;

            // Ensure that both ports are not already docked.
            if (portA.IsDocked) return HEDockingResultStatus.AlreadyDockedPortA;
            if (portB.IsDocked) return HEDockingResultStatus.AlreadyDockedPortB;

            // Proceed with docking operation.

            // Update portA.
            portA.DockedStructure = portB.OwnerObject;
            portA.DockedPort = portB;

            // Update portB.
            portB.DockedStructure = portA.OwnerObject;
            portB.DockedPort = portA;

            // Clean up Secondary Structures list.
            if (SecondaryStructures.Contains(portB.OwnerObject)) SecondaryStructures.Remove(portB.OwnerObject);

            // Mark the blueprint object as dirty.
            IsDirty = true;

            return HEDockingResultStatus.Success;
        }

        /// <summary>
        /// Un-docks the structure that the specified port belongs to from whatever it's docked to.
        /// </summary>
        /// <param name="portA"></param>
        /// <returns></returns>
        public HEDockingResultStatus UndockPort(HEBlueprintDockingPort portA)
        {
            if (portA == null) return HEDockingResultStatus.InvalidPortA;
            if (portA.OwnerObject == null) return HEDockingResultStatus.InvalidStructurePortA;
            if (!portA.IsDocked) return HEDockingResultStatus.PortsNotDocked;


            // Find portB (the other side)
            HEBlueprintDockingPort portB = portA.DockedPort;

            if (portB == null) return HEDockingResultStatus.InvalidPortB;
            if (portB.OwnerObject == null) return HEDockingResultStatus.InvalidStructurePortB;
            if (!portB.IsDocked) return HEDockingResultStatus.PortsNotDocked;


            HEBlueprintStructure structureB = portA.DockedStructure;

            if (portA.OwnerObject != portB.DockedStructure) return HEDockingResultStatus.PortsNotDocked;
            if (portB.OwnerObject != portA.DockedStructure) return HEDockingResultStatus.PortsNotDocked;

            // Process the un-docking.

            // Set the DockedStructures to null
            portA.DockedStructure = null;
            portA.DockedPort = null;

            portB.DockedStructure = null;
            portB.DockedPort = null;

            // Figure out which structure to add to the Secondary Structures list.



            SecondaryStructures.Add(structureB);

            IsDirty = true;

            return HEDockingResultStatus.Success;
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
                RootNode = new HEBlueprintStructureTreeNode(passedOwner: this,
                    nodeName: StructureType.ToString());

                //String.Format("[{0:000}] {1} ", StructureID, StructureType));

                //RootNode.PrefixNodeName = StructureID != null ? String.Format("[{0:000}] ", (int)StructureID) : "[ERR] ";
                //RootNode.RefreshName();

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
                    if (_structureID != value)
                    {
                        // Change detected, evaluate whether this is the root node (has ID of zero)
                        _previousStructureID = _structureID;
                        _structureID = value;

                        RefreshAfterStructureIDChange();
                    }
                }
            }

            /// <summary>
            /// The structure type - a value from the HEBlueprintStructureType enum.
            /// </summary>
            public HEBlueprintStructureType? StructureType
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

            public bool IsAHierarchyRoot { get; set; } = false;

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
                        HEBlueprintStructure result = OwnerObject.GetStructure(port.DockedStructureID);
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
                    if (!port.IsDocked)
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
            public HEBlueprintDockingPort GetDockingPort(string name)
            {
                if (name == null || name == "" || !(DockingPorts.Count > 0)) return null;

                IEnumerable<HEBlueprintDockingPort> results = DockingPorts.
                    Where(f => f.PortName.ToString() == name);

                return results.Count() == 1 ? results.Single() : null;
            }

            /// <summary>
            /// Gets a docking port by the ID of the structure it's docked to.
            /// </summary>
            /// <param name="dockedStructureID"></param>
            /// <returns></returns>
            public HEBlueprintDockingPort GetDockingPort(int dockedStructureID)
            {
                if (!(dockedStructureID >= 0) || !(dockedStructureID <= OwnerObject.Structures.Count) 
                    || !(DockingPorts.Count > 0)) return null;

                IEnumerable<HEBlueprintDockingPort> results = DockingPorts.
                    Where(f => f.DockedStructureID == dockedStructureID);

                return results.Count() == 1 ? results.Single() : null;
            }

            /// <summary>
            /// Gets a docking port by the structure it's docked to.
            /// </summary>
            /// <param name="dockedStructureID"></param>
            /// <returns></returns>
            public HEBlueprintDockingPort GetDockingPort(HEBlueprintStructure dockedStructure)
            {
                if (dockedStructure == null || DockingPorts.Count > 0) return null;

                IEnumerable<HEBlueprintDockingPort> results = DockingPorts.
                    Where(f => f.DockedStructure == dockedStructure);

                return results.Count() == 1 ? results.Single() : null;
            }

            /// <summary>
            /// Removes this structure, and any docked to it that would be orphaned by
            /// the removal operation.
            /// </summary>
            /// <returns>Returns true on success.</returns>
            public bool Remove(bool RemoveOrphanedStructures = false)
            {
                // TODO - Not yet implemented.

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

            /// <summary>
            /// Is called when the StructureID changes and updates the node's prefix and icon.
            /// </summary>
            protected void RefreshAfterStructureIDChange()
            {
                if (StructureID != null)
                {
                    // Module ID zero is always the docking root in a blueprint and has a different icon.
                    RootNode.DisplayRootStructureIcon = (StructureID != null && StructureID == 0) ? true : false;

                    // RootNode.BaseNodeName = StructureType.ToString();
                    RootNode.PrefixNodeName = String.Format("[{0:000}] ", (int)StructureID);
                    RootNode.RefreshName();

                    // Update Docking Port nodes for this node.
                    if (DockingPorts.Count > 0)
                    {
                        foreach (var port in DockingPorts)
                        {
                            port.RefreshAfterParentStructureIDChange();
                        }
                    }

                    
                    // Update docking structure relationships between this module and any docked modules.

                }
            }

            #endregion

            #region Fields

            protected HEBlueprintStructureType? _structureType = null;
            protected int? _structureID = null;
            protected int? _previousStructureID = null;

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
                RootNode = new HEBlueprintDockingPortTreeNode(passedOwner: this,
                    nodeName: PortName.ToString())
                {
                    PrefixNodeName = OwnerObject != null && OwnerObject.StructureID != null ? 
                        String.Format("[{0:000}] ", (int)OwnerObject.StructureID) : "[ERR] "
                };
                //RootNode.RefreshName();

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
            public HEBlueprintStructure OwnerObject
            {
                get => _ownerObject;
                set
                {
                    _ownerObject = value;
                    RefreshAfterParentStructureIDChange();
                }
            }

            /// <summary>
            /// Not to be serialised.
            /// </summary>
            public HEBlueprintDockingPortTreeNode RootNode { get; set; } = null;

            /// <summary>
            /// 
            /// </summary>
            public HEDockingPortType? PortName
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

            /// <summary>
            /// The ID of the structure this port is docked to.
            /// </summary>
            /// <remarks>
            /// This is to be serialised.
            /// This should only be set initially by the JToken.ToObject()
            /// Attempts to set the DockedStructure object.
            /// </remarks>
            public int? DockedStructureID
            {
                get => _dockedStructureID;
                set
                {
                    if (_dockedStructureID != value)
                    {
                        _dockedStructureID = value;

                        AttemptUpdateDockedStructure();

                    }
                }
            }

            /// <summary>
            /// The structure object of the port this port is docked to.
            /// </summary>
            /// <remarks>
            /// Updates the DockedStructureID.
            /// </remarks>
            public HEBlueprintStructure DockedStructure
            {
                get => _dockedStructure;
                set
                {
                    if (_dockedStructure != value)
                    {
                        _dockedStructure = value;

                        // Attempt to update the DockedSctuctureID
                        //if (_dockedStructure != null)
                        //{
                            DockedStructureID = _dockedStructure?.StructureID;
                        //}

                        if (_dockedStructure == null) DockedPort = null;


                    }
                }
            }

            /// <summary>
            /// The name of the port this port is docked to.
            /// </summary>
            /// <remarks>
            /// This is to be serialised.
            /// Updates the DockedStructureID.
            /// </remarks>
            public HEDockingPortType? DockedPortName
            {
                get => _dockedPortName;
                set
                {
                    if (_dockedPortName != value)
                    {
                        _dockedPortName = value;

                        AttemptUpdateDockedPort();
                    }
                }
            }

            /// <summary>
            /// The Port object this port is docked to.
            /// </summary>
            public HEBlueprintDockingPort DockedPort
            {
                get => _dockedPort;
                set
                {
                    if (_dockedPort != value)
                    {
                        _dockedPort = value;

                        // Attempt to update the DockedPortName

                        //if (_dockedPort != null)
                        //{
                            DockedPortName = _dockedPort?.PortName;
                        //}

                        if (_dockedPort == null) DockedStructure = null;


                    }
                }
            }

            /// <summary>
            /// Indicates whether this port is docked to a port on another structure.
            /// </summary>
            public bool IsDocked
            {
                get => DockedStructureID == null ? false : true;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Attempt to update the DockedStructure object.
            /// </summary>
            /// <returns>
            /// Return true if a non null was returned from the GetStructureByID.
            /// </returns>
            public bool AttemptUpdateDockedStructure()
            {
                if (OwnerObject == null || OwnerObject.OwnerObject == null) return false;
                DockedStructure = OwnerObject.OwnerObject.GetStructure(_dockedStructureID);
                return DockedStructure != null ? true : false;
            }

            /// <summary>
            /// Attempt to update the DockedPort object.
            /// </summary>
            /// <returns>
            /// Return true if a non null was returned from the GetStructureByID.
            /// </returns>
            public bool AttemptUpdateDockedPort()
            {
                if (DockedStructure == null || OwnerObject.OwnerObject == null) return false;
                DockedPort = DockedStructure.GetDockingPort(_dockedPortName.ToString());
                return DockedPort != null ? true : false;
            }


            /*
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

            */

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


            public void RefreshAfterParentStructureIDChange()
            {
                RootNode.PrefixNodeName = OwnerObject != null ? String.Format("[{0:000}] ", (int)OwnerObject.StructureID) : "[ERR] ";
                RootNode.RefreshName();
            }



            #endregion

            #region Fields

            private HEDockingPortType? _portName = null;
            private HEBlueprintStructure _ownerObject = null;
            private int? _dockedStructureID = null;
            private HEBlueprintStructure _dockedStructure = null;
            private HEDockingPortType? _dockedPortName = null;
            private HEBlueprintDockingPort _dockedPort = null;

            #endregion

        }
    }
}
