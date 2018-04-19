using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using HELLION.DataStructures;

namespace HELLION.Explorer
{
    public partial class BlueprintEditorForm : Form
    {
        #region Constructors

        /// <summary>
        /// Basic Constructor.
        /// </summary>
        public BlueprintEditorForm()
        {
            InitializeComponent();
            Icon = Program.MainForm.Icon;

            treeViewPrimaryStructure.ImageList = Program.hEImageList.IconImageList;
            treeViewSecondaryStructures.ImageList = Program.hEImageList.IconImageList;

            treeViewPrimaryStructure.ShowNodeToolTips = true;
            treeViewSecondaryStructures.ShowNodeToolTips = true;

            Text = "Blueprint Editor";
            RefreshDropDownModuleTypes();

            /*
            RefreshDropDownDockingDestinationSource();
            RefreshDestinationStructureList();
            */

            
        }

        /// <summary>
        /// Constructor that takes a HEBlueprintTreeNode.
        /// </summary>
        /// <param name="passedSourceNode"></param>
        public BlueprintEditorForm(HEBlueprintTreeNode passedSourceNode) : this()
        {
            SourceNode = passedSourceNode ?? throw new NullReferenceException("passedSourceNode was null.");
            FormTitleText = passedSourceNode.Name;
            RefreshBlueprintEditorFormTitleText();

            jsonBlueprintFile = (HEJsonBlueprintFile)passedSourceNode.OwnerObject;
            blueprint = jsonBlueprintFile.BlueprintObject ?? throw new NullReferenceException("jsonBlueprintFile.BlueprintObject was null.");

            GraftTreeInboundFromMainForm();

            // RefreshDropDownDockingDestinationSource();


            IsDirty = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The node that the editor was opened from.
        /// </summary>
        public HEBlueprintTreeNode SourceNode { get; private set; } = null;

        /// <summary>
        /// Determines whether the text has been changed.
        /// </summary>
        public bool IsDirty
        {
            get { return _isDirty; }
            private set
            {
                _isDirty = value;
                // Enable or disable the Apply Changes menu option.
                //applyChangesToolStripMenuItem.Enabled = value;

                // Update the form name
                RefreshBlueprintEditorFormTitleText();
            }
        }

        /// <summary>
        /// Represents the currently selected tree node in the Primary Structure TreeView.
        /// </summary>
        public HEBlueprintTreeNode SelectedPrimaryStructureNode
        {
            get => _selectedPrimaryStructureNode;
            set
            {
                if (_selectedPrimaryStructureNode != value)
                {
                    _selectedPrimaryStructureNode = value;

                    if (_selectedPrimaryStructureNode != null)
                    {
                        // Figure out whether it's a Structure node or a Docking Port node.
                        Type parentType = _selectedPrimaryStructureNode.OwnerObject.GetType();
                        if (parentType == typeof(HEBlueprint.HEBlueprintDockingPort))
                        {
                            // Docking Port node, need find the parent structure.
                            SelectedPrimaryDockingPort = (HEBlueprint.HEBlueprintDockingPort)_selectedPrimaryStructureNode.OwnerObject;

                            SelectedPrimaryStructure = SelectedPrimaryDockingPort?.OwnerObject;

                        }
                        else if (parentType == typeof(HEBlueprint.HEBlueprintStructure))
                        {
                            SelectedPrimaryDockingPort = null;
                            SelectedPrimaryStructure = (HEBlueprint.HEBlueprintStructure)_selectedPrimaryStructureNode.OwnerObject;
                        }
                        else throw new InvalidOperationException("Unrecognised OwnerObject type.");
                    }
                    else
                    {
                        SelectedPrimaryDockingPort = null;
                        SelectedPrimaryStructure = null;
                    }

                    Debug.Print("SelectedPrimaryStructureNode has caused new values to be set.");
                    if (SelectedPrimaryStructure == null) Debug.Print("CurrentStructure is null.");
                    else Debug.Print("CurrentStructure [" + SelectedPrimaryStructure.StructureID.ToString() + "] " + SelectedPrimaryStructure.StructureType.ToString());
                    if (SelectedPrimaryDockingPort == null) Debug.Print("CurrentDockingPort is null.");
                    else Debug.Print("CurrentDockingPort " + SelectedPrimaryDockingPort.PortName.ToString());

                    // Trigger updates.

                    RefreshLabelSelectedPrimaryStructure();
                    RefreshPictureBoxSelectedPrimaryStructure();
                    RefreshLabelSelectedPrimaryDockingPort();

                    RefreshDockButtonEnabledStatus();

                }
            }
        }

        /// <summary>
        /// Represents the currently selected tree node in the Secondary Structures TreeView.
        /// </summary>
        public HEBlueprintTreeNode SelectedSecondaryStructureNode
        {
            get => _selectedSecondaryStructureNode;
            set
            {
                if (_selectedSecondaryStructureNode != value)
                {
                    _selectedSecondaryStructureNode = value;

                    if (_selectedSecondaryStructureNode != null)
                    {
                        // Figure out whether it's a Structure node or a Docking Port node.
                        Type parentType = _selectedSecondaryStructureNode.OwnerObject.GetType();
                        if (parentType == typeof(HEBlueprint.HEBlueprintDockingPort))
                        {
                            // Docking Port node, need find the parent structure.
                            SelectedSecondaryDockingPort = (HEBlueprint.HEBlueprintDockingPort)_selectedSecondaryStructureNode.OwnerObject;

                            SelectedSecondaryStructure = SelectedSecondaryDockingPort?.OwnerObject;

                        }
                        else if (parentType == typeof(HEBlueprint.HEBlueprintStructure))
                        {
                            SelectedSecondaryDockingPort = null;
                            SelectedSecondaryStructure = (HEBlueprint.HEBlueprintStructure)_selectedSecondaryStructureNode.OwnerObject;
                        }
                        else throw new InvalidOperationException("Unrecognised OwnerObject type.");
                    }
                    else
                    {
                        SelectedSecondaryDockingPort = null;
                        SelectedSecondaryStructure = null;
                    }

                    Debug.Print("SelectedSecondaryStructureNode has caused new values to be set.");
                    if (SelectedSecondaryStructure == null) Debug.Print("SelectedSecondaryStructure is null.");
                    else Debug.Print("SelectedSecondaryStructure [" + SelectedSecondaryStructure.StructureID.ToString() + "] " + SelectedSecondaryStructure.StructureType.ToString());
                    if (SelectedSecondaryDockingPort == null) Debug.Print("SelectedSecondaryDockingPort is null.");
                    else Debug.Print("SelectedSecondaryDockingPort " + SelectedSecondaryDockingPort.PortName.ToString());

                    // Trigger updates.

                    RefreshLabelSelectedSecondaryStructure();
                    RefreshPictureBoxSelectedSecondaryStructure();
                    RefreshLabelSelectedSecondaryDockingPort();

                    RefreshDockButtonEnabledStatus();

                }
            }
        }



        /// <summary>
        /// Represents the currently selected structure.
        /// </summary>
        public HEBlueprint.HEBlueprintStructure SelectedPrimaryStructure
        {
            get => _currentStructure;
            private set
            {
                if (_currentStructure != value)
                {
                    _currentStructure = value;

                    // Trigger updates.

                    if (value == null) SelectedPrimaryDockingPort = null;

                    RefreshPictureBoxSelectedPrimaryStructure();
                    RefreshLabelSelectedPrimaryStructure();

                    //RefreshDropDownDockingSourcePort();
                }
            }
        }

        /// <summary>
        /// Represents the currently selected docking port.
        /// </summary>
        public HEBlueprint.HEBlueprintDockingPort SelectedPrimaryDockingPort
        {
            get => _currentDockingPort;
            private set
            {
                if (_currentDockingPort != value)
                {
                    _currentDockingPort = value;

                    // Trigger updates.
                    //RefreshDropDownDockingSourcePort();
                }
            }
        }

        /*
        /// <summary>
        /// Represents the currently selected source for dockable modules.
        /// </summary>
        public DockingDestSourceFilterType DockingDestinationSource
        {
            get => _dockingDestinationSource;
            private set
            {
                if (_dockingDestinationSource != value)
                {
                    _dockingDestinationSource = value;

                    // Trigger control update.
                    //RefreshDropDownDestinationStructures();

                    //RefreshDropDownDockingDestinationPort();

                }
            }
        }

        /// <summary>
        /// This list is populated with structures that are available for a docking.
        /// </summary>
        public List<HEBlueprint.HEBlueprintStructure> DestinationStructureList
        {
            get => destinationStructureList;
            private set
            {
                destinationStructureList = value;
                // The list has changed so trigger a refresh of the control's values.
                //RefreshDropDownDestinationStructures();
                //RefreshDropDownDockingDestinationPort();

            }
        }
        */

        /// <summary>
        /// Represents the selected destination structure for docking.
        /// </summary>
        public HEBlueprint.HEBlueprintStructure SelectedSecondaryStructure
        {
            get => _destinationStructure;
            private set
            {
                if (_destinationStructure != value)
                {
                    _destinationStructure = value;

                    // Trigger updates.

                    if (value == null) SelectedSecondaryDockingPort = null;


                    //RefreshDropDownDockingDestinationPort();
                }
            }
        }

        /// <summary>
        /// Represents the selected destination structures target docking port for docking.
        /// </summary>
        public HEBlueprint.HEBlueprintDockingPort SelectedSecondaryDockingPort
        {
            get => _destinationDockingPort;
            private set
            {
                if (_destinationDockingPort != value)
                {
                    _destinationDockingPort = value;

                    // Trigger updates.

                    // check all structures and ports are valid before enabling the dock button.
                    //RefreshDockButtonEnabledStatus();

                }
            }
        }

        #endregion

        #region Refresh Methods

        /// <summary>
        /// Updates the label text for the Primary Structure.
        /// </summary>
        private void RefreshLabelSelectedPrimaryStructure()
        {
            labelSelectedPrimaryStructure.Text = SelectedPrimaryStructure == null ? "Unspecified"
                : String.Format("[{0:000}] {1}", SelectedPrimaryStructure.StructureID, SelectedPrimaryStructure.StructureType);
        }

        /// <summary>
        /// Updates the image displayed by the Primary Structure PictureBox.
        /// </summary>
        private void RefreshPictureBoxSelectedPrimaryStructure()
        {
            pictureBoxSelectedPrimaryStructure.Image = SelectedPrimaryStructure == null ? null
                : Program.hEImageList.StructureImageList.Images[
                    HEImageList.GetStructureImageIndexByStructureType(SelectedPrimaryStructure.StructureType.Value)];
        }

        /// <summary>
        /// Updates the label text for the Primary Docking Port.
        /// </summary>
        private void RefreshLabelSelectedPrimaryDockingPort()
        {
            labelSelectedPrimaryDockingPort.Text = SelectedPrimaryDockingPort == null ? "Unspecified"
                : String.Format("[{0:000}] {1}", SelectedPrimaryStructure.StructureID, SelectedPrimaryDockingPort.PortName.ToString());

            //Debug.Print("SelectedPrimaryDockingPort.PortName = " + SelectedPrimaryDockingPort.PortName.ToString());
        }


        /// <summary>
        /// Updates the label text for the Secondary Structure.
        /// </summary>
        private void RefreshLabelSelectedSecondaryStructure()
        {
            labelSelectedSecondaryStructure.Text = SelectedSecondaryStructure == null ? "Unspecified"
                : String.Format("[{0:000}] {1}", SelectedSecondaryStructure.StructureID, SelectedSecondaryStructure.StructureType);
        }

        /// <summary>
        /// Updates the image displayed by the Secondary Structure PictureBox.
        /// </summary>
        private void RefreshPictureBoxSelectedSecondaryStructure()
        {
            pictureBoxSelectedSecondaryStructure.Image = SelectedSecondaryStructure == null ? null
                : Program.hEImageList.StructureImageList.Images[
                    HEImageList.GetStructureImageIndexByStructureType(SelectedSecondaryStructure.StructureType.Value)];
        }

        /// <summary>
        /// Updates the label text for the Secondary Docking Port.
        /// </summary>
        private void RefreshLabelSelectedSecondaryDockingPort()
        {
            labelSelectedSecondaryDockingPort.Text = SelectedSecondaryDockingPort == null ? "Unspecified"
                : String.Format("[{0:000}] {1}", SelectedSecondaryStructure.StructureID, SelectedSecondaryDockingPort.PortName.ToString());
        }
        

        /// <summary>
        /// Updates the form's title text with a marker if the object is dirty.
        /// </summary>
        private void RefreshBlueprintEditorFormTitleText()
        {
            Text = IsDirty ? FormTitleText + "*" : FormTitleText;
        }

        /// <summary>
        /// Populates drop-down boxes with the values from the enum.
        /// </summary>
        private void RefreshDropDownModuleTypes()
        {
            comboBoxStructureList.Items.Clear();
            Array enumValues = Enum.GetValues(typeof(HEBlueprintStructureType));
            foreach (int value in enumValues)
            {
                string display = Enum.GetName(typeof(HEBlueprintStructureType), value);
                comboBoxStructureList.Items.Add(display);
            }
            comboBoxStructureList.SelectedIndex = 0;
        }

        /*

        /// <summary>
        /// Populates the drop down for the source of modules to choose for docking.
        /// </summary>
        private void RefreshDropDownDockingDestinationSource()
        {
            comboBoxDockingDestinationSource.Items.Clear();
            Array enumValues = Enum.GetValues(typeof(DockingDestSourceFilterType));
            foreach (int value in enumValues)
            {
                string display = Enum.GetName(typeof(DockingDestSourceFilterType), value);
                comboBoxDockingDestinationSource.Items.Add(display);
            }
            comboBoxDockingDestinationSource.SelectedIndex = 0;
        }

        /// <summary>
        /// Refreshes the drop down for the current structures undocked ports.
        /// </summary>
        private void RefreshDropDownDockingSourcePort()
        {
            comboBoxDockingSourcePort.Items.Clear();

            bool dockingPortSet = false;

            if (SelectedPrimaryStructure != null && SelectedPrimaryStructure.AvailableDockingPorts() != null)
            {
                comboBoxDockingSourcePort.Items.Add("Unspecified");
                foreach (var port in SelectedPrimaryStructure.AvailableDockingPorts())
                {
                    string formattedPortName = String.Format("[{0:000}] {1}",
                        port.OwnerObject.StructureID, port.PortName);

                    comboBoxDockingSourcePort.Items.Add(formattedPortName);

                    if (SelectedPrimaryDockingPort != null && SelectedPrimaryDockingPort == port)
                    {
                        comboBoxDockingSourcePort.SelectedItem = formattedPortName;
                        dockingPortSet = true;
                    }
                }
            }
            else comboBoxDockingSourcePort.Items.Add("No available docking ports");

            // Attempt to select the current docking port from the list.
            if (!dockingPortSet) comboBoxDockingSourcePort.SelectedIndex = 0;
        }

        /// <summary>
        /// Called by the DestinationStructureList property when it has a new list set and
        /// updates the drop down's values.
        /// </summary>
        private void RefreshDropDownDestinationStructures()
        {
            comboBoxDockingDestinationStructure.Items.Clear();

            if (DestinationStructureList == null) Debug.Print("DestinationStructureList is null.");
            else Debug.Print("DestinationStructureList.Count " + DestinationStructureList.Count);

            if (DestinationStructureList != null && DestinationStructureList.Count > 0)
            {
                comboBoxDockingDestinationStructure.Items.Add("Unspecified");
                foreach (HEBlueprint.HEBlueprintStructure structure in DestinationStructureList)
                {
                    comboBoxDockingDestinationStructure.Items.Add(String.Format("[{0:000}] {1}",
                        (int)structure.StructureID, structure.StructureType));
                }
            }
            else comboBoxDockingDestinationStructure.Items.Add("No available structures");

            comboBoxDockingDestinationStructure.SelectedIndex = 0;
        }

        /// <summary>
        /// NOT FINISHED!
        /// </summary>
        private void RefreshDropDownDockingDestinationPort()
        {
            comboBoxDockingDestinationPort.Items.Clear();

            bool dockingPortSet = false;

            if (SelectedSecondaryStructure != null && SelectedSecondaryStructure.AvailableDockingPorts() != null)
            {
                comboBoxDockingDestinationPort.Items.Add("Unspecified");
                foreach (var port in SelectedSecondaryStructure.AvailableDockingPorts())
                {
                    string formattedPortName = String.Format("[{0:000}] {1}",
                        port.OwnerObject.StructureID, port.PortName);

                    comboBoxDockingDestinationPort.Items.Add(formattedPortName);

                    if (SelectedPrimaryDockingPort != null && SelectedPrimaryDockingPort == port)
                    {
                        comboBoxDockingDestinationPort.SelectedItem = formattedPortName;
                        dockingPortSet = true;
                    }
                }
            }
            else comboBoxDockingDestinationPort.Items.Add("No available docking ports");

            // Attempt to select the current docking port from the list.
            if (!dockingPortSet)
            {
                comboBoxDockingDestinationPort.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Generates a new list of structures with available docking ports based on the 
        /// specified filter (in the filter drop down)
        /// </summary>
        public void RefreshDestinationStructureList()
        {
            // Create a new list to hold the results.
            List<HEBlueprint.HEBlueprintStructure> newStructureList = new List<HEBlueprint.HEBlueprintStructure>();

            if (blueprint != null)
            {
                // Determine the filter parameters; parse the drop down.
                DockingDestSourceFilterType filterType = (DockingDestSourceFilterType)Enum.Parse(typeof(DockingDestSourceFilterType),
                    (string)comboBoxDockingDestinationSource.SelectedItem);

                switch (filterType)
                {
                    case DockingDestSourceFilterType.Primary_Structure:

                        // This needs to get all structures docked to the primary structure
                        // that have an available docking port, and the primary structure module
                        // if it has any available ports.

                        List<HEBlueprint.HEBlueprintStructure> _primaryResults = blueprint.GetDockingRoot().AllConnectedDockableStructures();

                        if (_primaryResults != null) newStructureList.AddRange(_primaryResults);

                        break;

                    case DockingDestSourceFilterType.Secondary_Structures:

                        // This needs to get the root of each structure chain or any individual
                        // structures not yet docked to anything.

                        //List<HEBlueprint.HEBlueprintStructure> _secondaryResults = new List<HEBlueprint.HEBlueprintStructure>();
                        
                        foreach (var _secStructure in blueprint.SecondaryStructures)
                        {
                            List<HEBlueprint.HEBlueprintStructure> _secondaryResults = _secStructure.AllConnectedDockableStructures();

                            if (_secondaryResults != null) newStructureList.AddRange(_secondaryResults);
                        }



                        //if (_secondaryResults.Count > 0) newStructureList.AddRange(_secondaryResults);


                        break;

                    //case DockingDestSourceFilterType.Undocked_Structures:

                        // This needs to get any individual structure not yet docked to anything.

                        break;
                }
            }
            // Set the DestinationStructureList.
            DestinationStructureList = newStructureList.Count > 0 ? newStructureList : null;
        }


        */

        /// <summary>
        /// Refreshes the enabled status of the Dock button.
        /// </summary>
        private void RefreshDockButtonEnabledStatus()
        {
            
            if (SelectedPrimaryStructure != null && SelectedPrimaryDockingPort != null
                && SelectedSecondaryStructure != null && SelectedSecondaryDockingPort != null)
            {
                buttonDockPort.Enabled = true;
            }
            else buttonDockPort.Enabled = false;
            
        }

        private void RefreshTreeViews()
        {

            treeViewPrimaryStructure.Nodes.Clear();
            treeViewSecondaryStructures.Nodes.Clear();

            // Trigger the reassembly of all node trees in the blueprint.
            blueprint.RefreshAllTreeNodes();

            // Add the primary structure.
            treeViewPrimaryStructure.Nodes.Add(blueprint.GetDockingRootNode());
            blueprint.GetDockingRootNode().ExpandAll();

            // Add secondary structures.
            foreach (HEBlueprint.HEBlueprintStructure _secondaryStructure in blueprint.SecondaryStructures)
            {
                treeViewSecondaryStructures.Nodes.Add(_secondaryStructure.RootNode);
                _secondaryStructure.RootNode.ExpandAll();
            }



        }

        private void RefreshSelectedPrimaryStructure()
        {
            RefreshLabelSelectedPrimaryStructure();
            RefreshPictureBoxSelectedPrimaryStructure();
            RefreshLabelSelectedPrimaryDockingPort();
        }

        private void RefreshSelectedSecondaryStructure()
        {
            RefreshLabelSelectedSecondaryStructure();
            RefreshPictureBoxSelectedSecondaryStructure();
            RefreshLabelSelectedSecondaryDockingPort();
        }

        public void RefreshEverything()
        {
            RefreshBlueprintEditorFormTitleText();
            RefreshTreeViews();

            RefreshSelectedPrimaryStructure();
            RefreshSelectedSecondaryStructure();

            RefreshDockButtonEnabledStatus();
        }


        /*
        public void RefreshSecondaryStructuresList()
        {
            
            
            // Create a new list to hold the results.
            List<HEBlueprint.HEBlueprintStructure> newSecondaryStructuresList = new List<HEBlueprint.HEBlueprintStructure>();

            if (blueprint != null)
            {
                // Get the list of nodes connected to the primary structure and find 

                newSecondaryStructuresList = blueprint.GetDockingRoot()
                    .DockedStructures(AllConnected: true, incomingLink: blueprint.GetDockingRoot())
                    .Except(blueprint.Structures).ToList();

                Debug.Print(Environment.NewLine + "Secondary Structures List");


                foreach (var thing in newSecondaryStructuresList)
                {
                    Debug.Print(thing.StructureID + " " + thing.StructureType);
                }
            }

            SecondaryStructureList = newSecondaryStructuresList.Count > 0 ? newSecondaryStructuresList : null;

            

        }
        */

        #endregion

        #region TreeNode Grafting Methods

        /// <summary>
        /// Grafts a node tree inbound from the Main Form.
        /// </summary>
        private void GraftTreeInboundFromMainForm()
        {
            HEBlueprintTreeNode drn = blueprint.GetDockingRootNode();
            if (drn != null)
            {
                blueprint.RootNode.Nodes.Remove(drn);

                RefreshTreeViews();

                //treeViewPrimaryStructure.Nodes.Add(drn);

                drn.RefreshToolTipText(includeSubtrees: true);
                drn.ExpandAll();
            }
        }

        /// <summary>
        /// Grafts a node tree outbound from the Main Form.
        /// </summary>
        private void GraftTreeOutboundToMainForm()
        {
            HEBlueprintTreeNode drn = blueprint.GetDockingRootNode();
            if (drn != null)
            {
                treeViewPrimaryStructure.Nodes.Remove(drn);
                blueprint.RootNode.Nodes.Add(drn);
                drn.RefreshToolTipText(includeSubtrees: true);
                drn.Collapse();
            }
        }

        #endregion

        #region Tool Pane ComboBox Event Methods

        /// <summary>
        /// Tracks currently selected item in the comboBoxStructureList and enables the 
        /// Add structure button if the selected item is non-zero (not the 'Unspecified'
        /// default choice.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxStructureList_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonAddStructure.Enabled = comboBoxStructureList.SelectedIndex != 0 ? true : false;
        }

