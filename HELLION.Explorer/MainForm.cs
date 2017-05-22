using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;  // To allow use of the Debug object

//using System.IO;
//using System.Threading.Tasks;
//using System.Drawing;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using HELLION.DataStructures;

namespace HELLION.Explorer
{
    public partial class MainForm : Form
    {
        //int iSearchButtonPadding = 5;

        public MainForm()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // User selected MainFile, Exit - call the exit routine
            Program.ControlledExit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens a file
            Program.FileOpen();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show About Dialog Box
            MessageBox.Show(Program.GenerateAboutBoxText(), "About " + Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the object information panel
            if (Program.docCurrent.IsFileReady)
            {

                HEOrbitalObjTreeNode node = null;

                ListView.SelectedListViewItemCollection selection = listView1.SelectedItems;

                foreach (ListViewItem item in selection)
                {
                    // We only process the first
                    node = (HEOrbitalObjTreeNode)item.Tag;
                    break;
                }

                if (node == null)
                {
                    // We seem to get two updates, the first one has no data 
                    
                    //MessageBox.Show("listView1_SelectedIndexChanged: node is null ");
                }
                else
                {
                    // Update the object path + name + Tag in the object identifier bar
                    Program.RefreshSelectedOjectPathBarText(node);
                    //Program.RefreshListView(node);
                    Program.RefreshSelectedObjectSummaryText(node);
                }
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Drill down on double click
            if (Program.docCurrent.IsFileReady)
            {
                // Create a node to represent the currently selected item
                HEOrbitalObjTreeNode node = null;

                // Set up a collection
                ListView.SelectedListViewItemCollection selection = listView1.SelectedItems;

                // Loop through collection but break after first iteration
                foreach (ListViewItem item in selection)
                {
                    // We only process the first item
                    node = (HEOrbitalObjTreeNode)item.Tag;
                    break;
                }
                // Null check - the ListView fires off two events when the selected imdex is changed the
                // first is the deselection of whatever was selected prior and as the ListView control is
                // configured to only allow a single item to be selected this returns null
                // The second firing of the event usually contains the selected item passed
                if (node == null)
                {
                    // We seem to get two updates, the first one has no data 
                }
                else
                {
                    if (node.Nodes.Count > 0)
                    {
                        // Expand the currently selected node
                        treeView1.SelectedNode = node;
                        treeView1.SelectedNode.Expand();
                    }
                }
            } // End of if (Program.docCurrent.IsFileReady)

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            // Update the object information panel
            if (Program.docCurrent.IsFileReady)
            {
                // Update the object path + name + Tag in the object identifier bar
                //Program.RefreshSelectedOjectPathBarText((HEOrbitalObjTreeNode)e.Node);
                //Program.RefreshListView((HEOrbitalObjTreeNode)e.Node);
                //Program.RefreshSelectedObjectSummaryText((HEOrbitalObjTreeNode)e.Node);
            }
        }

        private void setDataFolderLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.DefineGameFolder();
        }

        private void navigationPaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show/hide the Navigation Pane

            // Change the state of the bViewShowNavigationPane
            Program.bViewShowNavigationPane = !Program.bViewShowNavigationPane;

            splitContainer1.Panel1Collapsed = !Program.bViewShowNavigationPane;
            navigationPaneToolStripMenuItem.Checked = Program.bViewShowNavigationPane;
        }

        private void dynamicListPaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show/hide the Dynamic list (listview control)

            Program.bViewShowDynamicList = !Program.bViewShowDynamicList;

            splitContainer2.Panel1Collapsed = !Program.bViewShowDynamicList;

            Program.bViewShowInfoPane = !splitContainer2.Panel1Collapsed;


            dynamicListPaneToolStripMenuItem.Checked = Program.bViewShowDynamicList;
            infoPaneToolStripMenuItem.Checked = Program.bViewShowInfoPane;
        }

        private void infoPaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show/hide the info pane
            Program.bViewShowInfoPane = !Program.bViewShowInfoPane;

            splitContainer2.Panel2Collapsed = !Program.bViewShowInfoPane;
            Program.bViewShowDynamicList = !splitContainer2.Panel1Collapsed;

            dynamicListPaneToolStripMenuItem.Checked = Program.bViewShowDynamicList;
            infoPaneToolStripMenuItem.Checked = Program.bViewShowInfoPane;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.FileClose();
        }

        private void frmMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // User closed the window - call the exit routine
            Program.ControlledExit();
        }
    }
} // End of namespace HELLION.Explorer
