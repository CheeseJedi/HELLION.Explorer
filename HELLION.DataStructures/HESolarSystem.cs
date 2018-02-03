using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Windows.Forms;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Implements the Solar System view node tree.
    /// </summary>
    public class HESolarSystem
    {
        /// <summary>
        /// Root node of the Solar System tree.
        /// </summary>
        public HESolarSystemTreeNode RootNode { get; set; } = null;

        /// <summary>
        /// Used to store a reference to the GameData object.
        /// </summary>
        public HEGameData GameData { get; set; } = null;

        /// <summary>
        /// A list of all nodes in the Solar System, except the solar system root node.
        /// </summary>
        public List<HESolarSystemTreeNode> SolarSystemNodes { get; private set; } = null;

        /// <summary>
        /// Constructor that takes an HEGameData object and uses this as it's data source.
        /// </summary>
        /// <param name="gameData"></param>
        public HESolarSystem(HEGameData gameData)
        {
            // Basic constructor

            RootNode = new HESolarSystemTreeNode("SOLARSYSTEMVIEW", HETreeNodeType.SolarSystemView, "Solar System")
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
        /// Handles closing of the Solar System object. Sets RootNode to null.
        /// </summary>
        public void Close()
        {
            RootNode = null;
        }
        
        /// <summary>
        /// Builds tree nodes from the GameData nodes, with cross-references
        /// </summary>
        public void BuildSolarSystem()
        {
            // Basic operation
            //
            // The following types of HESolarSystemTreeNodes to be created as child nodes of the
            // Solar System root node, then the hierarchy will be applied and nodes re-parented
            // to the appropriate place.

            // 1. Create Planets nodes from GameData - CelestialBodies.json 
            // Note that this only needs to be run once and with one of the celestial body
            // HETreeNodeTypes - Star, Planet or Moon.
            AddSolarSystemObjectsByType(HETreeNodeType.Star);

            // 2. Create Asteroid nodes from GameData - save file
            AddSolarSystemObjectsByType(HETreeNodeType.Asteroid);

            // 3. Create Ship nodes from GameData - save file
            AddSolarSystemObjectsByType(HETreeNodeType.Ship);

            // 4. Create Player nodes from GameData - save file
            AddSolarSystemObjectsByType(HETreeNodeType.Player);

            // 5. Rehydrate GUID structure - attach nodes as children of the parent node based on GUID structure
            RehydrateGUIDHierarchy();

            // 6. Rehydrate docked ship/module structure.
            RehydrateDockedShips();

            // 7. Trigger the root node to recursively update the node counts.
            // RootNode.UpdateCounts();

        }

        /// <summary>
        /// Adds Solar System nodes of the specified type to the RootNode, generated from the 
        /// Game Data nodes.
        /// </summary>
        /// <param name="nodeType"></param>
        public void AddSolarSystemObjectsByType(HETreeNodeType nodeType)
        {
            switch (nodeType)
            {
                case HETreeNodeType.Star:
                case HETreeNodeType.Planet:
                case HETreeNodeType.Moon:
                    // These come from the Static Data - handled by the CelestialBodies.json member of the DataDictionary
                    if (GameData.StaticData.DataDictionary.TryGetValue("CelestialBodies.json", out HEJsonBaseFile celestialBodiesJsonBaseFile))
                    {
                        foreach (HEGameDataTreeNode node in celestialBodiesJsonBaseFile.RootNode.Nodes)
                        {
                            HETreeNodeType newNodeType = HETreeNodeType.Unknown;

                            JObject obj = (JObject)node.Tag;
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
                                    newNodeType = HETreeNodeType.Star;
                                    break;
                                case 1:
                                    // It's a planet, orbiting the star.
                                    newNodeType = HETreeNodeType.Planet;
                                    break;
                                default:
                                    // It's a moon, not a space station!
                                    newNodeType = HETreeNodeType.Moon;
                                    break;
                            }

                            HESolarSystemTreeNode newNode = node.CreateLinkedSolarSystemNode(newNodeType);
                            RootNode.Nodes.Add(newNode);
                        }
                    }


                    break;
                //
                case HETreeNodeType.Asteroid:
                case HETreeNodeType.Ship:
                case HETreeNodeType.Player:

                    // Set up the find key
                    string findKey = "";
                    switch (nodeType)
                    {
                        case HETreeNodeType.Asteroid:
                            findKey = "Asteroids";
                            break;
                        case HETreeNodeType.Ship:
                            findKey = "Ships";
                            break;
                        case HETreeNodeType.Player:
                            findKey = "Players";
                            break;
                    }
                    if (findKey == "") throw new Exception("findKey was empty.");

                    TreeNode[] tmpMatches = GameData.SaveFile.RootNode.Nodes.Find(findKey, searchAllChildren: false);

                    HEGameDataTreeNode sectionRootNode = null;
                    HEGameDataTreeNode arrayRootNode = null;

                    foreach (var match in tmpMatches)
                    {

                        sectionRootNode = (HEGameDataTreeNode)match;
                        break;
                    }
                    if (sectionRootNode == null) throw new NullReferenceException("sectionRootNode was null.");

                    foreach (var match2 in sectionRootNode.Nodes)
                    {
                        arrayRootNode = (HEGameDataTreeNode)match2;
                        break;
                    }
                    if (arrayRootNode == null) throw new NullReferenceException("subRootNode was null.");

                    foreach (HEGameDataTreeNode node in arrayRootNode.Nodes)
                    {
                        //HETreeNodeType newNodeType = HETreeNodeType.Asteroid;

                        JObject obj = (JObject)node.Tag;
                        long newNodeParentGUID = 0;
                        JToken testToken = obj["ParentGUID"];
                        if (testToken != null)
                        {
                            newNodeParentGUID = (long)obj["ParentGUID"];
                        }

                        HESolarSystemTreeNode newNode = node.CreateLinkedSolarSystemNode(nodeType);
                        RootNode.Nodes.Add(newNode);
                    }




                    break;
            }

        }

        /// <summary>
        /// Rehydrates (rebuilds) the node hierarchy based on GUID and ParentGUID.
        /// </summary>
        public void RehydrateGUIDHierarchy()
        {
            HESolarSystemTreeNode currentParentNode = null;

            foreach (HESolarSystemTreeNode node in RootNode.ListOfAllChildNodes)
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
                    HESolarSystemTreeNode newParentNode = RootNode.ListOfAllChildNodes
                        .Cast<HESolarSystemTreeNode>()
                        .Where(p => p.GUID == node.ParentGUID)
                        .Single();
                    // If the .Single() causes an exception, there's more than one module docked to that port :(

                    // Cast the node.Parent to an HESolarSystemTreeNode (so we can access ClearCachedData)
                    currentParentNode = (HESolarSystemTreeNode)node.Parent;

                    // Remove the ship to be re-parented from it's current parent's node collection.
                    currentParentNode.Nodes.Remove(node);

                    // Add the ship being re-parented to the new parent's node collection.
                    newParentNode.Nodes.Add(node);

                    // As both parent's node collections have changed, clear their cache to force regeneration.
                    currentParentNode.ClearCachedData();
                    newParentNode.ClearCachedData();
                }
            }
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
            IEnumerable<HESolarSystemTreeNode> shipsToBeReparented = RootNode.ListOfAllChildNodes
                .Cast<HESolarSystemTreeNode>()
                .Where(p => (p.NodeType == HETreeNodeType.Ship) && (p.DockedToShipGUID > 0));

            foreach (HESolarSystemTreeNode node in shipsToBeReparented)
            {
                // If this node has a non-zero value for DockedToShipGUID, process it.
                if (node.DockedToShipGUID != 0)
                {
                    // Find the node that has the GUID matching the DockedToShipGUID of this node.
                    // There can be only one!
                    HESolarSystemTreeNode newParentNode = RootNode.ListOfAllChildNodes
                        .Cast<HESolarSystemTreeNode>()
                        .Where(p => p.GUID == node.DockedToShipGUID)
                        .Single(); 
                    // If the .Single() causes an exception, there's more than one module docked to that port!

                    // Remove the ship to be re-parented from it's current parent's node collection.
                    node.Parent.Nodes.Remove(node);
                    // Add the ship being re-parented to the new parent's node collection.
                    newParentNode.Nodes.Add(node);
                    // As the new parent's node collection has changed, clear it's cache to force regeneration.
                    newParentNode.ClearCachedData();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        
    }
}
