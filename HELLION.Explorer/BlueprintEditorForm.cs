using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using HELLION.DataStructures;

namespace HELLION.Explorer
{
    public partial class BlueprintEditorForm : Form
    {
        /// <summary>
        /// Basic Constructor.
        /// </summary>
        public BlueprintEditorForm()
        {
            InitializeComponent();
            Icon = Program.frmMainForm.Icon;

            treeView1.ImageList = Program.hEImageList.IconImageList;
            //treeView1.ImageIndex = (int)HEImageList.HEIconsImageNames.Flag_16x;
            //treeView1.SelectedImageIndex = (int)HEImageList.HEIconsImageNames.Flag_16x;
            //treeView1.TreeViewNodeSorter = new HETNSorterSemiMajorAxis();
            treeView1.ShowNodeToolTips = true;
            Text = "Blueprint Editor";
            PopulateDropDownModuleTypes();

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
            blueprint = jsonBlueprintFile.BlueprintObject;

            GraftTreeInbound();

            IsDirty = false;
        }

        /// <summary>
        /// The node that the editor was opened from.
        /// </summary>
        public HEBlueprintTreeNode SourceNode { get; private set; } = null;

        /// <summary>
        /// Property to get/set the isDirty bool.
        /// </summary>
        public bool IsDirty
        {
            get { return isDirty; }
            private set
            {
                isDirty = value;
                // Enable or disable the Apply Changes menu option.
                //applyChangesToolStripMenuItem.Enabled = value;

                // Update the form name
                RefreshBlueprintEditorFormTitleText();
            }
        }

        /// <summary>
        /// Field that determines whether the text has been changed.
        /// </summary>
        private bool isDirty = false;

        /// <summary>
        /// Stores the form's initial title text.
        /// </summary>
        private string FormTitleText = null;

        private HEBlueprintTreeNode CurrentlySelectedNode
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
                            currentDockingPort = (HEBlueprint.HEBlueprintDockingPort)_currentlySelectedNode.OwnerObject;
                            currentStructure = currentDockingPort.OwnerObject;

                        }
                        else if (parentType == typeof(HEBlueprint.HEBlueprintStructure))
                        {
                            currentDockingPort = null;
                            currentStructure = (HEBlueprint.HEBlueprintStructure)_currentlySelectedNode.OwnerObject;
                        }
                        else  throw new InvalidOperationException("Unrecognised OwnerObject type.");

                        // Update form items related to the currently selected object.
                        pictureBoxSelectedStructure.Image = currentStructure == null ? null 
                            : Program.hEImageList.StructureImageList.Images[HEImageList.GetStructureImageIndexByStructureType(currentStructure.StructureType.Value)];

                        labelSelectedStructureType.Text = currentStructure == null ? null : currentStructure.StructureType.ToString();

                        //comboBoxDockingSourcePort.SelectedIndex = -1;
                        comboBoxDockingSourcePort.Items.Clear();

                        if (currentStructure.AvailableDockingPorts() != null) // && currentStructure.AvailableDockingPorts().Count > 0)
                        {
                            foreach (var port in currentStructure.AvailableDockingPorts())
                                comboBoxDockingSourcePort.Items.Add(port.PortName.ToString());
                            /*
                            // Attempt to select the current docking port from the list.
                            if (currentDockingPort == null)
                            {
                                comboBoxDockingSourcePort.SelectedIndex = -1;

                                comboBoxDockingSourcePort.SelectedItem = currentDockingPort;
                            }
                            */
                            //else
                            {
                                //comboBoxDockingSourcePort.SelectedIndex = -1;
                                comboBoxDockingSourcePort.SelectedIndex = 0;
                            }

                        }
                        else
                        {
                            comboBoxDockingSourcePort.Items.Add("No available docking ports.");
                            //comboBoxDockingSourcePort.SelectedIndex = -1;
                            comboBoxDockingSourcePort.SelectedIndex = 0;
                        }




                    }
                    else
                    {
                        // No currently selected node, clear the displays.
                        pictureBoxSelectedStructure.Image = null;
                        labelSelectedStructureType.Text = null;
                        //comboBoxDockingSourcePort.SelectedIndex = -1;
                        comboBoxDockingSourcePort.Items.Clear();

                    }
                }
            }
        }

        private HEBlueprintTreeNode _currentlySelectedNode = null;

        private HEBlueprint.HEBlueprintStructure currentStructure = null;
        private HEBlueprint.HEBlueprintDockingPort currentDockingPort = null;


        HEJsonBlueprintFile jsonBlueprintFile = null;
        HEBlueprint blueprint = null;

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
            blueprint.RootNode.Nodes.Remove(drn);
            treeView1.Nodes.Add(drn);
        }

        /// <summary>
        /// Grafts a node tree outbound from the Main Form.
        /// </summary>
        private void GraftTreeOutbound()
        {
            HEBlueprintTreeNode drn = blueprint.GetDockingRootNode();
            treeView1.Nodes.Remove(drn);
            blueprint.RootNode.Nodes.Add(drn);
        }

        /// <summary>
        /// Populates drop-down boxes with the values from the enum.
        /// </summary>
        public void PopulateDropDownModuleTypes()
        {
            Array enumValues = Enum.GetValues(typeof(HEBlueprintStructureTypes));
            foreach (int value in enumValues)
            {
                string display = Enum.GetName(typeof(HEBlueprintStructureTypes), value);
                //if (value == (int)HEBlueprintStructureTypes.UNKNOWN) display = "Select Type...";
                // ListViewItem item = new ListViewItem(display, value.ToString());
                toolStripComboBox1.Items.Add(display);
                comboBoxStructureList.Items.Add(display);
            }
            toolStripComboBox1.SelectedIndex = 0;
            comboBoxStructureList.SelectedIndex = 0;
        }


        #region Form Controls
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (blueprint != null)
            {
                // Do something - create the new structure in the blueprint.
                HEBlueprintStructureTypes newStructureType = (HEBlueprintStructureTypes)Enum.Parse(
                    typeof(HEBlueprintStructureTypes), (string)toolStripComboBox1.SelectedItem);

                HEBlueprint.HEBlueprintStructure newStructure = blueprint.AddStructure(newStructureType);
                treeView1.Nodes.Add(newStructure.RootNode);
            }


        }

        private void BlueprintEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsDirty)
            {
                // Unsaved changes, prompt the user to apply them before closing the window.
                MessageBox.Show("Un-applied changes!");
            }

            GraftTreeOutbound();
            // Remove the current JsonDataViewForm from the jsonDataViews list
            Program.blueprintEditorForms.Remove(this);
            GC.Collect();

        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Update the Selected
            CurrentlySelectedNode = (HEBlueprintTreeNode)treeView1.SelectedNode;

        }

        #endregion

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null) treeView1.SelectedNode.ExpandAll();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null) treeView1.SelectedNode.Collapse();
        }
    }
}
