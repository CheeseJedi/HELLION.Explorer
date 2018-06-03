using System.Collections.Generic;
using HELLION.DataStructures.StaticData;

namespace HELLION.DataStructures.StaticData
{
    public static class DockingPortHelper
    {
        /// <summary>
        /// Defines a dictionary of modules and contains a sub-dictionary of it's docking ports.
        /// </summary>
        public static Dictionary<StructureSceneID, Dictionary<int, DockingPortType>> DockingPortHints
            = new Dictionary<StructureSceneID, Dictionary<int, DockingPortType>>()
        {

        { StructureSceneID.AltCorp_CorridorModule, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.StandardDockingPortA },
            { 2, DockingPortType.StandardDockingPortB },
            { 3, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_CorridorIntersectionModule,  new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.StandardDockingPortA },
            { 3, DockingPortType.StandardDockingPortB },
            { 2, DockingPortType.StandardDockingPortC },
            { 4, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_Corridor45TurnModule,  new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.StandardDockingPortA },
            { 2, DockingPortType.StandardDockingPortB },
            { 3, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_Shuttle_SARA, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.AirlockDockingPort },
            { 2, DockingPortType.Anchor }
        } },

        { StructureSceneID.ALtCorp_PowerSupply_Module, new Dictionary<int, DockingPortType> {
            { 2, DockingPortType.StandardDockingPortA },
            { 1, DockingPortType.StandardDockingPortB },
            { 3, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_LifeSupportModule, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.StandardDockingPortA },
            { 2, DockingPortType.StandardDockingPortB },
            { 3, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_Cargo_Module,  new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.StandardDockingPortA },
            { 4, DockingPortType.IndustrialContainerPortA },
            { 5, DockingPortType.IndustrialContainerPortB },
            { 7, DockingPortType.IndustrialContainerPortC },
            { 6, DockingPortType.IndustrialContainerPortD },
            { 2, DockingPortType.CargoDockingPortA },
            { 3, DockingPortType.CargoDockingPortB },
            { 8, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_CorridorVertical, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.StandardDockingPortA },
            { 2, DockingPortType.StandardDockingPortB },
            { 3, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_Command_Module, new Dictionary<int, DockingPortType> {
            { 3, DockingPortType.StandardDockingPortA },
            { 2, DockingPortType.StandardDockingPortB },
            { 1, DockingPortType.StandardDockingPortC },
            { 4, DockingPortType.StandardDockingPortD },
            { 5, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_Corridor45TurnRightModule, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.StandardDockingPortA },
            { 2, DockingPortType.StandardDockingPortB },
            { 3, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_StartingModule, new Dictionary<int, DockingPortType> {
            { 2, DockingPortType.StandardDockingPortA },
            { 1, DockingPortType.StandardDockingPortB },
            { 3, DockingPortType.Anchor }
        } },

        { StructureSceneID.Generic_Debris_JuncRoom001, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.DerelictPort1 },
            { 2, DockingPortType.DerelictPort2 }
        } },

        { StructureSceneID.Generic_Debris_JuncRoom002, new Dictionary<int, DockingPortType> { } },

        { StructureSceneID.Generic_Debris_Corridor001, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.DerelictPort1 }
        } },

        { StructureSceneID.Generic_Debris_Corridor002, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.DerelictPort1 },
            { 2, DockingPortType.DerelictPort2 }
        } },

        { StructureSceneID.AltCorp_AirLock, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.AirlockDockingPort },
            { 2, DockingPortType.StandardDockingPortA },
            { 3, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_DockableContainer, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.StandardDockingPortA },
            { 2, DockingPortType.Anchor }
        } },

        { StructureSceneID.Generic_Debris_Outpost001, new Dictionary<int, DockingPortType> { } },

        { StructureSceneID.AltCorp_CrewQuarters_Module, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.StandardDockingPortA },
            { 2, DockingPortType.Anchor }
        } },

        { StructureSceneID.Generic_Debris_Spawn1, new Dictionary<int, DockingPortType> { } },

        { StructureSceneID.Generic_Debris_Spawn2, new Dictionary<int, DockingPortType> { } },

        { StructureSceneID.Generic_Debris_Spawn3, new Dictionary<int, DockingPortType> { } },

        { StructureSceneID.AltCorp_SolarPowerModule, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.StandardDockingPortA },
            { 2, DockingPortType.StandardDockingPortB },
            { 3, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_Shuttle_CECA, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_FabricatorModule, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.StandardDockingPortA },
            { 2, DockingPortType.Anchor }
        } },

        { StructureSceneID.MataPrefabs, new Dictionary<int, DockingPortType> { } },

        { StructureSceneID.FlatShipTest, new Dictionary<int, DockingPortType> { } },

        };

        public static DockingPortType GetDockingPortType(StructureSceneID sceneID, int orderID)
        {
            // Attempt to locate the correct port Dictionary by sceneID.
            if (DockingPortHints.TryGetValue(sceneID, out Dictionary<int, DockingPortType> portDictionary))
            {
                // We should have a Dictionary object containing the structure's port OrderIDs and Names.
                if (portDictionary.TryGetValue(orderID, out DockingPortType portType))
                {
                    return portType;
                }
            }
            return DockingPortType.Unspecified;
        }

        /// <summary>
        /// Docking Port Types Enum.
        /// </summary>
        public enum DockingPortType
        {
            Unspecified = 0,
            StandardDockingPortA,
            StandardDockingPortB,
            StandardDockingPortC,
            StandardDockingPortD,
            AirlockDockingPort,
            Grapple,    // LEGACY ITEM - DEPRECATED
            IndustrialContainerPortA,
            IndustrialContainerPortB,
            IndustrialContainerPortC,
            IndustrialContainerPortD,
            CargoDockingPortA,
            CargoDockingPortB,
            CargoDock,  // Dockable Cargo (IC) module
            Anchor,
            DerelictPort1,
            DerelictPort2,
        }

    /*
    /// <summary>
    /// LEGACY Structure Types Enum.
    /// </summary>
    /// <remarks>
    /// The numeric values of these correspond to the game's ItemID in the structures.json
    /// to allow for easier cross-referencing.
    /// </remarks>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum aHEBlueprintStructureType
    {
        Unspecified = 0,
        //BRONTES = 2,
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
        Generic_Debris_JuncRoom001 = 15, // 0x0000000F
        Generic_Debris_JuncRoom002 = 16, // 0x00000010
        Generic_Debris_Corridor001 = 17, // 0x00000011
        Generic_Debris_Corridor002 = 18, // 0x00000012
        IC = 19, // 0x00000013
        //MataPrefabs = 20, // 0x00000014
        //Generic_Debris_Outpost001 = 21, // 0x00000015
        CQM = 22, // 0x00000016
        // Generic_Debris_Spawn1 = 23, // 0x00000017
        // Generic_Debris_Spawn2 = 24, // 0x00000018
        // Generic_Debris_Spawn3 = 25, // 0x00000019
        SPM = 26, // 0x0000001A
        STEROPES = 27, // 0x0000001B
        FM = 28, // 0x0000001C
        // FlatShipTest = 29, // 0x0000001D
    }
    */

    }
}
