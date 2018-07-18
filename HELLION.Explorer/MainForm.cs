using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using HELLION.DataStructures;
using HELLION.DataStructures.Blueprints;
using HELLION.DataStructures.Document;
using HELLION.DataStructures.UI;
using HELLION.DataStructures.Utilities;

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
            HellionExplorerProgram.ControlledExit();
        }

        #endregion

        #region menuStrip1

        #region File Menu

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens a file
            HellionExplorerProgram.FileOpen();
        }

        private void revertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HellionExplorerProgram.FileRevert();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HellionExplorerProgram.FileSave();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HellionExplorerProgram.FileSaveAs();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HellionExplorerProgram.FileClose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // User selected MainFile, Exit - call the exit routine
            HellionExplorerProgram.ControlledExit();
        }

        #endregion

        #region Edit Menu

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HellionExplorerProgram.ShowFindForm();
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HellionExplorerProgram.FindForm.MainFormFindNextActivated();
        }

        #endregion

        #region View Menu

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((Base_TN)treeView1.SelectedNode)?.Refresh();
        }

        private void navigationPaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show/hide the Navigation Pane

            // Change the state of the bViewShowNavigationPane
            HellionExplorerProgram._viewShowNavigationPane = !HellionExplorerProgram._viewShowNavigationPane;

            splitContainer1.Panel1Collapsed = !HellionExplorerProgram._viewShowNavigationPane;
            navigationPaneToolStripMenuItem.Checked = HellionExplorerProgram._viewShowNavigationPane;
        }

        private void dynamicListPaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show/hide the Dynamic list (list view control)

            HellionExplorerProgram._viewShowDynamicList = !HellionExplorerProgram._viewShowDynamicList;

            splitContainer2.Panel1Collapsed = !HellionExplorerProgram._viewShowDynamicList;

            HellionExplorerProgram._viewShowInfoPane = !splitContainer2.Panel1Collapsed;


            dynamicListPaneToolStripMenuItem.Checked = HellionExplorerProgram._viewShowDynamicList;
            infoPaneToolStripMenuItem.Checked = HellionExplorerProgram._viewShowInfoPane;
        }

        private void infoPaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show/hide the info pane
            HellionExplorerProgram._viewShowInfoPane = !HellionExplorerProgram._viewShowInfoPane;

            splitContainer2.Panel2Collapsed = !HellionExplorerProgram._viewShowInfoPane;
            HellionExplorerProgram._viewShowDynamicList = !splitContainer2.Panel1Collapsed;

            dynamicListPaneToolStripMenuItem.Checked = HellionExplorerProgram._viewShowDynamicList;
            infoPaneToolStripMenuItem.Checked = HellionExplorerProgram._viewShowInfoPane;
        }

        private void observedGUIDsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HellionExplorerProgram.docCurrent != null)
            {
                HellionExplorerProgram.ObservedGuidsForm.Show();
            }
            else MessageBox.Show("Must have an open document first.");

        }

        #endregion

        #region Tools Menu

        private void setDataFolderLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HellionExplorerProgram.SetGameDataFolder();
        }

        private void verifyDataFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HellionExplorerProgram.VerifyGameDataFolder();
        }

        #region Steam Integration - currently disabled

        private void playerNameBySteamID64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _playerNameBySteamID64();
        }

        private void groupID64ByGroupNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _groupID64ByGroupName();
        }

        private void groupMembersByGroupID64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _groupMembersByGroupID64();
        }

        #endregion

        private void stationBlueprintEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && ((Base_TN)treeView1.SelectedNode)
                .NodeType == Base_TN_NodeType.StationBlueprintFile)
            {
                HellionExplorerProgram.CreateNewBlueprintEditor((Json_TN)treeView1.SelectedNode.FirstNode);
            }
            else
            {
                HellionExplorerProgram.CreateNewBlueprintEditor();
            }
        }

        #endregion

        #region Help Menu

        private void updatecheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HellionExplorerProgram.hEUpdateChecker.CheckForUpdates();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show About Dialog Box
            MessageBox.Show(HellionExplorerProgram.GenerateAboutBoxText(), "About " + Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Test Menu

        private void saveTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HellionExplorerProgram.docCurrent != null) //  && Program.docCurrent.IsDirty)
            {
                // Currently always returns false
                bool result = HellionExplorerProgram.docCurrent.GameData.SaveFile.SaveFile(CreateBackup: true);
            }
            else MessageBox.Show("Must have an open document first.");
        }

        private void generateStructureDefinitionsStubjsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Contents to be moved to the BlueprintsHandler_UI class once complete.
            MessageBox.Show("Feature Disabled.");
            //if (HellionExplorerProgram.docCurrent != null)
            //{
            //    if (!HellionExplorerProgram.docCurrent.GameData.StaticData.DataDictionary.
            //        TryGetValue("Structures.json", out Json_File_UI structuresJsonBaseFile))
            //        throw new InvalidOperationException(
            //            "Unable to access the Structures.json file from the Static Data Dictionary.");
            //    else
            //    {
            //        FileInfo newDefsFileInfo = new FileInfo(@"E:\HELLION\TestArea\Output.json");

            //        StructureDefinitions_File newDefsFile =
            //            new StructureDefinitions_File(newDefsFileInfo, structuresJsonBaseFile);

            //        if (newDefsFile.File.Exists) MessageBox.Show("Tentative Success");
            //        else MessageBox.Show("File not created!");
            //    }
            //}
            //else MessageBox.Show("Must have an open document first.");
        }

        #endregion

        #endregion

        #region treeView1

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Update the object path + name + Tag in the object identifier bar
            HellionExplorerProgram.RefreshSelectedOjectPathBarText(e.Node);
            HellionExplorerProgram.RefreshListView(e.Node);
            HellionExplorerProgram.RefreshSelectedObjectSummaryText(e.Node);

            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {
                Base_TN node = (Base_TN)e.Node;
                // Get the node that the user has clicked.
                if (node == null) return;

                // Select in the TreeView the node the user has clicked.
                treeView1.SelectedNode = node;

                // Determine node type and activate appropriate jump to menu items.
                Type t = node.GetType();
                // Handles Json_TN nodes for the Game Data representation.
                if (t.Equals(typeof(Json_TN)))
                {
                    // We're working with a GAME DATA node

                    // Enable the Json Data View
                    //jsonDataViewToolStripMenuItem.Enabled = true;

                    // Editing of Json is now handled by the Edit menu item on the 
                    // context menu. At this point only Objects and Arrays seem
                    // suitable for use in the Json editor - otherwise de-serialisation
                    // fails and changes can't be applied to the main document.

                    switch (node.NodeType)
                    {
                        case Base_TN_NodeType.SaveFile:
                        case Base_TN_NodeType.DataFile:
                        case Base_TN_NodeType.JsonObject:
                        case Base_TN_NodeType.JsonArray:
                            // Show the Edit menu item.
                            editToolStripMenuItem1.Enabled = true;
                            break;

                        default:
                            // Disable the Edit menu item.
                            editToolStripMenuItem1.Enabled = false;
                            break;
                    }



                    // Enable the Jump to sub-menu
                    jumpToToolStripMenuItem.Enabled = true;

                    // GD nodes always have load items visible, even if enabled
                    loadNextLevelToolStripMenuItem.Enabled = true;
                    loadAllLevelsToolStripMenuItem.Enabled = true;

                    // We're in the Game Data already, so disable selection of it
                    thisObjectInGameDataViewToolStripMenuItem.Enabled = false;
                    thisObjectInGameDataViewToolStripMenuItem.Checked = true;

                    // Disable these two as they're Solar System related
                    rootOfDockingTreeToolStripMenuItem.Enabled = false;
                    parentCelestialBodyToolStripMenuItem.Enabled = false;

                    // Cast the node to an Json_TN type
                    Json_TN gDnode = (Json_TN)treeView1.SelectedNode;

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
                
                // Handles SolarSystem_TN Nodes for the Solar System representation.
                else if (t.Equals(typeof(SolarSystem_TN)))
                {
                    // We're working with a SOLAR SYSTEM node

                    // Hide the Edit menu item.
                    editToolStripMenuItem1.Enabled = false;

                    // Disable the Json Data View option.
                    //jsonDataViewToolStripMenuItem.Enabled = false;

                    // Solar System nodes never have load options
                    loadNextLevelToolStripMenuItem.Enabled = false;
                    loadAllLevelsToolStripMenuItem.Enabled = false;

                    // We're in the Solar System already, so disable selection of it
                    thisObjectInSolarSystemViewToolStripMenuItem.Enabled = false;
                    thisObjectInSolarSystemViewToolStripMenuItem.Checked = true;

                    // Cast the node as an SolarSystem_TN type
                    SolarSystem_TN sSnode = (SolarSystem_TN)treeView1.SelectedNode;

                    if (sSnode.GUID == -1 || sSnode.NodeType == Base_TN_NodeType.SolarSystemView
                        || sSnode.NodeType == Base_TN_NodeType.BlueprintHierarchyView)
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
                        rootOfDockingTreeToolStripMenuItem.Enabled = sSnode.IsDockedToParent;
                    }
                }
                
                // Handles Blueprint_TN for blueprint files.
                else if (t.Equals(typeof(Blueprint_TN)))
                {

                    // Some decision making logic needed here

                    if (node.NodeType == Base_TN_NodeType.StationBlueprintFile)
                    {
                        // Show the Edit menu item.
                        editToolStripMenuItem1.Enabled = true;
                    }
                    else editToolStripMenuItem1.Enabled = false;



                    // Disable the Json Data View
                    //jsonDataViewToolStripMenuItem.Enabled = false;

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
                    loadNextLevelToolStripMenuItem.Enabled = false;
                    loadAllLevelsToolStripMenuItem.Enabled = false;

                }

                // Default behaviour - handles other node types.
                else
                {
                    // Hide the Edit menu item.
                    editToolStripMenuItem1.Enabled = false;

                    // Disable the Json Data View
                    //jsonDataViewToolStripMenuItem.Enabled = false;

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
                    loadNextLevelToolStripMenuItem.Enabled = false;
                    loadAllLevelsToolStripMenuItem.Enabled = false;
                }

                // Show the ContextMenuStrip.
                contextMenuStrip1.Show(treeView1, e.Location);

            }
        }

        /// <summary>
        /// Handles double clicks from the mouse - potentially triggering an additional
        /// level of node generation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            HandleExpansionRequest();
        }

        /// <summary>
        /// Handles KeyDown events when the TreeView has focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            //Debug.Print("treeView1_KeyDown fired. [" + e.KeyData.ToString() + "] [" + e.Modifiers.ToString() + "]");

            // Handle Control+Enter as an expansion/load ALL request.
            if (e.KeyData.HasFlag(Keys.Enter) && e.KeyData.HasFlag(Keys.Control))
            {
                Debug.Print("Load (all) Request");
                HandleExpansionRequest(GameData.Def_LoadAllNodeDepth);
                return;
            }


            // Handle Shift+Enter as an expansion/load request.
            if (e.KeyData.HasFlag(Keys.Enter) && e.KeyData.HasFlag(Keys.Shift))
            {
                Debug.Print("Load Request");
                HandleExpansionRequest();
                return;
            }
                        
            // Handle the Enter key similarly to a click.
            if (e.KeyData == Keys.Enter)
            {
                Debug.Print("Selection Request");
                HellionExplorerProgram.RefreshSelectedOjectPathBarText(treeView1.SelectedNode);
                HellionExplorerProgram.RefreshListView(treeView1.SelectedNode);
                HellionExplorerProgram.RefreshSelectedObjectSummaryText(treeView1.SelectedNode);
                return;
            }
        }

        #endregion

        #region listView1

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the object information panel
            if (HellionExplorerProgram.docCurrent != null) // && Program.docCurrent.IsFileReady)
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
                    HellionExplorerProgram.RefreshSelectedOjectPathBarText(node);
                    //Program.RefreshListView(node);
                    HellionExplorerProgram.RefreshSelectedObjectSummaryText(node);
                }
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Drill down on double click
            if (HellionExplorerProgram.docCurrent == null) throw new NullReferenceException("docCurrent was null.");
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
            HandleExpansionRequest();
        }

        private void loadAllLevelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadAllLevels();
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the node to expand all child items
            HellionExplorerProgram.MainForm.treeView1.SelectedNode.ExpandAll();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the node to expand all child items
            HellionExplorerProgram.MainForm.treeView1.SelectedNode.Collapse();
        }

        private void jsonDataViewToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Base_TN node = (Base_TN)HellionExplorerProgram.MainForm.treeView1.SelectedNode;

            Type t = HellionExplorerProgram.MainForm.treeView1.SelectedNode.GetType();

            if (t == typeof(Json_TN))
            {
                HellionExplorerProgram.CreateNewJsonDataView((Json_TN)HellionExplorerProgram.MainForm.treeView1.SelectedNode);

            }
            else if (t == typeof(Blueprint_TN)) // && node.NodeType == Base_TN_NodeType.)
            {
                // This needs updating to support the external Station Blueprint Editor.
                // HellionExplorerProgram.CreateNewBlueprintEditor((Blueprint_TN)HellionExplorerProgram.MainForm.treeView1.SelectedNode);
                MessageBox.Show("External editor not currently available.");

            }
        }

        private void thisObjectInSolarSystemViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null) throw new NullReferenceException("SelectedNode was null.");
            else
            {
                Type t = treeView1.SelectedNode.GetType();
                if (t.Equals(typeof(Json_TN)))
                {
                    Json_TN node = (Json_TN)treeView1.SelectedNode;
                    treeView1.SelectedNode = node.LinkedSolarSystemNode;

                    // Trigger refresh
                    HellionExplorerProgram.RefreshSelectedOjectPathBarText(treeView1.SelectedNode);
                    HellionExplorerProgram.RefreshListView(treeView1.SelectedNode);
                    HellionExplorerProgram.RefreshSelectedObjectSummaryText(treeView1.SelectedNode);
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
                if (t.Equals(typeof(SolarSystem_TN)))
                {
                    SolarSystem_TN node = (SolarSystem_TN)treeView1.SelectedNode;
                    treeView1.SelectedNode = node.LinkedGameDataNode;

                    // Trigger refresh
                    HellionExplorerProgram.RefreshSelectedOjectPathBarText(treeView1.SelectedNode);
                    HellionExplorerProgram.RefreshListView(treeView1.SelectedNode);
                    HellionExplorerProgram.RefreshSelectedObjectSummaryText(treeView1.SelectedNode);
                }
                else throw new InvalidOperationException("Unexpected node type " + t.ToString());
            }
        }

        private void parentCelestialBodyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This is only applicable in the Solar System View
            SolarSystem_TN node = (SolarSystem_TN)treeView1.SelectedNode;
            treeView1.SelectedNode = node.ParentCelestialBody;

            // Trigger refresh
            HellionExplorerProgram.RefreshSelectedOjectPathBarText(treeView1.SelectedNode);
            HellionExplorerProgram.RefreshListView(treeView1.SelectedNode);
            HellionExplorerProgram.RefreshSelectedObjectSummaryText(treeView1.SelectedNode);
        }

        private void rootOfDockingTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // This is only applicable in the Solar System View
            SolarSystem_TN node = (SolarSystem_TN)treeView1.SelectedNode;
            treeView1.SelectedNode = node.DockingTreeRoot;

            // Trigger refresh
            HellionExplorerProgram.RefreshSelectedOjectPathBarText(treeView1.SelectedNode);
            HellionExplorerProgram.RefreshListView(treeView1.SelectedNode);
            HellionExplorerProgram.RefreshSelectedObjectSummaryText(treeView1.SelectedNode);
        }

        #endregion




        private void _playerNameBySteamID64()
        {
            string result = General.Prompt.ShowDialog("Enter SteamID64:", "Lookup Steam Player Name", false, null);
            if (result != null && result != String.Empty)
            {
                MessageBox.Show(SteamIntegration.GetPlayerName(Convert.ToInt64(result)), "Result");
            }
        }

        private void _groupID64ByGroupName()
        {
            string groupName = General.Prompt.ShowDialog("Enter GroupName:", "Lookup Steam GroupID64", false, null);
            if (groupName != null && groupName != String.Empty)
            {
                long? result = SteamIntegration.GetGroupID(groupName);
                MessageBox.Show((result != null ? result.ToString() : "No result."), "GroupID64:");
            }
        }

        private void _groupMembersByGroupID64()
        {
            string result = General.Prompt.ShowDialog("Enter GroupName:", "Lookup Steam Group Members", false, null);
            if (result != null && result != String.Empty)
            {
                List<long> iDs = SteamIntegration.GetGroupMembers(result);

                StringBuilder sb = new StringBuilder();

                sb.Append("Results for lookup of membership of Steam Group:  " + result);
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);

                foreach (long iD in iDs)
                {
                    sb.Append("Player SteamID64: " + iD.ToString() + " " 
                        + SteamIntegration.GetPlayerName(iD) + Environment.NewLine);
                }

                sb.Append(Environment.NewLine);
                sb.Append(String.Format("Group contains {0} member(s).", iDs.Count));
                sb.Append(Environment.NewLine);

                MessageBox.Show(sb.ToString());
            }
        }



        private void triggerGarbageCollectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void findOwningFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (treeView1.SelectedNode.GetType() != typeof(Json_TN))
            {
                MessageBox.Show("Wasn't a Json_TN");
                return;
            }

            Json_TN node = (Json_TN)treeView1.SelectedNode;

            if (HellionExplorerProgram.docCurrent != null && node != null)
            {

                Json_File file = HellionExplorerProgram.docCurrent.GameData.FindOwningFile(node);

                if (file != null)
                {
                    MessageBox.Show(string.Format("TreeNode {0} is owned by file {1}.", node.FullPath, file.File.FullName));
                }
                else MessageBox.Show("No result found.");

            }
            else MessageBox.Show("No open document or selected TreeNode was null.");



        }


        /// <summary>
        /// Handles an expansion request when the keyboard has been used to issue the request.
        /// </summary>
        /// <param name="populateDepth"></param>
        private void HandleExpansionRequest(int populateDepth = 1)
        {
            TreeNode node = HellionExplorerProgram.MainForm.treeView1.SelectedNode;

            if (node != null && node.GetType() == typeof(Json_TN))
            {

                // Make a note of the starting time
                DateTime startingTime = DateTime.Now;

                //Application.UseWaitCursor = true;
                HellionExplorerProgram.MainForm.Cursor = Cursors.WaitCursor;

                // Suppress repainting the TreeView until all the objects have been created.
                HellionExplorerProgram.MainForm.treeView1.BeginUpdate();

                // Update the status bar
                HellionExplorerProgram.MainForm.toolStripStatusLabel1.Text =
                    String.Format("Starting node loading and generation...");

                // Cast the TreeNode to an HETreeNode to determine it's type
                Json_TN jsonNode = (Json_TN)node;
                if (populateDepth == 1)
                {
                    // Load next level
                    LoadNextLevel();

                }
                else
                {
                    LoadAllLevels();
                }
                
                // Expand the current node
                jsonNode.Expand();



                // Begin repainting the TreeView.
                HellionExplorerProgram.MainForm.treeView1.EndUpdate();

                //Application.UseWaitCursor = false;
                HellionExplorerProgram.MainForm.Cursor = Cursors.Default;

                // Update the status bar
                HellionExplorerProgram.MainForm.toolStripStatusLabel1.Text = 
                    String.Format("Node loading and generation completed in {0:mm}m{0:ss}s", 
                    DateTime.Now - startingTime);



            }
        }

        /// <summary>
        /// Triggers the selected node to load (create nodes from) the next level of data.
        /// </summary>
        private void LoadNextLevel() => LoadLevels(1, skipThroughPopulatedNodes: true);

        private void LoadAllLevels() => LoadLevels(GameData.Def_LoadAllNodeDepth, skipThroughPopulatedNodes: true);

        /// <summary>
        /// Triggers the selected node to load (create nodes from) all levels of data up to the
        /// DefaultLoadAllDepth unless another value is specified.
        /// </summary>
        private void LoadLevels(int depth, bool skipThroughPopulatedNodes = false)
        {
            // Load all levels (up to specified depth)
            Json_TN tempNode = (Json_TN)HellionExplorerProgram.MainForm.treeView1.SelectedNode;
            tempNode.RefreshChildNodesFromjData(depth, skipThroughPopulatedNodes);
            //tempNode.UpdateCounts();
            tempNode.Expand();
        }

    }
}
