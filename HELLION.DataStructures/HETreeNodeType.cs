/* HETreeNodeType.cs
 * CheeseJedi 2017
 * Defines an enum used to determine node type within the NavTree
 */


namespace HELLION.DataStructures
{
    public enum HETreeNodeType
    {
        Unknown = 0,        // Default for new nodes
        SolarSystemView,    // Node type for the root of the solar system view tree
        DataView,           // Node type for the root of the data view tree
        SearchResultsView,  // Nore type for the root of the search results view tree
        SystemNAV,          // Node type for a system navigation tree item
        Scene,              // Node type for a Scene item
        CelestialBody,      // Node type for Celestial Bodies (data usually loaded from CelestialBodies.json)
        Asteroid,           // Node type for Ateroids (loaded from save file, data usually loaded from Asteroids.json)
        Ship,               // Node type for Ships including modules (loaded from save file, data usually loaded from Structures.json)
        Player,             // Node type for player characters, probably includes corpses yet to despawn
        DynamicObject,      // Node type for Dynamic Objects (loaded from save file, data usually loaded from DynamicObjects.json)
        DefCelestialBody,   // Node type for a defintion of a celestial body
        DefAsteroid,        // Node type for a definition of an asteroid
        DefStructure,       // Node type for a definition of a structure (ship/module)
        DefDynamicObject,   // Node type for a definition of a dynamic object
        RespawnObject,      // Node type for a respawn object - these seem to be deprecated now
        SpawnPoint,         // Node type for a spawn point
        ArenaController,    // Node type for an arena controller - these also seem to be deprecated now
        DoomControllerData, // Node type for the doomed station controller data
        SpawnManagerData,   // Node type for the SpawnManager data
        ExpansionAvailable, // Node type used temporarily to indicate that an item can be evaluated further on-demand, replaced by real data
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
        SolSysStar,  // Node type for the star in the Solar System view (guid of the star)
        SolSysPlanet,// Node type for a planet (parent guid of the star)
        SolSysMoon,  // Node type for a moon (not the star, or orbiting it directly)
    };
} // End of namespace HELLION
