using System;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HELLION.DataStructures;

namespace HELLION.Explorer
{
    /// <summary>
    /// This is the main class that implements the HELLION Explorer program.
    /// </summary>
    /// <remarks>
    /// The primary object of note is the HEDocumentWorkspace, of which there is only a single
    /// instance at a time, created during a File Open operation.
    /// BROAD ACHITECTURE OVERVIEW -- The HEDocumentWorkspace creates two main objects: 
    /// 1. An HEGameDataobject which is responsible for both loading the .save file, all the
    /// Static Data files, and generating the HETreeNode trees representing the data.
    /// 2. An HESolarSystem object which is responsible for generating the Solar System view of
    /// hierarchical objects, representing the orbital structure of the objects in the game. In
    /// addition it represents docked ships (includes modules) in their hierarchical structure
    /// as these are represented as trees within the Dedicated Server.
    /// </remarks>
    static class Program
    {
        /// <summary>
        /// Defines the main form object.
        /// </summary>
        internal static MainForm frmMainForm { get; private set; }

        /// <summary>
        /// Defines an object to hold the current open document
        /// </summary>
        internal static HEDocumentWorkspace docCurrent = null;

        /// <summary>
        /// Initialises an HEImageList object to supply the image list to the tree view and
        /// list view controls, plus anywhere else the images may be used.
        /// </summary>
        internal static HEImageList hEImageList = new HEImageList();

        /// <summary>
        /// Defines an ImageList and set it to the HEImageList
        /// </summary>
        internal static ImageList ilObjectTypesImageList = hEImageList.ImageList;

        /// <summary>
        /// Initialise an HEUpdateChecker object and specify the GitHub user name and repository name.
        /// </summary>
        internal static HEUpdateChecker hEUpdateChecker = new HEUpdateChecker("CheeseJedi", "HELLION.Explorer");

        /// <summary>
        /// Used to trigger debugging comments
        /// </summary>
        internal static bool bLogToDebug = false;

        /// <summary>
        /// Determines whether the Navigation Pane (the split that contains the tree view control) is visible.
        /// </summary>
        internal static bool bViewShowNavigationPane = true;

        /// <summary>
        /// Determines whether the Dynamic List (the split that contains the list view control) is visible.
        /// </summary>
        /// <remarks>
        /// The Dynamic List and the Info Pane share a split container, and trying to set both to not visible
        /// will result in the underlying Split control re-activating the other split, so at least one split is
        /// visible at one time. TODO: Some better logic to handle this will be required.
        /// </remarks>
        internal static bool bViewShowDynamicList = true;

        /// <summary>
        /// Determines whether the Info Pane (the split that contains the tab control of info text, is visible.
        /// </summary>
        /// <remarks>
        /// The Dynamic List and the Info Pane share a split container, and trying to set both to not visible
        /// will result in the underlying Split control re-activating the other split, so at least one split is
        /// visible at one time. TODO: Some better logic to handle this will be required.
        /// </remarks>
        internal static bool bViewShowInfoPane = true;

        /// <summary>
        /// The DirectoryInfo object representing the Static Data directory.
        /// </summary>
        internal static DirectoryInfo dataDirectoryInfo = null;

        /// <summary>
        /// The FileInfo object that represents the currently open file when one is set.
        /// </summary>
        internal static FileInfo saveFileInfo = null;

        /// <summary>
        /// Holds a list of the JsonDataViewForm windows that have been created
        /// </summary>
        internal static List<JsonDataViewForm> jsonDataViews = new List<JsonDataViewForm>();

        /// <summary>
        /// Exits the program in a controlled manner - closes an open document etc.
        /// </summary>
        internal static void ControlledExit()
        {
            // Check the current document isn't null
            if (docCurrent != null)
            {
                // Looks like there was a document open, call the FileClose method.
                FileClose();
            }

            Application.Exit();

            /*
            if (System.Windows.Forms.Application.MessageLoop)
            {
                // WinForms application
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                // Console application
                System.Environment.Exit(1);
            }
            */
        }

