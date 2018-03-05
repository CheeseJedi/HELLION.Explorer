using System.Collections.Generic;

namespace HELLION.DataStructures
{
    public class HEBlueprintStructureDefinitions
    {
        public string __ObjectType = null;
        public decimal? Version = null;
        public List<HEBlueprintStructureDefinition> StructureDefinitions = null;

        public HEBlueprintStructureDefinitions()
        {
            StructureDefinitions = new List<HEBlueprintStructureDefinition>();
        }

        public class HEBlueprintStructureDefinition
        {
            public string SanitisedName = null;
            public int? ItemID = null;
            public string SceneName = null;
            public List<HEBlueprintStructureDefinitionDockingPort> DockingPorts = null;

            public HEBlueprintStructureDefinition()
            {
                DockingPorts = new List<HEBlueprintStructureDefinitionDockingPort>();
            }

            public class HEBlueprintStructureDefinitionDockingPort
            {
                public string PortName = null;
                public int? PortID = null;
                public int? OrderID = null;
            }
        }
    }



}
