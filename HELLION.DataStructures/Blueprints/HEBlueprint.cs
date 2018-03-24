using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HELLION.DataStructures
{
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

    public class HEBlueprint
    {
        /// <summary>
        /// To be serialised.
        /// </summary>
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
        /// Parent object - not to be included in serialisation.
        /// </summary>
        public HEJsonBlueprintFile OwnerObject { get; set; } = null;

        public HEBlueprintStructureDefinitions StructureDefinitions { get; set; } = null;

        /// <summary>
        /// Not to be serialised.
        /// </summary>
        public HEBlueprintTreeNode RootNode { get; set; } = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="passedParent"></param>
        public HEBlueprint(HEJsonBlueprintFile passedParent = null)
        {
            OwnerObject = passedParent;
            Structures = new List<HEBlueprintStructure>();
            RootNode = new HEBlueprintTreeNode(passedOwner: this, nodeName: "Hierarchy View",
                newNodeType: HETreeNodeType.BlueprintHierarchyView, nodeToolTipText: "Shows a tree-based view of the modules and their docking hierarchy.");
        }

        /// <summary>
        /// Links up all child object's parent reference to their parent.
        /// </summary>
        public void ReconnectChildParentStructure() //HEJsonBlueprintFile passedParent)
        {
            if (OwnerObject == null) throw new NullReferenceException("ParentJsonBlueprintFile was null.");

            //StructureDefinitions = OwnerObject.OwnerObject.OwnerObject
            //    .StructureDefinitionsFile.BlueprintStructureDefinitionsObject;

            foreach (HEBlueprintStructure structure in Structures)
            {
                // Set the structure's parent object (this, the blueprint object)
                structure.OwnerObject = this;
                foreach (HEBlueprintDockingPort port in structure.DockingPorts) port.OwnerObject = structure;
            }
        }

        public void ReassembleDockingStructure()
        {
            // Start with the root node, should be item zero in the list.
            HEBlueprintStructure dockingRoot = GetDockingRoot();

            // Process the docking root's ports slightly differently - always child nodes.
            RootNode.Nodes.Add(dockingRoot.RootNode);
            foreach (HEBlueprintDockingPort port in dockingRoot.DockingPorts.ToArray().Reverse())
            {
                dockingRoot.RootNode.Nodes.Add(port.RootNode);
                if (port.IsDocked()) Reassemble(GetStructureByID(port.DockedStructureID), dockingRoot);
            }
           
            void Reassemble(HEBlueprintStructure structure, HEBlueprintStructure parent)
            {
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
            if (id != null) return Structures.Where(f => f.StructureID == id).Single();
            else return null;
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

        /// <summary>
        /// A class to define structures (modules/ships) within the blueprint.
        /// </summary>
        public class HEBlueprintStructure
        {
            public int? StructureID { get; set; } = null;
            
            public HEBlueprintStructureTypes? StructureType
            {
                get { return structureType; }
                set
                {
                    structureType = value;
                    RootNode.Name = value.ToString();
                    RootNode.Text = RootNode.Name;
                }
            }
            protected HEBlueprintStructureTypes? structureType = null;

            public List<HEBlueprintDockingPort> DockingPorts = null;

            //public HEBlueprint Parent => OwnerObject;

            /// <summary>
            /// Parent object - not to be included in serialisation.
            /// </summary>
            public HEBlueprint OwnerObject { get; set; } = null;

            /// <summary>
            /// Not to be serialised.
            /// </summary>
            public HEBlueprintTreeNode RootNode { get; set; } = null;

            /// <summary>
            /// Basic constructor.
            /// </summary>
            public HEBlueprintStructure()
            {
                DockingPorts = new List<HEBlueprintDockingPort>();
                RootNode = new HEBlueprintTreeNode(passedOwner: this, nodeName: StructureType.ToString(), newNodeType: HETreeNodeType.BlueprintStructure);
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="ownerObject"></param>
            public HEBlueprintStructure(HEBlueprint ownerObject = null) : this()
            {
                OwnerObject = ownerObject;
            }

            /// <summary>
            /// Returns a list of directly docked structures, or all connected structures.
            /// </summary>
            /// <returns></returns>
            public List<HEBlueprintStructure> DockedStructures(bool AllConnected = false)
            {
                List<HEBlueprintStructure> dockedStructures = new List<HEBlueprintStructure>();
                foreach (HEBlueprintDockingPort port in DockingPorts)
                {
                    if (port.DockedStructureID != null)
                    {
                        //This port is docked to a structure, retrieve it by ID
                        HEBlueprintStructure result = OwnerObject.GetStructureByID(port.DockedStructureID);
                        dockedStructures.Add(result);

                        if (AllConnected) dockedStructures.AddRange(result.DockedStructures(AllConnected));
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
                    if (!port.IsDocked()) availableDockingPorts.Add(port);
                }
                return availableDockingPorts.Count > 0 ? availableDockingPorts : null;
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

                HEBlueprintStructureDefinitions.HEBlueprintStructureDefinition defn = OwnerObject.StructureDefinitions
                    .StructureDefinitions.Where(f => f.SanitisedName == structureType.ToString()).Single();

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

        }

        public class HEBlueprintDockingPort
        {
            public HEDockingPortTypes? PortName
            {
                get { return portName; }
                set
                {
                    portName = value;
                    RootNode.Name = portName.ToString();
                    RootNode.Text = RootNode.Name;
                }
            }

            private HEDockingPortTypes? portName = null;

            public int? OrderID { get; set; } = null;
            public int? DockedStructureID { get; set; } = null;
            public string DockedPortName { get; set; } = null;

            /// <summary>
            /// Parent object - not to be included in serialisation.
            /// </summary>
            public HEBlueprintStructure OwnerObject { get; set; } = null;

            /// <summary>
            /// Not to be serialised.
            /// </summary>
            public HEBlueprintTreeNode RootNode { get; set; } = null;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="passedParent"></param>
            public HEBlueprintDockingPort(HEBlueprintStructure passedParent = null)
            {
                OwnerObject = passedParent;
                RootNode = new HEBlueprintTreeNode(passedOwner: this, nodeName: PortName.ToString(), newNodeType: HETreeNodeType.BlueprintDockingPort);

            }

            /// <summary>
            /// Indicates whether this port is docked to a port on another structure.
            /// </summary>
            /// <returns></returns>
            public bool IsDocked()
            {
                return DockedStructureID == null ? false : true;
            }

            public bool Undock()
            {
                // Find the paired docking port



                // Reset it's settings

                // Reset local port settings
                DockedStructureID = null;
                DockedPortName = "";

                return false;
            }

        }
    }
}