        /*
        private void comboBoxDockingSourcePort_SelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO this needs to parse the ComboBox's selected item and find the port it relates to.
            
            if (blueprint != null)
            {
                string _sourcePortUnprocessedName = comboBoxDockingSourcePort.SelectedItem.ToString();
                string _sourcePortProcessedName = _sourcePortUnprocessedName.Length > 0 ? _sourcePortUnprocessedName.Substring(6) : "Error";

                Debug.Print("@@@@ _sourcePortUnprocessedName " + _sourcePortUnprocessedName + "  _sourcePortProcessedName " + _sourcePortProcessedName);

                if (SelectedPrimaryStructure == null || _sourcePortUnprocessedName == "" 
                    || _sourcePortUnprocessedName == "Unspecified" || _sourcePortProcessedName == "Error"
                    || _sourcePortUnprocessedName == "No available docking ports")
                {
                    SelectedPrimaryDockingPort = null;
                    return;
                }
                else
                {
                    SelectedPrimaryDockingPort = SelectedPrimaryStructure.GetDockingPortByName(_sourcePortProcessedName)
                        ?? throw new InvalidOperationException("Unable to retrieve structure by id.");
                }
            }
            

        }

        /// <summary>
        /// Handles changes in the docking destination source filter combobox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxDockingDestinationSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (blueprint != null)
            {
                DockingDestSourceFilterType dockDestSourceValue;
                if (Enum.TryParse((string)comboBoxDockingDestinationSource.SelectedItem, false, out dockDestSourceValue))
                {
                    DockingDestinationSource = dockDestSourceValue;
                }
                else throw new InvalidOperationException("Unable to parse Docking Destination Source.");

                RefreshDestinationStructureList();
            }
        }

        private void comboBoxDockingDestinationStructure_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (blueprint != null)
            {
                string _destStructureIDString = comboBoxDockingDestinationStructure.SelectedItem.ToString().Substring(1, 3);

                int _destStructureID;
                if (int.TryParse(_destStructureIDString, out _destStructureID))
                {
                    // We should have a StructureID
                    SelectedSecondaryStructure = blueprint.GetStructureByID(_destStructureID)
                        ?? throw new InvalidOperationException("Unable to retrieve structure by id.");
                }
                else SelectedSecondaryStructure = null;
            }
        }

        private void comboBoxDockingDestinationPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if a valid port is selected, enable the Dock button.

            if (blueprint != null)
            {
                string _destPortUnprocessedName = comboBoxDockingDestinationPort.SelectedItem.ToString();
                string _destPortProcessedName = _destPortUnprocessedName.Length > 0 ? _destPortUnprocessedName.Substring(6) : "Error";

                if (SelectedSecondaryStructure == null || _destPortUnprocessedName == "" 
                    || _destPortUnprocessedName == "Unspecified" || _destPortUnprocessedName == "Error"
                    || _destPortUnprocessedName == "No available docking ports")
                {
                    SelectedSecondaryDockingPort = null;
                    return;
                }
                else
                {
                    SelectedSecondaryDockingPort = SelectedSecondaryStructure.GetDockingPortByName(_destPortProcessedName)
                        ?? throw new InvalidOperationException("Unable to retrieve structure by id.");
                }
            }
        }
        */

