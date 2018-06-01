namespace HELLION.DataStructures.UI
{
    /// <summary>
    /// Defines an enum of node types applicable to the Base_TN class.
    /// </summary>
    public enum Base_TN_NodeType
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
        JsonObject,         // Node type for a json Object
        JsonArray,          // Node type for a json Array
        JsonProperty,       // Node type for a json Property
        JsonValue,          // Node type for a json Value
        JsonBoolean,        // Node type for a json value type of Boolean
        JsonBytes,          // Node type for a json value type of Bytes
        JsonComment,        // Node type for a json value type of Comment
        JsonDate,           // Node type for a json value type of Date
        JsonFloat,          // Node type for a json value type of Float
        JsonGuid,           // Node type for a json value type of Guid
        JsonInteger,        // Node type for a json value type of Integer
        JsonString,         // Node type for a json value type of String
        JsonTimeSpan,       // Node type for a json value type of TimeSpan
        JsonUri,            // Node type for a json value type of Uri
        JsonNull,           // Node type for a json value type of Null
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
        BlueprintRootStructure,
        BlueprintStructureDefinition,
        BlueprintDockingPort,
        BlueprintDockingPortDefinition,
    };
}
