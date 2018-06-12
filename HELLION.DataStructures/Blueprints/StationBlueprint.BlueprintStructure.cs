using System;
using System.Collections.Generic;
using System.Linq;
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
            /// Constructor.
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

            /// <summary>
            /// Determines whether this structure is connected to the blueprint's primary structure.
            /// </summary>
            /// <returns></returns>
            public bool IsConnectedToPrimaryStructure
            {
                get
                {
                    // Shortcut check.
                    if (this == OwnerObject.PrimaryStructureRoot) return true;

                    // Full check.
                    if (ConnectedStructures(new List<BlueprintStructure> { this })
                        .Contains(OwnerObject.PrimaryStructureRoot)) return true;

                    // This structure wasn't in the PrimaryStructureRoot's list of connected structures.
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

            /// <summary>
            /// The SceneName of the Structure. 
            /// Shadow Property - also comes from the SceneID field using the SceneID enum.
            /// </summary>
            [JsonProperty]
            [JsonConverter(typeof(StringEnumConverter))]
            public StructureSceneID? SceneName { get => SceneID; set => SceneID = value; }
            // public bool ShouldSerializeSceneName() { return OwnerStructure == null || (OwnerStructure != null && OwnerStructure.IsTemplate) ? true : false; }

            [JsonProperty]
            public String StructureType
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

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public float? StandbyPowerRequirement { get; set; } = null;

            // [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            // public float? NominalPowerRequirement { get; set; } = null;

            // [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            // public float? NominalPowerContribution { get; set; } = null;

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public float? NominalAirVolume { get; set; } = null;

            /// <summary>
            /// Auxiliary data for this blueprint. If this is set it applies to all structures in
            /// the blueprint. Individual structures can also implement an AuxData that will over-
            /// ride the blueprint level AuxData.
            /// </summary>
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
            public BlueprintStructure_AuxData AuxData { get; set; } = null;

            /// <summary>
            /// The list of docking ports for this individual structure.
            /// </summary>
            [JsonProperty]
            public List<BlueprintDockingPort> DockingPorts { get; set; } = null;

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
                if (name == null || name == String.Empty || !(DockingPorts.Count > 0)) return null;

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
            /// Removes this structure, and any docked to it that would be orphaned by
            /// the removal operation.
            /// </summary>
            /// <returns>Returns true on success.</returns>
            public bool Remove(bool RemoveOrphanedStructures = false)
            {
                // TODO - Not yet implemented.

                // Make list of directly docked structures
                // Find local docking ports that are in use
                // Loop through each and find the corresponding docking port and reset it's data with .Undock()
                //      check the previously docked structures and make a list of structures not



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
                    RootNode.Nodes.Add(port.RootNode);
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
                //    RootNode.Nodes.Add(newPort.RootNode);
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
                    RootNode.Text_Prefix = String.Format("[{0:000}] ", (int)StructureID);
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
