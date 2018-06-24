using System;
using System.Diagnostics;
using System.Windows.Forms;
using HELLION.DataStructures.Blueprints;
using HELLION.DataStructures.EmbeddedImages;
using HELLION.DataStructures.StaticData;
using HELLION.DataStructures.UI;
using HELLION.DataStructures.Utilities;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.Blueprints.StationBlueprint;
using static HELLION.StationBlueprintEditor.StationBlueprintEditorProgram;

namespace HELLION.StationBlueprintEditor
{
    public partial class StationBlueprintEditorForm : Form
    {
        #region Constructors

        /// <summary>
        /// Basic Constructor.
        /// </summary>
        public StationBlueprintEditorForm()
        {
            InitializeComponent();

            treeViewPrimaryStructure.ImageList = hEImageList.IconImageList;
            treeViewSecondaryStructures.ImageList = hEImageList.IconImageList;

            treeViewPrimaryStructure.ShowNodeToolTips = true;
            treeViewSecondaryStructures.ShowNodeToolTips = true;

            RefreshDropDownModuleTypes();
        }

        #endregion

        #region Properties

        public String FormTitleText
        {
            get => _formTitleText;
            set
            {
                if (_formTitleText != value)
                {
                    _formTitleText = value;

                    RefreshBlueprintEditorFormTitleText();
                }
            }
        }

        /// <summary>
        /// Determines whether the text has been changed.
        /// </summary>
        public bool IsDirty
        {
            get => DocCurrent?.BlueprintObject?.IsDirty ?? false;
        }