        #endregion

        #region Tool Pane Button Event Methods

        /// <summary>
        /// Adds a new structure of the type specified in the comboBoxStructureList.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddStructure_Click(object sender, EventArgs e)
        {
            if (blueprint != null && (string)comboBoxStructureList.SelectedItem != "Unspecified")
            {
                // Do something - create the new structure in the blueprint.
                HEBlueprintStructureType newStructureType = (HEBlueprintStructureType)Enum.Parse(
                    typeof(HEBlueprintStructureType), (string)comboBoxStructureList.SelectedItem);

                HEBlueprint.HEBlueprintStructure newStructure = blueprint.AddStructure(newStructureType);

                // Refresh tree views

                RefreshTreeViews();



                // treeViewPrimaryStructure.Nodes.Add(newStructure.RootNode);

                // Select the new node
                treeViewSecondaryStructures.SelectedNode = newStructure.RootNode;

                // Set the focus on the Secondary Structures TreeView.
                treeViewSecondaryStructures.Focus();

            }
        }

        private void buttonRemoveStructure_Click(object sender, EventArgs e)
        {

        }

        private void buttonDockPort_Click(object sender, EventArgs e)
        {
            HEBlueprint.HEBlueprintDockingPort a = SelectedPrimaryDockingPort ?? throw new NullReferenceException("SelectedPrimaryDockingPort was null.");
            HEBlueprint.HEBlueprintDockingPort b = SelectedSecondaryDockingPort ?? throw new NullReferenceException("SelectedSecondaryDockingPort was null.");

            HEDockingResultStatus result = blueprint.DockPorts(a, b);

            if (result == HEDockingResultStatus.Success) SelectedSecondaryStructureNode = null;
            else MessageBox.Show("Result: " + result.ToString(), "Docking Operation Result", MessageBoxButtons.OK, MessageBoxIcon.Error);

            RefreshEverything();
        }

