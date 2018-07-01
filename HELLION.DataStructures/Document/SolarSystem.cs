using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HELLION.DataStructures.UI;
using Newtonsoft.Json.Linq;

namespace HELLION.DataStructures.Document
{
    /// <summary>
    /// Implements the Solar System view node tree.
    /// </summary>
    public class SolarSystem : IParent_Base_TN
    {
        /// <summary>
        /// Constructor that takes an GameData object and uses this as it's data source.
        /// </summary>
        /// <param name="gameData"></param>
        public SolarSystem(GameData gameData)
        {
            // Basic constructor

            RootNode = new SolarSystem_TN(passedOwner: this, nodeName: "Solar System", nodeType: Base_TN_NodeType.SolarSystemView) //, "Solar System")
            {
                GUID = -1 // Hellion, the star, has a ParentGUID of -1, so we utilise this to attach it to the Solar System root node
            };

            if (gameData == null) throw new NullReferenceException("gameData was null.");
            else
            {
                // Store a reference to the GameData object
                GameData = gameData;

                // Create Solar System hierarchical structure from the Celestial Bodies
                BuildSolarSystem();
            }
        }

        /// <summary>
        /// Used to store a reference to the GameData object.
        /// </summary>
        public GameData GameData { get; protected set; } = null;

        /// <summary>
        /// Root node of the Solar System tree.
        /// </summary>
        public Base_TN RootNode { get; set; } = null;

        /// <summary>
        /// Builds tree nodes from the GameData nodes, with cross-references
        /// </summary>
        public void BuildSolarSystem()
        {
            // Basic operation
            // The following types of HESolarSystemTreeNodes to be created as child nodes of the
            // Solar System root node, then the hierarchy will be applied and nodes re-parented
            // to the appropriate place.

            // 1. Create Planets nodes from GameData - CelestialBodies.json 
            // Note that this only needs to be run once and with one of the celestial body
            // HETreeNodeTypes - Star, Planet or Moon.
            AddSolarSystemObjectsByType(Base_TN_NodeType.Star);

            // 2. Create Asteroid nodes from GameData - save file
            AddSolarSystemObjectsByType(Base_TN_NodeType.Asteroid);

            // 3. Create Ship nodes from GameData - save file
            AddSolarSystemObjectsByType(Base_TN_NodeType.Ship);

            // 4. Create Player nodes from GameData - save file
            AddSolarSystemObjectsByType(Base_TN_NodeType.Player);

            // 5. Rehydrate GUID structure - attach nodes as children of the parent node based on GUID structure
            RehydrateGUIDHierarchy();

            // 6. Rehydrate docked ship/module structure.
            RehydrateDockedShips();

        }

        /// <summary>
        /// Adds Solar System nodes of the specified type to the RootNode, generated from the 
        /// Game Data nodes.
        /// </summary>
        /// <param name="nodeType"></param>
        public void AddSolarSystemObjectsByType(Base_TN_NodeType nodeType)
        {
            switch (nodeType)
            {
                case Base_TN_NodeType.Star:
                case Base_TN_NodeType.Planet:
                case Base_TN_NodeType.Moon:
                    // These come from the Static Data - handled by the CelestialBodies.json member of the DataDictionary
                    if (!GameData.StaticData.DataDictionary.TryGetValue("CelestialBodies.json", out Json_File_UI celestialBodiesJsonBaseFile))
                        throw new InvalidOperationException("Unable to access the CelestialBodies.json from the Static Data Dictionary.");
                    else
                    {
                        if (!(celestialBodiesJsonBaseFile.RootNode.Nodes.Count > 0)) throw new InvalidOperationException("CelestialBodies RootNode had no child nodes.");

                        foreach (Json_TN node in celestialBodiesJsonBaseFile.RootNode.Nodes)
                        {
                            Base_TN_NodeType newNodeType = Base_TN_NodeType.Unknown;

                            JObject obj = (JObject)node.JData;

                            if (obj == null) throw new NullReferenceException("Adding CelestialBodies - obj was null.");

                            long newNodeParentGUID = 0;
                            JToken testToken = obj["ParentGUID"];
                            if (testToken != null)
                            {
                                newNodeParentGUID = (long)obj["ParentGUID"];
                            }

                            switch (newNodeParentGUID)
                            {
                                case 0:
                                    throw new Exception("Failed to read ParentGUID");
                                case -1:
                                    // It's the star, Hellion.
                                    newNodeType = Base_TN_NodeType.Star;
                                    break;
                                case 1:
                                    // It's a planet, orbiting the star.
                                    newNodeType = Base_TN_NodeType.Planet;
                                    break;
                                default:
                                    // It's a moon, not a space station!
                                    newNodeType = Base_TN_NodeType.Moon;
                                    break;
                            }

                            SolarSystem_TN newNode = node.CreateLinkedSolarSystemNode(newNodeType);
                            RootNode.Nodes.Insert(0, newNode);
                        }
                    }
                    break;
                case Base_TN_NodeType.Asteroid:
                case Base_TN_NodeType.Ship:
                case Base_TN_NodeType.Player:

                    // Set up the find key
                    string findKey = String.Empty;
                    switch (nodeType)
                    {
                        case Base_TN_NodeType.Asteroid:
                            findKey = "Asteroids";
                            break;
                        case Base_TN_NodeType.Ship:
                            findKey = "Ships";
                            break;
                        case Base_TN_NodeType.Player:
                            findKey = "Players";
                            break;
                    }
                    if (findKey == String.Empty) throw new Exception("findKey was empty.");

                    TreeNode[] tmpMatches = GameData.SaveFile.RootNode.Nodes.Find(findKey, searchAllChildren: false);

                    Json_TN sectionRootNode = null;
                    if (tmpMatches.Count() > 0)
                    {
                        sectionRootNode = (Json_TN)tmpMatches?[0];
                    }
                    //else Debug.Print("sectionRootNode was null.");

                    Json_TN arrayRootNode = null;
                    if (sectionRootNode?.Nodes.Count > 0)
                    {
                        arrayRootNode = (Json_TN)sectionRootNode?.Nodes[0];

                        foreach (Json_TN node in arrayRootNode.Nodes)
                        {
                            JObject obj = (JObject)node.JData;
                            long newNodeParentGUID = 0;
                            long newNodeFakeGUID = 0;
                            JToken testToken = obj["ParentGUID"];
                            if (testToken != null) newNodeParentGUID = (long)obj["ParentGUID"];

                            testToken = obj["FakeGUID"];
                            if (testToken != null) newNodeFakeGUID = (long)obj["FakeGUID"];

                            SolarSystem_TN newNode = node.CreateLinkedSolarSystemNode(nodeType);
                            if (nodeType == Base_TN_NodeType.Player)
                            {
                                if (newNodeParentGUID == newNodeFakeGUID)
                                {
                                    // Player is ALIVE and in space
                                    Debug.Print("FakeGUID: " + newNodeFakeGUID);
                                    Debug.Print("ParentGUID: " + newNodeParentGUID);

                                    newNode.ParentGUID = -1;
                                    // Needs to be greater than zero to place players below the star node.
                                    newNode.OrbitData.SemiMajorAxis = 1;
                                }
                                else
                                {
                                    // Player is dead, display below alive players.
                                    newNode.OrbitData.SemiMajorAxis = 10;
                                }
                            }

                            RootNode.Nodes.Insert(0, newNode);
                        }
                    }
                    else Debug.Print("subRootNode was null.");

                    break;
            }
        }

