using System;
using System.Text;
using Newtonsoft.Json.Linq;

/// <summary>
/// Defines a derived HETreeNode to handle objects in the Solar System view.
/// Also defines a node sorter that sorts by Semi-Major axis instead of by name which is
/// the default on a TreeView control, and a class to hold the orbital data.
/// </summary>
namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines a sub-derived TreeNode class to hold some additional parameters used to speed up working
    /// with the nodes by storing frequently used values.
    /// </summary>
    public class HESolarSystemTreeNode : HETreeNode
    {
        /// <summary>
        /// Constructor that takes a minimum of a name, but also optionally a type and text (display name) - inherits the base constructor
        /// </summary>
        /// <param name="nodeName">Name for the new node; required;</param>
        /// <param name="nodeType">HETreeNodeType of the new node; defaults to Unknown.</param>
        /// <param name="nodeText">The Text (DisplayName) of the node - uses the Name if omitted.</param>
        /// <param name="nodeToolTipText">The ToolTip text displayed; defaults to the nodeText if omitted.</param>
        public HESolarSystemTreeNode(object passedOwner, string nodeName = null,
            HETreeNodeType nodeType = HETreeNodeType.Unknown, string nodeText = "", string nodeToolTipText = "")
            : base(passedOwner, nodeName, nodeType, nodeText, nodeToolTipText)
        {
            OrbitData = new HEOrbitalData();
        }

        /// <summary>
        /// Constructor that takes a link to an HEGameDataTreeNode and an HETreeNodeType.
        /// </summary>
        /// <remarks>
        /// Uses the HEGameDataTreeNode's Tag field, cast to a JObject, to generate name, GUID
        /// and other properties.
        /// The Inherits the base() constructor and sets properties directly.
        /// </remarks>
        /// <param name="gameDataNodeToLink"></param>
        /// <param name="nodeType"></param>
        public HESolarSystemTreeNode(HEGameDataTreeNode gameDataNodeToLink, HETreeNodeType nodeType) : base(gameDataNodeToLink)
        {
            // Check the gameDataNodeToLink isn't null.
            // if (gameDataNodeToLink == null) throw new NullReferenceException("gameDataNodeToLink was null.");

            // Set up this end of the cross-link.
            LinkedGameDataNode = gameDataNodeToLink ?? throw new NullReferenceException("gameDataNodeToLink was null.");

            // Cast the LinkedGameDataNode.JData field in to a JObject.
            JObject linkedGameDataJson = (JObject)LinkedGameDataNode.JData;
            if (linkedGameDataJson == null) throw new NullReferenceException("linkedGameDataJson was null.");

            // Set the node type, this will trigger the icon type to change to an appropriate one.
            NodeType = nodeType;

            // If we're working with a blueprint node, translate the StructureID to GUID

            JToken tmpTkn = linkedGameDataJson["StructureID"];
            if (tmpTkn != null)
            {
                // Set the node's Name to the StructureID of the object.
                Name = (string)linkedGameDataJson["StructureID"];

                // Set the GUID to the StructureID
                GUID = (long)linkedGameDataJson["StructureID"];
            }
            else
            {
                // Set the node's Name to the GUID of the object.
                Name = (string)linkedGameDataJson["GUID"];

                // Set the GUID
                GUID = (long)linkedGameDataJson["GUID"];
            }

            // Generate a display name.
            Text = LinkedGameDataNode.GenerateDisplayName(linkedGameDataJson);

            JToken tempToken = null;
            switch (nodeType)
            {
                case HETreeNodeType.Star:
                case HETreeNodeType.Planet:
                case HETreeNodeType.Moon:
                    // It's a Celestial Body - the orbital data is not in an OrbitalData sub-object, but directly
                    // as properties of this JObject.

                    OrbitData = new HEOrbitalData(linkedGameDataJson);

                    break;
                case HETreeNodeType.Player:
                    // It's a player - doesn't have orbital data but may have a parent GUID if in a ship/module.

                    // Ensure OrbitData is null.
                    OrbitData = new HEOrbitalData();

                    tempToken = linkedGameDataJson["ParentGUID"];
                    if (tempToken != null) OrbitData.ParentGUID = (long)linkedGameDataJson["ParentGUID"];
                    else
                    {
                        // Set the parent GUID to -1 (the Solar System) as we currently can't place players who
                        // are outside ships or modules.
                        OrbitData.ParentGUID = -1;
                    }

                    break;
                case HETreeNodeType.Ship:
                    // Ships also need to set the docking info if present.

                    OrbitData = new HEOrbitalData((JObject)linkedGameDataJson["OrbitData"]);

                    tempToken = linkedGameDataJson["DockedToShipGUID"];
                    if (tempToken != null)
                    {
                        DockedToShipGUID = (long)linkedGameDataJson["DockedToShipGUID"];
                    }

                    tempToken = linkedGameDataJson["DockedPortID"];
                    if (tempToken != null)
                    {
                        DockedPortID = (int)linkedGameDataJson["DockedPortID"];
                    }

                    tempToken = linkedGameDataJson["DockedToPortID"];
                    if (tempToken != null)
                    {
                        DockedToPortID = (int)linkedGameDataJson["DockedToPortID"];
                    }


                    break;
                default:
                    // It's a regular orbital object with it's parameters in an OrbitData sub-object.
                    OrbitData = new HEOrbitalData((JObject)linkedGameDataJson["OrbitData"]);
                    break;
            }
            // Generate a ToolTipText for this node - done last as it references other data.
            ToolTipText = GenerateToolTipText();
        }

        /// <summary>
        /// The GUID for this object.
        /// </summary>
        public long GUID { get; set; } = 0;

        /// <summary>
        /// The GUID of the parent of this object, returned from the OrbitalData.
        /// </summary>
        public long ParentGUID
        {
            get
            {
                if (OrbitData == null) throw new NullReferenceException();
                else return OrbitData.ParentGUID; 
            }
            set
            {
                if (OrbitData == null) throw new NullReferenceException();
                else OrbitData.ParentGUID = value;
            }
        }

        /// <summary>
        /// The Type of the object, as defined by the game.
        /// </summary>
        public int Type { get; set; } = 0;

        /// <summary>
        /// The Semi-Major Axis of the orbiting body.
        /// </summary>
        /// <remarks>
        /// Primary field for sorting objects that have the same ParentGUID.
        /// </remarks>
        public double SemiMajorAxis
        {
            get { return OrbitData != null ? OrbitData.SemiMajorAxis 
                    : throw new NullReferenceException("OrbitData was null."); }
        }

        /// <summary>
        /// The Angle of Inclination of the orbiting body.
        /// </summary>
        public double Inclination
        {
            get { return OrbitData == null ? OrbitData.Inclination 
                    : throw new NullReferenceException("OrbitData was null."); }
        }

        /// <summary>
        /// The OrbitData for this node - a copy of the values directly from the Json data for
        /// Asteroids, Ships and Players.
        /// </summary>
        /// <remarks>
        /// Celestial Bodies do not populate this information as theirs is stored in a slightly
        /// different format, as they come from the Static Data rather than the .save file.
        /// </remarks>
        public HEOrbitalData OrbitData { get; set; } = null;

        /// <summary>
        /// If this ship/module is docked TO another, the other ship's 
        /// GUID will be populated here
        /// </summary>
        public long DockedToShipGUID { get; set; } = 0;

        /// <summary>
        /// Which local port is in use to dock to the other ship.
        /// </summary>
        public int DockedPortID { get; set; } = 0;

        /// <summary>
        /// Which port on the other ship is docked to this ship.
        /// </summary>
        public int DockedToPortID { get; set; } = 0;

        /// <summary>
        /// Used to store a reference to the tree node in the Game Data that this was created
        /// from and is cross-linked to.
        /// </summary>
        public HEGameDataTreeNode LinkedGameDataNode { get; set; } = null;

        /// <summary>
        /// Generates a tool tip text for the current Solar System node.
        /// </summary>
        /// <returns></returns>
        public string GenerateToolTipText()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("DisplayName: " + Text + Environment.NewLine);
            sb.Append("GUID: " + GUID + Environment.NewLine);
            sb.Append("NodeType: " + NodeType.ToString() + Environment.NewLine);
            if (OrbitData != null ) sb.Append("ParentGUID: " + OrbitData.ParentGUID + Environment.NewLine);

            return sb.ToString();
        }

        /// <summary>
        /// Gets the current object's parent Celestial Body.
        /// </summary>
        /// <returns></returns>
        public HESolarSystemTreeNode GetParentCelestialBody()
        {
            HESolarSystemTreeNode parent = (HESolarSystemTreeNode)Parent;
            if (parent == null)
            {
                if (GUID == -1)
                {
                    // We're at the Solar System View Root Node, return this
                    return this;
                }
                else
                    throw new NullReferenceException("parent was null.");
            }
            else
            {
                if (parent.NodeType == HETreeNodeType.Star
                    || parent.NodeType == HETreeNodeType.Planet
                    || parent.NodeType == HETreeNodeType.Moon)
                {
                    // This node's parent was a celestial body, return it.
                    return parent;
                }
                else
                {
                    return parent.GetParentCelestialBody();
                }
            }
        }

        /// <summary>
        /// Gets the top most node in a docked node tree.
        /// </summary>
        /// <returns></returns>
        public HESolarSystemTreeNode GetRootOfDockingTree()
        {
            if (DockedToShipGUID == 0)
            {
                // We're not docked to anything so we are the root, job done!
                return (HESolarSystemTreeNode)this;
            }
            else
            {
                HESolarSystemTreeNode parent = (HESolarSystemTreeNode)Parent;
                if (parent == null) throw new NullReferenceException("parent was null.");
                else
                {
                    return parent.GetRootOfDockingTree();
                }
            }
        }

        /// <summary>
        /// Determines whether this node is docked *to* something, that is it's parent node
        /// is of type ship rather than a celestial body.
        /// </summary>
        /// <returns></returns>
        public bool IsDockedToParent()
        {
            // Cast Parent as a solar system node.
            HESolarSystemTreeNode parent = (HESolarSystemTreeNode)Parent;
            if (parent == null) throw new NullReferenceException("parent was null.");
            else
            {
                if (parent.NodeType == HETreeNodeType.Ship) return true;
                else return false;
            }
        }

    }

    /// <summary>
    /// Extends the HEGameDataTreeNode with the ability to generate a new cross-linked
    /// HESolarSystemTreeNode from itself, used by the Solar System builder.
    /// </summary>
    public partial class HEGameDataTreeNode : HETreeNode
    {
        /// <summary>
        /// Used to store a reference to the tree node in the Solar System that was created
        /// from this node and is cross-linked to.
        /// </summary>
        public HESolarSystemTreeNode LinkedSolarSystemNode { get; set; } = null;

        /// <summary>
        /// Creates and returns a cross-linked HESolarSystemTreeNode from this HEGameDataTreeNode.
        /// </summary>
        /// <param name="nodeType"></param>
        /// <remarks>
        /// Utilises a HESolarSystemTreeNode constructor that takes a reference to an HEGameDataTreeNode
        /// and uses the JToken within it's Tag field to set various properties but MUST be passed a valid
        /// HETreeNodeType.
        /// </remarks>
        /// <returns></returns>
        public HESolarSystemTreeNode CreateLinkedSolarSystemNode(HETreeNodeType nodeType)
        {
            LinkedSolarSystemNode = new HESolarSystemTreeNode(this, nodeType);
            return LinkedSolarSystemNode;
        }
    }
}