        /// <summary>
        /// Loads a .save file in to memory - passes details on to the HEDocumentWorkspace and
        /// tells it to load.
        /// </summary>
        /// <param name="sFileName"></param>
        internal static void FileOpen(string sFileName = "")
        {
            // Make a note of the starting time
            DateTime StartingTime = DateTime.Now;

            // Check that the Data folder path has been defined and the expected files are there
            if (!IsGameDataFolderValid())
            {
                // The checks failed, throw up an error message and cancel the load
                MessageBox.Show("There was a problem with the Data Folder - use 'Set Data Folder' option in Tools menu"); // this needs to be massively improved!
                // Begin repainting the TreeView.
                frmMainForm.treeView1.EndUpdate();
                // Restore mouse cursor
                frmMainForm.Cursor = Cursors.Default;
                return;
            }

            // If the sFileName is set, check the file exists otherwise prompt the user to select a file
            if (sFileName == "")
            {
                // Create a new OpenFileDialog box and set some parameters
                var openFileDialog1 = new OpenFileDialog()
                {
                    Filter = "HELLION DS Save Files|*.save|JSON Files|*.json|All files|*.*",
                    Title = "Open .save file",
                    CheckFileExists = true
                };

                // Show the dialog.
                DialogResult dialogResult = openFileDialog1.ShowDialog();

                // Exit if the user clicked Cancel
                if (dialogResult == DialogResult.Cancel) return;

                // Check that the file exists when the user clicked OK
                if (dialogResult == DialogResult.OK)
                {
                    // Check for an existing document and close it if necessary
                    if (docCurrent != null)
                    {
                        FileClose();
                    }
                    sFileName = openFileDialog1.FileName;
                }
            }
            else
            {
                // We were passed a file name from the command line, check to see if it's actually there
                if (!System.IO.File.Exists(sFileName))
                {
                    // The file name passed doesn't exist
                    MessageBox.Show(String.Format("Error opening file:{1}{0}from command line - file doesn't exist.", Environment.NewLine, sFileName));

                    return;
                }
            }

            saveFileInfo = new FileInfo(sFileName);
            dataDirectoryInfo = new DirectoryInfo(Properties.HELLIONExplorer.Default.sGameDataFolder);

            if (saveFileInfo.Exists && dataDirectoryInfo.Exists)
            {
                // Set the status strip message
                frmMainForm.toolStripStatusLabel1.Text = ("Loading file: " + saveFileInfo.FullName);

                // Update the main window's title text to reflect the filename selected
                RefreshMainFormTitleText(saveFileInfo.FullName);

                //Application.UseWaitCursor = true;
                frmMainForm.Cursor = Cursors.WaitCursor;

                // Suppress repainting the TreeView until all the objects have been created.
                frmMainForm.treeView1.BeginUpdate();

                // Clear any existing nodes
                //frmMainForm.treeView1.Nodes.Clear();

                // Grab the Game Data Folder from Properties
                //string sGameDataFolder = Properties.HELLIONExplorer.Default.sGameDataFolder + "\\";

                //docCurrent.MainFile.FileName = sFileName;

                // Create a new DocumentWorkspace
                docCurrent = new HEDocumentWorkspace(saveFileInfo, dataDirectoryInfo, frmMainForm.treeView1, frmMainForm.listView1, hEImageList);

                // Set up the GuidManager
                HEGuidManager.ClearObservedGuidsList();
                if (docCurrent.GameData.StaticData.DataDictionary.TryGetValue("CelestialBodies.json", out HEJsonBaseFile celestialBodiesJsonBaseFile))
                {
                    HEGuidManager.PopulateObservedGuidsList(celestialBodiesJsonBaseFile.JData);
                }
                HEGuidManager.PopulateObservedGuidsList(docCurrent.GameData.SaveFile.JData);

                // Add the nodes to the TreeView control.
                frmMainForm.treeView1.Nodes.Add(docCurrent.SolarSystem.RootNode);
                frmMainForm.treeView1.Nodes.Add(docCurrent.GameData.RootNode);
                frmMainForm.treeView1.Nodes.Add(docCurrent.Blueprints.RootNode);
                frmMainForm.treeView1.Nodes.Add(docCurrent.SearchHandler.RootNode);

                // Display prettying - set the star as the selected node and expand it and the solar system root node.

                // Expand the Solar System root node.
                docCurrent.SolarSystem.RootNode.Expand();
                // Expand the star node, Hellion.
                docCurrent.SolarSystem.RootNode.FirstNode.Expand();
                // Set the star node as the selected node.
                frmMainForm.treeView1.SelectedNode = docCurrent.SolarSystem.RootNode.FirstNode;

                // Enable the Edit menu and the Find option, leaving the FindNext disabled
                // frmMainForm.editToolStripMenuItem.Enabled = true;
                frmMainForm.findToolStripMenuItem.Enabled = true;
                frmMainForm.findNextToolStripMenuItem.Enabled = false;

                // Begin repainting the TreeView.
                frmMainForm.treeView1.EndUpdate();

                //Application.UseWaitCursor = false;
                frmMainForm.Cursor = Cursors.Default;

                RefreshMainFormTitleText();
                //RefreshSelectedObjectSummaryText(docCurrent.SolarSystemRootNode);

                frmMainForm.toolStripStatusLabel1.Text = String.Format("File load and processing completed in {0:mm}m{0:ss}s", DateTime.Now - StartingTime);

                frmMainForm.closeToolStripMenuItem.Enabled = true;
                frmMainForm.revertToolStripMenuItem.Enabled = true;

                // Blueprint test hook-in
                foreach (HEJsonBlueprintFile entry in docCurrent.Blueprints.Blueprintcollection.DataDictionary.Values)
                {
                    Debug.Print(entry.BlueprintObject.Name);
                    foreach (HEBlueprint.HEBlueprintStructure structure in entry.BlueprintObject.Structures)
                    {
                        Debug.Print("  " + structure.StructureID + " " + structure.StructureType);

                        foreach (HEBlueprint.HEBlueprintDockingPort port in structure.DockingPorts)
                        {
                            Debug.Print("    " + port.PortName);
                            Debug.Print("      Docked to " + port.DockedStructureID + ":" + port.DockedPortName);

                        }

                    }

                }



            }

        }