        private void buttonUndockPort_Click(object sender, EventArgs e)
        {
            HEBlueprint.HEBlueprintDockingPort a = SelectedPrimaryDockingPort ?? throw new NullReferenceException("SelectedPrimaryDockingPort was null.");

            HEDockingResultStatus result = blueprint.UndockPort(a);

            if (result == HEDockingResultStatus.Success) SelectedSecondaryStructureNode = null;
            else MessageBox.Show("Result: " + result.ToString(), "Undocking Operation Result", MessageBoxButtons.OK, MessageBoxIcon.Error);

            RefreshEverything();
        }

        #endregion

        #region Form Control Event Methods

        /// <summary>
        /// Handles selection events within the PrimaryStructure TreeView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewPrimaryStructure_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Update the info display for the selected item.
            SelectedPrimaryStructureNode = (HEBlueprintTreeNode)treeViewPrimaryStructure.SelectedNode;
        }

        /// <summary>
        /// Handles selection events within the SecondaryStructures TreeView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewSecondaryStructures_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Update the info display for the selected item.
            SelectedSecondaryStructureNode = (HEBlueprintTreeNode)treeViewSecondaryStructures.SelectedNode;
        }

        /// <summary>
        /// Form Closing IsDirty check with prompt to save changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlueprintEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsDirty)
            {
                // Unsaved changes, prompt the user to apply them before closing the window.
                DialogResult result = MessageBox.Show("Do you want to save changes to this blueprint? ",
                    "Un-Saved Changes Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                switch (result)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;

                    case DialogResult.Yes:
                        MessageBox.Show("User selected to save changes.", "NonImplemented Notice",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;

                }
            }

            // TODO: More work to be done here to handle cleanup, and calling the save


            GraftTreeOutboundToMainForm();


            // Remove the current JsonDataViewForm from the jsonDataViews list
            Program.blueprintEditorForms.Remove(this);
            GC.Collect();

        }

        #endregion

        #region Form Menu Item Event Methods

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewPrimaryStructure.SelectedNode != null) treeViewPrimaryStructure.SelectedNode.ExpandAll();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewPrimaryStructure.SelectedNode != null) treeViewPrimaryStructure.SelectedNode.Collapse();
        }

        /// <summary>
        /// Hides or shows the Tool Panel UI element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolPaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int adjustmentAmount = 197; // This is basically the width of the Tool Panel, with a tweak.
            panelToolPanel.Visible = toolPaneToolStripMenuItem.Checked;
            if (toolPaneToolStripMenuItem.Checked) splitContainerTreeViews.Width -= adjustmentAmount;
            else splitContainerTreeViews.Width += adjustmentAmount;
        }

        /// <summary>
        /// Hides or shows the Secondary Structures Pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void secondaryStructuresPaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainerTreeViews.Panel2Collapsed = !secondaryStructuresPaneToolStripMenuItem.Checked;
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("User selected blueprint properties menu item.", "NonImplemented Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Fields

        private bool _isDirty = false;
        private string FormTitleText = null;
        private HEJsonBlueprintFile jsonBlueprintFile = null;
        private HEBlueprint blueprint = null;
        private HEBlueprintTreeNode _selectedPrimaryStructureNode = null;
        private HEBlueprintTreeNode _selectedSecondaryStructureNode = null;
        private HEBlueprint.HEBlueprintStructure _currentStructure = null;
        private HEBlueprint.HEBlueprintDockingPort _currentDockingPort = null;
        //private DockingDestSourceFilterType _dockingDestinationSource;
        //private List<HEBlueprint.HEBlueprintStructure> destinationStructureList = null;
        private HEBlueprint.HEBlueprintStructure _destinationStructure = null;
        private HEBlueprint.HEBlueprintDockingPort _destinationDockingPort = null;

        #endregion

        #region Enumerations

        /// <summary>
        /// Defines of filter type for the DockingDstinationSource list.
        /// </summary>
        public enum DockingDestSourceFilterType
        {
            Primary_Structure = 0,
            Secondary_Structures,
            //Undocked_Structures,

        }

        #endregion
    }
}
