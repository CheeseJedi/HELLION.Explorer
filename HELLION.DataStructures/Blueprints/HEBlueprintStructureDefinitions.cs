using System.Collections.Generic;

namespace HELLION.DataStructures
{
    public class HEBlueprintStructureDefinitions
    {
        /// <summary>
        /// To be serialised.
        /// </summary>
        public string __ObjectType = null;
        /// <summary>
        /// To be serialised.
        /// </summary>
        public decimal? Version = null;
        /// <summary>
        /// To be serialised.
        /// </summary>
        public List<HEBlueprintStructureDefinition> StructureDefinitions = null;
        /// <summary>
        /// Parent object - not to be included in serialisation.
        /// </summary>
        public object Parent = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        public HEBlueprintStructureDefinitions()
        {
            StructureDefinitions = new List<HEBlueprintStructureDefinition>();
        }

        public class HEBlueprintStructureDefinition
        {
            /// <summary>
            /// To be serialised.
            /// </summary>
            public string SanitisedName = null;
            /// <summary>
            /// To be serialised.
            /// </summary>
            public int? ItemID = null;
            /// <summary>
            /// To be serialised.
            /// </summary>
            public string SceneName = null;
            /// <summary>
            /// To be serialised.
            /// </summary>
            public List<HEBlueprintStructureDefinitionDockingPort> DockingPorts = null;
            /// <summary>
            /// Parent object - not to be included in serialisation.
            /// </summary>
            public HEBlueprintStructureDefinitions Parent = null;

            /// <summary>
            /// Constructor.
            /// </summary>
            public HEBlueprintStructureDefinition()
            {
                DockingPorts = new List<HEBlueprintStructureDefinitionDockingPort>();
            }
        }

        public class HEBlueprintStructureDefinitionDockingPort
        {

            /// <summary>
            /// Filled when de-serialised.
            /// </summary>
            public HEDockingPortType? PortName = null;
            
            /// <summary>
            /// Filled when de-serialised.
            /// </summary>
            public int? OrderID = null;

            /// <summary>
            /// This is not filled from de-serialisation, but is present so this
            /// docking port can be cloned for use in a blueprint directly.
            /// </summary>
            public int? DockedStructureID = null;
            
            /// <summary>
            /// This is not filled from de-serialisation, but is present so this
            /// docking port can be cloned for use in a blueprint directly.
            /// </summary>
            public string DockedPortName = null;

            /// <summary>
            /// Filled when de-serialised, but to be REMOVED when port is cloned to be used in
            /// a blueprint.
            /// </summary>
            public int? PortID = null;
        }
    }
}
