using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using static HELLION.DataStructures.StaticDataHelper;

namespace HELLION.DataStructures.Blueprints
{
    [JsonObject(MemberSerialization.OptIn)]
    public class OLD_BlueprintStructureDefinitions
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

        /// <summary>
        /// The Station's name.
        /// </summary>
        /// <remarks>
        /// To be serialised.
        /// </remarks>
        // [JsonProperty]
        // public string Name = null;

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
        public OLD_BlueprintStructureDefinitions()
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

            /*
            public string SanitisedName
            {
                get => DisplayName;
                set
                {
                    // Only set the DisplayName if it's null.
                    if (DisplayName == null)
                        DisplayName = value;
                }
            }
            */

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
            public OLD_BlueprintStructureDefinitions Parent = null;

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
