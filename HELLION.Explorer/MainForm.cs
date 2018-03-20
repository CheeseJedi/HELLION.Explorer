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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.FileSave();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.FileSaveAs();
        }

        private void revertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.FileRevert();
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

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.ShowFindForm();
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.frmFindForm.MainFormFindNextActivated();
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
                HETreeNode node = (HETreeNode)e.Node;
                // Get the node that the user has clicked.
                if (node != null)
                {
                    // Select the node the user has clicked.
                    treeView1.SelectedNode = node;
                    
                    // Determine node type and activate appropriate jump to menu items.
                    Type t = node.GetType();
                    if (t.Equals(typeof(HEGameDataTreeNode)))
                    {
                        // We're working with a GAME DATA node

                        // Hide the Edit menu item.
                        editToolStripMenuItem1.Visible = false;

                        // Enable the Json Data View
                        jsonDataViewToolStripMenuItem.Enabled = true;

                        // Enable the Jump to sub-menu
                        jumpToToolStripMenuItem.Enabled = true;

                        // GD nodes always have load items visible, even if enabled
                        loadNextLevelToolStripMenuItem.Visible = true;
                        loadAllLevelsToolStripMenuItem.Visible = true;
                        toolStripSeparator9.Visible = true;

                        // We're in the Game Data already, so disable selection of it
                        thisObjectInGameDataViewToolStripMenuItem.Enabled = false;
                        thisObjectInGameDataViewToolStripMenuItem.Checked = true;

                        // Disable these two as they're Solar System related
                        rootOfDockingTreeToolStripMenuItem.Enabled = false;
                        parentCelestialBodyToolStripMenuItem.Enabled = false;

                        // Cast the node to an HEGameDataTreeNode type
                        HEGameDataTreeNode gDnode = (HEGameDataTreeNode)treeView1.SelectedNode;

                        // Disable the LoadNextLevel item if it's already been loaded.
                        if (gDnode.ChildNodesLoaded) loadNextLevelToolStripMenuItem.Enabled = false;
                        else loadNextLevelToolStripMenuItem.Enabled = true;

                        if (gDnode.LinkedSolarSystemNode != null)
                        {
                            // It's a Game Data node that has a linked Solar System node 
                            // Enable the Jump to menu item
                            thisObjectInSolarSystemViewToolStripMenuItem.Enabled = true;
                            thisObjectInSolarSystemViewToolStripMenuItem.Checked = false;
                        }
                        else
                        {
                            thisObjectInSolarSystemViewToolStripMenuItem.Enabled = false;
                            thisObjectInSolarSystemViewToolStripMenuItem.Checked = false;
                        }
                    }
                    else if (t.Equals(typeof(HESolarSystemTreeNode)))
                    {
                        // We're working with a SOLAR SYSTEM node

                        // Hide the Edit menu item.
                        editToolStripMenuItem1.Visible = false;

                        // Disable the Json Data View option.
                        jsonDataViewToolStripMenuItem.Enabled = false;

                        // Solar System nodes never have load options
                        loadNextLevelToolStripMenuItem.Visible = false;
                        loadAllLevelsToolStripMenuItem.Visible = false;
                        toolStripSeparator9.Visible = false;

                        // We're in the Solar System already, so disable selection of it
                        thisObjectInSolarSystemViewToolStripMenuItem.Enabled = false;
                        thisObjectInSolarSystemViewToolStripMenuItem.Checked = true;

                        // Cast the node as an HESolarSystemTreeNode type
                        HESolarSystemTreeNode sSnode = (HESolarSystemTreeNode)treeView1.SelectedNode;

                        if (sSnode.GUID == -1 || sSnode.NodeType == HETreeNodeType.SolarSystemView
                            || sSnode.NodeType == HETreeNodeType.BlueprintHierarchyView)
                        {
                            // We're dealing with the Solar System Root Node or a Blueprint Hierarchy
                            // View node, special cases.

                            jumpToToolStripMenuItem.Enabled = false;
                            thisObjectInGameDataViewToolStripMenuItem.Enabled = false;
                            thisObjectInGameDataViewToolStripMenuItem.Checked = false;
                        }
                        else
                        {
                            if (sSnode.LinkedGameDataNode == null) // throw new NullReferenceException("sNode.LinkedGameDataNode was null.");
                            {
                                thisObjectInGameDataViewToolStripMenuItem.Enabled = false;
                                thisObjectInGameDataViewToolStripMenuItem.Checked = false;
                            }
                            else
                            {
                                thisObjectInGameDataViewToolStripMenuItem.Enabled = true;
                                thisObjectInGameDataViewToolStripMenuItem.Checked = false;
                            }
                            // Enable the Jump to sub-menu unless it's the Solar System root node
                            if (sSnode.GUID != -1) jumpToToolStripMenuItem.Enabled = true;
                            else jumpToToolStripMenuItem.Enabled = false;

                            // Enable the Root of Docking Tree option only if the node's parent type
                            // is a ship, indicating it is docked to something (rather than something
                            // being docked *to* this node i.e. child nodes).
                            rootOfDockingTreeToolStripMenuItem.Enabled = sSnode.IsDockedToParent();
                        }
                    }

                    else if (t.Equals(typeof(HEBlueprintTreeNode)))
                    {

                        // Some decision making logic needed here
                        // Show the Edit menu item.
                        editToolStripMenuItem1.Visible = true;


                        // Disable the Json Data View
                        jsonDataViewToolStripMenuItem.Enabled = false;

                        // Disable the Jump to sub-menu
                        jumpToToolStripMenuItem.Enabled = false;

                        // Disable the Solar System Jump to option
                        thisObjectInSolarSystemViewToolStripMenuItem.Enabled = false;
                        thisObjectInSolarSystemViewToolStripMenuItem.Checked = false;

                        // Disable the Game Data Jump to option
                        thisObjectInGameDataViewToolStripMenuItem.Enabled = false;
                        thisObjectInGameDataViewToolStripMenuItem.Checked = false;

                        // Disable these two as they're Solar System related
                        rootOfDockingTreeToolStripMenuItem.Enabled = false;
                        parentCelestialBodyToolStripMenuItem.Enabled = false;

                        // Disable the load options
                        loadNextLevelToolStripMenuItem.Visible = false;
                        loadAllLevelsToolStripMenuItem.Visible = false;
                        toolStripSeparator9.Visible = false;







                    }
                    else
                    {
                        // Hide the Edit menu item.
                        editToolStripMenuItem1.Visible = false;



                        // Disable the Json Data View
                        jsonDataViewToolStripMenuItem.Enabled = false;

                        // Disable the Jump to sub-menu
                        jumpToToolStripMenuItem.Enabled = false;

                        // Disable the Solar System Jump to option
                        thisObjectInSolarSystemViewToolStripMenuItem.Enabled = false;
                        thisObjectInSolarSystemViewToolStripMenuItem.Checked = false;

                        // Disable the Game Data Jump to option
                        thisObjectInGameDataViewToolStripMenuItem.Enabled = false;
                        thisObjectInGameDataViewToolStripMenuItem.Checked = false;

                        // Disable these two as they're Solar System related
                        rootOfDockingTreeToolStripMenuItem.Enabled = false;
                        parentCelestialBodyToolStripMenuItem.Enabled = false;

                        // Disable the load options
                        loadNextLevelToolStripMenuItem.Visible = false;
                        loadAllLevelsToolStripMenuItem.Visible = false;
                        toolStripSeparator9.Visible = false;
                    }

                    // throw new InvalidOperationException("Unexpected node type " + t.ToString());

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
            if (Program.docCurrent == null) throw new NullReferenceException("docCurrent was null.");
            else
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
                    //if (node.Nodes.Count > 0)
                    {
                        // Expand the currently selected node
                        treeView1.SelectedNode = node;
                        treeView1.Focus();
                        //.SelectedNode.Expand();
                    }
                }
            }

        }

        #endregion

        #region Pop-up menu

        private void loadNextLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Load next level
            HEGameDataTreeNode tempNode = (HEGameDataTreeNode)Program.frmMainForm.treeView1.SelectedNode;
            tempNode.CreateChildNodesFromjData(maxDepth: 1);
            //tempNode.UpdateCounts();
        }

        private void loadAllLevelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Load all levels (up to depth of 15)
            HEGameDataTreeNode tempNode = (HEGameDataTreeNode)Program.frmMainForm.treeView1.SelectedNode;
            tempNode.CreateChildNodesFromjData(maxDepth: 15);
            //tempNode.UpdateCounts();
            tempNode.Expand();
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

        private void jsonDataViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.CreateNewJsonDataView((HEGameDataTreeNode)Program.frmMainForm.treeView1.SelectedNode);
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Program.CreateNewBlueprintEditor((HEBlueprintTreeNode)Program.frmMainForm.treeView1.SelectedNode);
        }

        private void updateCountsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            HETreeNode tempNode = (HETreeNode)Program.frmMainForm.treeView1.SelectedNode;
            tempNode.UpdateCounts();
        }

        private void thisObjectInSolarSystemViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null) throw new NullReferenceException("SelectedNode was null.");
            else
            {
                Type t = treeView1.SelectedNode.GetType();
                if (t.Equals(typeof(HEGameDataTreeNode)))
                {
                    HEGameDataTreeNode node = (HEGameDataTreeNode)treeView1.SelectedNode;
                    treeView1.SelectedNode = node.LinkedSolarSystemNode;

                    // Trigger refresh
                    Program.RefreshSelectedOjectPathBarText(treeView1.SelectedNode);
                    Program.RefreshListView(treeView1.SelectedNode);
                    Program.RefreshSelectedObjectSummaryText(treeView1.SelectedNode);
                }
                else throw new InvalidOperationException("Unexpected node type " + t.ToString());
            }
        }

        private void thisObjectInGameDataViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null) throw new NullReferenceException("SelectedNode was null.");
            else
            {
                Type t = treeView1.SelectedNode.GetType();
                if (t.Equals(typeof(HESolarSystemTreeNode)))
                {
                    HESolarSystemTreeNode node = (HESolarSystemTreeNode)treeView1.SelectedNode;
                    treeView1.SelectedNode = node.LinkedGameDataNode;

                    // Trigger refresh
                    Program.RefreshSelectedOjectPathBarText(treeView1.SelectedNode);
                    Program.RefreshListView(treeView1.SelectedNode);
                    Program.RefreshSelectedObjectSummaryText(treeView1.SelectedNode);
                }
                else throw new InvalidOperationException("Unexpected node type " + t.ToString());
            }
        }

        private void parentCelestialBodyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This is only applicable in the Solar System View
            HESolarSystemTreeNode node = (HESolarSystemTreeNode)treeView1.SelectedNode;
            treeView1.SelectedNode = node.GetParentCelestialBody();

            // Trigger refresh
            Program.RefreshSelectedOjectPathBarText(treeView1.SelectedNode);
            Program.RefreshListView(treeView1.SelectedNode);
            Program.RefreshSelectedObjectSummaryText(treeView1.SelectedNode);
        }

        private void rootOfDockingTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This is only applicable in the Solar System View
            HESolarSystemTreeNode node = (HESolarSystemTreeNode)treeView1.SelectedNode;
            treeView1.SelectedNode = node.GetRootOfDockingTree();

            // Trigger refresh
            Program.RefreshSelectedOjectPathBarText(treeView1.SelectedNode);
            Program.RefreshListView(treeView1.SelectedNode);
            Program.RefreshSelectedObjectSummaryText(treeView1.SelectedNode);
        }

        #endregion

        private void saveTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (Program.docCurrent != null) //  && Program.docCurrent.IsDirty)
            {
                // Currently always returns false
                bool result = Program.docCurrent.GameData.SaveFile.SaveFile(CreateBackup: true);


            }
            else
                MessageBox.Show("Something went wrong during SaveFile");
        }

        private void observedGUIDsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.docCurrent != null)
            {
                Program.ObservedGuidsForm.Show();
            }
        }

    }
} // End of namespace HELLION.Explorer
