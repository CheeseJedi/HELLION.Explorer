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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // User closed the window - call the exit routine
                Program.ControlledExit();
        } // end of OnFormClosing

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // User selected MainFile, Exit - call the exit routine
            // This will trigger the OnFormClosing event to handle unsaved changes
            System.Windows.Forms.Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //opens a file
            Program.FileOpen();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show About Dialog Box

            // Define a StringBuilder to hold the string to be sent to the dalog box
            StringBuilder sb = new StringBuilder();

            // Assemble the About dialog text
            sb.Append(Environment.NewLine);

            sb.Append(Application.ProductName);
            sb.Append("   Version ");
            sb.Append(Application.ProductVersion);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            var anHELLIONDataStructures = System.Reflection.Assembly.GetAssembly(typeof(HEDocumentWorkspace)).GetName();
            sb.Append(anHELLIONDataStructures.Name);
            sb.Append("   Version ");
            sb.Append(anHELLIONDataStructures.Version);

            sb.Append(Environment.NewLine);

            var anNewtonsoftJson = System.Reflection.Assembly.GetAssembly(typeof(JObject)).GetName();
            sb.Append(anNewtonsoftJson.Name);
            sb.Append("   Version ");
            sb.Append(anNewtonsoftJson.Version);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append(" " + String.Format("Memory usage (bytes): {0:N0}", GC.GetTotalMemory(false)));

            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("Uses the Newtonsoft JSON library. http://www.newtonsoft.com/json");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("HELLION trademarks, content and materials are property of Zero Gravity games or it's licensors. http://http://www.zerogravitygames.com");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);


            sb.Append("This product is 100% certified Cheeseware* and is not dishwasher safe.");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);

            sb.Append("* cheeseware (Noun)");
            sb.Append(Environment.NewLine);
            sb.Append("  1. (computing, slang, pejorative) Exceptionally low-quality software.");
            sb.Append(Environment.NewLine);

            // Send to messagebox
            MessageBox.Show(sb.ToString(), "About " + Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

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
                    if (node.Nodes.Count > 0)
                    {
                        treeView1.SelectedNode = node;
                        treeView1.SelectedNode.Expand();

                        // Update the object path + name + Tag in the object identifier bar
                        //Program.RefreshSelectedOjectPathBarText(node);
                        //Program.RefreshListView(node);
                        //Program.RefreshSelectedObjectSummaryText(node);

                    }

                }
            }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            // Update the object information panel
            if (Program.docCurrent.IsFileReady)
            {
                // Update the object path + name + Tag in the object identifier bar
                Program.RefreshSelectedOjectPathBarText((HEOrbitalObjTreeNode)e.Node);
                Program.RefreshListView((HEOrbitalObjTreeNode)e.Node);
                Program.RefreshSelectedObjectSummaryText((HEOrbitalObjTreeNode)e.Node);
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
            dynamicListPaneToolStripMenuItem.Checked = Program.bViewShowDynamicList;
            infoPaneToolStripMenuItem.Checked = splitContainer2.Panel2Collapsed;
        }

        private void infoPaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show/hide the info pane
            Program.bViewShowInfoPane = !Program.bViewShowInfoPane;

            splitContainer2.Panel1Collapsed = !Program.bViewShowInfoPane;
            infoPaneToolStripMenuItem.Checked = Program.bViewShowInfoPane;
            dynamicListPaneToolStripMenuItem.Checked = splitContainer2.Panel1Collapsed;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.FileClose();
        }

    }
} // End of namespace HELLION.Explorer
