namespace HELLION.DataStructures.StaticData
{
    /// <summary>
    /// Docking Port Types Enumeration.
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

}
