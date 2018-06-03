using System.ComponentModel;

namespace HELLION.DataStructures.StaticData
{
    /// <summary>
    /// The primary Enum for referencing the structures and their IDs and Descriptions.
    /// </summary>
    public enum StructureSceneID
    {
        //SolarSystemSetup = -3,
        //ItemScene = -2,
        //None = -1,

        Unspecified = 0,

        //Slavica = 1,
        //[Description("BRO")] // AltCorp Brontes?
        //AltCorp_Ship_Tamara = 2,

        [Description("CIM")]
        AltCorp_CorridorModule = 3,
        [Description("CTM")]
        AltCorp_CorridorIntersectionModule = 4,
        [Description("CLM")]
        AltCorp_Corridor45TurnModule = 5,

        [Description("ARG")]
        AltCorp_Shuttle_SARA = 6, // AltCorp Arges (aka Mule)
        [Description("PSM")]
        ALtCorp_PowerSupply_Module = 7,
        [Description("LSM")]
        AltCorp_LifeSupportModule = 8,
        [Description("CBM")]
        AltCorp_Cargo_Module = 9,

        [Description("CSM")]
        AltCorp_CorridorVertical = 10, // 0x0000000A
        [Description("CM")]
        AltCorp_Command_Module = 11, // 0x0000000B
        [Description("CRM")]
        AltCorp_Corridor45TurnRightModule = 12, // 0x0000000C
        [Description("OUTPOST")]
        AltCorp_StartingModule = 13, // 0x0000000D
        [Description("AM")]
        AltCorp_AirLock = 14, // 0x0000000E

        Generic_Debris_JuncRoom001 = 15, // 0x0000000F
        Generic_Debris_JuncRoom002 = 16, // 0x00000010
        Generic_Debris_Corridor001 = 17, // 0x00000011
        Generic_Debris_Corridor002 = 18, // 0x00000012

        [Description("IC")]
        AltCorp_DockableContainer = 19, // 0x00000013

        MataPrefabs = 20, // 0x00000014
        Generic_Debris_Outpost001 = 21, // 0x00000015

        [Description("CQM")]
        AltCorp_CrewQuarters_Module = 22, // 0x00000016

        Generic_Debris_Spawn1 = 23, // 0x00000017
        Generic_Debris_Spawn2 = 24, // 0x00000018
        Generic_Debris_Spawn3 = 25, // 0x00000019

        [Description("SPM")]
        AltCorp_SolarPowerModule = 26, // 0x0000001A
        [Description("STE")]
        AltCorp_Shuttle_CECA = 27, // 0x0000001B // AltCorp Steropes.
        [Description("FM")]
        AltCorp_FabricatorModule = 28, // 0x0000001C
        FlatShipTest = 29, // 0x0000001D
                           // ActaeonProbe = 30, // 0x0000001E

        //Asteroid01 = 1000, // 0x000003E8
        //Asteroid02 = 1001, // 0x000003E9
        //Asteroid03 = 1002, // 0x000003EA
        //Asteroid04 = 1003, // 0x000003EB
        //Asteroid05 = 1004, // 0x000003EC
        //Asteroid06 = 1005, // 0x000003ED
        //Asteroid07 = 1006, // 0x000003EE
        //Asteroid08 = 1007, // 0x000003EF
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
