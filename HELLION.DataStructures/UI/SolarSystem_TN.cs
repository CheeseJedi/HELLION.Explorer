using System;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json.Linq;

/// <summary>
/// Defines a derived HETreeNode to handle objects in the Solar System view.
/// Also defines a node sorter that sorts by Semi-Major axis instead of by name which is
/// the default on a TreeView control, and a class to hold the orbital data.
/// </summary>
namespace HELLION.DataStructures.UI
{
    /// <summary>
    /// Defines a sub-derived TreeNode class to hold some additional parameters used to speed up working
    /// with the nodes by storing frequently used values.
    /// </summary>
    public class SolarSystem_TN : Base_TN
    {
        #region Constructors

        /// <summary>
        /// Constructor that takes a minimum of a name, but also optionally a type and text (display name) - inherits the base constructor
        /// </summary>
        /// <param name="nodeName">Name for the new node; required;</param>
        /// <param name="nodeType">Base_TN_NodeType of the new node; defaults to Unknown.</param>
        /// <param name="nodeText">The Text (DisplayName) of the node - uses the Name if omitted.</param>
        /// <param name="nodeToolTipText">The ToolTip text displayed; defaults to the nodeText if omitted.</param>
        public SolarSystem_TN(Iparent_Base_TN passedOwner, string nodeName = null,
            Base_TN_NodeType nodeType = Base_TN_NodeType.Unknown)
            : base(passedOwner, nodeName, nodeType)
        {
            OrbitData = new OrbitalData();
        }

        /// <summary>
        /// Constructor that takes a link to an Json_TN and an Base_TN_NodeType.
        /// </summary>
        /// <remarks>
        /// Uses the Json_TN's Tag field, cast to a JObject, to generate name, GUID
        /// and other properties.
        /// The Inherits the base() constructor and sets properties directly.
        /// </remarks>
        /// <param name="gameDataNodeToLink"></param>
        /// <param name="nodeType"></param>
        public SolarSystem_TN(Json_TN gameDataNodeToLink, Base_TN_NodeType nodeType) : base(gameDataNodeToLink, nodeType: nodeType)
        {
            // Set up this end of the cross-link.
            LinkedGameDataNode = gameDataNodeToLink ?? throw new NullReferenceException("gameDataNodeToLink was null.");

            // Cast the LinkedGameDataNode.JData field in to a JObject.
            JObject _linkedGameDataJson = (JObject)LinkedGameDataNode.JData;
            if (_linkedGameDataJson == null) throw new NullReferenceException("_linkedGameDataJson was null.");

            // Set the node type, this will trigger the icon type to change to an appropriate one.
            //NodeType = nodeType;

            // If we're working with a blueprint node, translate the StructureID to GUID
            JToken tmpTkn = _linkedGameDataJson["StructureID"];
            if (tmpTkn != null)
            {
                Debug.Print("Got here for some reason!");
                
                // Set the node's Name to the StructureID of the object.
                Name = (string)_linkedGameDataJson["StructureID"];

                // Set the GUID to the StructureID
                GUID = (long)_linkedGameDataJson["StructureID"];
            }
            else
            {
                // Set the node's Name to the GUID of the object.
                Name = (string)_linkedGameDataJson["GUID"];

                // Set the GUID
                GUID = (long)_linkedGameDataJson["GUID"];
            }

            JToken tempToken = null;
            switch (nodeType)
            {
                case Base_TN_NodeType.Star:
                case Base_TN_NodeType.Planet:
                case Base_TN_NodeType.Moon:
                    // It's a Celestial Body - the orbital data is not in an OrbitalData sub-object, but directly
                    // as properties of this JObject.

                    OrbitData = new OrbitalData(_linkedGameDataJson);

                    break;
                case Base_TN_NodeType.Player:
                    // It's a player - doesn't have orbital data but may have a parent GUID if in a ship/module.

                    // Ensure OrbitData is null.
                    OrbitData = new OrbitalData();

                    tempToken = _linkedGameDataJson["ParentGUID"];
                    if (tempToken != null) OrbitData.ParentGUID = (long)_linkedGameDataJson["ParentGUID"];
                    else
                    {
                        // Set the parent GUID to -1 (the Solar System) as we currently can't place players who
                        // are outside ships or modules.
                        OrbitData.ParentGUID = -1;
                    }

                    break;
                case Base_TN_NodeType.Ship:
                    // Ships also need to set the docking info if present.

                    OrbitData = new OrbitalData((JObject)_linkedGameDataJson["OrbitData"]);

                    tempToken = _linkedGameDataJson["DockedToShipGUID"];
                    if (tempToken != null)
                    {
                        DockedToShipGUID = (long)_linkedGameDataJson["DockedToShipGUID"];
                    }

                    tempToken = _linkedGameDataJson["DockedPortID"];
                    if (tempToken != null)
                    {
                        DockedPortID = (int)_linkedGameDataJson["DockedPortID"];
                    }

                    tempToken = _linkedGameDataJson["DockedToPortID"];
                    if (tempToken != null)
                    {
                        DockedToPortID = (int)_linkedGameDataJson["DockedToPortID"];
                    }


                    break;
                default:
                    // It's a regular orbital object with it's parameters in an OrbitData sub-object.
                    OrbitData = new OrbitalData((JObject)_linkedGameDataJson["OrbitData"]);
                    break;
            }

        }

