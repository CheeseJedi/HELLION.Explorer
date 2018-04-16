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
            //treeView1.ImageIndex = (int)HEImageList.HEIconsImageNames.Flag_16x;
            //treeView1.SelectedImageIndex = (int)HEImageList.HEIconsImageNames.Flag_16x;
            //treeView1.TreeViewNodeSorter = new HETNSorterSemiMajorAxis();
            treeViewPrimaryStructure.ShowNodeToolTips = true;
            Text = "Blueprint Editor";
            RefreshDropDownModuleTypes();
            RefreshDropDownDockingDestinationSource();
            RefreshDestinationStructureList();

            //
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
            RefreshDropDownDockingDestinationSource();


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
        /// Represents the currently selected tree node.
        /// </summary>
        public HEBlueprintTreeNode CurrentlySelectedNode
        {
            get => _currentlySelectedNode;
            set
            {
                if (_currentlySelectedNode != value)
                {
                    _currentlySelectedNode = value;

                    if (_currentlySelectedNode != null)
                    {
                        // Figure out whether it's a Structure node or a Docking Port node.
                        Type parentType = _currentlySelectedNode.OwnerObject.GetType();
                        if (parentType == typeof(HEBlueprint.HEBlueprintDockingPort))
                        {
                            // Docking Port node, need find the parent structure.
                            CurrentDockingPort = (HEBlueprint.HEBlueprintDockingPort)_currentlySelectedNode.OwnerObject;
                            CurrentStructure = CurrentDockingPort.OwnerObject;

                        }
                        else if (parentType == typeof(HEBlueprint.HEBlueprintStructure))
                        {
                            CurrentDockingPort = null;
                            CurrentStructure = (HEBlueprint.HEBlueprintStructure)_currentlySelectedNode.OwnerObject;
                        }
                        else throw new InvalidOperationException("Unrecognised OwnerObject type.");
                    }
                    else
                    {
                        CurrentDockingPort = null;
                        CurrentStructure = null;
                    }

                    Debug.Print("CurrentlySelectedNode has caused new values to be set.");


                    if (CurrentStructure == null) Debug.Print("CurrentStructure is null.");
                    else Debug.Print("CurrentStructure [" + CurrentStructure.StructureID.ToString() + "] " + CurrentStructure.StructureType.ToString());

                    if (CurrentDockingPort == null) Debug.Print("CurrentDockingPort is null.");
                    else Debug.Print("CurrentDockingPort " + CurrentDockingPort.PortName.ToString());

                    // TEMPORARY
                    //RefreshDestinationStructureList();
                }
            }
        }

        /// <summary>
        /// Represents the currently selected structure.
        /// </summary>
        public HEBlueprint.HEBlueprintStructure CurrentStructure
        {
            get => _currentStructure;
            private set
            {
                if (_currentStructure != value)
                {
                    _currentStructure = value;

                    // Trigger updates.

                    if (value == null) CurrentDockingPort = null;

                    RefreshPictureBoxSelectedStructure();
                    RefreshLabelSelectedStructureType();

                    RefreshDropDownDockingSourcePort();
                }
            }
        }

        /// <summary>
        /// Represents the currently selected docking port.
        /// </summary>
        public HEBlueprint.HEBlueprintDockingPort CurrentDockingPort
        {
            get => _currentDockingPort;
            private set
            {
                if (_currentDockingPort != value)
                {
                    _currentDockingPort = value;

                    // Trigger updates.
                    RefreshDropDownDockingSourcePort();
                }
            }
        }

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
                    RefreshDropDownDestinationStructures();

                    RefreshDropDownDockingDestinationPort();

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
                RefreshDropDownDestinationStructures();
                RefreshDropDownDockingDestinationPort();

            }
        }

        /// <summary>
        /// Represents the selected destination structure for docking.
        /// </summary>
        public HEBlueprint.HEBlueprintStructure DestinationStructure
        {
            get => _destinationStructure;
            private set
            {
                if (_destinationStructure != value)
                {
                    _destinationStructure = value;

                    // Trigger updates.

                    if (value == null) DestinationDockingPort = null;


                    RefreshDropDownDockingDestinationPort();
                }
            }
        }

        /// <summary>
        /// Represents the selected destination structures target docking port for docking.
        /// </summary>
        public HEBlueprint.HEBlueprintDockingPort DestinationDockingPort
        {
            get => _destinationDockingPort;
            private set
            {
                if (_destinationDockingPort != value)
                {
                    _destinationDockingPort = value;

                    // Trigger updates.

                    // check all structures and ports are valid before enabling the dock button.
                    RefreshDockButtonEnabledStatus();

                }
            }
        }

        #endregion

        #region Refresh Methods

        /// <summary>
        /// Updates the image displayed by the picture box based on the currently selected structure.
        /// </summary>
        private void RefreshPictureBoxSelectedStructure()
        {
            pictureBoxSelectedStructure.Image = CurrentStructure == null ? null
                            : Program.hEImageList.StructureImageList.Images[HEImageList.GetStructureImageIndexByStructureType(CurrentStructure.StructureType.Value)];
        }

        /// <summary>
        /// Updates the label text for the selected structure type.
        /// </summary>
        private void RefreshLabelSelectedStructureType()
        {
            labelSelectedStructureType.Text = CurrentStructure == null ? null : String.Format("[{0:000}] {1}", CurrentStructure.StructureID, CurrentStructure.StructureType);
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

            if (CurrentStructure != null && CurrentStructure.AvailableDockingPorts() != null)
            {
                foreach (var port in CurrentStructure.AvailableDockingPorts())
                {
                    string formattedPortName = String.Format("[{0:000}] {1}",
                        port.OwnerObject.StructureID, port.PortName);

                    comboBoxDockingSourcePort.Items.Add(formattedPortName);

                    if (CurrentDockingPort != null && CurrentDockingPort == port)
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

            if (DestinationStructure != null && DestinationStructure.AvailableDockingPorts() != null)
            {
                foreach (var port in DestinationStructure.AvailableDockingPorts())
                {
                    string formattedPortName = String.Format("[{0:000}] {1}",
                        port.OwnerObject.StructureID, port.PortName);

                    comboBoxDockingDestinationPort.Items.Add(formattedPortName);

                    if (CurrentDockingPort != null && CurrentDockingPort == port)
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
        /// Refreshes the enabled status of the Dock button.
        /// </summary>
        private void RefreshDockButtonEnabledStatus()
        {
            if (CurrentStructure != null && CurrentDockingPort != null
                && DestinationStructure != null && DestinationDockingPort != null)
            {
                buttonDockPort.Enabled = true;
            }
            else buttonDockPort.Enabled = false;
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


        public void RefreshTreeViews()
        {

            treeViewPrimaryStructure.Nodes.Clear();
            treeViewSecondaryStructures.Nodes.Clear();

            // Trigger the reassembly of all node trees in the blueprint.
            blueprint.RefreshAllTreeNodes();

            // Add the primary structure.

            Debug.Print("DockingRootNode parent = " + (blueprint.GetDockingRootNode().Parent != null ? blueprint.GetDockingRootNode().Parent.Name : "null"));

            treeViewPrimaryStructure.Nodes.Add(blueprint.GetDockingRootNode());

            // Add secondary structures.
            foreach (HEBlueprint.HEBlueprintStructure _secondaryStructure in blueprint.SecondaryStructures)
            {
                treeViewSecondaryStructures.Nodes.Add(_secondaryStructure.RootNode);

            }



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

        private void comboBoxDockingSourcePort_SelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO this needs to parse the ComboBox's selected item and find the port it relates to.

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
                    DestinationStructure = blueprint.GetStructureByID(_destStructureID)
                        ?? throw new InvalidOperationException("Unable to retrieve structure by id.");
                }
                else DestinationStructure = null;
            }
        }

        private void comboBoxDockingDestinationPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if a valid port is selected, enable the Dock button.

            if (blueprint != null)
            {
                string _destPortUnprocessedName = comboBoxDockingDestinationPort.SelectedItem.ToString();
                string destPortProcessedName = _destPortUnprocessedName.Length > 0 ? _destPortUnprocessedName.Substring(6) : "Error";

                if (DestinationStructure == null || _destPortUnprocessedName == "No available docking ports")
                {
                    DestinationDockingPort = null;
                    return;
                }
                else
                {
                    DestinationDockingPort = DestinationStructure.GetDockingPortByName(destPortProcessedName)
                        ?? throw new InvalidOperationException("Unable to retrieve structure by id.");
                }
            }
        }

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

        private void buttonUndockPort_Click(object sender, EventArgs e)
        {
            MessageBox.Show("User selected UndockPort button.", "NonImplemented Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonDockPort_Click(object sender, EventArgs e)
        {
            MessageBox.Show("User selected DockPort.", "NonImplemented Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            CurrentlySelectedNode = (HEBlueprintTreeNode)treeViewPrimaryStructure.SelectedNode;
        }

        /// <summary>
        /// Handles selection events within the SecondaryStructures TreeView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewSecondaryStructures_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Update the info display for the selected item.
            CurrentlySelectedNode = (HEBlueprintTreeNode)treeViewSecondaryStructures.SelectedNode;
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
        private HEBlueprintTreeNode _currentlySelectedNode = null;
        private HEBlueprint.HEBlueprintStructure _currentStructure = null;
        private HEBlueprint.HEBlueprintDockingPort _currentDockingPort = null;
        private DockingDestSourceFilterType _dockingDestinationSource;
        private List<HEBlueprint.HEBlueprintStructure> destinationStructureList = null;
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
