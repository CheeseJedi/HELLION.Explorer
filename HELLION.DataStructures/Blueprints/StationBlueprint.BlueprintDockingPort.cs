using System;
using System.Diagnostics;
using HELLION.DataStructures.StaticData;
using HELLION.DataStructures.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using static HELLION.DataStructures.StaticData.DockingPortHelper;

namespace HELLION.DataStructures.Blueprints
{
    public partial class StationBlueprint
    {
        /// <summary>
        /// A class to define the docking ports of a structure (module/ship) within the 
        /// blueprint.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        public class BlueprintDockingPort : IParent_Base_TN
        {
            #region Constructors

            /// <summary>
            /// Basic Constructor.
            /// </summary>
            public BlueprintDockingPort()
            {
                // Set a Default port name.
                //PortName = DockingPortType.Unspecified;

                RootNode = new BlueprintDockingPort_TN(passedOwner: this)
                //    nodeName: PortName.ToString())
                {
                    Text_Prefix = OwnerStructure != null && OwnerStructure.StructureID != null ?
                        String.Format("[{0:000}] ", (int)OwnerStructure.StructureID) : "[ERR] "
                };
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="passedParent"></param>
            public BlueprintDockingPort(BlueprintStructure passedParent) : this()
            {
                RootNode.AutoGenerateName = true;
                OwnerStructure = passedParent;

            }

            /// <summary>
            /// The type of parent structure this docking port is part of.
            /// </summary>
            /// <remarks>
            /// Used to assist in lookup of OrderID to PortName and vice versa.
            /// </remarks>
            /// <param name="parentType"></param>
            public BlueprintDockingPort(StructureSceneID parentType) : this()
            {
                RootNode.AutoGenerateName = true;
                StructureSceneID = parentType;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Parent object - not to be included in serialisation.
            /// </summary>
            public BlueprintStructure OwnerStructure
            {
                get => _ownerObject;
                set
                {
                    _ownerObject = value;

                    RootNode.Refresh();
                    RefreshAfterParentStructureIDChange();

                    if (OwnerStructure != null)
                    {
                        if (OwnerStructure.SceneID == null) throw new Exception("OwnerStructure's SceneID was null.");
                        else
                        {
                            StructureSceneID = (StructureSceneID)OwnerStructure.SceneID;
                        }

                    }

                    if (OrderID != null && OwnerStructure != null) AttemptSetPortNameFromOrderID(); // This was causing an issue

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
            public BlueprintDockingPort DockedPort
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

            public bool AutoConvert_PortName_OrderID { get; set; } = false;

            /// <summary>
            /// The parent structure's SceneID - is updated when an appropriate OwnerStructure is set
            /// or can be set manually when there is no parent object linked.
            /// </summary>
            private StructureSceneID StructureSceneID
            {
                get => _structureSceneID;
                set
                {
                    if (_structureSceneID != value)
                    {
                        _structureSceneID = value;


                    }
                }
            }

            #endregion

            #region Serialised Properties

            /// <summary>
            /// The (standardised) name of the docking port.
            /// </summary>
            [JsonProperty]
            [JsonConverter(typeof(StringEnumConverter))]
            public DockingPortType? PortName
            {
                get => _portName;
                set
                {
                    if (_portName != value)
                    {
                        _portName = value;

                        if (AutoConvert_PortName_OrderID)
                        {
                            if (OrderID != null && OwnerStructure != null)
                            {
                                AttemptSetOrderIDFromPortName();
                            }
                        }

                        RootNode.Refresh();
                    }
                }
            }

            /// <summary>
            /// The OrderId of the docking port - these are used by the game deserialiser to spawn
            /// blueprints.
            /// </summary>
            [JsonProperty]
            public int? OrderID
            {
                get => _orderID;
                set
                {
                    if (_orderID != value && value != null)
                    {

                        // Debug.Print("New value for OrderID on [{0}]: [{1}] (previously [{2}])", OwnerStructure?.SceneName, value, _orderID);
                        _orderID = value;


                        if (AutoConvert_PortName_OrderID)
                        {
                            if (OrderID != null && OwnerStructure != null)
                            {
                                AttemptSetPortNameFromOrderID(); // This is/was causing an issue
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Whether this docking port is locked (and not advertised to the docking system.)
            /// </summary>
            [JsonProperty]
            public bool? Locked
            {
                get => _locked;
                set
                {
                    if (_locked != value)
                    {
                        _locked = value;
                    }
                }
            }

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
            public DockingPortType? DockedPortName
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
            /// Handles a port docking operation - calls itself on the other docking port.
            /// </summary>
            /// <param name="otherPort"></param>
            /// <param name="notifyPartner"></param>
            public DockingResultStatus Dock(BlueprintDockingPort otherPort, bool notifyPartner = true)
            {
                // portA should be the Primary Structure side, otherPort the Secondary Structure.

                // Check the passed port is valid.
                if (otherPort == null) return DockingResultStatus.InvalidPortB;

                // check the ports parent structures are valid.
                if (OwnerStructure == null) return DockingResultStatus.InvalidStructurePortA;
                if (otherPort.OwnerStructure == null) return DockingResultStatus.InvalidStructurePortB;

                // Ensure that the two ports aren't on the same structure.
                if (OwnerStructure == otherPort.OwnerStructure) return DockingResultStatus.PortsOnSameStructure;

                // Ensure that both ports are not already docked.
                if (IsDocked) return DockingResultStatus.AlreadyDockedPortA;
                if (otherPort.IsDocked) return DockingResultStatus.AlreadyDockedPortB;

                // Proceed with docking operation.

                if (notifyPartner)
                {
                    DockingResultStatus partnerResult = otherPort.Dock(this, notifyPartner: false);

                    if (partnerResult != DockingResultStatus.Success)
                    {
                        Debug.Print("Dock Partner Error: " + partnerResult.ToString());
                        return DockingResultStatus.PartnerError;
                    }
                }


                // Update this port.
                DockedStructure = otherPort.OwnerStructure;
                DockedPort = otherPort;

                // Update otherPort.
                //otherPort.DockedStructure = portA.OwnerStructure;
                //otherPort.DockedPort = portA;
                otherPort.OwnerStructure.IsStructureHierarchyRoot = false;

                // Mark the blueprint object as dirty.
                OwnerStructure.OwnerObject.IsDirty = true;

                return DockingResultStatus.Success;




            }

            /// <summary>
            /// Handles a port un-docking operation - calls itself on the other docking port.
            /// </summary>
            /// <param name="notifyPartner"></param>
            public DockingResultStatus Undock(bool notifyPartner = true)
            {

                if (!IsDocked) return DockingResultStatus.PortANotDocked;

                // Find structure A (the one selected)
                if (OwnerStructure == null) return DockingResultStatus.InvalidStructurePortA;

                // Find otherPort (the other side)
                BlueprintDockingPort otherPort = DockedPort;

                if (otherPort == null) return DockingResultStatus.InvalidPortB;
                if (!otherPort.IsDocked) return DockingResultStatus.PortBNotDocked;

                // Find structureB
                BlueprintStructure structureB = DockedStructure;
                if (structureB == null) return DockingResultStatus.InvalidStructurePortB;


                if (OwnerStructure != otherPort.DockedStructure)
                    return DockingResultStatus.PortAandBNotDocked;

                // Process the un-docking.


                // TODO: the notifyPartner checks etc need to be implemented.

                // Set the DockedStructures to null
                DockedStructure = null;
                DockedPort = null;

                //otherPort.DockedStructure = null;
                //otherPort.DockedPort = null;

                //otherPort.OwnerStructure.IsStructureHierarchyRoot = true;

                // Figure out which structure to add to the Secondary Structures list.

                if (OwnerStructure.IsConnectedToPrimaryStructure)
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
                    if (OwnerStructure.IsConnectedToPrimaryStructure) throw new InvalidOperationException("Both structures connected to primary.");

                    // structureB is not connected to the Primary.
                    // Check if structureB is connected to a secondary root.
                    if (OwnerStructure.GetStructureRoot() != null) throw new InvalidOperationException("structureA is connected to a root.");

                    // Structure B is not connected to a root, make it one.
                    OwnerStructure.IsStructureHierarchyRoot = true;
                }



                structureB.IsStructureHierarchyRoot = true;

                //SecondaryStructures.Add(structureB);

                IsDirty = true;

                return DockingResultStatus.Success;










            }


            /// <summary>
            /// Attempt to update the DockedStructure object.
            /// </summary>
            /// <returns>
            /// Return true if a non null was returned from the GetStructureByID.
            /// </returns>
            public bool AttemptUpdateDockedStructure()
            {
                if (OwnerStructure == null || OwnerStructure.OwnerObject == null) return false;
                DockedStructure = OwnerStructure.OwnerObject.GetStructure(_dockedStructureID);
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
                if (DockedStructure == null || OwnerStructure.OwnerObject == null) return false;
                DockedPort = DockedStructure.GetDockingPort(_dockedPortName.ToString());
                return DockedPort != null ? true : false;
            }

            public void RefreshAfterParentStructureIDChange()
            {
                RootNode.Text_Prefix = OwnerStructure != null ? String.Format("[{0:000}] ", (int)OwnerStructure.StructureID) : "[ERR] ";
                RootNode.Name = _portName.ToString();

                //RootNode.RefreshText();
                //RootNode.RefreshName();
            }

            /// <summary>
            /// Prepares the DockingPort for removal from memory.
            /// </summary>
            /// <param name="RemoveOrphanedStructures"></param>
            /// <returns>Returns a failure state; true if the operation encountered a problem, false if successful.</returns>
            public bool PrepareForRemoval(bool RemoveOrphanedStructures = false)
            {
                if (IsDocked)
                {
                    // TODO: Trigger un-docking if this port is docked.
                    return true;
                }

                // Remove the ports tree node from it's parent's Nodes collection.
                if (RootNode.Parent != null) RootNode.Parent.Nodes.Remove(RootNode);

                return false;
            }

            /// <summary>
            /// Attempts to set the PortName from OrderID.
            /// </summary>
            public void AttemptSetPortNameFromOrderID()
            {
                // Look up the correct port name for this structure and orderID
                if (OwnerStructure != null) PortName = GetDockingPortType(
                    (StructureSceneID)OwnerStructure.SceneID, (int)OrderID);
                else Debug.Print("AttemptSetPortNameFromOrderID - OwnerStructure was null");
            }

            /// <summary>
            /// Attempts to set the OrderID from the PortName.
            /// </summary>
            public void AttemptSetOrderIDFromPortName()
            {
                if (OwnerStructure != null) OrderID = (int)GetOrderID(
                    (StructureSceneID)OwnerStructure.SceneID, (DockingPortType)PortName);
                else Debug.Print("AttemptSetOrderIDFromPortName - OwnerStructure was null");
            }




            #endregion

            #region Fields

            private DockingPortType? _portName = null; // DockingPortType.Unspecified;
            private BlueprintStructure _ownerObject = null;
            private int? _dockedStructureID = null;
            private BlueprintStructure _dockedStructure = null;
            private DockingPortType? _dockedPortName = null;
            private BlueprintDockingPort _dockedPort = null;
            private int? _orderID = null;
            private bool? _locked = null;
            private StructureSceneID _structureSceneID;

            #endregion

        }
    }
}
