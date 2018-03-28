using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using HELLION.DataStructures;

namespace HELLION.Explorer
{
    public partial class BlueprintEditorForm : Form
    {
        /// <summary>
        /// Defines of filter type for the DockingDstinationSource list.
        /// </summary>
        public enum DockingDestSourceFilterType
        {
            Primary_Structure = 0,
            Non_Primary_Structures,
            Undocked_Structures,

        }

        /// <summary>
        /// Basic Constructor.
        /// </summary>
        public BlueprintEditorForm()
        {
            InitializeComponent();
            Icon = Program.frmMainForm.Icon;

            treeViewHierarchy.ImageList = Program.hEImageList.IconImageList;
            //treeView1.ImageIndex = (int)HEImageList.HEIconsImageNames.Flag_16x;
            //treeView1.SelectedImageIndex = (int)HEImageList.HEIconsImageNames.Flag_16x;
            //treeView1.TreeViewNodeSorter = new HETNSorterSemiMajorAxis();
            treeViewHierarchy.ShowNodeToolTips = true;
            Text = "Blueprint Editor";
            RefreshDropDownModuleTypes();
            RefreshDropDownDockingDestinationSource();
            GenerateDestinationStructureList();

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

            GraftTreeInbound();
            RefreshDropDownDockingDestinationSource();


            IsDirty = false;
        }

        /// <summary>
        /// The node that the editor was opened from.
        /// </summary>
        public HEBlueprintTreeNode SourceNode { get; private set; } = null;

        /// <summary>
        /// Determines whether the text has been changed.
        /// </summary>
        private bool _isDirty = false;
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
        private HEBlueprintTreeNode _currentlySelectedNode = null;
        public HEBlueprintTreeNode CurrentlySelectedNode
        {
            get { return _currentlySelectedNode; }
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
                        else  throw new InvalidOperationException("Unrecognised OwnerObject type.");

                        // Update form items related to the currently selected object.
                        pictureBoxSelectedStructure.Image = CurrentStructure == null ? null 
                            : Program.hEImageList.StructureImageList.Images[HEImageList.GetStructureImageIndexByStructureType(CurrentStructure.StructureType.Value)];

                        labelSelectedStructureType.Text = CurrentStructure == null ? null : CurrentStructure.StructureType.ToString();
                    }
                    else
                    {
                        // No currently selected node, clear the displays.
                        pictureBoxSelectedStructure.Image = null;
                        labelSelectedStructureType.Text = null;
                    }
                    RefreshDropDownDockingSourcePort();
                    
