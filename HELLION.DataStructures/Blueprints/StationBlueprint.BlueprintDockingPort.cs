using System;
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

                        AttemptSetOrderIDFromPortName();

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



                        if (OrderID != null && OwnerStructure != null) AttemptSetPortNameFromOrderID(); // This is causing an issue
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
            /// Attempts to set the PortName from OrderID.
            /// </summary>
            private void AttemptSetPortNameFromOrderID()
            {
                // Look up the correct port name for this structure and orderID
                if (OwnerStructure != null) PortName = GetDockingPortType((StructureSceneID)OwnerStructure?.SceneID, (int)OrderID);
            }

            /// <summary>
            /// Attempts to set the OrderID from the PortName.
            /// </summary>
            private void AttemptSetOrderIDFromPortName()
            {
                if (OwnerStructure != null) OrderID = (int)GetOrderID((StructureSceneID)OwnerStructure?.SceneID, (DockingPortType)PortName);
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