        /// <summary>
        /// Rehydrates (rebuilds) the node hierarchy based on GUID and ParentGUID.
        /// </summary>
        public bool RehydrateGUIDHierarchy()
        {
            SolarSystem_TN currentParentNode = null;
            bool errorState = false;
            foreach (SolarSystem_TN node in RootNode.GetChildNodes(includeSubtrees: true))
            {
                // If this node has a non-zero value for DockedToShipGUID, process it.
                if (node.ParentGUID == 0)
                {
                    // There should be only one case where the node processed has a value of zero that is valid and
                    // that's the Solar System node, which has no parent node - it's parent is the TreeViewControl.
                    if (node.GUID != -1) throw new InvalidOperationException(node.Text + " - Node's parent GUID is zero.");
                }
                else
                {
                    // Find the single node that has the GUID matching the DockedToShipGUID of this node.
                    // There can be only one!
                    try
                    {
                        SolarSystem_TN newParentNode = RootNode.GetChildNodes(includeSubtrees: true)
                            .Cast<SolarSystem_TN>()
                            .Where(p => p.GUID == node.ParentGUID)
                            .Single();
                        // If the .Single() causes an exception, there's more than one module docked to that port, 
                        // or the GUID that it's docked to can't be found :(

                        // Cast the node.Parent to an SolarSystem_TN (so we can access ClearCachedData)
                        currentParentNode = (SolarSystem_TN)node.Parent;

                        // Remove the ship to be re-parented from it's current parent's node collection.
                        // FIXME - the following line may be why the try catch is here.
                        currentParentNode.Nodes.Remove(node);

                        // Add the ship being re-parented to the new parent's node collection.
                        newParentNode.Nodes.Insert(0, node);

                    }
                    catch (Exception e)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Exception in RehydrateGUIDHierarchy" + Environment.NewLine);
                        sb.Append("Text: " + node.Text + Environment.NewLine);
                        sb.Append("GUID: " + node.GUID + Environment.NewLine);
                        sb.Append("ParentGUID: " + node.ParentGUID + Environment.NewLine);
                        sb.Append(e.ToString());
                        Debug.Print(sb.ToString());
                        errorState = true;
                    }
                }
            }
            return errorState;
        }

        /// <summary>
        /// Re-arranges (rehydrates) existing ship nodes by their DockedToShipGUID forming a tree where the
        /// root node is the parent vessel of the docked ships (and is what shows up on radar in-game).
        /// </summary>
        /// <remarks>
        /// Although this particular function is non-recursive, recursive calls are made when calling
        /// the HETreeNode.GetAllNodes() to get sub-nodes.
        /// </remarks>
        public void RehydrateDockedShips()
        {
            IEnumerable<SolarSystem_TN> shipsToBeReparented = RootNode.GetChildNodes(includeSubtrees: true)
                .Cast<SolarSystem_TN>()
                .Where(p => (p.NodeType == Base_TN_NodeType.Ship) && (p.DockedToShipGUID > 0));

            foreach (SolarSystem_TN node in shipsToBeReparented)
            {
                // If this node has a non-zero value for DockedToShipGUID, process it.
                if (node.DockedToShipGUID != 0)
                {
                    // Find the node that has the GUID matching the DockedToShipGUID of this node.
                    // There can be only one!
                    SolarSystem_TN newParentNode = RootNode.GetChildNodes(includeSubtrees: true)
                        .Cast<SolarSystem_TN>()
                        .Where(p => p.GUID == node.DockedToShipGUID)
                        .Single(); 
                    // If the .Single() causes an exception, there's not exactly one module docked to that port!

                    // Remove the ship to be re-parented from it's current parent's node collection.
                    node.Parent.Nodes.Remove(node);
                    // Add the ship being re-parented to the new parent's node collection.
                    newParentNode.Nodes.Insert(0, node);
                }
                else throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Handles closing of the Solar System object. Sets RootNode to null.
        /// </summary>
        public void Close()
        {
            RootNode = null;
        }

    }
}
