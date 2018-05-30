using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HELLION.DataStructures.UI;
using HELLION.DataStructures.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using static HELLION.DataStructures.StaticDataHelper;

namespace HELLION.DataStructures.Blueprints
{
    /// <summary>
    /// A class to handle station blueprint data structures.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class StationBlueprint : Iparent_Base_TN
    {
        public const decimal StationBlueprintFormatVersion = 0.38m;

        #region Constructors

        /// <summary>
        /// Default Constructor, called directly by the JToken.ToObject<T>()
        /// </summary>
        public StationBlueprint()
        {
            Structures = new List<BlueprintStructure>();
            RootNode = new Blueprint_TN(passedOwner: this, nodeName: "Hierarchy View",
                newNodeType: HETreeNodeType.BlueprintHierarchyView); 
                //nodeToolTipText: "Shows a tree-based view of the modules and their docking hierarchy.");
        }

        /// <summary>
        /// Constructor that takes a HEStationonBlueprintFile Owner Object and reference to the 
        /// structure definitions object.
        /// </summary>
        /// <param name="ownerObject"></param>
        /// <param name="structureDefs"></param>
        public StationBlueprint(StationBlueprint_File ownerObject, StationBlueprint structureDefs) : this()
        {
            OwnerObject = ownerObject ?? throw new NullReferenceException("passedParent was null.");
            StructureDefinitions = structureDefs ?? throw new NullReferenceException("structureDefs was null.");
            __ObjectType = BlueprintObjectType.StationBlueprint;
            Version = StationBlueprintFormatVersion;
        }

        #endregion

        #region Properties

        /// <summary>
        /// A reference to the Parent object.
        /// </summary>
        public StationBlueprint_File OwnerObject { get; set; } = null;

        public Base_TN RootNode { get; set; } = null;

        public StationBlueprint StructureDefinitions { get; set; } = null;

        public bool IsTemplate { get; protected set; } = false;

        public bool IsDirty { get; protected set; } = false;

        public BlueprintStructure PrimaryStructureRoot
        {
            get => _primaryStructureRoot;
            set
            {
                if (_primaryStructureRoot != value)
                {
                    BlueprintStructure oldValue = _primaryStructureRoot;
                    _primaryStructureRoot = value;
                }
            }
        }

        /// <summary>
        /// The list of all Secondary Structures - these are not technically part of a finished
        /// blueprint and are linked here to allow processing separately to the primary structure.
        /// </summary>
        public List<BlueprintStructure> SecondaryStructureRoots
        {
            get => Structures.Where(f => f.IsStructureHierarchyRoot && f != PrimaryStructureRoot).ToList();
        }

        #endregion

        #region Serialised Properties

        /// <summary>
        /// Type of object - to aid in class identification during de-serialisation in-game.
        /// Should be either "StationBlueprint" for a blueprint object or "StructureDefinitions"
        /// for a StructureDefinitions template object.)
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public BlueprintObjectType? __ObjectType { get; set; } = null;

        /// <summary>
        /// The Hellion Station Blueprint Format version number.
        /// </summary>
        /// <remarks>
        /// Should reflect the version no. of the StructureDefinitions.json file.
        /// </remarks>
        [JsonProperty]
        public decimal? Version { get; set; } = null;

        /// <summary>
        /// The Station's name as displayed in-game.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; } = null;

        /// <summary>
        /// The link URI for the blueprint data source.
        /// </summary>
        /// <remarks>
        /// If this is generated in Hellion Explorer, this will be a link to the GitHub Repo.
        /// If the blueprint originated from HSP, this will be a link to the station in HSP.
        /// </remarks>
        [JsonProperty]
        public Uri LinkURI { get; set; } = null;

        /// <summary>
        /// Auxiliary data for this blueprint. If this is set it applies to all structures in
        /// the blueprint. Individual structures can also implement an AuxData that will over-
        /// ride the blueprint level AuxData.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public HEBlueprintStructureAuxData AuxData { get; set; } = null;

        /// <summary>
        /// The list of all structures in the blueprint.
        /// </summary>
        /// <remarks>
        /// May contain secondary structures during the process of editing.
        /// To be serialised.
        /// </remarks>
        [JsonProperty]
        public List<BlueprintStructure> Structures { get; set; } = null;

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void PostDeserialisationInit()
        {
            Debug.Print("got to PostDeserialisation.");
            if (__ObjectType != null && __ObjectType == BlueprintObjectType.StationBlueprint)
            {
                ReconnectChildToParentObjectHierarchy();
                ReconnectDockedObjects();

                SetPrimaryStructureRoot();
            }
        }

        /// <summary>
        /// Links up all child object's parent reference to their parent.
        /// </summary>
        private void ReconnectChildToParentObjectHierarchy()
        {
            if (OwnerObject == null) throw new NullReferenceException("ParentJsonBlueprintFile was null.");

            foreach (BlueprintStructure structure in Structures)
            {
                // Set the structure's parent object (this, the blueprint object)
                structure.OwnerObject = this;
                foreach (HEBlueprintDockingPort port in structure.DockingPorts)
                {
                    port.OwnerObject = structure;
                }
            }
        }

        /// <summary>
        /// Links up all child object's parent reference to their parent.
        /// </summary>
        private void ReconnectDockedObjects()
        {
            foreach (BlueprintStructure structure in Structures)
            {
                foreach (HEBlueprintDockingPort port in structure.DockingPorts)
                {
                    port.AttemptUpdateDockedStructure();
                    port.AttemptUpdateDockedPort();
                }
            }
        }

        /// <summary>
        /// Attempts to set the root structure of the Primary Structure.
        /// </summary>
        /// <remarks>
        /// Called by PostDeserialisationInit() 
        /// A valid blueprint contains a structure with ID of zero - this is the root
        /// structure for the main blueprint superstructure (one or more structures
        /// docked together).
        /// </remarks>
        private void SetPrimaryStructureRoot()
        {
            BlueprintStructure result = GetStructure(0);
            PrimaryStructureRoot = result ?? throw new NullReferenceException("Unable to locate the blueprint's root structure (index zero).");
            PrimaryStructureRoot.IsStructureHierarchyRoot = true;
        }

        /// <summary>
        /// Refreshes all tree node structures for the Primary and Secondary structures.
        /// </summary>
        public void RefreshAllTreeNodes()
        {
            // Break any existing parent/child relationships.
            foreach (BlueprintStructure structure in Structures)
            {
                structure.OwnerObject.RootNode.Nodes.Remove(structure.RootNode);
                structure.RootNode.Refresh();

                // Rebuild name.
                //structure.

                foreach (HEBlueprintDockingPort port in structure.DockingPorts)
                {
                    port.OwnerObject.RootNode.Nodes.Remove(port.RootNode);
                    port.RootNode.Refresh();
                }
            }

            // Assemble the primary blueprint structure's docked node tree.
            ReassembleTreeNodeDockingStructure(PrimaryStructureRoot);

            // Assemble the secondary structures node trees.
            foreach (BlueprintStructure _secondaryStructure in SecondaryStructureRoots)
            {
                ReassembleTreeNodeDockingStructure(_secondaryStructure);
            }
        }

        /// <summary>
        /// Builds a tree of nodes representing the docking structure, including nodes for each docking port.
        /// </summary>
        public void ReassembleTreeNodeDockingStructure(BlueprintStructure hierarchyRoot, bool AttachToBlueprintTreeNode = false)
        {
            // Start with the root node, should be item zero in the list.

            // Process the docking root's ports slightly differently - always child nodes.
            if (AttachToBlueprintTreeNode) RootNode.Nodes.Add(hierarchyRoot.RootNode);

            foreach (HEBlueprintDockingPort port in hierarchyRoot.DockingPorts.ToArray().Reverse())
            {
                hierarchyRoot.RootNode.Nodes.Add(port.RootNode);
                if (port.IsDocked)
                {
                    Reassemble(port.DockedStructure, hierarchyRoot);
                }
            }

            /// <summary>
            /// The recursive bit.
            /// </summary>
            void Reassemble(BlueprintStructure structure, BlueprintStructure parent)
            {
                // Figure out which port is docking us to the parent and vice versa.
                // TODO - This probably should use the object references now they are available instead of the simple de-serialised IDs.
                HEBlueprintDockingPort linkToParent = structure.GetDockingPort((int)parent.StructureID);
                HEBlueprintDockingPort linkFromParent = parent.GetDockingPort((int)structure.StructureID);

                // Add the node for the link to parent to the link from parent's node collection.
                linkFromParent.RootNode.Nodes.Add(linkToParent.RootNode);

                // Add the structures's node to the link to parent node collection.
                linkToParent.RootNode.Nodes.Add(structure.RootNode);

                foreach (HEBlueprintDockingPort port in structure.DockingPorts.ToArray().Reverse())
                {
                    if (port != linkToParent)
                    {
                        structure.RootNode.Nodes.Add(port.RootNode);
                        if (port.IsDocked) Reassemble(GetStructure(port.DockedStructureID), structure);
                    }
                }

            }
        }

        /// <summary>
        /// Gets a structure by its id.
        /// </summary>
        /// <param name="structureID"></param>
        /// <returns></returns>
        public BlueprintStructure GetStructure(int? structureID)
        {
            if (structureID == null) return null;
            IEnumerable<BlueprintStructure> results = Structures.Where(f => f.StructureID == structureID);
            if (results.Count() < 1 || results.Count() > 1) return null;
            return results.Single();
        }

        /// <summary>
        /// Adds a new structure to the Secondary Structures list.
        /// </summary>
        /// <returns>Returns true if the added structure is in the Structures list once created.</returns>
        public BlueprintStructure AddStructure(HEStructureSceneID sceneID)
        {

            BlueprintStructure newStructure = new BlueprintStructure
            {
                SceneID = sceneID,

                StructureID = Structures.Count()
            };
            // Set this blueprint as the owner.
            newStructure.OwnerObject = this;
            // Populate the DockingPorts collection with ports appropriate for this structure type.
            newStructure.AddAppropriateDockingPorts();

            // Add the new structure to the main Structures list.
            Structures.Add(newStructure);

            // Add the new structure to the Secondary Structures list.
            //SecondaryStructures.Add(newStructure);

            newStructure.IsStructureHierarchyRoot = true;


            ReconnectChildToParentObjectHierarchy();

            IsDirty = true;

            return newStructure;


        }

        public bool RemoveStructure(int? id)
        {
            return RemoveStructure(GetStructure(id));
        }

        public bool RemoveStructure(BlueprintStructure structure)
        {

            if (structure != null)
                structure.Remove(RemoveOrphanedStructures: true);
            else
                return false;

            IsDirty = true;



            return Structures.Contains(structure) ? true : false;

        }

        /// <summary>
        /// Docks the structures that the two specified ports belong to.
        /// </summary>
        /// <param name="portA"></param>
        /// <param name="portB"></param>
        /// <returns></returns>
        public DockingResultStatus DockPorts(HEBlueprintDockingPort portA, HEBlueprintDockingPort portB)
        {
            // portA should be the Primary Structure side, portB the Secondary Structure.

            // Check the passed ports are valid.
            if (portA == null) return DockingResultStatus.InvalidPortA;
            if (portB == null) return DockingResultStatus.InvalidPortB;

            // check the ports parent structures are valid.
            if (portA.OwnerObject == null) return DockingResultStatus.InvalidStructurePortA;
            if (portB.OwnerObject == null) return DockingResultStatus.InvalidStructurePortB;

            // Ensure that the two ports aren't on the same structure.
            if (portA.OwnerObject == portB.OwnerObject) return DockingResultStatus.PortsOnSameStructure;

            // Ensure that both ports are not already docked.
            if (portA.IsDocked) return DockingResultStatus.AlreadyDockedPortA;
            if (portB.IsDocked) return DockingResultStatus.AlreadyDockedPortB;

            // Proceed with docking operation.

            // Update portA.
            portA.DockedStructure = portB.OwnerObject;
            portA.DockedPort = portB;

            // Update portB.
            portB.DockedStructure = portA.OwnerObject;
            portB.DockedPort = portA;
            portB.OwnerObject.IsStructureHierarchyRoot = false;

            // Mark the blueprint object as dirty.
            IsDirty = true;

            return DockingResultStatus.Success;
        }

        /// <summary>
        /// Un-docks the structure that the specified port belongs to from whatever it's docked to.
        /// </summary>
        /// <param name="portA"></param>
        /// <returns></returns>
        public DockingResultStatus UndockPort(HEBlueprintDockingPort portA)
        {
            if (portA == null) return DockingResultStatus.InvalidPortA;
            if (!portA.IsDocked) return DockingResultStatus.PortANotDocked;

            // Find structure A (the one selected)
            BlueprintStructure structureA = portA.OwnerObject;
            if (structureA == null) return DockingResultStatus.InvalidStructurePortA;

            // Find portB (the other side)
            HEBlueprintDockingPort portB = portA.DockedPort;

            if (portB == null) return DockingResultStatus.InvalidPortB;
            if (!portB.IsDocked) return DockingResultStatus.PortBNotDocked;

            // Find structureB
            BlueprintStructure structureB = portA.DockedStructure;
            if (structureB == null) return DockingResultStatus.InvalidStructurePortB;


            if (portA.DockedStructure != structureB || portB.DockedStructure != structureA)
                return DockingResultStatus.PortAandBNotDocked;

            // Process the un-docking.

            // Set the DockedStructures to null
            portA.DockedStructure = null;
            portA.DockedPort = null;

            portB.DockedStructure = null;
            portB.DockedPort = null;

            //portB.OwnerObject.IsStructureHierarchyRoot = true;

            // Figure out which structure to add to the Secondary Structures list.

            if (structureA.IsConnectedToPrimaryStructure)
            {
                if (structureB.IsConnectedToPrimaryStructure) throw new InvalidOperationException("Both structures connected to primary.");

                // structureB is not connected to the Primary.
                // Check if structureB is connected to a secondary root.
                if (structureB.GetStructureRoot() != null) throw new InvalidOperationException("structureB is connected to a root.");

                // Structure B is not connected to a root, make it one.
                structureB.IsStructureHierarchyRoot = true;
            }

            if (structureB.IsConnectedToPrimaryStructure)
            {
                if (structureA.IsConnectedToPrimaryStructure) throw new InvalidOperationException("Both structures connected to primary.");

                // structureB is not connected to the Primary.
                // Check if structureB is connected to a secondary root.
                if (structureA.GetStructureRoot() != null) throw new InvalidOperationException("structureA is connected to a root.");

                // Structure B is not connected to a root, make it one.
                structureA.IsStructureHierarchyRoot = true;
            }



            structureB.IsStructureHierarchyRoot = true;

            //SecondaryStructures.Add(structureB);

            IsDirty = true;

            return DockingResultStatus.Success;
        }

        /*
        /// <summary>
        /// Generates a serialisation template for this blueprint.
        /// </summary>
        /// <returns></returns>
        public SerialisationTemplate_Blueprint GetSerialisationTemplate()
        {

            if (StructureDefinitions == null) throw new NullReferenceException("Unable to serialise - StructureDefinitions is null.");

            SerialisationTemplate_Blueprint tp_Blueprint = new SerialisationTemplate_Blueprint
            {
                Name = Name,
                LinkURI = LinkURI
            };

            if (PrimaryStructureRoot == null) throw new NullReferenceException("Unable to serialise - PrimaryStrctureRoot was null.");

            // Define a list to track structures that have been visited and processed to prevent loops.
            List<BlueprintStructure> _visitedStructures = new List<BlueprintStructure>();

            // Start at the primary root.
            CloneStructuresToTemplate(PrimaryStructureRoot);

            // Return the blueprint serialisation template.
            return tp_Blueprint;


            /// <summary>
            /// Clones the blueprints structure list in to the new template's structures list.
            /// </summary>
            SerialisationTemplate_Structure CloneStructuresToTemplate(BlueprintStructure blueprintStructure)
            {
                // Check that this structure hasn't already been processed.
                if (!_visitedStructures.Contains(blueprintStructure))
                {
                    // Add the blueprintStructure to the visited list to prevent it being processed again.
                    _visitedStructures.Add(blueprintStructure);

                    // Build the new structure's serialisation template.
                    SerialisationTemplate_Structure structureTemplate = BuildTemplateStructure(
                        (HEBlueprintStructureType)blueprintStructure.StructureType);

                    // Add the new serialisation template to the blueprint template's structures list.
                    tp_Blueprint.Structures.Add(structureTemplate);

                    // Set the ID as per the blueprint Structure's ID.
                    structureTemplate.StructureID = (int)blueprintStructure.StructureID;

                    // Process any ports that are docked to another structure.
                    foreach (HEBlueprintDockingPort dockedPort in blueprintStructure.DockingPorts
                        .Where(p => p.IsDocked))
                    {
                        BlueprintStructure nextStructure = dockedPort.DockedStructure
                            ?? throw new NullReferenceException("nextStructure was null.");

                        
                        Debug.Print("structureTemplate: " + structureTemplate.StructureType.ToString());
                        Debug.Print("nextStructure: " + nextStructure.StructureType.ToString());
                        foreach (var port in nextStructure.DockingPorts)
                            Debug.Print("Port: " + port.PortName.ToString() 
                                + " DockedTo: " + (port.IsDocked ? port.DockedStructure.StructureType.ToString() : "Not Docked"));
                        

                        // Find the next structures port that connects it to this structure.
                        HEBlueprintDockingPort nextStructurePort = nextStructure.GetDockingPort(blueprintStructure)
                            ?? throw new NullReferenceException("nextStructurePort was null.");

                        // Add the next structure template via recursion and get a reference to it.
                        SerialisationTemplate_Structure nextStructureTemplate =
                            CloneStructuresToTemplate(nextStructure)
                            ?? throw new NullReferenceException("nextStructureTemplate was null.");


                        //Find the relevant local template port
                        IEnumerable<SerialisationTemplate_DockingPort> localPortResults = structureTemplate.DockingPorts
                            .Where(lp => lp.PortName == dockedPort.PortName);

                        if (localPortResults.Count() > 1) throw new InvalidOperationException(
                            "More than one template port where there should be only one.");
                        if (localPortResults.Count() < 1) throw new InvalidOperationException(
                            "Specified port not found.");

                        SerialisationTemplate_DockingPort localTemplatePort = localPortResults.Single();

                        // Find the next structures template port.

                        IEnumerable<SerialisationTemplate_DockingPort> remotePortResults = nextStructureTemplate.DockingPorts
                        .Where(rp => rp.PortName == nextStructurePort.PortName);

                        if (remotePortResults.Count() > 1) throw new InvalidOperationException(
                            "More than one template port where there should be only one.");
                        if (remotePortResults.Count() < 1) throw new InvalidOperationException(
                            "Specified port not found.");

                        SerialisationTemplate_DockingPort remoteTemplatePort = remotePortResults.Single();

                        // Connect up the ports.

                        // Set local port.
                        localTemplatePort.DockedStructureID = nextStructureTemplate.StructureID;
                        localTemplatePort.DockedPortName = remoteTemplatePort.PortName;

                        // Set remote port.
                        remoteTemplatePort.DockedStructureID = structureTemplate.StructureID;
                        remoteTemplatePort.DockedPortName = localTemplatePort.PortName;



                    }

                    return structureTemplate;


                }
                else
                {
                    // The structure has been processed previously - find the item and return a 
                    // reference to it rather than creating it.

                    IEnumerable<SerialisationTemplate_Structure> _results = tp_Blueprint.Structures
                        .Where(s => s.StructureID == (int)blueprintStructure.StructureID);

                    // There should be only one result.
                    return _results.Count() == 1 ? _results.Single()
                        : throw new InvalidOperationException("More or less than a single result where only one was expected.");
                }

            }

            /// <summary>
            /// Adds a structure of specified type to the Structures list.
            /// </summary>
            SerialisationTemplate_Structure BuildTemplateStructure(HEBlueprintStructureType structureType)
            {
                SerialisationTemplate_Structure templateStructure = new SerialisationTemplate_Structure
                {
                    StructureType = structureType
                };

                // Find the matching definition type for this structures type.
                HEBlueprintStructureDefinitions.HEBlueprintStructureDefinition defn = StructureDefinitions
                    .StructureDefinitions.Where(f => (f.DisplayName == structureType.ToString()
                    
                    )).Single();

                foreach (HEBlueprintStructureDefinitions.HEBlueprintStructureDefinitionDockingPort dockingPort in defn.DockingPorts)
                {
                    SerialisationTemplate_DockingPort newPort = new SerialisationTemplate_DockingPort()
                    {
                        PortName = (HEDockingPortType)dockingPort.PortName,
                        OrderID = (int)dockingPort.OrderID,
                    };
                    templateStructure.DockingPorts.Add(newPort);
                }
                return templateStructure;
            }


        }
        */

        #endregion

        #region Fields

        private BlueprintStructure _primaryStructureRoot = null;

        #endregion

        #region Enumerations

        /// <summary>
        /// Defines  types of Blueprint Object Type.
        /// </summary>
        public enum BlueprintObjectType
        {
            Unspecified = 0,
            Unknown,
            BlueprintStructureDefinitions,
            StationBlueprint,
        }

        /// <summary>
        /// Enumeration for the results of a docking operation.
        /// </summary>
        public enum DockingResultStatus
        {
            Success = 0,
            InvalidPortA,
            InvalidPortB,
            InvalidStructurePortA,
            InvalidStructurePortB,
            AlreadyDockedPortA,
            AlreadyDockedPortB,
            PortAandBNotDocked,
            PortANotDocked,
            PortBNotDocked,
            PortsOnSameStructure,
            IncompatiblePortTypes,
            WillCauseOrphanedStructure,
        }

        #endregion

        /// <summary>
        /// A class to define structures (modules/ships) within the blueprint.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class BlueprintStructure : Iparent_Base_TN
        {
            #region Constructors

            /// <summary>
            /// Basic constructor.
            /// </summary>
            public BlueprintStructure()
            {
                DockingPorts = new List<HEBlueprintDockingPort>();
                RootNode = new BlueprintStructure_TN(passedOwner: this);
                //    , nodeName: StructureType?.ToString() ?? "Error");

                // RefreshNodeName();

                //String.Format("[{0:000}] {1} ", StructureID, StructureType));

                //RootNode.PrefixNodeName = StructureID != null ? String.Format("[{0:000}] ", (int)StructureID) : "[ERR] ";
                //RootNode.RefreshName();

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
            public HEStructureSceneID? SceneID
            {
                get => _sceneID;
                set
                {
                    if (_sceneID != value)
                    {
                        _sceneID = value;

                        // Update the RootNode's BaseNodeName.


                        //RootNode.Name = value.GetEnumDescription();
                        // RootNode.RefreshText();  // This should now be handled by the P
                        
                    }
                }
            }

            /// <summary>
            /// The SceneName of the Structure. 
            /// Shadow Property - also comes from the SceneID field using the SceneID enum.
            /// </summary>
            [JsonProperty]
            [JsonConverter(typeof(StringEnumConverter))]
            public HEStructureSceneID? SceneName { get => SceneID; set => SceneID = value; }
            // public bool ShouldSerializeSceneName() { return OwnerObject == null || (OwnerObject != null && OwnerObject.IsTemplate) ? true : false; }

            [JsonProperty]
            public String StructureType
            {
                get => SceneID?.GetEnumDescription(); // ?? HEBlueprintStructureSceneID.Unspecified.ToString();
                set
                {
                    if (value != null && value.Length > 0)
                    {

                        // Attempt to parse the given description to an available one in the enum.
                        HEStructureSceneID descriptionParseResult = value.ParseToEnumDescriptionOrEnumerator<HEStructureSceneID>();

                        if (descriptionParseResult != HEStructureSceneID.Unspecified)
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
            public HEBlueprintStructureAuxData AuxData { get; set; } = null;


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

            /// <summary>
            /// The list of docking ports for this individual structure.
            /// </summary>
            [JsonProperty]
            public List<HEBlueprintDockingPort> DockingPorts { get; set; } = null;

            #endregion

            #region Methods

            /// <summary>
            /// Gets a docking port by name.
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public HEBlueprintDockingPort GetDockingPort(string name)
            {
                if (name == null || name == "" || !(DockingPorts.Count > 0)) return null;

                IEnumerable<HEBlueprintDockingPort> results = DockingPorts.
                    Where(f => f.PortName.ToString() == name);

                return results.Count() == 1 ? results.Single() : null;
            }

            /// <summary>
            /// Gets a docking port by the ID of the structure it's docked to.
            /// </summary>
            /// <param name="dockedStructureID"></param>
            /// <returns></returns>
            public HEBlueprintDockingPort GetDockingPort(int dockedStructureID)
            {
                if (!(dockedStructureID >= 0) || !(dockedStructureID <= OwnerObject.Structures.Count)
                    || !(DockingPorts.Count > 0)) return null;

                IEnumerable<HEBlueprintDockingPort> results = DockingPorts.
                    Where(f => f.DockedStructureID == dockedStructureID);

                return results.Count() == 1 ? results.Single() : null;
            }

            /// <summary>
            /// Gets a docking port by the structure it's docked to.
            /// </summary>
            /// <param name="dockedStructureID"></param>
            /// <returns></returns>
            public HEBlueprintDockingPort GetDockingPort(BlueprintStructure dockedStructure)
            {
                if (dockedStructure == null || !(DockingPorts.Count > 0)) return null;

                IEnumerable<HEBlueprintDockingPort> results = DockingPorts.
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
                foreach (HEBlueprintDockingPort port in DockingPorts.Where(p => p.IsDocked))
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
                // Find the matching definition type for this structures type.
                if (OwnerObject.StructureDefinitions == null) throw new NullReferenceException();

                BlueprintStructure defn = OwnerObject.StructureDefinitions.Structures
                    .Where(f => (int)f.SceneID == (int)SceneID).Single();

                foreach (HEBlueprintDockingPort dockingPort in defn.DockingPorts)
                {
                    HEBlueprintDockingPort newPort = new HEBlueprintDockingPort()
                    {
                        OwnerObject = this,
                        PortName = dockingPort.PortName,
                        OrderID = dockingPort.OrderID,
                        DockedStructureID = null
                    };
                    DockingPorts.Add(newPort);
                    RootNode.Nodes.Add(newPort.RootNode);
                }
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
                RootNode.NodeType = IsStructureHierarchyRoot ? HETreeNodeType.BlueprintRootStructure : HETreeNodeType.BlueprintStructure;
            }

            #endregion

            #region Fields

            protected int? _structureID = null;
            protected HEStructureSceneID? _sceneID = null;
            //protected HEBlueprintStructureType? _structureType = null;
            protected int? _previousStructureID = null;
            protected bool _isStructureHierarchyRoot = false;

            #endregion

        }

        /// <summary>
        /// A class to define the docking ports of a structure (module/ship) within the 
        /// blueprint.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class HEBlueprintDockingPort : Iparent_Base_TN
        {
            #region Constructors

            /// <summary>
            /// Basic Constructor.
            /// </summary>
            public HEBlueprintDockingPort()
            {
                RootNode = new BlueprintDockingPort_TN(passedOwner: this,
                    nodeName: PortName.ToString())
                {
                    Text_Prefix = OwnerObject != null && OwnerObject.StructureID != null ?
                        String.Format("[{0:000}] ", (int)OwnerObject.StructureID) : "[ERR] "
                };
                //RootNode.RefreshName();

            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="passedParent"></param>
            public HEBlueprintDockingPort(BlueprintStructure passedParent = null) : this()
            {
                OwnerObject = passedParent;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Parent object - not to be included in serialisation.
            /// </summary>
            public BlueprintStructure OwnerObject
            {
                get => _ownerObject;
                set
                {
                    _ownerObject = value;
                    RefreshAfterParentStructureIDChange();
                }
            }

            /// <summary>
            /// Not to be serialised.
            /// </summary>
            public Base_TN RootNode { get; set; } = null;


            /// <summary>
            /// The structure object of the port this port is docked to.
            /// </summary>
            /// <remarks>
            /// Updates the DockedStructureID.
            /// </remarks>
            public BlueprintStructure DockedStructure
            {
                get => _dockedStructure;
                set
                {
                    if (_dockedStructure != value)
                    {
                        _dockedStructure = value;

                        // Attempt to update the DockedSctuctureID
                        //if (_dockedStructure != null)
                        //{
                        DockedStructureID = _dockedStructure?.StructureID;
                        //}

                        if (_dockedStructure == null) DockedPort = null;


                    }
                }
            }


            /// <summary>
            /// The Port object this port is docked to.
            /// </summary>
            public HEBlueprintDockingPort DockedPort
            {
                get => _dockedPort;
                set
                {
                    if (_dockedPort != value)
                    {
                        _dockedPort = value;

                        // Attempt to update the DockedPortName

                        //if (_dockedPort != null)
                        //{
                        DockedPortName = _dockedPort?.PortName;
                        //}

                        if (_dockedPort == null) DockedStructure = null;


                    }
                }
            }

            /// <summary>
            /// Indicates whether this port is docked to a port on another structure.
            /// </summary>
            public bool IsDocked
            {
                get => DockedStructure == null || DockedStructureID == null ? false : true;
            }

            #endregion

            #region Serialised Properties

            /// <summary>
            /// The (standardised) name of the docking port.
            /// </summary>
            [JsonProperty]
            [JsonConverter(typeof(StringEnumConverter))]
            public HEDockingPortType? PortName
            {
                get => _portName;
                set
                {
                    if (_portName != value)
                    {
                        _portName = value;
                        RootNode.Name = _portName.ToString();
                        //RootNode.BaseNodeText = RootNode.Name;
                    }
                }
            }

            /// <summary>
            /// The OrderId of the docking port - these are used by the game deserialiser to spawn
            /// blueprints.
            /// </summary>
            [JsonProperty]
            public int? OrderID { get; set; } = null;

            /// <summary>
            /// Whether this docking port is locked (and not advertised to the docking system.)
            /// </summary>
            [JsonProperty]
            public bool? Locked { get; set; } = null;

            /// <summary>
            /// The ID of the structure this port is docked to.
            /// </summary>
            /// <remarks>
            /// This is to be serialised.
            /// This should only be set initially by the JToken.ToObject()
            /// Attempts to set the DockedStructure object.
            /// </remarks>
            [JsonProperty]
            public int? DockedStructureID
            {
                get => _dockedStructureID;
                set
                {
                    if (_dockedStructureID != value)
                    {
                        _dockedStructureID = value;

                        AttemptUpdateDockedStructure();

                    }
                }
            }

            /// <summary>
            /// The name of the port this port is docked to.
            /// </summary>
            /// <remarks>
            /// This is to be serialised.
            /// Updates the DockedStructureID.
            /// </remarks>
            [JsonProperty]
            [JsonConverter(typeof(StringEnumConverter))]
            public HEDockingPortType? DockedPortName
            {
                get => _dockedPortName;
                set
                {
                    if (_dockedPortName != value)
                    {
                        _dockedPortName = value;

                        AttemptUpdateDockedPort();
                    }
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Attempt to update the DockedStructure object.
            /// </summary>
            /// <returns>
            /// Return true if a non null was returned from the GetStructureByID.
            /// </returns>
            public bool AttemptUpdateDockedStructure()
            {
                if (OwnerObject == null || OwnerObject.OwnerObject == null) return false;
                DockedStructure = OwnerObject.OwnerObject.GetStructure(_dockedStructureID);
                return DockedStructure != null ? true : false;
            }

            /// <summary>
            /// Attempt to update the DockedPort object.
            /// </summary>
            /// <returns>
            /// Return true if a non null was returned from the GetStructureByID.
            /// </returns>
            public bool AttemptUpdateDockedPort()
            {
                if (DockedStructure == null || OwnerObject.OwnerObject == null) return false;
                DockedPort = DockedStructure.GetDockingPort(_dockedPortName.ToString());
                return DockedPort != null ? true : false;
            }

            public void RefreshAfterParentStructureIDChange()
            {
                RootNode.Text_Prefix = OwnerObject != null ? String.Format("[{0:000}] ", (int)OwnerObject.StructureID) : "[ERR] ";
                RootNode.Name = _portName.ToString();

                //RootNode.RefreshText();
                //RootNode.RefreshName();
            }

            #endregion

            #region Fields

            private HEDockingPortType? _portName = null;
            private BlueprintStructure _ownerObject = null;
            private int? _dockedStructureID = null;
            private BlueprintStructure _dockedStructure = null;
            private HEDockingPortType? _dockedPortName = null;
            private HEBlueprintDockingPort _dockedPort = null;

            #endregion

        }


        /// <summary>
        /// Defines optional parameters for a station blueprint structure.
        /// </summary>
        /// <remarks>
        /// Can also be attached to a blueprint directly.
        /// </remarks>
        [JsonObject(MemberSerialization.OptOut)]
        public class HEBlueprintStructureAuxData
        {
            // Optional parameters - structure level
            public bool? Invulnerable = null;                   // Invulnerable or will take damage.
            public bool? SystemsOnline = null;                  // Spawns powered on.
            public bool? PowerGeneratorsOnline = null;          // Power generators (solar panels/reactors) spawn powered on.
            public bool? DoorsLocked = null;                    // Doors are locked.
            public bool? DockingPortsLocked = null;             // Docking ports are locked (not advertised to docking systems).
            public bool? CryoPodsDisabled = null;               // Cryo pods are deactivated (non interact-able)
            public bool? DockingReleaseHandlesDisabled = null;  // Modules' docking release handles are visible.
            public bool? TextLabelEditingDisabled = null;       // Text labels (doors, parts boxes) are editable.
            public bool? SecurityPanelsDisabled = null;         // Security panel(s) are disabled or deactivated.
            public bool? SystemPartsInteractionDisabled = null; // Dynamic Object Parts in systems cannot be interacted with.

            /// <summary>
            /// Default constructor - able to pre-set values for a prefab.
            /// </summary>
            /// <param name="isPrefabStation"></param>
            public HEBlueprintStructureAuxData(bool? isPrefabStation = null)
            {
                SetAllBools(isPrefabStation);
            }

            public void SetAllBools(bool? value)
            {
                Invulnerable = value;
                SystemsOnline = value;
                PowerGeneratorsOnline = value;
                DoorsLocked = value;    // Current prefabs don't (usually) have locked doors.
                DockingPortsLocked = value;
                CryoPodsDisabled = value;
                DockingReleaseHandlesDisabled = value;
                TextLabelEditingDisabled = value;
                SecurityPanelsDisabled = value;
                SystemPartsInteractionDisabled = value;

            }

            public void ResetAuxData()
            {
                Invulnerable = null;
                SystemsOnline = null;
                PowerGeneratorsOnline = null;
                DoorsLocked = null;
                DockingPortsLocked = null;
                CryoPodsDisabled = null;
                DockingReleaseHandlesDisabled = null;
                TextLabelEditingDisabled = null;
                SecurityPanelsDisabled = null;
                SystemPartsInteractionDisabled = null;
            }
        }

    }

}
