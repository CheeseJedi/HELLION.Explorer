using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HELLION.DataStructures;

namespace HELLION.Explorer
{
    public partial class BlueprintEditorForm : Form
    {
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

        private string FormTitleText = null;

        private HEBlueprintTreeNode sourceNode = null;

        public HEBlueprintTreeNode SourceNode => sourceNode;

        HEJsonBlueprintFile jsonBlueprintFile = null;
        HEBlueprint blueprint = null;

        /// <summary>
        /// Basic Constructor.
        /// </summary>
        public BlueprintEditorForm()
        {
            InitializeComponent();
            Icon = Program.frmMainForm.Icon;

            treeView1.ImageList = Program.hEImageList.ImageList;
            //treeView1.ImageIndex = (int)HEImageList.HEObjectTypesImageList.Flag_16x;
            //treeView1.SelectedImageIndex = (int)HEImageList.HEObjectTypesImageList.Flag_16x;
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
            sourceNode = passedSourceNode ?? throw new NullReferenceException("passedSourceNode was null.");
            FormTitleText = passedSourceNode.Name;
            RefreshBlueprintEditorFormTitleText();

            jsonBlueprintFile = (HEJsonBlueprintFile)passedSourceNode.OwnerObject;
            blueprint = jsonBlueprintFile.BlueprintObject;

            GraftTreeInbound();

            IsDirty = false;
        }

        /// <summary>
        /// Updates the form's title text with a marker if the object is dirty.
        /// </summary>
        private void RefreshBlueprintEditorFormTitleText()
        {
            Text = IsDirty ? FormTitleText + "*" : FormTitleText;
        }


        private void GraftTreeInbound()
        {
            HEBlueprintTreeNode drn = blueprint.GetDockingRootNode();
            blueprint.RootNode.Nodes.Remove(drn);
            treeView1.Nodes.Add(drn);
        }

        private void GraftTreeOutbound()
        {
            HEBlueprintTreeNode drn = blueprint.GetDockingRootNode();
            treeView1.Nodes.Remove(drn);
            blueprint.RootNode.Nodes.Add(drn);
        }


        public void PopulateDropDownModuleTypes()
        {
            Array enumValues = Enum.GetValues(typeof(HEBlueprintStructureTypes));
            foreach (int value in enumValues)
            {
                string display = Enum.GetName(typeof(HEBlueprintStructureTypes), value);
                //if (value == (int)HEBlueprintStructureTypes.UNKNOWN) display = "Select Type...";
                // ListViewItem item = new ListViewItem(display, value.ToString());
                toolStripComboBox1.Items.Add(display);
                toolStripComboBox1.SelectedIndex = 0;
            }
        }
        
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
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
    }
}
