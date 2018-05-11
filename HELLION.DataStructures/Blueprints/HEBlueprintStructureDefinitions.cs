using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HELLION.DataStructures
{
    [JsonObject(MemberSerialization.OptIn)]
    public class HEBlueprintStructureDefinitions
    {
        /// <summary>
        /// Object Type Def: should always be BlueprintStructureDefinitions
        /// </summary>
        [JsonProperty]
        public string __ObjectType = null;

        /// <summary>
        /// To be serialised.
        /// </summary>
        [JsonProperty]
        public decimal? Version = null;

        [JsonProperty]
        public Uri LinkURI = null;

        /// <summary>
        /// To be serialised.
        /// </summary>
        [JsonProperty]
        public List<HEBlueprintStructureDefinition> StructureDefinitions = null;

        /// <summary>
        /// Parent object - not to be included in serialisation.
        /// </summary>
        public object Parent = null;

        /// <summary>
        /// Basic Constructor.
        /// </summary>
        public HEBlueprintStructureDefinitions()
        {
            StructureDefinitions = new List<HEBlueprintStructureDefinition>();
        }


        [JsonObject(MemberSerialization.OptIn)]
        public class HEBlueprintStructureDefinition
        {
            /// <summary>
            /// To be serialised.
            /// </summary>
            [JsonProperty]
            public string DisplayName = null; // not required by the game spawner

            public string SanitisedName = null;

            /// <summary>
            /// To be serialised.
            /// </summary>
            [JsonProperty]
            public int? ItemID = null;
            /// <summary>
            /// To be serialised.
            /// </summary>
            [JsonProperty]
            public string SceneName = null;
            /// <summary>
            /// To be serialised.
            /// </summary>
            [JsonProperty]
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

        [JsonObject(MemberSerialization.OptIn)]
        public class HEBlueprintStructureDefinitionDockingPort
        {
            /// <summary>
            /// Filled when de-serialised.
            /// </summary>
            [JsonProperty]
            public HEDockingPortType? PortName = null; // not required by the game spawner

            /// <summary>
            /// Filled when de-serialised.
            /// </summary>
            [JsonProperty]
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
            public string DockedPortName = null; // not required by the game spawner

            /// <summary>
            /// Filled when de-serialised, but to be REMOVED when port is cloned to be used in
            /// a blueprint.
            /// </summary>
            public int? PortID = null; // probably irrelevant now.
        }

    }

}
