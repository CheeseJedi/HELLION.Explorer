namespace HELLION.DataStructures.StaticData
{
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
        DerelictPort1, // This currently has no in-game representation and so is an abstract name.
        DerelictPort2, // This currently has no in-game representation and so is an abstract name.
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
