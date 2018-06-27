using System;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json.Linq;

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
        /// Constructor that takes a minimum of an Owner, but also optionally a name and type.
        /// </summary>
        /// <param name="nodeName">Name for the new node; required;</param>
        /// <param name="nodeType">Base_TN_NodeType of the new node; defaults to Unknown.</param>
        public SolarSystem_TN(IParent_Base_TN passedOwner, string nodeName = null,
            Base_TN_NodeType nodeType = Base_TN_NodeType.Unknown)
            : base(passedOwner, nodeType, nodeName)
        {
            // OrbitData = new OrbitalData();
        }

        /// <summary>
        /// Constructor that takes a link to an Json_TN and an Base_TN_NodeType.
        /// </summary>
        /// <param name="gameDataNodeToLink">The node to link to.</param>
        /// <param name="nodeType">The node Type for the new linked node.</param>
        public SolarSystem_TN(Json_TN gameDataNodeToLink, Base_TN_NodeType nodeType) : base(gameDataNodeToLink, nodeType: nodeType)
        {
            // Set up this end of the cross-link.
            LinkedGameDataNode = gameDataNodeToLink ?? throw new NullReferenceException("gameDataNodeToLink was null.");

            // Cast the LinkedGameDataNode.JData field in to a JObject.
            JObject _linkedGameDataJson = (JObject)LinkedGameDataNode.JData ?? throw new NullReferenceException("_linkedGameDataJson was null.");

            // If we're working with a linked blueprint node, translate the StructureID to GUID
            JToken tmpTkn = _linkedGameDataJson["StructureID"];
            if (tmpTkn != null)
            {
                throw new Exception("Got here for some reason!");
                Debug.Print("Got here for some reason!");
                
                // Set the node's Name to the StructureID of the object.
                Name = (string)_linkedGameDataJson["StructureID"];

                // Set the GUID to the StructureID
                GUID = (long)_linkedGameDataJson["StructureID"];
            }
            else
            {
                switch (nodeType)
                {
                    case Base_TN_NodeType.Ship:
                        if (string.IsNullOrEmpty((string)_linkedGameDataJson["Name"]))
                        {
                            // The ship has no name - most non-player vessels fall in to this category.
                            Name = (string)_linkedGameDataJson["Registration"];
                        }
                        else
                        {
                            // The ship has a name, append it on to the end of the registration
                            Name = (string)_linkedGameDataJson["Registration"] + " " + (string)_linkedGameDataJson["Name"];
                        }
                        break;
                    default:
                        Name = ((string)_linkedGameDataJson["Name"]).Trim();
                        break;
                }

                // Can't use these presently as it messes with the TreeView's Path system,
                // which annoyingly uses the TreeNode's Text field rather than the Name field
                // in path generation.
                // Text_Prefix = ((string)_linkedGameDataJson["Name"]).Trim();
                // Text_Suffix = ((string)_linkedGameDataJson["GUID"]).Trim();

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
                    // OrbitData = new OrbitalData();

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

                    DockedToShipGUID = (long?)_linkedGameDataJson["DockedToShipGUID"];
                    DockedPortID = (int?)_linkedGameDataJson["DockedPortID"];
                    DockedToPortID = (int?)_linkedGameDataJson["DockedToPortID"];

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
        /// The OrbitData for this node - de-serialised directly from the Json data for
        /// Asteroids, Ships and Players.
        /// </summary>
        /// <remarks>
        /// Celestial Bodies do not populate this information as theirs is stored in a slightly
        /// different format, as they come from the Static Data rather than the .save file.
        /// </remarks>
        public OrbitalData OrbitData { get; set; } = new OrbitalData();

        /// <summary>
        /// The GUID of the parent of this object, returned from the OrbitalData.
        /// </summary>
        public long ParentGUID
        {
            get => OrbitData.ParentGUID != null ? (long)OrbitData.ParentGUID : -1L;
            set => OrbitData.ParentGUID = value;
        }

        /// <summary>
        /// The Semi-Major Axis of the orbiting body.
        /// </summary>
        /// <remarks>
        /// Primary field for sorting objects that have the same ParentGUID.
        /// </remarks>
        public double SemiMajorAxis => OrbitData.SemiMajorAxis != null ? (double)OrbitData.SemiMajorAxis : -1L;

        /// <summary>
        /// The Angle of Inclination of the orbiting body.
        /// </summary>
        //public double Inclination => OrbitData.Inclination;

        /// <summary>
        /// The Type of the object, as defined by the game.
        /// </summary>
        // public int Type { get; set; } = 0;

        /// <summary>
        /// If this ship/module is docked TO another, the other ship's 
        /// GUID will be populated here
        /// </summary>
        public long? DockedToShipGUID { get; set; } = null;

        /// <summary>
        /// Which local port is in use to dock to the other ship.
        /// </summary>
        public int? DockedPortID { get; set; } = null;

        /// <summary>
        /// Which port on the other ship is docked to this ship.
        /// </summary>
        public int? DockedToPortID { get; set; } = null;

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
                    return null;
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
                    return parent.ParentCelestialBody;
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
        /// Determines whether this node is docked *to* it's parent, or just orbiting
        /// it in the case of a celestial body parent.
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
