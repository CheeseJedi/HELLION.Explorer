using System;
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

        public void ConnectTheDots()
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

        public class HEBlueprintStructure
        {
            public int? StructureID = null;
            public string StructureType = null;
            public List<HEBlueprintDockingPort> DockingPorts = null;
            public HEBlueprint Parent = null;

            public HEBlueprintStructure(HEBlueprint passedParent = null)
            {
                Parent = passedParent;
                DockingPorts = new List<HEBlueprintDockingPort>();
            }
        }

        public class HEBlueprintDockingPort
        {
            public string PortName = null;
            public int? OrderID = null;
            public int? DockedStructureID = null;
            public string DockedPortName = null;
            public HEBlueprintStructure Parent = null;

            public HEBlueprintDockingPort(HEBlueprintStructure passedParent = null)
            {
                Parent = passedParent;
            }
        }
    }
}
