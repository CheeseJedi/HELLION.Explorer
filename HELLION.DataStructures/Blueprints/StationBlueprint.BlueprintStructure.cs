using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HELLION.DataStructures.StaticData;
using HELLION.DataStructures.UI;
using HELLION.DataStructures.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using static HELLION.DataStructures.StaticData.DockingPortHelper;

namespace HELLION.DataStructures.Blueprints
{
    public partial class StationBlueprint
    {
        /// <summary>
        /// A class to define structures (modules/ships) within the blueprint.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class BlueprintStructure : IParent_Base_TN
        {
            #region Constructors

            /// <summary>
            /// Basic constructor.
            /// </summary>
            public BlueprintStructure()
            {
                DockingPorts = new List<BlueprintDockingPort>();
                SceneID = StructureSceneID.Unspecified;
                RootNode = new BlueprintStructure_TN(passedOwner: this);
            }

            /// <summary>
            /// Constructor that takes an ownerObject reference.
            /// </summary>
            /// <param name="ownerObject"></param>
            public BlueprintStructure(StationBlueprint ownerObject = null) : this()
            {
                OwnerObject = ownerObject;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Parent object - not to be included in serialisation.
            /// </summary>
            public StationBlueprint OwnerObject { get; set; } = null;

            /// <summary>
            /// Not to be serialised.
            /// </summary>
            public Base_TN RootNode { get; set; } = null;

            /// <summary>
            /// Used to determine whether this structure is a hierarchy root.
            /// </summary>
            /// <remarks>
            /// Hierarchy root markers are used to identify graph nodes to be used as tree roots.
            /// Primarily this is used in building the Secondary Structures list, as all structures
            /// (singular) that exist in a blueprint in memory will appear in the main Structures
            /// list of the blueprint object, but only the 'root' of each chain of modules is marked
            /// for later retrieval. 
            /// </remarks>
            public bool IsStructureHierarchyRoot
            {
                get => _isStructureHierarchyRoot;
                set
                {
                    if (_isStructureHierarchyRoot != value)
                    {
                        _isStructureHierarchyRoot = value;

                        // Trigger refresh.
                        RefreshAfterStructureHierarchyRootChange();
                    }

                }
            }

            public bool IsPrimaryStructure => this == OwnerObject.PrimaryStructureRoot ? true : false;


            /// <summary>
            /// Determines whether this structure is connected to the blueprint's primary structure.
            /// </summary>
            /// <returns></returns>
            public bool IsConnectedToPrimaryStructure
            {
                get
                {
                    // Full check.
                    if (ConnectedStructures(visitedtedStructures: new List<BlueprintStructure> { this })
                        .Contains(OwnerObject.PrimaryStructureRoot)) return true;

                    // This structure wasn't in the PrimaryStructureRoot's list of connected structures.
                    return false;
                }
            }

            /// <summary>
            /// Determines whether the structure is docked to another.
            /// </summary>
            public bool IsDocked
            {
                get
                {
                    foreach (BlueprintDockingPort port in DockingPorts)
                        if (port.IsDocked) return true;

                    return false;
                }
            }

            #endregion

            /// <summary>
            /// Legacy de-serialisation helper.
            /// </summary>
            /*
            public int? ItemID
            {
                set => SceneID = (HEBlueprintStructureSceneID)Enum
                    .Parse(typeof(HEBlueprintStructureSceneID), value.ToString());
            }
            */

            #region Serialised Properties

            /// <summary>
            /// The unique ID of this structure - set if the object is a blueprint. 
            /// If a template this will be null as there is no docking hierarchy represented
            /// in a template.
            /// </summary>
            [JsonProperty]
            public int? StructureID
            {
                get => _structureID;
                set
                {
                    if (_structureID != value)
                    {
                        // Change detected, evaluate whether this is the root node (has ID of zero)
                        _previousStructureID = _structureID;
                        _structureID = value;

                        RefreshAfterStructureIDChange();
                    }
                }
            }

            /// <summary>
            /// The SceneID of the structure - Critical! used by the in-game deserialiser to determine
            /// the type of structure (ship/module) to spawn. Critical! 
            /// </summary>
            [JsonProperty]
            public StructureSceneID? SceneID
            {
                get => _sceneID;
                set
                {
                    if (_sceneID != value)
                    {
                        _sceneID = value;

                        // Update the RootNode's BaseNodeName.
                        
                        //RootNode.Name = value.GetEnumDescription();
                        
                    }
                }
            }

            [JsonProperty]
            public string StructureType
            {
                get => SceneID?.GetEnumDescription(); // ?? HEBlueprintStructureSceneID.Unspecified.ToString();
                set
                {
                    if (value != null && value.Length > 0)
                    {

                        // Attempt to parse the given description to an available one in the enum.
                        StructureSceneID descriptionParseResult = value.ParseToEnumDescriptionOrEnumerator<StructureSceneID>();

                        if (descriptionParseResult != StructureSceneID.Unspecified)
                        {
                            SceneID = descriptionParseResult;
                        }
                        // else throw new Exception();

                    }

                }
            }

            /// <summary>
            /// The list of docking ports for this individual structure.
            /// </summary>
            [JsonProperty]
            public List<BlueprintDockingPort> DockingPorts { get; set; } = null;

            [JsonProperty]
            public bool? SystemsOnline { get; set; }
            [JsonProperty]
            public bool? Invulnerable { get; set; }
            [JsonProperty]
            public bool DoorsLocked { get; set; }
            [JsonProperty]
            public string Tag { get; set; }
            [JsonProperty]
            public float? HealthMultiplier { get; set; }
            [JsonProperty]
            public bool? DockingControlsDisabled { get; set; }
            [JsonProperty]
            public float? AirPressure { get; set; }
            [JsonProperty]
            public float? AirQuality { get; set; }

            /// <summary>
            /// The SceneName of the Structure. 
            /// Shadow Property - also comes from the SceneID field using the SceneID enum.
            /// </summary>
            [JsonProperty]
            [JsonConverter(typeof(StringEnumConverter))]
            public StructureSceneID? SceneName { get => SceneID; set => SceneID = value; }
            // public bool ShouldSerializeSceneName() { return OwnerStructure == null || (OwnerStructure != null && OwnerStructure.IsTemplate) ? true : false; }

            //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            //public float? StandbyPowerRequirement { get; set; } = null;
            //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            //public float? NominalPowerRequirement { get; set; } = null;
            //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            //public float? NominalPowerContribution { get; set; } = null;
            //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            //public float? NominalAirVolume { get; set; } = null;

            /// <summary>
            /// Auxiliary data for this blueprint. If this is set it applies to all structures in
            /// the blueprint. Individual structures can also implement an AuxData that will over-
            /// ride the blueprint level AuxData.
            /// </summary>
            //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            //public BlueprintStructure_AuxData AuxData { get; set; } = null;


            /*
            /// <summary>
            /// The structure type - a value from the HEBlueprintStructureType enum.
            /// </summary>
            [JsonProperty]
            [JsonConverter(typeof(StringEnumConverter))]
            public HEBlueprintStructureType? StructureType
            {
                get => _structureType;
                set
                {
                    _structureType = value;
                    RootNode.BaseNodeName = value.ToString();
                    //RootNode.BaseNodeText = RootNode.Name;
                }
            }
            */

            #endregion

            #region Methods

            /// <summary>
            /// Gets a docking port by name.
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public BlueprintDockingPort GetDockingPort(string name)
            {
                if (name == null || name == string.Empty || !(DockingPorts.Count > 0)) return null;

                IEnumerable<BlueprintDockingPort> results = DockingPorts.
                    Where(f => f.PortName.ToString() == name);

                return results.Count() == 1 ? results.Single() : null;
            }

            /// <summary>
            /// Gets a docking port by the ID of the structure it's docked to.
            /// </summary>
            /// <param name="dockedStructureID"></param>
            /// <returns></returns>
            public BlueprintDockingPort GetDockingPort(int dockedStructureID)
            {
                if (!(dockedStructureID >= 0) || !(dockedStructureID <= OwnerObject.Structures.Count)
                    || !(DockingPorts.Count > 0)) return null;

                IEnumerable<BlueprintDockingPort> results = DockingPorts.
                    Where(f => f.DockedStructureID == dockedStructureID);

                return results.Count() == 1 ? results.Single() : null;
            }

            /// <summary>
            /// Gets a docking port by the structure it's docked to.
            /// </summary>
            /// <param name="dockedStructureID"></param>
            /// <returns></returns>
            public BlueprintDockingPort GetDockingPort(BlueprintStructure dockedStructure)
            {
                if (dockedStructure == null || !(DockingPorts.Count > 0)) return null;

                IEnumerable<BlueprintDockingPort> results = DockingPorts.
                    Where(f => f.DockedStructure == dockedStructure);

                return results.Count() == 1 ? results.Single() : null;
            }


            public BlueprintDockingPort GetDockingPortByOrderID(int orderID)
            {
                //if (orderID > 0 || !(DockingPorts.Count > 0)) return null;

                IEnumerable<BlueprintDockingPort> results = DockingPorts.
                    Where(f => f.OrderID == orderID);

                return results.Count() == 1 ? results.Single() : null;

            }


            /// <summary>
            /// Gets the Structure Root for this structure.
            /// </summary>
            /// <returns></returns>
            public BlueprintStructure GetStructureRoot()
            {
                // Is this structure a root itself?
                if (OwnerObject.PrimaryStructureRoot == this || OwnerObject.SecondaryStructureRoots.Contains(this)) return this;

                // Is this structure connected to the Primary Structure?
                if (IsConnectedToPrimaryStructure) return OwnerObject.PrimaryStructureRoot;

                // Full test - the intersection of this structure's connected structures list, and 
                // the Secondary Structures list.
                IEnumerable<BlueprintStructure> results = ConnectedStructures(new List<BlueprintStructure> { this })
                    .Intersect(OwnerObject.SecondaryStructureRoots);

                if (results.Count() > 1) throw new InvalidOperationException("More than one root found.");

                return results.Count() == 1 ? results.Single() : null;
            }

            /// <summary>
            /// Returns a list of directly docked structures, or all connected structures.
            /// </summary>
            /// <returns>
            /// Returns an empty list if no docked structures.
            /// </returns>
            /// <remarks>
            /// Is recursive if passed a list containing the starting structure, otherwise it only
            /// returns the directly connected structures.
            /// </remarks>
            public List<BlueprintStructure> ConnectedStructures(List<BlueprintStructure> visitedtedStructures = null)
            {
                bool includeAllConnected = true;
                if (visitedtedStructures == null)
                {
                    includeAllConnected = false;
                    visitedtedStructures = new List<BlueprintStructure>();
                }

                // Loop over all of this structure's ports that are docked to something.
                foreach (BlueprintDockingPort port in DockingPorts.Where(p => p.IsDocked))
                {
                    // The port is docked; retrieve the structure by its ID.
                    BlueprintStructure _result = port.DockedStructure;

                    if (!visitedtedStructures.Contains(_result))
                    {
                        // Add the current structure to the visited list.
                        visitedtedStructures.Add(_result);

                        // Recurse if we're including all connected structures.
                        if (includeAllConnected) visitedtedStructures.AddRange(
                            _result.ConnectedStructures(visitedtedStructures)
                            .Where(s => !visitedtedStructures.Contains(s)));
                    }
                }
                return visitedtedStructures;
            }

            /// <summary>
            /// Removes this structure only - it must not be docked to another.
            /// </summary>
            /// <returns>Returns a failure state; true if the operation encountered a problem, false if successful.</returns>
            public bool PrepareForRemoval(bool removeOrphanedStructures = false)
            {
                // The RemoveOrphanedStructures is currently ignored.
                // Ideally each docking port would be notified, and if necessary trigger an un-docking.

                // Immediately fail (return) if the structure is docked.
                if (IsDocked) return true;

                // Set up a StringBuilder to append debug info to.
                StringBuilder sb = new StringBuilder();

                //
                sb.Append(string.Format("Removal called on StructureType {0} StructureID {1}", StructureType, StructureID) + Environment.NewLine);

                // Process DockingPorts first - prepare them for removal.
                foreach (BlueprintDockingPort port in DockingPorts)
                {
                    sb.Append(string.Format("* Processing PortName {0}; OrderID {1}; IsDocked {2}", port.PortName, port.OrderID, IsDocked) + Environment.NewLine);

                    if (!port.PrepareForRemoval(removeOrphanedStructures)) return true;
                    
                }

                
                
                // Set the DockingPorts list to null - the garbage collector will deal with the docking port objects.
                //DockingPorts = null;

                


                Debug.Print(sb.ToString());

                return false;
            }

            /// <summary>
            /// Adds docking ports appropriate for this type of structure.
            /// </summary>
            public void AddAppropriateDockingPorts()
            {

                DockingPorts = GenerateBlueprintDockingPorts((StructureSceneID)SceneID);
                
                foreach (BlueprintDockingPort port in DockingPorts)
                {
                    RootNode.Nodes.Insert(0, port.RootNode);
                }


                //// Find the matching definition type for this structures type.
                //if (OwnerStructure.StructureDefinitions == null) throw new NullReferenceException();

                //BlueprintStructure defn = OwnerStructure.StructureDefinitions.Structures
                //    .Where(f => (int)f.SceneID == (int)SceneID).Single();

                //foreach (BlueprintDockingPort dockingPort in defn.DockingPorts)
                //{
                //    BlueprintDockingPort newPort = new BlueprintDockingPort()
                //    {
                //        OwnerStructure = this,
                //        PortName = dockingPort.PortName,
                //        OrderID = dockingPort.OrderID,
                //        DockedStructureID = null
                //    };
                //    DockingPorts.Add(newPort);
                //    RootNode.Nodes.Insert(0, newPort.RootNode);
                //}
            }

            /// <summary>
            /// Is called when the StructureID changes and updates the node's prefix and icon.
            /// </summary>
            protected void RefreshAfterStructureIDChange()
            {
                if (StructureID != null)
                {

                    // Module ID zero is always the docking root in a blueprint and has a different icon.
                    //RootNode.DisplayRootStructureIcon = (StructureID != null && StructureID == 0) ? true : false;

                    // RootNode.BaseNodeName = StructureType.ToString();
                    RootNode.Text_Prefix = string.Format("[{0:000}] ", (int)StructureID);
                    //RootNode.RefreshText();
                    //RootNode.RefreshName();

                    // Update Docking Port nodes for this node.
                    if (DockingPorts.Count > 0)
                    {
                        foreach (var port in DockingPorts)
                        {
                            // Update docking structure relationships between this module and any docked modules.
                            port.RefreshAfterParentStructureIDChange();
                        }
                    }
                }
            }

            /// <summary>
            /// Is called when the StructurehierarchyRoot bool changes status and updates the TreeNode's node type.
            /// </summary>
            protected void RefreshAfterStructureHierarchyRootChange()
            {
                RootNode.NodeType = IsStructureHierarchyRoot ? Base_TN_NodeType.BlueprintRootStructure : Base_TN_NodeType.BlueprintStructure;
            }

            #endregion

            #region Fields

            protected int? _structureID = null;
            protected StructureSceneID? _sceneID = null;
            //protected HEBlueprintStructureType? _structureType = null;
            protected int? _previousStructureID = null;
            protected bool _isStructureHierarchyRoot = false;

            #endregion

        }
    }
}
