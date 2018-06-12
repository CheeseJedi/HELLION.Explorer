using Newtonsoft.Json;

namespace HELLION.DataStructures.Blueprints
{
    public partial class StationBlueprint
    {
        /// <summary>
        /// Defines optional parameters for a station blueprint structure.
        /// </summary>
        /// <remarks>
        /// Can also be attached to a blueprint directly.
        /// </remarks>
        [JsonObject(MemberSerialization.OptOut)]
        public class BlueprintStructure_AuxData
        {
            // Optional parameters - structure level
            public bool? Invulnerable = null;                   // Invulnerable or will take damage.
            public bool? SystemsOnline = null;                  // Spawns powered on.
            public bool? PowerGeneratorsOnline = null;          // Power generators (solar panels/reactors) spawn powered on.
            public bool? DoorsLocked = null;                    // Doors are locked.
            public bool? DockingPortsLocked = null;             // Docking ports are locked (not advertised to docking systems).
            public bool? CryoPodsDisabled = null;               // Cryo pods are deactivated (non interact-able)
            public bool? DockingReleaseHandlesDisabled = null;  // Modules' docking release handles are visible.
            public bool? TextLabelEditingDisabled = null;       // Text labels (doors, parts boxes) are editable.
            public bool? SecurityPanelsDisabled = null;         // Security panel(s) are disabled or deactivated.
            public bool? SystemPartsInteractionDisabled = null; // Dynamic Object Parts in systems cannot be interacted with.

            /// <summary>
            /// Default constructor - able to pre-set values for a prefab.
            /// </summary>
            /// <param name="isPrefabStation"></param>
            public BlueprintStructure_AuxData(bool? isPrefabStation = null)
            {
                SetAllBools(isPrefabStation);
            }

            public void SetAllBools(bool? value)
            {
                Invulnerable = value;
                SystemsOnline = value;
                PowerGeneratorsOnline = value;
                DoorsLocked = value;    // Current prefabs don't (usually) have locked doors.
                DockingPortsLocked = value;
                CryoPodsDisabled = value;
                DockingReleaseHandlesDisabled = value;
                TextLabelEditingDisabled = value;
                SecurityPanelsDisabled = value;
                SystemPartsInteractionDisabled = value;

            }

            public void ResetAuxData()
            {
                Invulnerable = null;
                SystemsOnline = null;
                PowerGeneratorsOnline = null;
                DoorsLocked = null;
                DockingPortsLocked = null;
                CryoPodsDisabled = null;
                DockingReleaseHandlesDisabled = null;
                TextLabelEditingDisabled = null;
                SecurityPanelsDisabled = null;
                SystemPartsInteractionDisabled = null;
            }
        }

    }

}
