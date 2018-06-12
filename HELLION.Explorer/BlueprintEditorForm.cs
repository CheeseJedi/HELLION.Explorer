using System;
using System.Diagnostics;
using System.Windows.Forms;
using HELLION.DataStructures.Blueprints;
using HELLION.DataStructures.EmbeddedImages;
using HELLION.DataStructures.StaticData;
using static HELLION.DataStructures.Blueprints.StationBlueprint;

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
            Icon = HellionExplorerProgram.MainForm.Icon;

            treeViewPrimaryStructure.ImageList = HellionExplorerProgram.hEImageList.IconImageList;
            treeViewSecondaryStructures.ImageList = HellionExplorerProgram.hEImageList.IconImageList;

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
        /// Constructor that takes a Blueprint_TN.
        /// </summary>
        /// <param name="passedSourceNode"></param>
        public BlueprintEditorForm(Blueprint_TN passedSourceNode) : this()
        {
            SourceNode = passedSourceNode ?? throw new NullReferenceException("passedSourceNode was null.");
            FormTitleText = passedSourceNode.Name;
            RefreshBlueprintEditorFormTitleText();

            jsonBlueprintFile = (StationBlueprint_File)passedSourceNode.OwnerObject;
            blueprint = jsonBlueprintFile.BlueprintObject ?? throw new NullReferenceException("jsonBlueprintFile.BlueprintObject was null.");

            GraftTreeInboundFromMainForm();

        }

        #endregion

        #region Properties

        /// <summary>
        /// The node that the editor was opened from.
        /// </summary>
        public Blueprint_TN SourceNode { get; private set; } = null;

        /// <summary>
        /// Determines whether the text has been changed.
        /// </summary>
        public bool IsDirty
        {
            get => blueprint != null ? blueprint.IsDirty : false;
        }

        /// <summary>
        /// Represents the currently selected tree node in the Primary Structure TreeView.
        /// </summary>
        public Blueprint_TN SelectedPrimaryStructureNode
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
                        if (parentType == typeof(StationBlueprint.BlueprintDockingPort))
                        {
                            // Docking Port node, need find the parent structure.
                            SelectedPrimaryDockingPort = (StationBlueprint.BlueprintDockingPort)_selectedPrimaryStructureNode.OwnerObject;

                            SelectedPrimaryStructure = SelectedPrimaryDockingPort?.OwnerStructure;

                        }
                        else if (parentType == typeof(StationBlueprint.BlueprintStructure))
                        {
                            SelectedPrimaryDockingPort = null;
                            SelectedPrimaryStructure = (StationBlueprint.BlueprintStructure)_selectedPrimaryStructureNode.OwnerObject;
                        }
                        else throw new InvalidOperationException("Unrecognised OwnerStructure type.");
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
                    RefreshUndockButtonEnabledStatus();

                }
            }
        }

        /// <summary>
        /// Represents the currently selected tree node in the Secondary Structures TreeView.
        /// </summary>
        public Blueprint_TN SelectedSecondaryStructureNode
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
                        if (parentType == typeof(StationBlueprint.BlueprintDockingPort))
                        {
                            // Docking Port node, need find the parent structure.
                            SelectedSecondaryDockingPort = (StationBlueprint.BlueprintDockingPort)_selectedSecondaryStructureNode.OwnerObject;

                            SelectedSecondaryStructure = SelectedSecondaryDockingPort?.OwnerStructure;

                        }
                        else if (parentType == typeof(StationBlueprint.BlueprintStructure))
                        {
                            SelectedSecondaryDockingPort = null;
                            SelectedSecondaryStructure = (StationBlueprint.BlueprintStructure)_selectedSecondaryStructureNode.OwnerObject;
                        }
                        else throw new InvalidOperationException("Unrecognised OwnerStructure type.");
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
                    RefreshUndockButtonEnabledStatus();

                }
            }
        }



        /// <summary>
        /// Represents the currently selected structure.
        /// </summary>
        public StationBlueprint.BlueprintStructure SelectedPrimaryStructure
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
        public StationBlueprint.BlueprintDockingPort SelectedPrimaryDockingPort
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
        public List<HEBlueprint.BlueprintStructure> DestinationStructureList
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
        public StationBlueprint.BlueprintStructure SelectedSecondaryStructure
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
        public StationBlueprint.BlueprintDockingPort SelectedSecondaryDockingPort
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
                : HellionExplorerProgram.hEImageList.StructureImageList.Images[
                    EmbeddedImages_ImageList.GetStructureImageIndexBySceneID(SelectedPrimaryStructure.SceneID.Value)];
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
                : HellionExplorerProgram.hEImageList.StructureImageList.Images[
                    EmbeddedImages_ImageList.GetStructureImageIndexBySceneID(SelectedSecondaryStructure.SceneID.Value)];
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
            string newText = "Blueprint Editor [" + FormTitleText + "]";
            Text = IsDirty ? newText + "*" : newText;
        }

        /// <summary>
        /// Populates drop-down boxes with the values from the enum.
        /// </summary>
        private void RefreshDropDownModuleTypes()
        {
            comboBoxStructureList.Items.Clear();
            Array enumValues = Enum.GetValues(typeof(StructureSceneID));
            foreach (int value in enumValues)
            {
                string display = Enum.GetName(typeof(StructureSceneID), value);
                comboBoxStructureList.Items.Add(display);
            }
            comboBoxStructureList.SelectedIndex = 0;
        }


        /// <summary>
        /// Refreshes the enabled status of the Dock button.
        /// </summary>
        private void RefreshDockButtonEnabledStatus()
        {
            if (SelectedPrimaryStructure != null && SelectedPrimaryDockingPort != null
                && SelectedSecondaryStructure != null && SelectedSecondaryDockingPort != null
                && !SelectedPrimaryDockingPort.IsDocked && !SelectedSecondaryDockingPort.IsDocked)
            {
                buttonDockPort.Enabled = true;
            }
            else buttonDockPort.Enabled = false;
        }

        /// <summary>
        /// Refreshes the enabled status of the Undock button.
        /// </summary>
        private void RefreshUndockButtonEnabledStatus()
        {
            if (SelectedPrimaryStructure != null && SelectedPrimaryDockingPort != null
                && SelectedPrimaryDockingPort.IsDocked)
            {
                buttonUndockPort.Enabled = true;
            }
            else buttonUndockPort.Enabled = false;
        }

        /// <summary>
        /// Refreshes all the nodes in both TreeViews.
        /// </summary>
        private void RefreshTreeViews()
        {
            treeViewPrimaryStructure.Nodes.Clear();
            treeViewSecondaryStructures.Nodes.Clear();

            // Trigger the reassembly of all node trees in the blueprint.
            blueprint.RefreshAllTreeNodes();

            // Add the primary structure.
            treeViewPrimaryStructure.Nodes.Add(blueprint.PrimaryStructureRoot.RootNode);
            blueprint.PrimaryStructureRoot.RootNode.ExpandAll();

            // Add secondary structures.
            foreach (StationBlueprint.BlueprintStructure _secondaryStructure in blueprint.SecondaryStructureRoots)
            {
                treeViewSecondaryStructures.Nodes.Add(_secondaryStructure.RootNode);
                _secondaryStructure.RootNode.ExpandAll();
            }
        }

        /// <summary>
        /// Refreshes form controls related to the Primary Structure.
        /// </summary>
        private void RefreshSelectedPrimaryStructure()
        {
            RefreshLabelSelectedPrimaryStructure();
            RefreshPictureBoxSelectedPrimaryStructure();
            RefreshLabelSelectedPrimaryDockingPort();
        }

        /// <summary>
        /// Refreshes form controls related to the Secondary Structure.
        /// </summary>
        private void RefreshSelectedSecondaryStructure()
        {
            RefreshLabelSelectedSecondaryStructure();
            RefreshPictureBoxSelectedSecondaryStructure();
            RefreshLabelSelectedSecondaryDockingPort();
        }

        /// <summary>
        /// Refreshes the enabled state of the Save (and SaveAs) menu item(s).
        /// </summary>
        private void RefreshFileSaveMenuStatus()
        {
            saveToolStripMenuItem.Enabled = IsDirty;
            // saveAsToolStripMenuItem.Enabled = IsDirty;
        }

        /// <summary>
        /// Refreshes everything!
        /// </summary>
        public void RefreshEverything()
        {
            RefreshBlueprintEditorFormTitleText();
            RefreshFileSaveMenuStatus();

            RefreshTreeViews();

            RefreshSelectedPrimaryStructure();
            RefreshSelectedSecondaryStructure();

            RefreshDockButtonEnabledStatus();
            RefreshUndockButtonEnabledStatus();
        }

        #endregion

        #region TreeNode Grafting Methods

        /// <summary>
        /// Grafts a node tree inbound from the Main Form. OLD
        /// </summary>
        private void GraftTreeInboundFromMainForm()
        {
            Blueprint_TN drn = (Blueprint_TN)blueprint.PrimaryStructureRoot.RootNode;
            if (drn != null)
            {
                blueprint.RootNode.Nodes.Remove(drn);

                RefreshTreeViews();

                //treeViewPrimaryStructure.Nodes.Add(drn);

                //drn.RefreshToolTipText(includeSubTrees: true);
                drn.ExpandAll();
            }
        }

        /// <summary>
        /// Grafts a node tree outbound from the Main Form. OLD
        /// </summary>
        private void GraftTreeOutboundToMainForm()
        {
            Blueprint_TN drn = (Blueprint_TN)blueprint.PrimaryStructureRoot.RootNode;
            if (drn != null)
            {
                treeViewPrimaryStructure.Nodes.Remove(drn);
                blueprint.RootNode.Nodes.Add(drn);
                //drn.RefreshToolTipText(includeSubTrees: true);
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

        #endregion

        #region Tool Pane Button Event Methods

        /// <summary>
        /// Adds a new structure of the type specified in the comboBoxStructureList.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddStructure_Click(object sender, EventArgs e)
        {
            // Temporarily disabled.
            /*
            if (blueprint != null && (string)comboBoxStructureList.SelectedItem != "Unspecified")
            {
                // Do something - create the new structure in the blueprint.
                HEBlueprintStructureType newStructureType = (HEBlueprintStructureType)Enum.Parse(
                    typeof(HEBlueprintStructureType), (string)comboBoxStructureList.SelectedItem);

                StationBlueprint.BlueprintStructure newStructure = blueprint.AddStructure(newStructureType);

                // Refresh tree views

                RefreshTreeViews();



                // treeViewPrimaryStructure.Nodes.Add(newStructure.RootNode);

                // Select the new node
                treeViewSecondaryStructures.SelectedNode = newStructure.RootNode;

                // Set the focus on the Secondary Structures TreeView.
                treeViewSecondaryStructures.Focus();

                RefreshBlueprintEditorFormTitleText();
                RefreshFileSaveMenuStatus();
            }
            */
        }

        private void buttonRemoveStructure_Click(object sender, EventArgs e)
        {

            RefreshBlueprintEditorFormTitleText();
            RefreshFileSaveMenuStatus();

        }

        private void buttonDockPort_Click(object sender, EventArgs e)
        {
            BlueprintDockingPort a = SelectedPrimaryDockingPort ?? throw new NullReferenceException("SelectedPrimaryDockingPort was null.");
            BlueprintDockingPort b = SelectedSecondaryDockingPort ?? throw new NullReferenceException("SelectedSecondaryDockingPort was null.");

            DockingResultStatus result = blueprint.DockPorts(a, b);

            if (result == DockingResultStatus.Success) SelectedSecondaryStructureNode = null;
            else MessageBox.Show("Result: " + result.ToString(), "Docking Operation Result", MessageBoxButtons.OK, MessageBoxIcon.Error);

            RefreshEverything();
        }

        private void buttonUndockPort_Click(object sender, EventArgs e)
        {
            BlueprintDockingPort a = SelectedPrimaryDockingPort ?? throw new NullReferenceException("SelectedPrimaryDockingPort was null.");

            DockingResultStatus result = blueprint.UndockPort(a);

            if (result == DockingResultStatus.Success) SelectedSecondaryStructureNode = null;
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
            SelectedPrimaryStructureNode = (Blueprint_TN)treeViewPrimaryStructure.SelectedNode;
        }

        /// <summary>
        /// Handles selection events within the SecondaryStructures TreeView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewSecondaryStructures_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Update the info display for the selected item.
            SelectedSecondaryStructureNode = (Blueprint_TN)treeViewSecondaryStructures.SelectedNode;
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
            HellionExplorerProgram.blueprintEditorForms.Remove(this);
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            jsonBlueprintFile.SerialiseFromBlueprintObject();

            jsonBlueprintFile.SaveFile(CreateBackup: true);

            //HEBlueprint.SerialisationTemplate_Blueprint newTemplate = blueprint.GetSerialisationTemplate();
            //JToken newJData = JToken.FromObject(newTemplate);
            //MessageBox.Show(newJData.ToString());
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Fields

        private string FormTitleText = null;
        private StationBlueprint_File jsonBlueprintFile = null;
        private StationBlueprint blueprint = null;
        private Blueprint_TN _selectedPrimaryStructureNode = null;
        private Blueprint_TN _selectedSecondaryStructureNode = null;
        private StationBlueprint.BlueprintStructure _currentStructure = null;
        private StationBlueprint.BlueprintDockingPort _currentDockingPort = null;
        private StationBlueprint.BlueprintStructure _destinationStructure = null;
        private StationBlueprint.BlueprintDockingPort _destinationDockingPort = null;

        #endregion

    }
}