        /// <summary>
        /// Represents the currently selected tree node in the Primary Structure TreeView.
        /// </summary>
        public Base_TN SelectedPrimaryStructureNode
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
        public Base_TN SelectedSecondaryStructureNode
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
                        if (parentType == typeof(BlueprintDockingPort))
                        {
                            // Docking Port node, need find the parent structure.
                            SelectedSecondaryDockingPort = (BlueprintDockingPort)_selectedSecondaryStructureNode.OwnerObject;

                            SelectedSecondaryStructure = SelectedSecondaryDockingPort?.OwnerStructure;

                        }
                        else if (parentType == typeof(BlueprintStructure))
                        {
                            SelectedSecondaryDockingPort = null;
                            SelectedSecondaryStructure = (BlueprintStructure)_selectedSecondaryStructureNode.OwnerObject;
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
                    RefreshRemoveButtonEnabledStatus();

                }
            }
        }

        /// <summary>
        /// Represents the currently selected structure.
        /// </summary>
        public BlueprintStructure SelectedPrimaryStructure
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

                }
            }
        }

        /// <summary>
        /// Represents the currently selected docking port.
        /// </summary>
        public BlueprintDockingPort SelectedPrimaryDockingPort
        {
            get => _currentDockingPort;
            private set
            {
                if (_currentDockingPort != value)
                {
                    _currentDockingPort = value;

                    // Trigger updates.
                    RefreshLabelSelectedPrimaryDockingPort();

                }
            }
        }

        /// <summary>
        /// Represents the selected destination structure for docking.
        /// </summary>
        public BlueprintStructure SelectedSecondaryStructure
        {
            get => _destinationStructure;
            private set
            {
                if (_destinationStructure != value)
                {
                    _destinationStructure = value;

                    // Trigger updates.

                    if (value == null) SelectedSecondaryDockingPort = null;

                    RefreshPictureBoxSelectedSecondaryStructure();
                    RefreshLabelSelectedSecondaryStructure();

                }
            }
        }

        /// <summary>
        /// Represents the selected destination structures target docking port for docking.
        /// </summary>
        public BlueprintDockingPort SelectedSecondaryDockingPort
        {
            get => _destinationDockingPort;
            private set
            {
                if (_destinationDockingPort != value)
                {
                    _destinationDockingPort = value;

                    // Trigger updates.

                    RefreshLabelSelectedSecondaryDockingPort();

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
                : SelectedPrimaryStructure.RootNode.Text;
        }

        /// <summary>
        /// Updates the image displayed by the Primary Structure PictureBox.
        /// </summary>
        private void RefreshPictureBoxSelectedPrimaryStructure()
        {
            pictureBoxSelectedPrimaryStructure.Image = SelectedPrimaryStructure == null ? null
                : StationBlueprintEditorProgram.hEImageList.StructureImageList.Images[
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
                : SelectedSecondaryStructure.RootNode.Text;
        }

        /// <summary>
        /// Updates the image displayed by the Secondary Structure PictureBox.
        /// </summary>
        private void RefreshPictureBoxSelectedSecondaryStructure()
        {
            pictureBoxSelectedSecondaryStructure.Image = SelectedSecondaryStructure == null ? null
                : StationBlueprintEditorProgram.hEImageList.StructureImageList.Images[
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
            string newText = DocCurrent?.File == null ? _baseText + " [untitled]" : _baseText + " [" + FormTitleText + "]";
            Text = IsDirty ? newText + "*" : newText;
        }

        /// <summary>
        /// Populates drop-down boxes with the values from the enum.
        /// </summary>
        private void RefreshDropDownModuleTypes()
        {
            comboBoxStructureList.Items.Clear();
            Array enumValues = Enum.GetValues(typeof(StructureSceneID));
            foreach (StructureSceneID value in enumValues)
            {
                string display = value.GetEnumDescription();
                //string display = Enum.GetName(typeof(StructureSceneID), value);
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
        /// Refreshes the status of the Remove (structure) button.
        /// </summary>
        private void RefreshRemoveButtonEnabledStatus()
        {
            if (SelectedSecondaryStructure != null && !(SelectedSecondaryStructure.IsDocked))
            {
                buttonRemoveStructure.Enabled = true;
            }
            else buttonRemoveStructure.Enabled = false;
        }

        /// <summary>
        /// Refreshes all the nodes in both TreeViews.
        /// </summary>
        private void RefreshTreeViews()
        {
            treeViewPrimaryStructure.Nodes.Clear();
            treeViewSecondaryStructures.Nodes.Clear();

            if (DocCurrent?.BlueprintObject != null)
            {
                // Trigger the reassembly of all node trees in the Blueprint.
                DocCurrent.BlueprintObject.RefreshAllTreeNodes();

                if (DocCurrent.BlueprintObject.PrimaryStructureRoot != null)
                {
                    // Add the primary structure.
                    treeViewPrimaryStructure.Nodes.Add(DocCurrent.BlueprintObject.PrimaryStructureRoot.RootNode);
                    DocCurrent.BlueprintObject.PrimaryStructureRoot.RootNode.ExpandAll();

                    // Add secondary structures.
                    foreach (BlueprintStructure _secondaryStructure in DocCurrent.BlueprintObject.SecondaryStructureRoots)
                    {
                        treeViewSecondaryStructures.Nodes.Add(_secondaryStructure.RootNode);
                        _secondaryStructure.RootNode.ExpandAll();
                    }
                }
            }
            else Debug.Print("RefreshTreeViews - DocCurrent or Blueprint was null");
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
            //saveToolStripMenuItem.Enabled = IsDirty;
            // saveAsToolStripMenuItem.Enabled = IsDirty;
        }

        /// <summary>
        /// Refreshes everything.
        /// </summary>
        public void RefreshEverything()
        {
            Debug.Print("---### RefreshEverything called. ###---");
            
            RefreshBlueprintEditorFormTitleText();
            RefreshFileSaveMenuStatus();

            RefreshTreeViews();

            RefreshSelectedPrimaryStructure();
            RefreshSelectedSecondaryStructure();

            RefreshDockButtonEnabledStatus();
            RefreshUndockButtonEnabledStatus();
            RefreshRemoveButtonEnabledStatus();
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
            if (DocCurrent.BlueprintObject != null && (string)comboBoxStructureList.SelectedItem != "Unspecified")
            {
                // Create the new structure in the Blueprint.
                //StructureSceneID newStructureType = (StructureSceneID)Enum.Parse(
                //    typeof(StructureSceneID), (string)comboBoxStructureList.SelectedItem);

                // Attempt to parse the given description to an available one in the enum.
                StructureSceneID newStructureType = ((string)comboBoxStructureList.SelectedItem)
                    .ParseToEnumDescriptionOrEnumerator<StructureSceneID>();

                BlueprintStructure newStructure = DocCurrent.BlueprintObject.AddStructure(newStructureType);

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
        }

        private void buttonRemoveStructure_Click(object sender, EventArgs e)
        {
            // Check the selected secondary current structure is not docked to anything.
            // The button should be disabled so this shouldn't be necessary.
            if (SelectedSecondaryStructure.IsDocked) MessageBox.Show("Unable to remove a docked structure - un-dock first.");

            if (DocCurrent.BlueprintObject.RemoveStructure(SelectedSecondaryStructure)) MessageBox.Show("Removal failed.");
            SelectedSecondaryStructure = null;

            // Trigger refresh.
            RefreshLabelSelectedSecondaryStructure();
            RefreshLabelSelectedSecondaryDockingPort();
            RefreshPictureBoxSelectedSecondaryStructure();
            RefreshTreeViews();
            RefreshRemoveButtonEnabledStatus();
        }

        private void buttonDockPort_Click(object sender, EventArgs e)
        {
            BlueprintDockingPort a = SelectedPrimaryDockingPort ?? throw new NullReferenceException("SelectedPrimaryDockingPort was null.");
            BlueprintDockingPort b = SelectedSecondaryDockingPort ?? throw new NullReferenceException("SelectedSecondaryDockingPort was null.");

            DockingResultStatus result = DocCurrent.BlueprintObject.DockPorts(a, b);

            if (result == DockingResultStatus.Success) SelectedSecondaryStructureNode = null;
            else MessageBox.Show("Result: " + result.ToString(), "Docking Operation Result", MessageBoxButtons.OK, MessageBoxIcon.Error);

            RefreshEverything();
        }

        private void buttonUndockPort_Click(object sender, EventArgs e)
        {
            BlueprintDockingPort a = SelectedPrimaryDockingPort ?? throw new NullReferenceException("SelectedPrimaryDockingPort was null.");

            DockingResultStatus result = DocCurrent.BlueprintObject.UndockPort(a);

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
            SelectedPrimaryStructureNode = (Base_TN)treeViewPrimaryStructure.SelectedNode;
        }

        /// <summary>
        /// Handles selection events within the SecondaryStructures TreeView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewSecondaryStructures_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Update the info display for the selected item.
            SelectedSecondaryStructureNode = (Base_TN)treeViewSecondaryStructures.SelectedNode;
        }

        /// <summary>
        /// Form Closing IsDirty check with prompt to save changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlueprintEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StationBlueprintEditorProgram.ControlledExit();
        }

        #endregion

        #region Form Menu Item Event Methods

        #region File Menu

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileNew();
        }

        private void newFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                // Get the clipboard contents.
                string clipboardText = Clipboard.GetText(TextDataFormat.Text);

                Debug.Print(clipboardText);

                // Attempt to parse the text in to a JToken.
                if (clipboardText.TryParseJson(out JToken jtoken))
                {
                    Debug.Print("JToken HasValues: {0}", jtoken.HasValues.ToString());
                    Debug.Print(jtoken.ToString());

                    // We should have a valid JToken - trigger FileNew.
                    FileNew(jtoken);
                }

                else
                    Debug.Print("newFromClipboardToolStripMenuItem - Parse failure.");
            }
            else MessageBox.Show("Clipboard needs to contain valid JSON data as text.", "Error creating new blueprint from clipboard",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StationBlueprintEditorProgram.FileOpen();
        }

        private void revertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StationBlueprintEditorProgram.FileRevert();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DocCurrent.Serialise();
            DocCurrent.SaveFile(CreateBackup: true);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StationBlueprintEditorProgram.FileSaveAs();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileClose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StationBlueprintEditorProgram.ControlledExit();
        }

        #endregion

        #region Edit Menu
        #endregion

        #region View Menu

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewPrimaryStructure.SelectedNode != null) treeViewPrimaryStructure.SelectedNode.ExpandAll();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeViewPrimaryStructure.SelectedNode != null) treeViewPrimaryStructure.SelectedNode.Collapse();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshEverything();
        }

        /// <summary>
        /// Hides or shows the Tool Panel UI element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolPaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainerMain.Panel2Collapsed = !toolPaneToolStripMenuItem.Checked;
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

        #endregion

        #region Help Menu

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(StationBlueprintEditorProgram.GenerateAboutBoxText(), "About "
                + Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #endregion

        #region Fields

        private StationBlueprint_File _jsonBlueprintFile = null;
        private StationBlueprint _blueprint = null;
        private string _formTitleText = null;
        private Base_TN _selectedPrimaryStructureNode = null;
        private Base_TN _selectedSecondaryStructureNode = null;
        private BlueprintStructure _currentStructure = null;
        private BlueprintDockingPort _currentDockingPort = null;
        private BlueprintStructure _destinationStructure = null;
        private BlueprintDockingPort _destinationDockingPort = null;

        #endregion

        private const string _baseText = "Station Blueprint Editor";
    }
}
