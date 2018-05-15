using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HELLION.DataStructures
{
    /// <summary>
    /// A class to handle station blueprint data structures.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class HEStationBlueprint
    {
        public const decimal StationBlueprintFormatVersion = 0.37m;

        #region Constructors

        /// <summary>
        /// Default Constructor, called directly by the JToken.ToObject<T>()
        /// </summary>
        public HEStationBlueprint()
        {
            Structures = new List<HEBlueprintStructure>();
            //SecondaryStructures = new List<HEBlueprintStructure>();
            RootNode = new HEBlueprintTreeNode(passedOwner: this, nodeName: "Hierarchy View",
                newNodeType: HETreeNodeType.BlueprintHierarchyView,
                nodeToolTipText: "Shows a tree-based view of the modules and their docking hierarchy.");


        }

        /// <summary>
        /// Constructor that takes a HEStationonBlueprintFile Owner Object and reference to the 
        /// structure definitions object.
        /// </summary>
        /// <param name="ownerObject"></param>
        /// <param name="structureDefs"></param>
        public HEStationBlueprint(HEStationBlueprintFile ownerObject, HEBlueprintStructureDefinitions structureDefs) : this()
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
        public HEStationBlueprintFile OwnerObject { get; set; } = null;

        public HEBlueprintTreeNode RootNode { get; set; } = null;

        public HEBlueprintStructureDefinitions StructureDefinitions { get; set; } = null;

        public bool IsTemplate { get; protected set; } = false;

        public bool IsDirty { get; protected set; } = false;

        public HEBlueprintStructure PrimaryStructureRoot
        {
            get => _primaryStructureRoot;
            set
            {
                if (_primaryStructureRoot != value)
                {
                    HEBlueprintStructure oldValue = _primaryStructureRoot;
                    _primaryStructureRoot = value;
                }
            }
        }

        /// <summary>
        /// The list of all Secondary Structures - these are not technically part of a finished
        /// blueprint and are linked here to allow processing separately to the primary structure.
        /// </summary>
        public List<HEBlueprintStructure> SecondaryStructureRoots
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
        public List<HEBlueprintStructure> Structures { get; set; } = null;

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void PostDeserialisationInit()
        {
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

            StructureDefinitions = OwnerObject.OwnerObject.OwnerObject
                .StructureDefinitionsFile.BlueprintStructureDefinitionsObject;

            foreach (HEBlueprintStructure structure in Structures)
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
            foreach (HEBlueprintStructure structure in Structures)
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
            HEBlueprintStructure result = GetStructure(0);
            PrimaryStructureRoot = result ?? throw new NullReferenceException("Unable to locate the blueprint's root structure (index zero).");
            PrimaryStructureRoot.IsStructureHierarchyRoot = true;
        }

        /// <summary>
        /// Refreshes all tree node structures for the Primary and Secondary structures.
        /// </summary>
        public void RefreshAllTreeNodes()
        {
            // Break any existing parent/child relationships.
            foreach (HEBlueprintStructure structure in Structures)
            {
                structure.OwnerObject.RootNode.Nodes.Remove(structure.RootNode);
                foreach (HEBlueprintDockingPort port in structure.DockingPorts)
                {
                    port.OwnerObject.RootNode.Nodes.Remove(port.RootNode);
                }
            }

            // Assemble the primary blueprint structure's docked node tree.
            ReassembleTreeNodeDockingStructure(PrimaryStructureRoot);

            // Assemble the secondary structures node trees.
            foreach (HEBlueprintStructure _secondaryStructure in SecondaryStructureRoots)
            {
                ReassembleTreeNodeDockingStructure(_secondaryStructure);
            }
        }

        /// <summary>
        /// Builds a tree of nodes representing the docking structure, including nodes for each docking port.
        /// </summary>
        public void ReassembleTreeNodeDockingStructure(HEBlueprintStructure hierarchyRoot, bool AttachToBlueprintTreeNode = false)
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
            void Reassemble(HEBlueprintStructure structure, HEBlueprintStructure parent)
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
        public HEBlueprintStructure GetStructure(int? structureID)
        {
            if (structureID == null) return null;
            IEnumerable<HEBlueprintStructure> results = Structures.Where(f => f.StructureID == structureID);
            if (results.Count() < 1 || results.Count() > 1) return null;
            return results.Single();
        }

        /// <summary>
        /// Adds a new structure to the Secondary Structures list.
        /// </summary>
        /// <returns>Returns true if the added structure is in the Structures list once created.</returns>
        public HEBlueprintStructure AddStructure(HEBlueprintStructureSceneID sceneID)
        {

            HEBlueprintStructure newStructure = new HEBlueprintStructure
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

        public bool RemoveStructure(HEBlueprintStructure structure)
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
        public HEDockingResultStatus DockPorts(HEBlueprintDockingPort portA, HEBlueprintDockingPort portB)
        {
            // portA should be the Primary Structure side, portB the Secondary Structure.

            // Check the passed ports are valid.
            if (portA == null) return HEDockingResultStatus.InvalidPortA;
            if (portB == null) return HEDockingResultStatus.InvalidPortB;

            // check the ports parent structures are valid.
            if (portA.OwnerObject == null) return HEDockingResultStatus.InvalidStructurePortA;
            if (portB.OwnerObject == null) return HEDockingResultStatus.InvalidStructurePortB;

            // Ensure that the two ports aren't on the same structure.
            if (portA.OwnerObject == portB.OwnerObject) return HEDockingResultStatus.PortsOnSameStructure;

            // Ensure that both ports are not already docked.
            if (portA.IsDocked) return HEDockingResultStatus.AlreadyDockedPortA;
            if (portB.IsDocked) return HEDockingResultStatus.AlreadyDockedPortB;

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

            return HEDockingResultStatus.Success;
        }

        /// <summary>
        /// Un-docks the structure that the specified port belongs to from whatever it's docked to.
        /// </summary>
        /// <param name="portA"></param>
        /// <returns></returns>
        public HEDockingResultStatus UndockPort(HEBlueprintDockingPort portA)
        {
            if (portA == null) return HEDockingResultStatus.InvalidPortA;
            if (!portA.IsDocked) return HEDockingResultStatus.PortANotDocked;

            // Find structure A (the one selected)
            HEBlueprintStructure structureA = portA.OwnerObject;
            if (structureA == null) return HEDockingResultStatus.InvalidStructurePortA;

            // Find portB (the other side)
            HEBlueprintDockingPort portB = portA.DockedPort;

            if (portB == null) return HEDockingResultStatus.InvalidPortB;
            if (!portB.IsDocked) return HEDockingResultStatus.PortBNotDocked;

            // Find structureB
            HEBlueprintStructure structureB = portA.DockedStructure;
            if (structureB == null) return HEDockingResultStatus.InvalidStructurePortB;


            if (portA.DockedStructure != structureB || portB.DockedStructure != structureA)
                return HEDockingResultStatus.PortAandBNotDocked;

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

            return HEDockingResultStatus.Success;
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
            List<HEBlueprintStructure> _visitedStructures = new List<HEBlueprintStructure>();

            // Start at the primary root.
            CloneStructuresToTemplate(PrimaryStructureRoot);

            // Return the blueprint serialisation template.
            return tp_Blueprint;


            /// <summary>
            /// Clones the blueprints structure list in to the new template's structures list.
            /// </summary>
            SerialisationTemplate_Structure CloneStructuresToTemplate(HEBlueprintStructure blueprintStructure)
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
                        HEBlueprintStructure nextStructure = dockedPort.DockedStructure
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

        private HEBlueprintStructure _primaryStructureRoot = null;

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
        /// The primary Enum for referencing the structures and their IDs and Descriptions.
        /// </summary>
        public enum HEBlueprintStructureSceneID
        {
            //SolarSystemSetup = -3,
            //ItemScene = -2,
            //None = -1,

            Unspecified = 0,

            //Slavica = 1,
            //[Description("BRONTES")]
            //AltCorp_Ship_Tamara = 2,

            [Description("CIM")]
            AltCorp_CorridorModule = 3,
            [Description("CTM")]
            AltCorp_CorridorIntersectionModule = 4,
            [Description("CLM")]
            AltCorp_Corridor45TurnModule = 5,

            [Description("ARGES")]
            AltCorp_Shuttle_SARA = 6,
            [Description("PSM")]
            ALtCorp_PowerSupply_Module = 7,
            [Description("LSM")]
            AltCorp_LifeSupportModule = 8,
            [Description("CBM")]
            AltCorp_Cargo_Module = 9,

            [Description("CSM")]
            AltCorp_CorridorVertical = 10, // 0x0000000A
            [Description("CM")]
            AltCorp_Command_Module = 11, // 0x0000000B
            [Description("CRM")]
            AltCorp_Corridor45TurnRightModule = 12, // 0x0000000C
            [Description("OUTPOST")]
            AltCorp_StartingModule = 13, // 0x0000000D
            [Description("AM")]
            AltCorp_AirLock = 14, // 0x0000000E

            Generic_Debris_JuncRoom001 = 15, // 0x0000000F
            Generic_Debris_JuncRoom002 = 16, // 0x00000010
            Generic_Debris_Corridor001 = 17, // 0x00000011
            Generic_Debris_Corridor002 = 18, // 0x00000012

            [Description("IC")]
            AltCorp_DockableContainer = 19, // 0x00000013

            MataPrefabs = 20, // 0x00000014
            Generic_Debris_Outpost001 = 21, // 0x00000015

            [Description("CQM")]
            AltCorp_CrewQuarters_Module = 22, // 0x00000016

            Generic_Debris_Spawn1 = 23, // 0x00000017
            Generic_Debris_Spawn2 = 24, // 0x00000018
            Generic_Debris_Spawn3 = 25, // 0x00000019

            [Description("SPM")]
            AltCorp_SolarPowerModule = 26, // 0x0000001A
            [Description("STEROPES")]
            AltCorp_Shuttle_CECA = 27, // 0x0000001B
            [Description("FM")]
            AltCorp_FabricatorModule = 28, // 0x0000001C
            FlatShipTest = 29, // 0x0000001D
            //Asteroid01 = 1000, // 0x000003E8
            //Asteroid02 = 1001, // 0x000003E9
            //Asteroid03 = 1002, // 0x000003EA
            //Asteroid04 = 1003, // 0x000003EB
            //Asteroid05 = 1004, // 0x000003EC
            //Asteroid06 = 1005, // 0x000003ED
            //Asteroid07 = 1006, // 0x000003EE
            //Asteroid08 = 1007, // 0x000003EF
        }

        /// <summary>
        /// Docking Port Types Enum.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum HEDockingPortType
        {
            Unspecified = 0,
            StandardDockingPortA,
            StandardDockingPortB,
            StandardDockingPortC,
            StandardDockingPortD,
            AirlockDockingPort,
            Grapple,    // LEGACY ITEM - DEPRECATED
            IndustrialContainerPortA,
            IndustrialContainerPortB,
            IndustrialContainerPortC,
            IndustrialContainerPortD,
            CargoDockingPortA,
            CargoDockingPortB,
            CargoDock,  // Dockable Cargo (IC) module
            Anchor,
            DerelictPort1,
            DerelictPort2,
        }

        /// <summary>
        /// Enumeration for the results of a docking operation.
        /// </summary>
        public enum HEDockingResultStatus
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

        public Dictionary<HEBlueprintStructureSceneID, Dictionary<HEDockingPortType, int>> DockingPortHints 
            = new Dictionary<HEBlueprintStructureSceneID, Dictionary<HEDockingPortType, int>>()
        {
            
            { HEBlueprintStructureSceneID.AltCorp_CorridorModule, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 1 },
                { HEDockingPortType.StandardDockingPortB, 2 },
                { HEDockingPortType.Anchor, 3}
            } },
            
            { HEBlueprintStructureSceneID.AltCorp_CorridorIntersectionModule, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 1 },
                { HEDockingPortType.StandardDockingPortB, 3 },
                { HEDockingPortType.StandardDockingPortC, 2 },
                { HEDockingPortType.Anchor, 4}
            } },

            { HEBlueprintStructureSceneID.AltCorp_Corridor45TurnModule, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 1 },
                { HEDockingPortType.StandardDockingPortB, 2 },
                { HEDockingPortType.Anchor, 3}
            } },

            { HEBlueprintStructureSceneID.AltCorp_Shuttle_SARA, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.AirlockDockingPort, 1 },
                { HEDockingPortType.Anchor, 2}
            } },

            { HEBlueprintStructureSceneID.ALtCorp_PowerSupply_Module, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 2 },
                { HEDockingPortType.StandardDockingPortB, 1 },
                { HEDockingPortType.Anchor, 3}
            } },

            { HEBlueprintStructureSceneID.AltCorp_LifeSupportModule, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 1 },
                { HEDockingPortType.StandardDockingPortB, 2 },
                { HEDockingPortType.Anchor, 3}
            } },

            { HEBlueprintStructureSceneID.AltCorp_Cargo_Module, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 1 },
                { HEDockingPortType.IndustrialContainerPortA, 4 },
                { HEDockingPortType.IndustrialContainerPortB, 5 },
                { HEDockingPortType.IndustrialContainerPortC, 7 },
                { HEDockingPortType.IndustrialContainerPortD, 6 },
                { HEDockingPortType.CargoDockingPortA, 2 },
                { HEDockingPortType.CargoDockingPortB, 3 },
                { HEDockingPortType.Anchor, 8}
            } },

            { HEBlueprintStructureSceneID.AltCorp_CorridorVertical, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 1 },
                { HEDockingPortType.StandardDockingPortB, 2 },
                { HEDockingPortType.Anchor, 3}
            } },

            { HEBlueprintStructureSceneID.AltCorp_Command_Module, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 3 },
                { HEDockingPortType.StandardDockingPortB, 2 },
                { HEDockingPortType.StandardDockingPortC, 1 },
                { HEDockingPortType.StandardDockingPortD, 4 },
                { HEDockingPortType.Anchor, 5}
            } },

            { HEBlueprintStructureSceneID.AltCorp_Corridor45TurnRightModule, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 1 },
                { HEDockingPortType.StandardDockingPortB, 2 },
                { HEDockingPortType.Anchor, 3}
            } },

            { HEBlueprintStructureSceneID.AltCorp_StartingModule, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 2 },
                { HEDockingPortType.StandardDockingPortB, 1 },
                { HEDockingPortType.Anchor, 3}
            } },

            { HEBlueprintStructureSceneID.Generic_Debris_JuncRoom001, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.DerelictPort1, 1 },
                { HEDockingPortType.DerelictPort2, 2 }
            } },

            { HEBlueprintStructureSceneID.Generic_Debris_JuncRoom002, new Dictionary<HEDockingPortType, int> { } },

            { HEBlueprintStructureSceneID.Generic_Debris_Corridor001, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.DerelictPort1, 1 }
            } },

            { HEBlueprintStructureSceneID.Generic_Debris_Corridor002, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.DerelictPort1, 1 },
                { HEDockingPortType.DerelictPort2, 2 }
            } },

            { HEBlueprintStructureSceneID.AltCorp_AirLock, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.AirlockDockingPort, 1 },
                { HEDockingPortType.StandardDockingPortA, 2 },
                { HEDockingPortType.Anchor, 3}
            } },

            { HEBlueprintStructureSceneID.AltCorp_DockableContainer, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 1 },
                { HEDockingPortType.Anchor, 2}
            } },

            { HEBlueprintStructureSceneID.Generic_Debris_Outpost001, new Dictionary<HEDockingPortType, int> { } },

            { HEBlueprintStructureSceneID.AltCorp_CrewQuarters_Module, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 1 },
                { HEDockingPortType.Anchor, 2}
            } },

            { HEBlueprintStructureSceneID.Generic_Debris_Spawn1, new Dictionary<HEDockingPortType, int> { } },

            { HEBlueprintStructureSceneID.Generic_Debris_Spawn2, new Dictionary<HEDockingPortType, int> { } },

            { HEBlueprintStructureSceneID.Generic_Debris_Spawn3, new Dictionary<HEDockingPortType, int> { } },

            { HEBlueprintStructureSceneID.AltCorp_SolarPowerModule, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 1 },
                { HEDockingPortType.StandardDockingPortB, 2 },
                { HEDockingPortType.Anchor, 3}
            } },

            { HEBlueprintStructureSceneID.AltCorp_Shuttle_CECA, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.Anchor, 1}
            } },

            { HEBlueprintStructureSceneID.AltCorp_FabricatorModule, new Dictionary<HEDockingPortType, int> {
                { HEDockingPortType.StandardDockingPortA, 1 },
                { HEDockingPortType.Anchor, 2}
            } },

            { HEBlueprintStructureSceneID.MataPrefabs, new Dictionary<HEDockingPortType, int> { } },

            { HEBlueprintStructureSceneID.FlatShipTest, new Dictionary<HEDockingPortType, int> { } },

        };
        
        

        /// <summary>
        /// A class to define structures (modules/ships) within the blueprint.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class HEBlueprintStructure
        {
            #region Constructors

            /// <summary>
            /// Basic constructor.
            /// </summary>
            public HEBlueprintStructure()
            {
                DockingPorts = new List<HEBlueprintDockingPort>();
                RootNode = new HEBlueprintStructureTreeNode(passedOwner: this,
                    nodeName: StructureType?.ToString() ?? "Error");

                //String.Format("[{0:000}] {1} ", StructureID, StructureType));

                //RootNode.PrefixNodeName = StructureID != null ? String.Format("[{0:000}] ", (int)StructureID) : "[ERR] ";
                //RootNode.RefreshName();

            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="ownerObject"></param>
            public HEBlueprintStructure(HEStationBlueprint ownerObject = null) : this()
            {
                OwnerObject = ownerObject;

            }

            #endregion

            #region Properties

            /// <summary>
            /// Parent object - not to be included in serialisation.
            /// </summary>
            public HEStationBlueprint OwnerObject { get; set; } = null;

            /// <summary>
            /// Not to be serialised.
            /// </summary>
            public HEBlueprintStructureTreeNode RootNode { get; set; } = null;

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
                    if (ConnectedStructures(new List<HEBlueprintStructure> { this })
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
            public HEBlueprintStructureSceneID? SceneID
            {
                get => _sceneID;
                set
                {
                    if (_sceneID != value)
                    {
                        _sceneID = value;

                        // Update the RootNode's BaseNodeName.
                        //RootNode.BaseNodeName = value.GetEnumDescription();

                    }


                }
            }

            /// <summary>
            /// The SceneName of the Structure. 
            /// Shadow Property - also comes from the SceneID field using the SceneID enum.
            /// </summary>
            [JsonProperty]
            [JsonConverter(typeof(StringEnumConverter))]
            public HEBlueprintStructureSceneID? SceneName { get => SceneID; set => SceneID = value; }
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
                        HEBlueprintStructureSceneID descriptionParseResult = value.ParseToEnumDescriptionOrEnumerator<HEBlueprintStructureSceneID>();

                        if (descriptionParseResult != HEBlueprintStructureSceneID.Unspecified)
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
            public HEBlueprintDockingPort GetDockingPort(HEBlueprintStructure dockedStructure)
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
            public HEBlueprintStructure GetStructureRoot()
            {
                // Is this structure a root itself?
                if (OwnerObject.PrimaryStructureRoot == this || OwnerObject.SecondaryStructureRoots.Contains(this)) return this;

                // Is this structure connected to the Primary Structure?
                if (IsConnectedToPrimaryStructure) return OwnerObject.PrimaryStructureRoot;

                // Full test - the intersection of this structure's connected structures list, and 
                // the Secondary Structures list.
                IEnumerable<HEBlueprintStructure> results = ConnectedStructures(new List<HEBlueprintStructure> { this })
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
            public List<HEBlueprintStructure> ConnectedStructures(List<HEBlueprintStructure> visitedtedStructures = null)
            {
                bool includeAllConnected = true;
                if (visitedtedStructures == null)
                {
                    includeAllConnected = false;
                    visitedtedStructures = new List<HEBlueprintStructure>();
                }

                // Loop over all of this structure's ports that are docked to something.
                foreach (HEBlueprintDockingPort port in DockingPorts.Where(p => p.IsDocked))
                {
                    // The port is docked; retrieve the structure by its ID.
                    HEBlueprintStructure _result = port.DockedStructure;

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

            public void AddAppropriateDockingPorts()
            {
                // Find the matching definition type for this structures type.

                if (OwnerObject.StructureDefinitions == null) throw new NullReferenceException();

                HEBlueprintStructureDefinitions.HEBlueprintStructureDefinition defn = OwnerObject.StructureDefinitions
                    .StructureDefinitions.Where(f => f.ItemID == (int)SceneID).Single();


                foreach (HEBlueprintStructureDefinitions.HEBlueprintStructureDefinitionDockingPort dockingPort in defn.DockingPorts)
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
                    RootNode.PrefixNodeName = String.Format("[{0:000}] ", (int)StructureID);
                    RootNode.RefreshName();

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
            protected HEBlueprintStructureSceneID? _sceneID = null;
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
        public class HEBlueprintDockingPort
        {
            #region Constructors

            /// <summary>
            /// Basic Constructor.
            /// </summary>
            public HEBlueprintDockingPort()
            {
                RootNode = new HEBlueprintDockingPortTreeNode(passedOwner: this,
                    nodeName: PortName.ToString())
                {
                    PrefixNodeName = OwnerObject != null && OwnerObject.StructureID != null ?
                        String.Format("[{0:000}] ", (int)OwnerObject.StructureID) : "[ERR] "
                };
                //RootNode.RefreshName();

            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="passedParent"></param>
            public HEBlueprintDockingPort(HEBlueprintStructure passedParent = null) : this()
            {
                OwnerObject = passedParent;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Parent object - not to be included in serialisation.
            /// </summary>
            public HEBlueprintStructure OwnerObject
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
            public HEBlueprintDockingPortTreeNode RootNode { get; set; } = null;


            /// <summary>
            /// The structure object of the port this port is docked to.
            /// </summary>
            /// <remarks>
            /// Updates the DockedStructureID.
            /// </remarks>
            public HEBlueprintStructure DockedStructure
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
                        RootNode.BaseNodeName = _portName.ToString();
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
                RootNode.PrefixNodeName = OwnerObject != null ? String.Format("[{0:000}] ", (int)OwnerObject.StructureID) : "[ERR] ";
                RootNode.RefreshName();
            }

            #endregion

            #region Fields

            private HEDockingPortType? _portName = null;
            private HEBlueprintStructure _ownerObject = null;
            private int? _dockedStructureID = null;
            private HEBlueprintStructure _dockedStructure = null;
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
                SetPrefabStation(isPrefabStation);
            }

            public void SetPrefabStation(bool? isPrefabStation)
            {
                Invulnerable = isPrefabStation;
                SystemsOnline = isPrefabStation;
                PowerGeneratorsOnline = isPrefabStation;
                DoorsLocked = false;    // Current prefabs don't (usually) have locked doors.
                DockingPortsLocked = isPrefabStation;
                CryoPodsDisabled = isPrefabStation;
                DockingReleaseHandlesDisabled = isPrefabStation;
                TextLabelEditingDisabled = isPrefabStation;
                SecurityPanelsDisabled = isPrefabStation;
                SystemPartsInteractionDisabled = isPrefabStation;

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

        /*
        public class SerialisationTemplate_Blueprint
        {
            public string __ObjectType = "StationBlueprint";
            public decimal Version = StationBlueprintFormatVersion;
            public string Name = null;
            public Uri LinkURI = null;
            public List<SerialisationTemplate_Structure> Structures = new List<SerialisationTemplate_Structure>();
        }
        public class SerialisationTemplate_Structure
        {
            public int StructureID;
            public HEBlueprintStructureType StructureType;
            public List<SerialisationTemplate_DockingPort> DockingPorts = new List<SerialisationTemplate_DockingPort>();
        }
        public class SerialisationTemplate_DockingPort
        {
            public HEDockingPortType PortName;
            public int OrderID;
            public int? DockedStructureID = null;
            public HEDockingPortType? DockedPortName = null;
        }
        */              
    }

    /*
    public class SceneIDStringEnumConverter : StringEnumConverter
    {

        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
        

        
        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            bool isNullable = ReflectionUtils.IsNullableType(objectType);
            Type t = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            if (t != typeof(HEBlueprintStructureSceneID))
                throw new Exception("Was called, but wrong type.");

            try
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string enumText = reader.Value.ToString();

                    if (enumText == string.Empty && isNullable)
                    {
                        return null;
                    }

                    HEBlueprintStructureSceneID descriptionParseResult = EnumExtensions.ParseToEnumDescriptionOrEnumerator<HEBlueprintStructureSceneID>(enumText);
                    if (descriptionParseResult != HEBlueprintStructureSceneID.Unspecified) return descriptionParseResult;

                    return Enum.TryParse(enumText, out HEBlueprintStructureSceneID result) ? (object)result : null;


                    //return EnumUtils.ParseEnum(t, enumText, !AllowIntegerValues);
                }

                if (reader.TokenType == JsonToken.Integer)
                {
                    if (!AllowIntegerValues)
                    {
                        throw new JsonSerializationException(String.Format("Integer value {0} is not allowed.", reader.Value));
                    }

                    return (HEBlueprintStructureSceneID)reader.Value;
                }
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException(String.Format("Error converting value {0} to type '{1}'.{2}{3}",
                    reader.Value, objectType, Environment.NewLine, ex));
            }

            // we don't actually expect to get here.
            throw new JsonSerializationException(String.Format("Unexpected token {0} when parsing enum.", reader.TokenType));
        }

        internal static class ValidationUtils
        {
            public static void ArgumentNotNull(object value, string parameterName)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(parameterName);
                }
            }
        }


        internal static class ReflectionUtils
        {

            public static bool IsNullable(Type t)
            {
                ValidationUtils.ArgumentNotNull(t, nameof(t));

                if (t.IsValueType)
                {
                    return IsNullableType(t);
                }

                return true;
            }

            public static bool IsNullableType(Type t)
            {
                ValidationUtils.ArgumentNotNull(t, nameof(t));

                return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
            }
        }
    }
    */

    /*
    /// <summary>
    /// LEGACY Structure Types Enum.
    /// </summary>
    /// <remarks>
    /// The numeric values of these correspond to the game's ItemID in the structures.json
    /// to allow for easier cross-referencing.
    /// </remarks>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum aHEBlueprintStructureType
    {
        Unspecified = 0,
        //BRONTES = 2,
        CIM = 3,
        CTM = 4,
        CLM = 5,
        ARG = 6,
        PSM = 7,
        LSM = 8,
        CBM = 9,
        CSM = 10,
        CM = 11,
        CRM = 12,
        OUTPOST = 13,
        AM = 14,
        Generic_Debris_JuncRoom001 = 15, // 0x0000000F
        Generic_Debris_JuncRoom002 = 16, // 0x00000010
        Generic_Debris_Corridor001 = 17, // 0x00000011
        Generic_Debris_Corridor002 = 18, // 0x00000012
        IC = 19, // 0x00000013
        //MataPrefabs = 20, // 0x00000014
        //Generic_Debris_Outpost001 = 21, // 0x00000015
        CQM = 22, // 0x00000016
        // Generic_Debris_Spawn1 = 23, // 0x00000017
        // Generic_Debris_Spawn2 = 24, // 0x00000018
        // Generic_Debris_Spawn3 = 25, // 0x00000019
        SPM = 26, // 0x0000001A
        STEROPES = 27, // 0x0000001B
        FM = 28, // 0x0000001C
        // FlatShipTest = 29, // 0x0000001D
    }
    */

    }
