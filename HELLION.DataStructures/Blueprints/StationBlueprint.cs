using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HELLION.DataStructures.StaticData;
using HELLION.DataStructures.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HELLION.DataStructures.Blueprints
{
    /// <summary>
    /// A class to handle station blueprint data structures.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class StationBlueprint : IParent_Base_TN
    {

        #region Constructors

        /// <summary>
        /// Default Constructor, called directly by the JToken.ToObject<T>()
        /// </summary>
        public StationBlueprint()
        {
            Structures = new List<BlueprintStructure>();
            RootNode = new Blueprint_TN(passedOwner: this, newNodeType: Base_TN_NodeType.BlueprintHierarchyView,
                nodeName: "Hierarchy View"); 
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
            StructureDefinitions = structureDefs; // ?? throw new NullReferenceException("structureDefs was null.");
            __ObjectType = BlueprintObjectType.StationBlueprint;
            Version = StationBlueprintFormatVersion;
        }

        #endregion

        #region Properties

        /// <summary>
        /// A reference to the Parent object.
        /// </summary>
        public StationBlueprint_File OwnerObject { get; set; } = null;

        public Base_TN RootNode { get; protected set; } = null;

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
        public BlueprintStructure_AuxData AuxData { get; set; } = null;

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
                foreach (BlueprintDockingPort port in structure.DockingPorts)
                {
                    port.RootNode.AutoGenerateName = true;
                    port.OwnerStructure = structure;
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
                foreach (BlueprintDockingPort port in structure.DockingPorts)
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

                foreach (BlueprintDockingPort port in structure.DockingPorts)
                {
                    port.OwnerStructure.RootNode.Nodes.Remove(port.RootNode);
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
            if (hierarchyRoot != null)
            {
                // Start with the root node, should be item zero in the list.
                Debug.Print("### Reassemble-main [{0}] [{1}]", hierarchyRoot.StructureType, hierarchyRoot.StructureID);
                // Process the docking root's ports slightly differently - always child nodes.
                if (AttachToBlueprintTreeNode) RootNode.Nodes.Add(hierarchyRoot.RootNode);

                foreach (BlueprintDockingPort port in hierarchyRoot.DockingPorts.ToArray().Reverse())
                {
                    hierarchyRoot.RootNode.Nodes.Add(port.RootNode);
                    if (port.IsDocked)
                    {
                        if (port.DockedStructure == null)
                        {
                            Debug.Print("&&& REASSEMBLE &&& Port.IsDocked = true but docked structure was null");
                            return;
                        }
                        Reassemble(port.DockedStructure, hierarchyRoot);
                    }
                }
            }

            /// <summary>
            /// The recursive bit.
            /// </summary>
            void Reassemble(BlueprintStructure structure, BlueprintStructure parent)
                {
                    if (structure == null) throw new NullReferenceException("Structure was null.");
                    Debug.Print("###  Reassemble-recurse [{0}] [{1}]", structure.StructureType, structure.StructureID);

                    // Figure out which port is docking us to the parent and vice versa.
                    BlueprintDockingPort linkToParent = structure.GetDockingPort(parent);
                    BlueprintDockingPort linkFromParent = parent.GetDockingPort(structure);

                    // Add the node for the link to parent to the link from parent's node collection.
                    linkFromParent.RootNode.Nodes.Add(linkToParent.RootNode);

                    // Add the structures's node to the link to parent node collection.
                    linkToParent.RootNode.Nodes.Add(structure.RootNode);

                    foreach (BlueprintDockingPort port in structure.DockingPorts.ToArray().Reverse())
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
        public BlueprintStructure AddStructure(StructureSceneID sceneID)
        {

            BlueprintStructure newStructure = new BlueprintStructure
            {
                SceneID = sceneID,
                StructureID = Structures.Count()
            };
            
            // Set this blueprint as the owner.
            newStructure.OwnerObject = this;

            // Populate the DockingPorts collection with ports appropriate
            // for this structure type.
            newStructure.AddAppropriateDockingPorts(); 

            // Add the new structure to the main Structures list.
            Structures.Add(newStructure);

            // Mark the new structure as a hierarchy root so that it appears
            // as a primary root or in the secondary list.
            newStructure.IsStructureHierarchyRoot = true;

            if (Structures.Count == 1 && PrimaryStructureRoot == null)
            {
                PrimaryStructureRoot = newStructure;
                IsDirty = true;
            }

            ReconnectChildToParentObjectHierarchy();


            return newStructure;

        }

        public bool RemoveStructure(int? id)
        {
            return RemoveStructure(GetStructure(id));
        }

        /// <summary>
        /// Removes a structure from the blueprint.
        /// </summary>
        /// <remarks>
        /// Currently the structure muse be undocked to be removed.
        /// </remarks>
        /// <param name="structure"></param>
        /// <returns></returns>
        public bool RemoveStructure(BlueprintStructure structure)
        {

            if (structure != null)
                structure.PrepareForRemoval(removeOrphanedStructures: false);
            else
                return false;

            // Actually remove the structure.
            Structures.Remove(structure);
            
            // Modifications to the secondary structures list don't constitute a modification of the blueprint.
            // IsDirty = true;
            
            return Structures.Contains(structure);

        }

        /// <summary>
        /// Docks the structures that the two specified ports belong to.
        /// </summary>
        /// <param name="portA"></param>
        /// <param name="portB"></param>
        /// <returns></returns>
        public DockingResultStatus DockPorts(BlueprintDockingPort portA, BlueprintDockingPort portB)
        {
            // portA should be the Primary Structure side, portB the Secondary Structure.

            // Check the passed ports are valid.
            if (portA == null) return DockingResultStatus.InvalidPortA;
            if (portB == null) return DockingResultStatus.InvalidPortB;

            // check the ports parent structures are valid.
            if (portA.OwnerStructure == null) return DockingResultStatus.InvalidStructurePortA;
            if (portB.OwnerStructure == null) return DockingResultStatus.InvalidStructurePortB;

            // Ensure that the two ports aren't on the same structure.
            if (portA.OwnerStructure == portB.OwnerStructure) return DockingResultStatus.PortsOnSameStructure;

            // Ensure that both ports are not already docked.
            if (portA.IsDocked) return DockingResultStatus.AlreadyDockedPortA;
            if (portB.IsDocked) return DockingResultStatus.AlreadyDockedPortB;

            // Proceed with docking operation.

            // Update portA.
            portA.DockedStructure = portB.OwnerStructure;
            portA.DockedPort = portB;

            // Update portB.
            portB.DockedStructure = portA.OwnerStructure;
            portB.DockedPort = portA;
            portB.OwnerStructure.IsStructureHierarchyRoot = false;

            // Mark the blueprint object as dirty.
            IsDirty = true;

            return DockingResultStatus.Success;
        }

        /// <summary>
        /// Un-docks the structure that the specified port belongs to from whatever it's docked to.
        /// </summary>
        /// <param name="portA"></param>
        /// <returns></returns>
        public DockingResultStatus UndockPort(BlueprintDockingPort portA)
        {
            if (portA == null) return DockingResultStatus.InvalidPortA;
            if (!portA.IsDocked) return DockingResultStatus.PortANotDocked;

            // Find structure A (the one selected)
            BlueprintStructure structureA = portA.OwnerStructure;
            if (structureA == null) return DockingResultStatus.InvalidStructurePortA;

            // Find portB (the other side)
            BlueprintDockingPort portB = portA.DockedPort;

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

            //portB.OwnerStructure.IsStructureHierarchyRoot = true;

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
                    foreach (BlueprintDockingPort dockedPort in blueprintStructure.DockingPorts
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
                        BlueprintDockingPort nextStructurePort = nextStructure.GetDockingPort(blueprintStructure)
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
                        PortName = (DockingPortType)dockingPort.PortName,
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

        public const decimal StationBlueprintFormatVersion = 0.38m;
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

    }

}
