/// <summary>
/// Defines an enum of node types applicable to the HETreeNode class.
/// </summary>

namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines an enum of HETreeNode types
    /// </summary>
    public enum HETreeNodeType
    {
        Unknown = 0,        // Default for new nodes
        SolarSystemView,    // Node type for the root of the solar system view tree
        DataView,           // Node type for the root of the data view tree
        SearchResultsView,  // Node type for the root of the search results view tree, parent to all SearchHandlers
        SearchHandler,      // Node type for a SearchHandler (and the FindHandler)
        SearchResultsSet,   // Node type for a search results set, part of a SearchHandler
        Asteroid,           // Node type for Asteroids (loaded from save file, data usually loaded from Asteroids.json)
        Ship,               // Node type for Ships including modules (loaded from save file, data usually loaded from Structures.json)
        Player,             // Node type for player characters, probably includes corpses yet to de-spawn
        JsonArray,          // Node type for a json Array
        JsonObject,         // Node type for a json Object
        JsonProperty,       // Node type for a json Property
        JsonValue,          // Node type for a json Value
        SaveFile,           // Node type for the save file as represented in the node tree
        SaveFileError,      // Node type for the save file in error state as represented in the node tree
        DataFolder,         // Node type for the data folder
        DataFolderError,    // Node type for the data folder
        DataFile,           // Node type for a data file
        DataFileError,      // Node type for a data file
        Star,               // Node type for the star in the Solar System view (GUID of the star)
        Planet,             // Node type for a planet (parent GUID of the star)
        Moon,               // Node type for a moon (not the star, or orbiting it directly)
        BlueprintsView,     // Node type for the Blueprints view
        Blueprint,          // Node type for a Blueprint file
        BlueprintCollection,
        BlueprintHierarchyView, // Node type for the root of a Blueprint's Hierarchy View
        BlueprintDataView,
        BlueprintStructureDefinitionView,
        BlueprintStructure,
        BlueprintStructureDefinition,
        BlueprintDockingPort,
        BlueprintDockingPortDefinition,
    };
}