        /// <summary>
        /// Closes the current document workspace.
        /// </summary>
        internal static void FileClose()
        {
            // Handles closing of files and cleanup of the document workspace.

            // Clear the GuidManager observed GUIDs list
            HEGuidManager.ClearObservedGuidsList();

            // Close down any jsonDataView windows.
            while (jsonDataViews.Count > 0) jsonDataViews[0].Close();

            // isFileDirty check before exiting
            if (docCurrent.IsDirty)
            {
                // Unsaved changes, prompt user to save
                string sMessage = docCurrent.SaveFileInfo.FullName + Environment.NewLine + "This file has been modified. Do you want to save changes before exiting?";
                const string sCaption = "Unsaved Changes";
                var result = MessageBox.Show(sMessage, sCaption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                // If the yes button was pressed ...
                if (result == DialogResult.Yes)
                {
                    // User selected Yes, call save routine
                    MessageBox.Show("User selected Yes to save changes", "Unsaved Changes Dialog", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("Saving is not yet implemented.");
                }
            }


            // Enable the Find option, leaving the FindNext disabled
            // frmMainForm.editToolStripMenuItem.Enabled = false;
            frmMainForm.findToolStripMenuItem.Enabled = false;
            frmMainForm.findNextToolStripMenuItem.Enabled = false;



            // Clear any existing nodes from the tree view
            frmMainForm.treeView1.Nodes.Clear();

            // Clear any items from the list view
            frmMainForm.listView1.Items.Clear();

            // Check for an existing document and close it if necessary
            if (docCurrent != null)
            {
                // Clear the existing document
                docCurrent.Close();
                docCurrent = null;
            }

            frmMainForm.closeToolStripMenuItem.Enabled = false;
            frmMainForm.revertToolStripMenuItem.Enabled = false;

            // Trigger refresh of UI elements
            RefreshMainFormTitleText();

            RefreshSelectedOjectPathBarText(null);
            RefreshListView(null);
            RefreshSelectedObjectSummaryText(null);

            // Initiate Garbage Collection
            GC.Collect();
        }

        /// <summary>
        /// Reverts the current document to the on-disk version.
        /// </summary>
        /// <remarks>
        /// Achieves this by closing the current file 
        /// </remarks>
        internal static void FileRevert()
        {
            string currentFileName = docCurrent.GameData.SaveFile.File.FullName;
            FileClose();
            FileOpen(currentFileName);
        }

        internal static string findSearchKey = null;
        internal static HETreeNode findStartingNode = null;
        internal static List<HETreeNode>.Enumerator findEnumerator;

        /// <summary>
        /// Handles Find and Find Next simple searching of a node in the tree view control's 
        /// currently selected node's Nodes collection.
        /// </summary>
        /// <param name="next"></param>
        internal static void EditFind()
        {
            findSearchKey = HEUtilities.Prompt.ShowDialog("Enter search term to find (not case sensitive):", "Find Node from: "
                + frmMainForm.treeView1.SelectedNode.Text, frmMainForm.Icon);

            HESearchHandler.HESearchOperatorType searchType = HESearchHandler.HESearchOperatorType.Unknown;

            if (true)
            {
                searchType = HESearchHandler.HESearchOperatorType.Find;
                findStartingNode = (HETreeNode)frmMainForm.treeView1.SelectedNode;
            }
            else
            {
                searchType = HESearchHandler.HESearchOperatorType.FindNodesByPath;

            }


            docCurrent.SearchHandler.CreateSearchOperator(searchType);
            if (docCurrent.SearchHandler.CurrentOperator == null) throw new NullReferenceException("CurrentOperator was null.");

            docCurrent.SearchHandler.CurrentOperator.Query = findSearchKey;
            docCurrent.SearchHandler.CurrentOperator.StartingNode = findStartingNode;

            // Execute the query, which updates the results list.
            if (docCurrent.SearchHandler.CurrentOperator.Execute())
            {
                // Get a reference to the Results list enumerator.
                findEnumerator = docCurrent.SearchHandler.CurrentOperator.Results.GetEnumerator();

                frmMainForm.findNextToolStripMenuItem.Enabled = true;
                EditFindNext();
            }
            else
            {
                MessageBox.Show("No results for search term " + findSearchKey);
            }
        }

        /// <summary>
        /// Handles Find and Find Next simple searching of a node in the tree view control's 
        /// currently selected node's Nodes collection.
        /// </summary>
        /// <param name="next"></param>
        internal static void EditFindNext()
        {
            // Most of this code needs to be migrated to the FindHandler
            if (docCurrent.SearchHandler.CurrentOperator.Results.Count > 0)
            {
                if (findEnumerator.MoveNext())
                {
                    // There's a next record
                    frmMainForm.treeView1.SelectedNode = findEnumerator.Current;
                }
                else
                {
                    MessageBox.Show("End of results for search term " + findSearchKey);
                }
            }
            else
            {
                MessageBox.Show("Results count was zero :(");
            }
        }

        /// <summary>
        /// Generates the About dialog text, to be returned to the user by the program in a
        /// MessageBox.
        /// </summary>
        /// <returns></returns>
        internal static string GenerateAboutBoxText()
        {
            // Define a StringBuilder to hold the string to be sent to the dialog box
            StringBuilder sb = new StringBuilder();

            // Create a 'shorthand' for the new line character appropriate for this environment
            string sNL = Environment.NewLine;
            string sNL2 = sNL + sNL;

            // Assemble the About dialog text
            sb.Append(sNL);

            // Add the product name and version
            sb.Append(Application.ProductName);
            sb.Append("   Version ");
            sb.Append(Application.ProductVersion);
            sb.Append(sNL2);

            // Add version information for HELLION.DataStructures.dll
            var anHELLIONDataStructures = System.Reflection.Assembly.GetAssembly(typeof(HEUtilities)).GetName();
            sb.Append(anHELLIONDataStructures.Name);
            sb.Append("   Version ");
            sb.Append(anHELLIONDataStructures.Version);
            sb.Append(sNL);

            // Add version information for NewtonsoftJson.dll -  this is inaccurate and only reports v 10.0.0
            var anNewtonsoftJson = System.Reflection.Assembly.GetAssembly(typeof(JObject)).GetName();
            sb.Append(anNewtonsoftJson.Name);
            sb.Append("   Version ");
            sb.Append(anNewtonsoftJson.Version);
            sb.Append(sNL);

            // Add version information for FastColoredTextBox.dll
            var anFastColoredTextBox = System.Reflection.Assembly.GetAssembly(typeof(FastColoredTextBoxNS.FastColoredTextBox)).GetName();
            sb.Append(anFastColoredTextBox.Name);
            sb.Append("   Version ");
            sb.Append(anFastColoredTextBox.Version);
            sb.Append(sNL2);

            // Add an estimate of current memory usage from the garbage collector
            sb.Append(String.Format("Approx. memory usage (bytes): {0:N0}", GC.GetTotalMemory(false)));
            sb.Append(sNL2);
            sb.Append(sNL);

            // Credit
            sb.Append("Uses the Newtonsoft JSON library. http://www.newtonsoft.com/json");
            sb.Append(sNL2);

            // Credit
            sb.Append("Uses the FastColoredTextBox library. https://github.com/PavelTorgashov/FastColoredTextBox");
            sb.Append(sNL2);

            // Credit
            sb.Append("HELLION trademarks, content and materials are property of Zero Gravity Games or it's licensors. http://www.zerogravitygames.com");
            sb.Append(sNL2);
            sb.Append(sNL);

            // Thanks
            sb.Append("Thanks to all who have helped out in testing, and provided advice and feedback.");
            sb.Append(sNL2);

            // Cheeseware statement
            sb.Append("This product is 100% certified Cheeseware* and is not dishwasher safe.");
            sb.Append(sNL2);

            // Cheeseware definition ;)
            sb.Append("* cheeseware (Noun)");
            sb.Append(sNL);
            sb.Append("  1. (computing, slang, pejorative) Exceptionally low-quality software.");
            sb.Append(sNL);

            return sb.ToString();
        }

        /// <summary>
        /// Called indirectly by menu option on the main form, and directly when opening a file, or by other means.
        /// </summary>
        /// <remarks>
        /// Verifies that there's a data folder defined, and that it's got files in it with familiar names, but honours
        /// the loading flags in the config file and doesn't check files that are marked as not to be loaded.
        /// Does not check the contents of the files, only the existence of them.
        /// </remarks>
        /// <returns></returns>
        internal static bool IsGameDataFolderValid()
        {
            string StoredDataFolderPath = Properties.HELLIONExplorer.Default.sGameDataFolder.Trim();

            // Check GameDataFolder path in settings is not null or empty
            if (StoredDataFolderPath == null || StoredDataFolderPath == "") return false;

            // Check the folder exists
            if (!Directory.Exists(StoredDataFolderPath)) return false;

            // Check the Celestial Bodies file - this one is particularly critical
            if (!File.Exists(StoredDataFolderPath + "\\CelestialBodies.json")) return false;

            // Check the Asteroids file
            if (!File.Exists(StoredDataFolderPath + "\\Asteroids.json")) return false;

            // Check the Structures file
            if (!File.Exists(StoredDataFolderPath + "\\Structures.json")) return false;

            // Check the Dynamic Objects file
            if (!File.Exists(StoredDataFolderPath + "\\DynamicObjects.json")) return false;

            // No checks failed, assume folder is OK
            return true;

        }

        /// <summary>
        /// Checks that the Static Data folder is valid. Called by menu option on the main
        /// form and Is interactive and will prompt the user to set a valid folder.
        /// </summary>
        internal static void VerifyGameDataFolder(bool NotifySuccess = true)
        {
            // Check that the Data folder path has been defined and there's stuff there
            if (!IsGameDataFolderValid())
            {
                // The checks failed, throw up an error message and cancel the load
                MessageBox.Show("There was a problem with the Data Folder - use Set Data Folder option in Tools menu."); // this needs to be massively improved!
                return;
            }
            else if (NotifySuccess)
            {
                MessageBox.Show("Game Data folder: " + Properties.HELLIONExplorer.Default.sGameDataFolder + " seems valid.");
            }
        }

        /// <summary>
        /// Sets the Game Data folder
        /// </summary>
        internal static void SetGameDataFolder(string passedFolder = "")
        {
            if (passedFolder == "")
            {
                // We weren't passed a folder so prompt the user to select one.
                MessageBox.Show("Please use the following folder browser window to select the location of the game Data folder." + Environment.NewLine + Environment.NewLine +
                    "The Data folder and the files within can be obtained as part of the HELLION Dedicated Server installation and are required to load a .save file."
                    , "Please set the Data folder location", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Create a new OpenFileDialog box and set some parameters
                var folderBrowserDialog1 = new FolderBrowserDialog()
                {
                    Description = "Select location of Data folder",
                    RootFolder = Environment.SpecialFolder.Desktop,
                    // Pre-populate the path with whatever is stored in the Properties.
                    SelectedPath = Properties.HELLIONExplorer.Default.sGameDataFolder,
                };
                // If the user clicked OK then set the game data path on the settings.
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) // && folderBrowserDialog1. CheckFolderExists)
                {
                    passedFolder = folderBrowserDialog1.SelectedPath;
                }
            }
            // Set the path and save the settings.
            Properties.HELLIONExplorer.Default.sGameDataFolder = passedFolder;
            Properties.HELLIONExplorer.Default.Save();
        }

        /// <summary>
        /// Regenerates and sets the application's main window title text.
        /// </summary>
        /// <param name="FullFileNameHint"></param>
        internal static void RefreshMainFormTitleText(string FullFileNameHint = "")
        {
            StringBuilder sb = new StringBuilder();

            // Add the product name
            sb.Append(Application.ProductName);

            if (FullFileNameHint != "")
            {
                sb.Append(" [" + FullFileNameHint + "] ");
            }
            else if (docCurrent != null && docCurrent.GameData.SaveFile.File != null && docCurrent.IsWorkspaceReady)
            {
                sb.Append(" [" + docCurrent.GameData.SaveFile.File.FullName + "] ");

                if (docCurrent.IsDirty) sb.Append("*");
            }

            frmMainForm.Text = sb.ToString();
        }

        /// <summary>
        /// Regenerates and sets the object path bar on the main form.
        /// </summary>
        /// <param name="nSelectedNode"></param>
        internal static void RefreshSelectedOjectPathBarText(TreeNode nSelectedNode)
        {
            if (docCurrent != null) //  && docCurrent.IsFileReady)
            {
                // Update the object path + name + Tag in the object summary bar
                StringBuilder sb = new StringBuilder();

                sb.Append(">> ");
                sb.Append(nSelectedNode.FullPath);
                //sb.Append("  (");
                //sb.Append(nSelectedNode.NodeType.ToString());
                //sb.Append(")");

                frmMainForm.label1.Text = sb.ToString();
            }
            else
            {
                frmMainForm.label1.Text = ">>";
            }
        }

        /// <summary>
        /// Regenerates the list view based on the currently selected tree node.
        /// </summary>
        /// <param name="nSelectedNode"></param>
        internal static void RefreshListView(TreeNode nSelectedNode)
        {
            if (nSelectedNode != null) // && docCurrent != null && docCurrent.IsFileReady) // temp change to allow unloaded document tree display
            {
                HETreeNode nSelectedHETNNode = (HETreeNode)nSelectedNode;

                // Clear the list view's items
                frmMainForm.listView1.Items.Clear();

                // Test to see if we're drawing a <PARENT> and <THIS> item in the list view (option not yet implemented, on by default)
                const bool bFakeTestResult = true;
                if (bFakeTestResult)
                {
                    // Only draw the <PARENT> node if it's not null
                    if (nSelectedNode.Parent != null)
                    {
                        HETreeNode nodeParent = (HETreeNode)nSelectedHETNNode.Parent;

                        string[] arrParentItem = new string[2];
                        arrParentItem[0] = "<" + nodeParent.Text + ">";
                        arrParentItem[1] = "<PARENT>";

                        ListViewItem liParentItem = new ListViewItem(arrParentItem)
                        {
                            Name = "<PARENT>",
                            Text = "<" + nSelectedNode.Parent.Text + ">",
                            Tag = nSelectedNode.Parent,
                            ImageIndex = HEImageList.GetImageIndexByNodeType(nodeParent.NodeType)
                        };
                        // Add the item
                        frmMainForm.listView1.Items.Add(liParentItem);
                    }

                    // Draw the <THIS> node if it's not null
                    //if (nSelectedNode.Parent != null && )
                    {
                        string[] arrCurrentItem = new string[2];
                        arrCurrentItem[0] = "<" + nSelectedNode.Text + ">";
                        arrCurrentItem[1] = "<CURRENT>";

                        ListViewItem liCurrentItem = new ListViewItem(arrCurrentItem)
                        {
                            Name = "<CURRENT>",
                            Text = "<" + nSelectedNode.Text + ">",
                            Tag = nSelectedNode,
                            ImageIndex = HEImageList.GetImageIndexByNodeType(nSelectedHETNNode.NodeType)
                        };
                        // Add the item
                        frmMainForm.listView1.Items.Add(liCurrentItem);
                    }
                }

                if (nSelectedHETNNode.NodeType == HETreeNodeType.SearchResultsSet)
                {
                    HESearchHandlerTreeNode nSelectedHESearchHandlerNode = (HESearchHandlerTreeNode)nSelectedNode;

                    if (nSelectedHESearchHandlerNode.ParentSearchOperator.Results != null)
                    {
                        foreach (HETreeNode listItem in nSelectedHESearchHandlerNode.ParentSearchOperator.Results)
                        {
                            string[] arr = new string[7];
                            arr[0] = listItem.Text;
                            arr[1] = listItem.NodeType.ToString();
                            arr[2] = listItem.CountOfChildNodes.ToString();
                            arr[3] = listItem.CountOfAllChildNodes.ToString();
                            arr[4] = listItem.Path();
                            arr[5] = ""; // listItem.GUID.ToString();
                            arr[6] = ""; // nodeChild.SceneID.ToString();

                            ListViewItem liNewItem = new ListViewItem(arr)
                            {
                                Name = listItem.Name,
                                Text = listItem.Text,
                                Tag = listItem
                            };

                            liNewItem.ImageIndex = HEImageList.GetImageIndexByNodeType(listItem.NodeType);

                            // Add the item
                            frmMainForm.listView1.Items.Add(liNewItem);
                        }
                    }
                }
                else
                {
                    foreach (HETreeNode nodeChild in nSelectedNode.Nodes)
                    {
                        string[] arr = new string[7];
                        arr[0] = nodeChild.Text;
                        arr[1] = nodeChild.NodeType.ToString();
                        arr[2] = nodeChild.CountOfChildNodes.ToString();
                        arr[3] = nodeChild.CountOfAllChildNodes.ToString();
                        arr[4] = nodeChild.Path();
                        arr[5] = ""; // nodeChild.GUID.ToString();
                        arr[6] = ""; // nodeChild.SceneID.ToString();

                        ListViewItem liNewItem = new ListViewItem(arr)
                        {
                            Name = nodeChild.Text,
                            Text = nodeChild.Text,
                            Tag = nodeChild
                        };

                        liNewItem.ImageIndex = HEImageList.GetImageIndexByNodeType(nodeChild.NodeType);

                        // Add the item
                        frmMainForm.listView1.Items.Add(liNewItem);
                    }
                }

            }
            else
            {
                //MessageBox.Show("RefreshListView was passed a null nSelectedNode");
            }
        }

        /// <summary>
        /// Regenerates the object summary texts.
        /// </summary>
        /// <param name="nSelectedNode"></param>
        internal static void RefreshSelectedObjectSummaryText(TreeNode nSelectedNode)
        {
            // Updates the Object Information panel

            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();

            if (nSelectedNode != null) //  && docCurrent != null && docCurrent.IsFileReady) // temp change to allow for tree use without a doc loaded
            {
                HETreeNode nSelectedHETNNode = (HETreeNode)nSelectedNode;

                sb1.Append("BASIC NODE DATA");
                sb1.Append(Environment.NewLine);
                sb1.Append("NodeType: " + nSelectedHETNNode.NodeType.ToString());
                sb1.Append(Environment.NewLine);
                sb1.Append("Name: " + nSelectedHETNNode.Name);
                sb1.Append(Environment.NewLine);
                sb1.Append("Text: " + nSelectedHETNNode.Text);
                sb1.Append(Environment.NewLine);
                sb1.Append("FullPath: " + nSelectedHETNNode.FullPath);
                sb1.Append(Environment.NewLine);
                sb1.Append("ToolTipText:" + Environment.NewLine + nSelectedHETNNode.ToolTipText);
                sb1.Append(Environment.NewLine);
                //sb1.Append("SceneID: " + nSelectedNode.SceneID.ToString());
                sb1.Append(Environment.NewLine);
                //sb1.Append("Type: " + nSelectedNode.Type.ToString());
                sb1.Append(Environment.NewLine);
                sb1.Append(Environment.NewLine);

                if (nSelectedHETNNode.NodeType == HETreeNodeType.Star
                    || nSelectedHETNNode.NodeType == HETreeNodeType.Planet
                    || nSelectedHETNNode.NodeType == HETreeNodeType.Moon
                    || nSelectedHETNNode.NodeType == HETreeNodeType.Ship
                    || nSelectedHETNNode.NodeType == HETreeNodeType.Asteroid)
                {

                    HESolarSystemTreeNode nSelectedOrbitalObjNode = (HESolarSystemTreeNode)nSelectedNode;

                    sb1.Append("GUID: " + nSelectedOrbitalObjNode.GUID.ToString());
                    sb1.Append(Environment.NewLine);

                    sb1.Append(Environment.NewLine);
                    sb1.Append("DOCKING DATA");
                    sb1.Append(Environment.NewLine);
                    sb1.Append("DockedToShipGUID: " + nSelectedOrbitalObjNode.DockedToShipGUID.ToString());

                    /*
                    HESolarSystemTreeNode tempNode = null;
                    TreeNode[] tempNodes = docCurrent.SolarSystem.RootNode.Nodes.Find(nSelectedOrbitalObjNode.DockedToShipGUID.ToString(), searchAllChildren: true);
                    if (tempNodes.Length > 0)
                    {
                        tempNode = (HESolarSystemTreeNode)tempNodes[0];

                        if (tempNode == null) throw new NullReferenceException("tempNode was null.");
                        else
                        {
                            sb1.Append(" (" + tempNode.Text + ")");
                        }
                    }
                    else throw new InvalidOperationException("tempNodes array length not greater than zero.");
                    */
                    sb1.Append(Environment.NewLine);
                    sb1.Append("DockedPortID: " + nSelectedOrbitalObjNode.DockedPortID.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("DockedToPortID: " + nSelectedOrbitalObjNode.DockedToPortID.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append(Environment.NewLine);
                    sb1.Append("ORBITAL DATA");
                    sb1.Append(Environment.NewLine);
                    sb1.Append("ParentGUID: " + nSelectedOrbitalObjNode.OrbitData.ParentGUID.ToString());
                    /*
                    tempNodes = null;
                    tempNode = null;
                    tempNodes = docCurrent.SolarSystem.RootNode.Nodes.Find(nSelectedOrbitalObjNode.OrbitData.ParentGUID.ToString(), searchAllChildren: true);
                    if (tempNodes.Length > 0) tempNode = (HESolarSystemTreeNode)tempNodes[0];
                    if (tempNode != null)
                    {
                        sb1.Append(" (" + tempNode.Text + ")");
                    }
                    */
                    sb1.Append(Environment.NewLine);


                    sb1.Append("SemiMajorAxis: " + nSelectedOrbitalObjNode.OrbitData.SemiMajorAxis.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("Inclination: " + nSelectedOrbitalObjNode.OrbitData.Inclination.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("Eccentricity: " + nSelectedOrbitalObjNode.OrbitData.Eccentricity.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("LongitudeOfAscendingNode: " + nSelectedOrbitalObjNode.OrbitData.LongitudeOfAscendingNode.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("ArgumentOfPeriapsis: " + nSelectedOrbitalObjNode.OrbitData.ArgumentOfPeriapsis.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("OrbitData.TimeSincePeriapsis: " + nSelectedOrbitalObjNode.OrbitData.TimeSincePeriapsis.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("OrbitData.SolarSystemPeriapsisTime: " + nSelectedOrbitalObjNode.OrbitData.SolarSystemPeriapsisTime.ToString());
                    sb1.Append(Environment.NewLine);
                }

                if (true) // nSelectedHETNNode.NodeType != HETreeNodeType.SystemNAV) // temp addition
                {
                    // Get the count of the child nodes contained in the selected node
                    decimal iTotalNodeCount = docCurrent.SolarSystem.RootNode.CountOfAllChildNodes;
                    int iThisNodeCount = nSelectedHETNNode.CountOfChildNodes;
                    int iThisNodeAndSubsCount = nSelectedHETNNode.CountOfAllChildNodes;

                    decimal dThisNodeCountAsPercentage = ((decimal)iThisNodeCount / iTotalNodeCount) * 100;
                    decimal dThisNodeAndSubsCountAsPercentage = ((decimal)iThisNodeAndSubsCount / iTotalNodeCount) * 100;

                    sb2.Append("Node object counts for object " + nSelectedHETNNode.Name + " of type " + nSelectedHETNNode.NodeType.ToString());
                    sb2.Append(Environment.NewLine);
                    sb2.Append(Environment.NewLine);

                    sb2.Append("Immediate SubNodes (all types): ");
                    sb2.Append(iThisNodeCount.ToString());
                    sb2.Append(Environment.NewLine);
                    sb2.Append(string.Format(" {0:###.##}% of total ({1})", dThisNodeCountAsPercentage, iTotalNodeCount));
                    sb2.Append(Environment.NewLine);

                    sb2.Append(Environment.NewLine);
                    sb2.Append("All SubNodes (all types): ");
                    sb2.Append(iThisNodeAndSubsCount).ToString();
                    sb2.Append(Environment.NewLine);
                    sb2.Append(string.Format(" {0:###.##}% of total ({1})", dThisNodeAndSubsCountAsPercentage, iTotalNodeCount));
                }
            }

            // Pass results to various text boxes
            frmMainForm.textBox1.Text = sb1.ToString();
            frmMainForm.textBox2.Text = sb2.ToString();


        }

        /// <summary>
        /// Opens up the Json data view form for the selected (HE)TreeNode
        /// </summary>
        /// <param name="nSelectedNode"></param>
        internal static void CreateNewJsonDataView(TreeNode nSelectedNode)
        {
            if (nSelectedNode != null && nSelectedNode.Tag != null)
            {
                Debug.Print("passed node type {0}", nSelectedNode.GetType());

                // define a new window here but somehow make it not static?
                JsonDataViewForm newDataView = new JsonDataViewForm
                {
                    Text = nSelectedNode.FullPath
                };
                //newDataView.label1.Text = nSelectedNode.FullPath;

                // Add the form to the jsonDataViews list so we can work with it later
                jsonDataViews.Add(newDataView);

                // set some FastColouredTextBox properties
                newDataView.fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.JS;

                // fill the data
                newDataView.fastColoredTextBox1.Text = nSelectedNode.Tag.ToString();

                // Show the new form
                newDataView.Show();
            }
        }

        /// <summary>
        /// Returns a single TreeNode with a given path.
        /// </summary>
        /// <param name="tv"></param>
        /// <param name="passedPath"></param>
        /// <returns></returns>
        internal static TreeNode GetNodeByPath(TreeView tv, string passedPath)
        {
            List<string> pathTokens = new List<string>(passedPath.Split('>'));

            TreeNode previousNode = null;

            TreeNode[] currentNodeArray = tv.Nodes.Find(pathTokens[0], false);


            if (currentNodeArray.Length > 0)
            {
                TreeNode currentNode = currentNodeArray[0];

                // Setting the current node was successful, remove it from the list and continue.
                // From here on in we're working with TreeNode's .Nodes collections instead of 
                // the TreeView control itself.
                pathTokens.RemoveAt(0);
                foreach (string token in pathTokens)
                {
                    previousNode = currentNode;

                    currentNodeArray = currentNode.Nodes.Find(token, false);

                    if (currentNodeArray.Length > 0) currentNode = currentNodeArray[0];
                    else
                    {
                        MessageBox.Show("Node not found: " + token);
                    }

                    if (currentNode == null) throw new NullReferenceException("currentNode is null.");
                }
                return currentNode;
            }
            else
            {
                // No results, return null.
                return null;
            }
        }


        /*
        internal static void NodePathSearch2()
        {
            string nodePathSearchKey = HEUtilities.Prompt.ShowDialog("Enter PATH search term to find:", "Find Node from: entered path", frmMainForm.Icon);

            TreeNode result = GetNodeByPath(frmMainForm.treeView1, nodePathSearchKey);

            if (result == null) MessageBox.Show("No results for search term " + nodePathSearchKey);
            else
            {
                MessageBox.Show("Result found: " + result.Text);
                frmMainForm.treeView1.SelectedNode = result;
            }
        }
        */

        /*
        internal static void NodePathSearch()
        {
            string nodePathSearchKey = HEUtilities.Prompt.ShowDialog("Enter PATH search term to find:", "Find Node from: entered path", frmMainForm.Icon);

            docCurrent.SearchHandler.FindNodeByPathOperator.Query = nodePathSearchKey;

            // Execute the query, which updates the results list.
            if (docCurrent.SearchHandler.FindNodeByPathOperator.Execute())
            {
                // Get a reference to the Results list enumerator.
                findEnumerator = docCurrent.SearchHandler.FindNodeByPathOperator.Results.GetEnumerator();

                //frmMainForm.findNextToolStripMenuItem.Enabled = true;
                // EditFindNext(); // this needs replacing

                if (findEnumerator.MoveNext())
                {
                    // There's a next record
                    //frmMainForm.treeView1.SelectedNode = findEnumerator.Current;
                }


            }
            else
            {
                MessageBox.Show("No results for search term " + Environment.NewLine + nodePathSearchKey);
            }
        }
        */






        /// <summary>
        /// Temporary test option, called from the temp menu item.
        /// </summary>
        internal static void TestOption1()
        {
            // Scratch-pad area for testing new stuff out 
            // Make a note of the starting time
            DateTime StartingTime = DateTime.Now;

            // Some async test stuff

            // Task to run asynchronously
            //List<Task> tasks = new List<Task>();
            //Task t1 = Task.Run(() => 

            //tasks.Add(t1);
            //HEJsonFileCollection testDataFileCollection = null;

            //if (Properties.HELLIONExplorer.Default.sGameDataFolder != "")
            {

                //Task t2 = Task.Run(() => //testDataFileCollection = new HEJsonFileCollection(Properties.HELLIONExplorer.Default.sGameDataFolder);
                //tasks.Add(t2);
            }

            // Wait for tasks to complete
            //Task.WaitAll(tasks.ToArray());

            //tempParent.Nodes.Add(testDataFileCollection.RootNode ?? new HETreeNode("DATAFOLDER", HETreeNodeType.DataFolderError, "Data Folder - ERROR"));

            //tempParent.Nodes.Add(nodeSaveFile);


            //foreach (Task t in tasks)
                //Debug.Print("Task {0} Status: {1}", t.Id, t.Status);
            Debug.Print("Process completed in {0:mm}m{0:ss}s", DateTime.Now - StartingTime);

            
            //tempParent.UpdateCounts();
            GC.Collect();
    }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args"></param>
    [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine(Application.ProductName + " - " + Application.ProductVersion);

            #if DEBUG
                Console.WriteLine("Mode=Debug");
            #else
                Console.WriteLine("Mode=Release"); 
            #endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialise the main form
            frmMainForm = new MainForm();

            // Set the form's icon
            var exe = System.Reflection.Assembly.GetExecutingAssembly();
            var iconStream = exe.GetManifestResourceStream("HELLION.Explorer.HELLION.Explorer.ico");
            if (iconStream != null)
                frmMainForm.Icon = new Icon(iconStream);

            // Update the main form's title text - this adds the application name
            RefreshMainFormTitleText();

            // Disable the File/Close menu item - this is re-enabled when a file is loaded
            frmMainForm.closeToolStripMenuItem.Enabled = false;
            frmMainForm.revertToolStripMenuItem.Enabled = false;

            // Show the main form
            frmMainForm.Show();

            if (bLogToDebug)
            {
                // The Length property provides the number of array elements
               Debug.Print("parameter count = {0}", args.Length);

                //for (int i = 0; i < args.Length; i++)
                foreach (string s in args)
                {
                    Debug.Print(s + " ");
                }
            }

            // Valid command line options
            //
            // Hellion.Explorer.exe [<full file name of .save file to open>] [/data <full path to the dedi's Data folder>]
            // 1 argument - just the .save file, Data folder needs to be already defined
            // 2 arguments - Sets the data folder path 
            // 3 arguments - Sets the data folder path and opens the .save file

            if (args.Length > 0)
            {
                // There are arguments 
                string saveFilePath = "";
                string dataFolderPath = "";
                string helpText = Application.ProductName + ".exe [<full file name of .save file to open>] [/data <full path to the dedi's Data folder>]";

                for (int i = 0; i < args.Length; i++)
                {
                    // Try to figure out what's in this argument
                    if (args[i].EndsWith(".save", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's a .save file
                        saveFilePath = args[i];
                        Console.WriteLine("Argument: Save File " + saveFilePath);
                    }
                    else if (args[i].Equals("/data", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's the /data argument, increment i by one to prevent the next element being
                        // processed in case there are other(?) arguments.
                        i++;
                        dataFolderPath = args[i];
                        Console.WriteLine("Argument: Data Folder " + dataFolderPath);
                    }
                    else if (args[i].Equals("/?") || args[i].ToLower().Contains("help"))
                    {
                        Console.WriteLine(helpText);
                    }
                    else
                    {
                        Console.WriteLine("Unexpected Argument: " + args[i]);
                        Console.WriteLine("Use /? or /help to show available arguments.");
                    }
                }

                if (dataFolderPath != "")
                {
                    // Set the Data folder
                    SetGameDataFolder(dataFolderPath);
                }

                if (saveFilePath != "")
                {
                    // Open the .save file
                    FileOpen(saveFilePath);
                }
            }

            // Start the Windows Forms message loop
            Application.Run(); // Application.Run(new MainForm());

        }
    }
}


