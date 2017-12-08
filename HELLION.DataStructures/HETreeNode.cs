using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

/// <summary>
/// Defines a derived HETreeNode to handle objects in the Game Data view.
/// Also defines an enum of node types applicable to the HETreeNode class.
/// </summary>

namespace HELLION.DataStructures
{
    /// <summary>
    /// Derives a custom TreeNode class to hold some additional parameters used to speed up searches
    /// </summary>
    public class HETreeNode : TreeNode
    {
        private HETreeNodeType nodeType = HETreeNodeType.Unknown;
        private int countOfChildNodes = -1;
        private int countOfAllChildNodes = -1;
        private List<HETreeNode> listOfChildNodes = null;
        private List<HETreeNode> listOfAllChildNodes = null;

        /// <summary>
        /// Constructor that takes a minimum of a name, but also optionally a type and text (display name).
        /// </summary>
        /// <param name="nodeName">Name of the new node.</param>
        /// <param name="nodeType">Type of the new node (HETreeNodeType enum)</param>
        /// <param name="nodeText">Text of the new node (Display Name). If not specified this defaults to the node's name.</param>
        /// <param name="nodeToolTipText">Tool tip text of the new node. If not specified this defaults to the node's text.</param>
        public HETreeNode(string nodeName, HETreeNodeType nodeType = HETreeNodeType.Unknown, string nodeText = "", string nodeToolTipText = "")
        {
            if (nodeName != null && nodeName != "") Name = nodeName;
            else Name = "node " + DateTime.Now.ToString();

            if (nodeText == "") Text = nodeName;
            else Text = nodeText;

            if (nodeToolTipText == "") ToolTipText = nodeName + "(" + nodeType.ToString() + ")";
            else ToolTipText = nodeToolTipText;

            NodeType = nodeType;
        }

        /// <summary>
        /// Gets/Sets the node type. On a Set operation it triggers the ImageIndex and SelectedImageIndex
        /// to refresh based 
        /// </summary>
        public HETreeNodeType NodeType
        {
            get { return nodeType; }
            set
            {
                // Only update the nodeType if it is different.
                if (value != nodeType)
                {
                    nodeType = value;
                    ImageIndex = HEUtilities.GetImageIndexByNodeType(nodeType);
                    SelectedImageIndex = ImageIndex;
                }
            }
        }
        
        /// <summary>
        /// Builds and returns a count of direct descendent nodes.
        /// </summary>
        public int CountOfChildNodes
        {
            get
            {
                if (countOfChildNodes == -1) UpdateCounts();
                return countOfChildNodes;
            }
        }
        
        /// <summary>
        /// Builds and returns a could of all descendent nodes.
        /// </summary>
        public int CountOfAllChildNodes
        {
            get
            {
                if (countOfAllChildNodes == -1) UpdateCounts();
                return countOfAllChildNodes;
            }
        }

        /// <summary>
        /// Returns a list of direct descendants
        /// </summary>
        /// <returns>List<HETreeNode></HETreeNode></returns>
        public List<HETreeNode> ListOfChildNodes
        {
            get
            {
                if (listOfChildNodes == null)
                {
                    listOfChildNodes = new List<HETreeNode>();
                    listOfChildNodes.Add(this);
                    foreach (HETreeNode child in Nodes)
                    {
                        listOfChildNodes.Add(child);
                    }
                }
                return listOfChildNodes;
            }
        }

        /// <summary>
        /// Returns a list of all child nodes via a seperate recursive function
        /// </summary>
        /// <returns></returns>
        public List<HETreeNode> ListOfAllChildNodes
        {
            get
            {
                if (listOfAllChildNodes == null)
                {
                    listOfAllChildNodes = GetAllNodes();
                }
                return listOfAllChildNodes;
            }
        }

        /// <summary>
        /// Updates the counts of subnodes for this child (recursive).
        /// </summary>
        public void UpdateCounts()
        {
            countOfChildNodes = GetNodeCount(includeSubTrees: false);
            countOfAllChildNodes = GetNodeCount(includeSubTrees: true);

            if (CountOfChildNodes > 0)
            {
                // This node has child nodes, recursively process them
                foreach (HETreeNode nChildNode in Nodes)
                {
                    nChildNode.UpdateCounts();
                }
            }
        }

        /// <summary>
        /// Used to reset the counts and lists back to uninitialised state.
        /// This is intended to be called by a newly-added child node upon adding to this
        /// node's Nodes TreeNodeCollection and will ensure the regeneration of this
        /// data next time it's needed.
        /// </summary>
        public void ClearCachedData()
        {
            countOfChildNodes = -1;
            countOfAllChildNodes = -1;
            listOfChildNodes = null;
            listOfAllChildNodes = null;
        }

        /// <summary>
        /// Returns a List(HETreeNode) of nodes containing this node plus all descendents.
        /// </summary>
        /// <returns></returns>
        private List<HETreeNode> GetAllNodes()
        {
            List<HETreeNode> result = new List<HETreeNode>();
            result.Add(this);
            foreach (HETreeNode child in Nodes)
            {
                result.AddRange(child.GetAllNodes());
            }
            return result;
        }

        /*

        private List<HETreeNode> GetDirectNodes()
        {
            List<HETreeNode> result = new List<HETreeNode>();
            result.Add(this);
            foreach (HETreeNode child in Nodes)
            {
                result.Add(child);
            }
            return result;
        }
        */

    } // End of class HETreeNode

    /// <summary>
    /// Defines an enum of HETreeNode types
    /// </summary>
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
        SolSysStar,         // Node type for the star in the Solar System view (guid of the star)
        SolSysPlanet,       // Node type for a planet (parent guid of the star)
        SolSysMoon,         // Node type for a moon (not the star, or orbiting it directly)
    }; // End of enum HETreeNodeType

} // End of namespace HELLION.DataStructures