                    RefreshDropDownDestinationStructures();
                    RefreshDropDownDockingDestinationPort();
                }
            }
        }

        /// <summary>
        /// This list is populated with structures that are available for a docking.
        /// </summary>
        private List<HEBlueprint.HEBlueprintStructure> destinationStructureList = null;
        public List<HEBlueprint.HEBlueprintStructure> DestinationStructureList
        {
            get { return destinationStructureList; }
            private set
            {
                destinationStructureList = value;
                // The list has changed so trigger a refresh of the control's values.
                RefreshDropDownDestinationStructures();
            }
        }

        /// <summary>
        /// Represents the currently selected source for dockable modules.
        /// </summary>
        private DockingDestSourceFilterType _dockingDestinationSource;
        public DockingDestSourceFilterType DockingDestinationSource
        {
            get { return _dockingDestinationSource; }
            private set
            {
                if (_dockingDestinationSource != value)
                {
                    _dockingDestinationSource = value;

                    // Trigger control update.
                }
            }
        }

        /// <summary>
        /// Represents the currently selected structure.
        /// </summary>
        private HEBlueprint.HEBlueprintStructure _currentStructure = null;
        public HEBlueprint.HEBlueprintStructure CurrentStructure
        {
            get { return _currentStructure; }
            private set
            {
                if (_currentStructure != value)
                {
                    _currentStructure = value;

                    // Trigger updates.
                }
            }
        }

        /// <summary>
        /// Represents the currently selected docking port.
        /// </summary>
        private HEBlueprint.HEBlueprintDockingPort _currentDockingPort = null;
        public HEBlueprint.HEBlueprintDockingPort CurrentDockingPort
        {
            get { return _currentDockingPort; }
            private set
            {
                if (_currentDockingPort != value)
                {
                    _currentDockingPort = value;

                    // Trigger updates.
                }
            }
        }

        /// <summary>
        /// Represents the selected destination structure for docking.
        /// </summary>
        private HEBlueprint.HEBlueprintStructure _destinationStructure = null;
        public HEBlueprint.HEBlueprintStructure DestinationStructure
        {
            get { return _destinationStructure; }
            private set
            {
                if (_destinationStructure != value)
                {
                    _destinationStructure = value;

                    // Trigger updates.
                    RefreshDropDownDockingDestinationPort();
                }
            }
        }

        /// <summary>
        /// Represents the selected destination structures target docking port for docking.
        /// </summary>
        private HEBlueprint.HEBlueprintDockingPort _destinationDockingPort = null;
        public HEBlueprint.HEBlueprintDockingPort DestinationDockingPort
        {
            get { return _destinationDockingPort; }
            private set
            {
                if (_destinationDockingPort != value)
                {
                    _destinationDockingPort = value;

                    // Trigger updates.
                }
            }
        }

        /// <summary>
        /// Stores the form's initial title text.
        /// </summary>
        private string FormTitleText = null;
        /// <summary>
        /// A reference to the jsonBlueprintFile the blueprint is from.
        /// </summary>
        private HEJsonBlueprintFile jsonBlueprintFile = null;
        /// <summary>
        /// A reference to the blueprint object being worked on.
        /// </summary>
        private HEBlueprint blueprint = null;

        /// <summary>
        /// Updates the form's title text with a marker if the object is dirty.
        /// </summary>
        private void RefreshBlueprintEditorFormTitleText()
        {
            Text = IsDirty ? FormTitleText + "*" : FormTitleText;
        }

        /// <summary>
        /// Grafts a node tree inbound from the Main Form.
        /// </summary>
        private void GraftTreeInbound()
        {
            HEBlueprintTreeNode drn = blueprint.GetDockingRootNode();
            if (drn != null)
            {
                blueprint.RootNode.Nodes.Remove(drn);
                treeViewHierarchy.Nodes.Add(drn);
            }
        }

        /// <summary>
        /// Grafts a node tree outbound from the Main Form.
        /// </summary>
        private void GraftTreeOutbound()
        {
            HEBlueprintTreeNode drn = blueprint.GetDockingRootNode();
            if (drn != null)
            {
                treeViewHierarchy.Nodes.Remove(drn);
                blueprint.RootNode.Nodes.Add(drn);
            }
        }

        /// <summary>
        /// Populates drop-down boxes with the values from the enum.
        /// </summary>
        private void RefreshDropDownModuleTypes()
        {
            comboBoxStructureList.Items.Clear();
            Array enumValues = Enum.GetValues(typeof(HEBlueprintStructureTypes));
            foreach (int value in enumValues)
            {
                string display = Enum.GetName(typeof(HEBlueprintStructureTypes), value);
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
        /// 
        /// </summary>
        private void RefreshDropDownDockingSourcePort()
        {
            comboBoxDockingSourcePort.Items.Clear();

            bool dockingPortSet = false;

            if (CurrentStructure != null && CurrentStructure.AvailableDockingPorts() != null)
            {
                foreach (var port in CurrentStructure.AvailableDockingPorts())
                {
                    comboBoxDockingSourcePort.Items.Add(port.PortName.ToString());
                    if (CurrentDockingPort != null && CurrentDockingPort == port)
                    {
                        comboBoxDockingSourcePort.SelectedItem = port.PortName.ToString();
                        dockingPortSet = true;
                    }
                }
            }
            else comboBoxDockingSourcePort.Items.Add("No available docking ports.");

            // Attempt to select the current docking port from the list.
            if (!dockingPortSet)
            {
                comboBoxDockingSourcePort.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Called by the DestinationStructureList property when it has a new list set and
        /// updates the drop down's values.
        /// </summary>
        private void RefreshDropDownDestinationStructures()
        {
            comboBoxDockingDestinationStructure.Items.Clear();

            if (DestinationStructureList != null && DestinationStructureList.Count > 0)
            {
                foreach (HEBlueprint.HEBlueprintStructure structure in DestinationStructureList)
                {
                    // needs to also display structure ids
                    comboBoxDockingDestinationStructure.Items.Add(structure.StructureType.ToString());
                }
            }
            else comboBoxDockingDestinationStructure.Items.Add("No available structures.");

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
                    comboBoxDockingDestinationPort.Items.Add(port.PortName.ToString());
                    if (CurrentDockingPort != null && CurrentDockingPort == port)
                    {
                        comboBoxDockingDestinationPort.SelectedItem = port.PortName.ToString();
                        dockingPortSet = true;
                    }
                }
            }
            else comboBoxDockingSourcePort.Items.Add("No available docking ports.");

            // Attempt to select the current docking port from the list.
            if (!dockingPortSet)
            {
                comboBoxDockingSourcePort.SelectedIndex = 0;
            }
        }




        /// <summary>
        /// Generates a new list of structures with available docking ports based on the 
        /// specified filter (in the filter drop down)
        /// </summary>
        public void GenerateDestinationStructureList()
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

                        List<HEBlueprint.HEBlueprintStructure> results = blueprint.GetDockingRoot().AllConnectedDockableStructures();

                        if (results != null) newStructureList.AddRange(results);

                        break;

                    case DockingDestSourceFilterType.Non_Primary_Structures:

                        // This needs to get the root of each structure chain or any individual
                        // structures not yet docked to anything.


                        break;

                    case DockingDestSourceFilterType.Undocked_Structures:

                        // This needs to get any individual structure not yet docked to anything.

                        break;
                }
            }
            // Set the DestinationStructureList.
            DestinationStructureList = newStructureList.Count > 0 ? newStructureList : null;
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
                        return;

                    case DialogResult.Yes:
                        MessageBox.Show("User selected to save changes.", "NonImplemented Notice",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;

                }
            }

            // TODO: More work to be done here to handle cleanup, and calling the save


            GraftTreeOutbound();


            // Remove the current JsonDataViewForm from the jsonDataViews list
            Program.blueprintEditorForms.Remove(this);
            GC.Collect();

        }

        #region Form Menu Items

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewHierarchy.SelectedNode != null) treeViewHierarchy.SelectedNode.ExpandAll();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewHierarchy.SelectedNode != null) treeViewHierarchy.SelectedNode.Collapse();
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
            if (toolPaneToolStripMenuItem.Checked) treeViewHierarchy.Width -= adjustmentAmount;
            else treeViewHierarchy.Width += adjustmentAmount;
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("User selected blueprint properties menu item.", "NonImplemented Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

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

        #endregion



        #region ToolPane

        /// <summary>
        /// Triggers an update of the currentlySelectedNode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewHierarchy_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Update the info display for the selected item.
            CurrentlySelectedNode = (HEBlueprintTreeNode)treeViewHierarchy.SelectedNode;
        }

        /// <summary>
        /// Adds a new structure of the type specified in the comboBoxStructureList.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddStructure_Click(object sender, EventArgs e)
        {
            if (blueprint != null && (string)comboBoxStructureList.SelectedItem != "Unspecified" )
            {
                // Do something - create the new structure in the blueprint.
                HEBlueprintStructureTypes newStructureType = (HEBlueprintStructureTypes)Enum.Parse(
                    typeof(HEBlueprintStructureTypes), (string)comboBoxStructureList.SelectedItem);

                HEBlueprint.HEBlueprintStructure newStructure = blueprint.AddStructure(newStructureType);
                treeViewHierarchy.Nodes.Add(newStructure.RootNode);

                // Select the new node
                treeViewHierarchy.SelectedNode = newStructure.RootNode;

            }
        }

        private void comboBoxDockingSourcePort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxDockingDestinationSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (blueprint != null)
            {
                // DockingDestSourceFilterType
                //DockingDestSourceFilterType newStructureType = (DockingDestSourceFilterType)Enum.Parse(
                //    typeof(DockingDestSourceFilterType), (string)comboBoxDockingDestinationSource.SelectedItem);
                DockingDestSourceFilterType dockDestSourceValue;
                if (Enum.TryParse((string)comboBoxDockingDestinationSource.SelectedItem, false, out dockDestSourceValue))
                {
                    DockingDestinationSource = dockDestSourceValue;
                }
                else throw new InvalidOperationException("Unable to parse Docking Destination Source.");
            }
        }

        private void comboBoxDockingDestinationStructure_SelectedIndexChanged(object sender, EventArgs e)
        {
            // update the list of ports drop down.
        }

        private void comboBoxDockingDestinationPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if a valid port is selected, enable the Dock button.
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

    }
}
