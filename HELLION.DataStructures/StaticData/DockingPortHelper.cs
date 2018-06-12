using System.Collections.Generic;
using System.Linq;
using static HELLION.DataStructures.Blueprints.StationBlueprint;

namespace HELLION.DataStructures.StaticData
{
    /// <summary>
    /// A class that implements a nested dictionary structure detailing
    /// the docking ports for each structure type.
    /// </summary>
    public static class DockingPortHelper
    {
        /// <summary>
        /// Defines a two-layer dictionary - the upper dictionary contains structures and each contains
        /// a sub-dictionary of it's docking ports with the game's orderID as the key.
        /// </summary>
        public static Dictionary<StructureSceneID, Dictionary<int, DockingPortType>> DockingPortHints
            = new Dictionary<StructureSceneID, Dictionary<int, DockingPortType>>()
        {

        { StructureSceneID.AltCorp_CorridorModule, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.StandardDockingPortA },
            { 2, DockingPortType.StandardDockingPortB },
            { 3, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_CorridorIntersectionModule, new Dictionary<int, DockingPortType> {
            { 1, DockingPortType.StandardDockingPortA },
            { 3, DockingPortType.StandardDockingPortB },
            { 2, DockingPortType.StandardDockingPortC },
            { 4, DockingPortType.Anchor }
        } },

        { StructureSceneID.AltCorp_Corridor45TurnModule, new Dictionary<int, DockingPortType> {
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

        { StructureSceneID.AltCorp_Cargo_Module, new Dictionary<int, DockingPortType> {
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

        /// <summary>
        /// Looks up a port type (name) from the port dictionary from the specified sceneID and orderID.
        /// </summary>
        /// <param name="sceneID"></param>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static DockingPortType GetDockingPortType(StructureSceneID sceneID, int orderID)
        {
            // Attempt to locate the correct port Dictionary by sceneID.
            if (DockingPortHints.TryGetValue(sceneID, out Dictionary<int, DockingPortType> portDictionary))
            {
                // We should have a Dictionary object containing the structure's port OrderIDs and Names (aka types).
                if (portDictionary.TryGetValue(orderID, out DockingPortType portType))
                {
                    return portType;
                }
            }
            return DockingPortType.Unspecified;
        }

        /// <summary>
        /// Looks up the orderID from the port dictionary for the specified sceneID and portType.
        /// </summary>
        /// <param name="sceneID"></param>
        /// <param name="portType"></param>
        /// <returns></returns>
        public static int? GetOrderID(StructureSceneID sceneID, DockingPortType portType)
        {
            // Attempt to locate the correct port Dictionary by sceneID.
            if (DockingPortHints.TryGetValue(sceneID, out Dictionary<int, DockingPortType> portDictionary))
            {
                // We should have a Dictionary object containing the structure's port OrderIDs and Names (aka types).

                // Look up the keys of ports with matching name (type).
                IEnumerable<int> results = portDictionary.Keys.Where(k => portDictionary[k] == portType);

                // There should only be one result otherwise there's a duplicate name in the definitions.
                if (results.Count() < 1 || results.Count() > 1) return null;
                return results.Single();
            }
            return null;
        }

        /// <summary>
        /// Generates a list of BlueprintDockingPort objects appropriate to the specified sceneID (module type).
        /// </summary>
        /// <param name="sceneID"></param>
        /// <returns></returns>
        public static List<BlueprintDockingPort> GenerateBlueprintDockingPorts(StructureSceneID sceneID)
        {
            List<BlueprintDockingPort> results = new List<BlueprintDockingPort>();

            // Attempt to locate the correct port Dictionary by sceneID.
            if (DockingPortHints.TryGetValue(sceneID, out Dictionary<int, DockingPortType> portDictionary))
            {
                // We should have a Dictionary object containing the structure's port OrderIDs and Names (aka types).


                foreach (KeyValuePair<int, DockingPortType> item in portDictionary)
                {
                    // Create a new BlueprintDockingPort.
                    BlueprintDockingPort newPort = new BlueprintDockingPort()
                    {
                        PortName = item.Value,
                        OrderID = item.Key,
                    };
                    // Add it to the results list.
                    results.Add(newPort);
                }
            }

            return results;
        }

    }
}
