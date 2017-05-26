/* HETreeNodeType.cs
 * CheeseJedi 2017
 * Defines an enum used to determine node type within the NavTree
 */


namespace HELLION.DataStructures
{
    public enum HETreeNodeType
    {
        Unknown = 0,        // Default for new nodes
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
        DefDynamicObject    // Node type for a definition of a dynamic object
    };
} // End of namespace HELLION
