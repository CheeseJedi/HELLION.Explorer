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
                        HEBlueprint.HEBlueprintStructure parentStructure = null;
                        HEBlueprint.HEBlueprintDockingPort dockingPort = null;

                        StringBuilder sb = new StringBuilder();
                        sb.Append("Node Name: " + _currentlySelectedNode.Name + Environment.NewLine);
                        sb.Append("Node Text: " + _currentlySelectedNode.Text + Environment.NewLine);
                        sb.Append("Node Path: " + _currentlySelectedNode.Path() + Environment.NewLine);

                        
                        Type parentType = _currentlySelectedNode.OwnerObject.GetType();
                        if (parentType == typeof(HEBlueprint.HEBlueprintDockingPort))
                        {
                            // Docking Port node, find the parent structure
                            //HEBlueprintStructureTreeNode parentNode = (HEBlueprintStructureTreeNode)CurrentlySelectedNode.Parent;

                            Debug.Print("parentType == typeof(HEBlueprint.HEBlueprintDockingPort)");

                            //dockingPort = (HEBlueprint.HEBlueprintStructure)CurrentlySelectedNode.OwnerObject;
                            //if (CurrentlySelectedNode != dockingPort.RootNode) throw new InvalidOperationException("CurrentlySelectedNode != thisDockingPort.RootNode");


                            //object obj = dockingPort.OwnerObject;

                            //parentStructure = dockingPort.OwnerObject;

                        }
                        else if (parentType == typeof(HEBlueprint.HEBlueprintStructure))
                        {
                            Debug.Print("parentType == typeof(HEBlueprint.HEBlueprintStructure)");
                            parentStructure = (HEBlueprint.HEBlueprintStructure)CurrentlySelectedNode.OwnerObject;
                        }
                        else
                        {
                            Debug.Print("parentType is not blueprint structure or docking port");
                        }
                        



                        // Update form items related to the currently selected object.
                        pictureBoxSelectedStructure.Image = parentStructure == null ? null 
                            : Program.hEImageList.IconImageList.Images[HEImageList.GetStructureImageIndexByStructureType(parentStructure.StructureType.Value)];

                        labelSelectedStructureType.Text = parentStructure == null ? null : parentStructure.StructureType.ToString();


                    }
                    else
                    {
                        // No currently selected node, clear the displays.
                        pictureBoxSelectedStructure.Image = null;
                        labelSelectedStructureType.Text = null;
                    }
                }
            }
        }

        private HEBlueprintTreeNode _currentlySelectedNode = null;





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
