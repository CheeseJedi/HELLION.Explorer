using System;
using System.Linq;
using System.Collections.Generic;

namespace HELLION.DataStructures
{
    public class HEBlueprint
    {
        public string __ObjectType = null;
        public decimal? Version = null;
        public string Name = null;
        public Uri LinkURI = null;
        public List<HEBlueprintStructure> Structures = null;
        public object Parent = null;

        public HEBlueprint(object passedParent = null)
        {
            Parent = passedParent;
            Structures = new List<HEBlueprintStructure>();
        }

        /// <summary>
        /// Links up all child object's parent reference to their parent.
        /// </summary>
        public void ReconnectChildParentStructure()
        {
            foreach (HEBlueprintStructure structure in Structures)
            {
                structure.Parent = this;
                foreach (HEBlueprintDockingPort port in structure.DockingPorts)
                {
                    port.Parent = structure;
                }
            }
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
        public bool AddStructure()
        {
            HEBlueprintStructure newStructure = new HEBlueprintStructure();

            newStructure.StructureType = HEBlueprintStructure.HEBlueprintStructureTypes.CM;
            newStructure.StructureID = Structures.Count() + 1;
            Structures.Add(newStructure);
            return Structures.Contains(newStructure) ? true : false;
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
            // Structure Types
            // The numeric values of these correspond to the game's ItemID in the structures.json
            // to allow for easier cross-referencing.
            public enum HEBlueprintStructureTypes
            {
                UNKNOWN = 0,
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


            public int? StructureID = null;
            // was a string type
            public HEBlueprintStructureTypes? StructureType = null;
            public List<HEBlueprintDockingPort> DockingPorts = null;
            public HEBlueprint Parent = null;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="passedParent"></param>
            public HEBlueprintStructure(HEBlueprint passedParent = null)
            {
                Parent = passedParent;
                DockingPorts = new List<HEBlueprintDockingPort>();
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
                        HEBlueprintStructure result = Parent.GetStructureByID(port.DockedStructureID);
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
        }

        public class HEBlueprintDockingPort
        {
            public enum HEDockingPortTypes
            {
                Unknown = 0,
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
                CargoDock  // Dockable Cargo module
            }

            public HEDockingPortTypes? PortName = null;
            public int? OrderID = null;
            public int? DockedStructureID = null;
            public string DockedPortName = null;
            public HEBlueprintStructure Parent = null;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="passedParent"></param>
            public HEBlueprintDockingPort(HEBlueprintStructure passedParent = null)
            {
                Parent = passedParent;
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
