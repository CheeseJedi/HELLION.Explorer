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
        CelestialBody,      // Node type for Celestial Bodies (data usually loaded from CelestialBodies.json)
        Asteroid,           // Node type for Ateroids (loaded from save file, data usually loaded from Asteroids.json)
        Ship,               // Node type for Ships including modules (loaded from save file, data usually loaded from Structures.json)
        Player,             // Node type for player characters, probably includes corpses yet to despawn
        DynamicObject       // Node type for Dynamic Objects (loaded from save file, data usually loaded from DynamicObjects.json)
    };
} // End of namespace HELLION