        #endregion

        #region Properties


        /// <summary>
        /// The GUID for this object.
        /// </summary>
        public long GUID { get; set; } = 0;

        /// <summary>
        /// The OrbitData for this node - a copy of the values directly from the Json data for
        /// Asteroids, Ships and Players.
        /// </summary>
        /// <remarks>
        /// Celestial Bodies do not populate this information as theirs is stored in a slightly
        /// different format, as they come from the Static Data rather than the .save file.
        /// </remarks>
        public OrbitalData OrbitData { get; set; } = null;

        /// <summary>
        /// The GUID of the parent of this object, returned from the OrbitalData.
        /// </summary>
        public long ParentGUID
        {
            get => (long)OrbitData?.ParentGUID;
            set
            {
                if (OrbitData == null) throw new NullReferenceException();
                else OrbitData.ParentGUID = value;
            }
        }

        /// <summary>
        /// The Semi-Major Axis of the orbiting body.
        /// </summary>
        /// <remarks>
        /// Primary field for sorting objects that have the same ParentGUID.
        /// </remarks>
        public double SemiMajorAxis
        {
            get => (double)OrbitData?.SemiMajorAxis;
        }

        /// <summary>
        /// The Angle of Inclination of the orbiting body.
        /// </summary>
        public double Inclination
        {
            get => (double)OrbitData?.Inclination;
        }

        /// <summary>
        /// The Type of the object, as defined by the game.
        /// </summary>
        public int Type { get; set; } = 0;


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
        /// Gets the current object's parent Celestial Body.
        /// </summary>
        /// <returns></returns>
        public SolarSystem_TN ParentCelestialBody
        {
            get
            {
                if (Parent == null)
                {
                    // We're at the Solar System View Root Node, return this
                    if (GUID == -1) return this;
                    throw new NullReferenceException("parent was null.");
                }
                else
                {
                    SolarSystem_TN parent = (SolarSystem_TN)Parent;
                    if (parent.NodeType == Base_TN_NodeType.Star
                        || parent.NodeType == Base_TN_NodeType.Planet
                        || parent.NodeType == Base_TN_NodeType.Moon)
                    {
                        // This node's parent was a celestial body, return it.
                        return parent;
                    }
                    else return parent.ParentCelestialBody;
                }
            }
        }

        /// <summary>
        /// Gets the root (top most) node in a docked node tree hierarchy.
        /// </summary>
        public SolarSystem_TN DockingTreeRoot
        {
            get
            {
                // We're not docked to anything so we are the root, job done!
                if (DockedToShipGUID == 0) return (SolarSystem_TN)this;
                // Otherwise keep going to find the 
                return ((SolarSystem_TN)Parent)?.DockingTreeRoot;
            }
        }

        /// <summary>
        /// Determines whether this node is docked *to* it's parent, or just orbiting it.
        /// </summary>
        public bool IsDockedToParent
        {
            get
            {
                // If the parent node type is a Ship, then we're docked to it.
                if (((SolarSystem_TN)Parent)?.NodeType == Base_TN_NodeType.Ship) return true;
                // Parent object was a celestial body.
                else return false;
            }
        }

        /// <summary>
        /// Used to store a reference to the tree node in the Game Data that this was created
        /// from and is cross-linked to.
        /// </summary>
        public Json_TN LinkedGameDataNode { get; set; } = null;

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Generates a tool tip text for the current Solar System node.
        /// </summary>
        /// <returns></returns>
        protected override string GenerateToolTipText()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("DisplayName: " + Text + Environment.NewLine);
            sb.Append("GUID: " + GUID + Environment.NewLine);
            sb.Append("NodeType: " + NodeType.ToString() + Environment.NewLine);
            sb.Append("ParentGUID: " + OrbitData?.ParentGUID + Environment.NewLine);

            return sb.ToString();
        }


        #endregion



    }

}
