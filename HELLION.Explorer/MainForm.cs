using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;  // To allow use of the Debug object
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HELLION.DataStructures;
//using System.IO;
//using System.Threading.Tasks;
//using System.Drawing;

namespace HELLION.Explorer
{
    public partial class MainForm : Form
    {
        #region MainForm

        /// <summary>
        /// Default constructor - calls InitializeComponent.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the main form is closing, used to catch the user clicking the X control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // User closed the window - call the exit routine
            Program.ControlledExit();
        }

        #endregion

        #region menuStrip1

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

        private void setDataFolderLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.SetGameDataFolder();
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
            // Show/hide the Dynamic list (list view control)

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

        private void updatecheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.hEUpdateChecker.CheckForUpdates();
        }

        private void verifyDataFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.VerifyGameDataFolder();
        }

        private void testOption1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.TestOption1();
        }

        #endregion

        #region treeView1

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Update the object information panel
            //if (Program.docCurrent != null && Program.docCurrent.IsFileReady)
            {
            }

            // Update the object path + name + Tag in the object identifier bar
            Program.RefreshSelectedOjectPathBarText(e.Node);
            Program.RefreshListView(e.Node);

            Program.RefreshSelectedObjectSummaryText(e.Node);



            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {
                // Point where the mouse is clicked.
                //Point p = new Point(e.X, e.Y);

                HETreeNode node = (HETreeNode)e.Node;
                // Get the node that the user has clicked.
                //TreeNode node = treeView1.GetNodeAt(p);
                if (node != null)
                {

                    // Select the node the user has clicked.
                    // The node appears selected until the menu is displayed on the screen.
                    //m_OldSelectNode = (HETreeNode)treeView1.SelectedNode;
                    treeView1.SelectedNode = node;

                    
                    // Find the appropriate ContextMenu depending on the selected node.
                    switch (node.NodeType)
                    {
                        case HETreeNodeType.SaveFile:
                        case HETreeNodeType.DataFile:
                        case HETreeNodeType.JsonArray:
                        case HETreeNodeType.JsonObject:
                        case HETreeNodeType.JsonProperty:
                        case HETreeNodeType.JsonValue:
                            // Enable the 'load' sub-node options
                            loadNextLevelToolStripMenuItem.Visible = true;

                            HEGameDataTreeNode tempNode = (HEGameDataTreeNode)node;
                            if (tempNode.ChildNodesLoaded)
                                loadNextLevelToolStripMenuItem.Enabled = false;
                            else
                                loadNextLevelToolStripMenuItem.Enabled = true;
                            tempNode = null;

                            loadAllLevelsToolStripMenuItem.Visible = true;

                            break;
                        default:
                            loadNextLevelToolStripMenuItem.Visible = false;
                            loadAllLevelsToolStripMenuItem.Visible = false;
                            break;
                    }
                    
                    contextMenuStrip1.Show(treeView1, e.Location);

                    // Re-select the previously selected node.
                    //treeView1.SelectedNode = m_OldSelectNode;
                    //m_OldSelectNode = null;
                }
            }
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Program.frmMainForm.treeView1.SelectedNode != null)
            {
                // Cast the TreeNode to an HETreeNode to determine it's type
                HETreeNode tempHETreeNode = (HETreeNode)Program.frmMainForm.treeView1.SelectedNode;

                switch (tempHETreeNode.NodeType)
                {
                    case HETreeNodeType.SaveFile:
                    case HETreeNodeType.DataFile:
                    case HETreeNodeType.JsonArray:
                    case HETreeNodeType.JsonObject:
                    case HETreeNodeType.JsonProperty:
                    case HETreeNodeType.JsonValue:
                        // We're in the Game Data section

                        HEGameDataTreeNode tempGameDataNode = (HEGameDataTreeNode)Program.frmMainForm.treeView1.SelectedNode;
                        if (!tempGameDataNode.ChildNodesLoaded)
                        {
                            // Load next level
                            tempGameDataNode.CreateChildNodesFromjData(maxDepth: 1);

                            // Update node counts, this may need to be triggered from the parent(s) also.
                            //tempNode.UpdateCounts();
                            
                            // Expand the current node
                            tempGameDataNode.Expand();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region listView1

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the object information panel
            if (Program.docCurrent != null) // && Program.docCurrent.IsFileReady)
            {

                TreeNode node = null;

                ListView.SelectedListViewItemCollection selection = listView1.SelectedItems;

                foreach (ListViewItem item in selection)
                {
                    // We only process the first
                    node = (TreeNode)item.Tag;
                    break;
                }

                if (node == null)
                {
                    // We get two updates, the first one has no data as it's the 
                    // deselection event, in this case we do nothing
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
            if (Program.docCurrent != null) // && Program.docCurrent.IsFileReady)
            {
                // Create a node to represent the currently selected item
                TreeNode node = null;

                // Set up a collection
                ListView.SelectedListViewItemCollection selection = listView1.SelectedItems;

                // Loop through collection but break after first iteration
                foreach (ListViewItem item in selection)
                {
                    // We only process the first item
                    node = (TreeNode)item.Tag;
                    break;
                }
                // Null check - the ListView fires off two events when the selected index is changed the
                // first is the deselection of whatever was selected prior and as the ListView control is
                // configured to only allow a single item to be selected this returns null
                // The second firing of the event usually contains the selected item passed
                if (node == null)
                {
                    // We seem to get two updates, the first one has no data
                    // in this case we do nothing
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

        #endregion

        #region Pop-up menu

        private void loadNextLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Load next level
            HEGameDataTreeNode tempNode = (HEGameDataTreeNode)Program.frmMainForm.treeView1.SelectedNode;
            tempNode.CreateChildNodesFromjData(maxDepth: 1);



            //tempNode.UpdateCounts();
            //tempNode.Expand();

            //tempNode = null;


        }

        private void loadAllLevelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Load all levels (up to internal maximum)
            HEGameDataTreeNode tempNode = (HEGameDataTreeNode)Program.frmMainForm.treeView1.SelectedNode;
            tempNode.CreateChildNodesFromjData(maxDepth: 15);
            //tempNode.UpdateCounts();
            tempNode.Expand();

            //tempNode = null;

        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the node to expand all child items
            Program.frmMainForm.treeView1.SelectedNode.ExpandAll();

        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the node to expand all child items
            Program.frmMainForm.treeView1.SelectedNode.Collapse();
        }

        private void updateCountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HETreeNode tempNode = (HETreeNode)Program.frmMainForm.treeView1.SelectedNode;
            tempNode.UpdateCounts();
        }

        private void jTokenLengthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Tag != null)
                {
                    Debug.Print("Node {0} has a null or empty tag", treeView1.SelectedNode.Text);
                }
            }
            else
            {
                Debug.Print("{0} was called but there was no SelectedNode", this );
            }
        }

        private void findToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Program.FindNodeByName(treeView1);
        }

        private void jsonDataViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.CreateNewJsonDataView(Program.frmMainForm.treeView1.SelectedNode);
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // This item is currently disabled but will be enabled for editable items.
        }


        #endregion

        private void findToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Program.FindNodeByName(treeView1);
        }

        private void updateCountsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            HETreeNode tempNode = (HETreeNode)Program.frmMainForm.treeView1.SelectedNode;
            tempNode.UpdateCounts();
        }
    }
} // End of namespace HELLION.Explorer
