using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HELLION.DataStructures;
using Newtonsoft.Json.Linq;

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
        #region Form Related Objects

        /// <summary>
        /// Defines the main form object.
        /// </summary>
        internal static MainForm MainForm { get;  private set; }

        /// <summary>
        /// Defines the main form object.
        /// </summary>
        internal static FindForm FindForm { get;  private set; }

        /// <summary>
        /// Reference to the current ObservedGuidsForm, only valid if a document is open.
        /// </summary>
        internal static ObservedGuidsForm ObservedGuidsForm = null;

        /// <summary>
        /// Holds a list of the JsonDataViewForm windows that have been created.
        /// </summary>
        internal static List<JsonDataViewForm> jsonDataViews = new List<JsonDataViewForm>();

        /// <summary>
        /// Holds a list of the BlueprintEditorForm windows that have been created.
        /// </summary>
        internal static List<BlueprintEditorForm> blueprintEditorForms = new List<BlueprintEditorForm>();

        #endregion

        #region File Handling Objects

        /// <summary>
        /// The DirectoryInfo object representing the Static Data directory.
        /// </summary>
        internal static DirectoryInfo dataDirectoryInfo = null;

        /// <summary>
        /// The FileInfo object that represents the currently open file when one is set.
        /// </summary>
        internal static FileInfo saveFileInfo = null;

        /// <summary>
        /// Defines an object to hold the current open document
        /// </summary>
        internal static HEDocumentWorkspace docCurrent = null;

        #endregion

        #region View Settings

        /// <summary>
        /// Determines whether the Navigation Pane (the split that contains the tree view control) is visible.
        /// </summary>
        internal static bool _viewShowNavigationPane = true;

        /// <summary>
        /// Determines whether the Dynamic List (the split that contains the list view control) is visible.
        /// </summary>
        /// <remarks>
        /// The Dynamic List and the Info Pane share a split container, and trying to set both to not visible
        /// will result in the underlying Split control re-activating the other split, so at least one split is
        /// visible at one time. TODO: Some better logic to handle this will be required.
        /// </remarks>
        internal static bool _viewShowDynamicList = true;

        /// <summary>
        /// Determines whether the Info Pane (the split that contains the tab control of info text, is visible.
        /// </summary>
        /// <remarks>
        /// The Dynamic List and the Info Pane share a split container, and trying to set both to not visible
        /// will result in the underlying Split control re-activating the other split, so at least one split is
        /// visible at one time. TODO: Some better logic to handle this will be required.
        /// </remarks>
        internal static bool _viewShowInfoPane = true;

        #endregion

        #region Misc Objects

        /// <summary>
        /// Initialise an HEUpdateChecker object and specify the GitHub user name and repository name.
        /// </summary>
        internal static HEUpdateChecker hEUpdateChecker = new HEUpdateChecker
            ("CheeseJedi", "HELLION.Explorer");

        /// <summary>
        /// Initialises an HEImageList object to supply the image list to the tree view and
        /// list view controls, plus anywhere else the images may be used.
        /// </summary>
        internal static HEImageList hEImageList = new HEImageList();

        /// <summary>
        /// Defines an ImageList and set it to the HEImageList
        /// </summary>
        internal static ImageList ilObjectTypesImageList = hEImageList.IconImageList;

        #endregion

        #region New SubForm Creation

        /// <summary>
        /// Opens a new or existing JsonDataView form for the selected (HE)TreeNode.
        /// </summary>
        /// <param name="nSelectedNode"></param>
        internal static void CreateNewJsonDataView(HEGameDataTreeNode nSelectedNode)
        {
            if (nSelectedNode != null && nSelectedNode.JData != null)
            {
                // Look for an existing form for this node.
                JsonDataViewForm newDataView = null;
                foreach (JsonDataViewForm form in jsonDataViews)
                {
                    if (form.SourceNode == nSelectedNode)
                    {
                        newDataView = form;
                        break;
                    }
                }

                if (newDataView == null)
                {
                    // No existing form for this node was found, create a new one.
                    newDataView = new JsonDataViewForm(nSelectedNode);

                    // Add the form to the jsonDataViews list.
                    jsonDataViews.Add(newDataView);
                }

                // Show the form.
                newDataView.Show();
                newDataView.Activate();
            }
        }

        /// <summary>
        /// Opens a new or existing JsonDataView form for the selected (HE)TreeNode.
        /// </summary>
        /// <param name="selectedNode"></param>
        internal static void CreateNewBlueprintEditor(HEBlueprintTreeNode selectedNode)
        {
            if (selectedNode != null)
            {
                // Look for an existing form for this node.
                BlueprintEditorForm newBlueprintEditorForm = null;
                foreach (BlueprintEditorForm form in blueprintEditorForms)
                {
                    if (form.SourceNode == selectedNode)
                    {
                        newBlueprintEditorForm = form;
                        break;
                    }
                }

                if (newBlueprintEditorForm == null)
                {
                    // No existing form for this node was found, create a new one.
                    newBlueprintEditorForm = new BlueprintEditorForm(selectedNode);

                    // Add the form to the jsonDataViews list.
                    blueprintEditorForms.Add(newBlueprintEditorForm);
                }

                // Show the form.
                newBlueprintEditorForm.Show();
                newBlueprintEditorForm.Activate();
            }
        }

        #endregion

        #region Misc Methods

        /// <summary>
        /// Processes any command line arguments issued to the program.
        /// </summary>
        /// <remarks>
        /// Hellion.Explorer.exe [(full file name of .save file to open)] [/data (full path to the dedi's Data folder)]
        /// 1 argument - just the .save file, Data folder needs to be already defined
        /// 2 arguments - Sets the data folder path 
        /// 3 arguments - Sets the data folder path and opens the .save file
        /// </remarks>
        /// <param name="arguments"></param>
        internal static void ProcessCommandLineArguments(string[] arguments)
        {
            if (arguments.Length > 0)
            {
                // There are arguments 
                string saveFilePath = "";
                string dataFolderPath = "";
                string helpText = Application.ProductName + ".exe [<full file name of .save file to open>] [/data <full path to the dedi's Data folder>]";

                for (int i = 0; i < arguments.Length; i++)
                {
                    // Try to figure out what's in this argument
                    if (arguments[i].EndsWith(".save", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's a .save file
                        saveFilePath = arguments[i];
                        Console.WriteLine("Argument: Save File " + saveFilePath);
                    }
                    else if (arguments[i].Equals("/data", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's the /data argument, increment i by one to prevent the next element being
                        // processed in case there are other(?) arguments.
                        i++;
                        dataFolderPath = arguments[i];
                        Console.WriteLine("Argument: Data Folder " + dataFolderPath);
                    }
                    else if (arguments[i].Equals("/?") || arguments[i].ToLower().Contains("help"))
                    {
                        Console.WriteLine(helpText);
                    }
                    else
                    {
                        Console.WriteLine("Unexpected Argument: " + arguments[i]);
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

        #endregion

        #region File Menu Methods

        /// <summary>
        /// Loads a .save file in to memory - passes details on to the HEDocumentWorkspace and
        /// tells it to load.
        /// </summary>
        /// <param name="sFileName"></param>
        internal static void FileOpen(string sFileName = "")
        {
            // Make a note of the starting time
            DateTime startingTime = DateTime.Now;

            // Check that the Data folder path has been defined and the expected files are there
            if (!IsGameDataFolderValid())
            {
                // The checks failed, throw up an error message and cancel the load
                MessageBox.Show("There was a problem with the Data Folder - use 'Set Data Folder' option in Tools menu."); // this needs to be massively improved!
                // Begin repainting the TreeView.
                MainForm.treeView1.EndUpdate();
                // Restore mouse cursor
                MainForm.Cursor = Cursors.Default;
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
                MainForm.toolStripStatusLabel1.Text = ("Loading file: " + saveFileInfo.FullName);

                // Update the main window's title text to reflect the filename selected
                RefreshMainFormTitleText(saveFileInfo.FullName);

                //Application.UseWaitCursor = true;
                MainForm.Cursor = Cursors.WaitCursor;

                // Suppress repainting the TreeView until all the objects have been created.
                MainForm.treeView1.BeginUpdate();

                // Create a new DocumentWorkspace
                docCurrent = new HEDocumentWorkspace(saveFileInfo, dataDirectoryInfo, MainForm.treeView1, MainForm.listView1, hEImageList);

                // Set up the GuidManager
                HEGuidManager.ClearObservedGuidsList();
                // Add the Celestial Bodies GUIDs.
                if (docCurrent.GameData.StaticData.DataDictionary.TryGetValue("CelestialBodies.json", out HEBaseJsonFile celestialBodiesJsonBaseFile))
                {
                    HEGuidManager.PopulateObservedGuidsList(celestialBodiesJsonBaseFile.JData);
                }
                // Add the GUIDs from the save file.
                HEGuidManager.PopulateObservedGuidsList(docCurrent.GameData.SaveFile.JData);

                ObservedGuidsForm = new ObservedGuidsForm();
                ObservedGuidsForm.Hide();

                // Add the nodes to the TreeView control.
                MainForm.treeView1.Nodes.Add(docCurrent.SolarSystem.RootNode);
                MainForm.treeView1.Nodes.Add(docCurrent.GameData.RootNode);
                MainForm.treeView1.Nodes.Add(docCurrent.Blueprints.RootNode);
                MainForm.treeView1.Nodes.Add(docCurrent.SearchHandler.RootNode);

                // Trigger a refresh on each of the node trees.
                docCurrent.SolarSystem.RootNode.RefreshToolTipText(includeSubtrees: true);
                docCurrent.GameData.RootNode.RefreshToolTipText(includeSubtrees: true);
                docCurrent.Blueprints.RootNode.RefreshToolTipText(includeSubtrees: true);
                docCurrent.SearchHandler.RootNode.RefreshToolTipText(includeSubtrees: true);




                //MainForm.treeView1.Sort();
                // Display prettying - set the star as the selected node and expand it and the solar system root node.

                // Expand the Solar System root node.
                docCurrent.SolarSystem.RootNode.Expand();
                // Expand the star node, Hellion.
                docCurrent.SolarSystem.RootNode.FirstNode.Expand();
                // Set the star node as the selected node.
                MainForm.treeView1.SelectedNode = docCurrent.SolarSystem.RootNode.FirstNode;

                // Enable the Find option, leaving the FindNext disabled.
                MainForm.findToolStripMenuItem.Enabled = true;
                MainForm.findNextToolStripMenuItem.Enabled = false;

                // Enable the Save and Save As menu items.
                MainForm.saveToolStripMenuItem.Enabled = true;
                MainForm.saveAsToolStripMenuItem.Enabled = true;

                // Enable the Observed GUIDs menu item.
                MainForm.observedGUIDsToolStripMenuItem.Enabled = true;

                // Begin repainting the TreeView.
                MainForm.treeView1.EndUpdate();

                //Application.UseWaitCursor = false;
                MainForm.Cursor = Cursors.Default;

                RefreshMainFormTitleText();
                //RefreshSelectedObjectSummaryText(docCurrent.SolarSystemRootNode);

                MainForm.toolStripStatusLabel1.Text = String.Format("File load and processing completed in {0:mm}m{0:ss}s", DateTime.Now - startingTime);

                MainForm.closeToolStripMenuItem.Enabled = true;
                MainForm.revertToolStripMenuItem.Enabled = true;

                
            }
        }

        /// <summary>
        /// Performs a save operation of the current document, or if a string file name is
        /// passed it will perform a SaveAs operation to the new desired file name.
        /// </summary>
        /// <param name="passedFileName"></param>
        internal static void FileSave(string passedFileName = null)
        {
            if (docCurrent == null) throw new NullReferenceException("docCurrent was null.");
            else
            {
                string newFileName = passedFileName ?? docCurrent.GameData.SaveFile.File.FullName;
                // Call the docCurrent's save file's .Save() method.
                //docCurrent.GameData.SaveFile.SaveFile(CreateBackup: true);
            }
        }

        /// <summary>
        /// Prompts the user to input a new file name then calls the FileSave method.
        /// </summary>
        internal static void FileSaveAs()
        {
            if (docCurrent == null) throw new NullReferenceException("docCurrent was null.");
            else
            {

                // Display Save As dialog, with current file name passed to be used as the
                // default file name.
                var saveFileDialog1 = new SaveFileDialog()
                {
                    FileName = docCurrent.GameData.SaveFile.File.FullName,
                    Filter = "HELLION DS Save Files|*.save|JSON Files|*.json|All files|*.*",
                    Title = "Save As",

                };

                // Show the dialog.
                DialogResult dialogResult = saveFileDialog1.ShowDialog();

                // Exit if the user clicked Cancel
                if (dialogResult == DialogResult.Cancel) return;

                // Check that the file exists when the user clicked Save As.
                if (dialogResult == DialogResult.OK)
                {
                    // Call FileSave with the supplied path + file name.
                    FileSave(saveFileDialog1.FileName);
                }
            }
        }

        /// <summary>
        /// Closes the current document workspace.
        /// </summary>
        internal static void FileClose()
        {
            // Handles closing of files and cleanup of the document workspace.

            // isFileDirty check before exiting
            if (docCurrent.IsDirty)
            {
                // Unsaved changes, prompt user to save
                string sMessage = docCurrent.SaveFileInfo.FullName + Environment.NewLine + "This file has been modified. Do you want to save changes before exiting?";
                const string sCaption = "Unsaved Changes";
                var result = MessageBox.Show(sMessage, sCaption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Cancel) return;

                // If the yes button was pressed ...
                if (result == DialogResult.Yes)
                {
                    // User selected Yes, call save routine
                    MessageBox.Show("User selected Yes to save changes", "Unsaved Changes Dialog", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("Saving is not yet implemented.");
                }
            }

            // Close and remove the ObservedGuidsForm
            ObservedGuidsForm.Close();
            ObservedGuidsForm = null;
            // Clear the GuidManager observed GUIDs list
            HEGuidManager.ClearObservedGuidsList();

            // Close down any jsonDataView windows.
            while (jsonDataViews.Count > 0) jsonDataViews[0].Close();

            // Disable both the Find and FindNext menu items.
            MainForm.findToolStripMenuItem.Enabled = false;
            MainForm.findNextToolStripMenuItem.Enabled = false;

            // Disable the Save and Save As menu items.
            MainForm.saveToolStripMenuItem.Enabled = false;
            MainForm.saveAsToolStripMenuItem.Enabled = false;

            // Disable the Observed GUIDs menu item.
            MainForm.observedGUIDsToolStripMenuItem.Enabled = false;

            // Clear any existing nodes from the tree view
            MainForm.treeView1.Nodes.Clear();

            // Clear any items from the list view
            MainForm.listView1.Items.Clear();

            // Check for an existing document and close it if necessary
            if (docCurrent != null)
            {
                // Clear the existing document
                docCurrent.Close();
                docCurrent = null;
            }

            MainForm.closeToolStripMenuItem.Enabled = false;
            MainForm.revertToolStripMenuItem.Enabled = false;

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

        #endregion

        #region Edit Menu Methods

        internal static string findSearchKey = null;
        internal static HETreeNode findStartingNode = null;
        internal static List<HETreeNode>.Enumerator findEnumerator;

        /// <summary>
        /// Resets and shows the Find form.
        /// </summary>
        internal static void ShowFindForm()
        {
            FindForm.ResetForm();
            FindForm.Show();
        }

        /// <summary>
        /// Handles Find and Find Next simple searching of a node in the tree view control's 
        /// currently selected node's Nodes collection.
        /// </summary>
        /// <param name="next"></param>
        internal static void EditFind(bool NewQuery = false, bool JumpToResultsSet = false)
        {
            Debug.Print("EditFind Called NewQuery:" + NewQuery.ToString() + " JumpToResultSet: " + JumpToResultsSet.ToString());

            if (NewQuery)
            {
                HESearchHandler.HESearchOperatorFlags operatorFlags = 0;
                if (FindForm.PathSearchValue) operatorFlags |= HESearchHandler.HESearchOperatorFlags.ByPath;
                if (FindForm.MatchCaseValue)
                {
                    Debug.Print("Match Case ON");
                    operatorFlags |= HESearchHandler.HESearchOperatorFlags.MatchCase;
                }

                Debug.Print("OPERATOR_FLAGS=" + operatorFlags);

                docCurrent.SearchHandler.CreateSearchOperator(operatorFlags);
                if (docCurrent.SearchHandler.CurrentOperator == null) throw new NullReferenceException("CurrentOperator was null.");

                docCurrent.SearchHandler.CurrentOperator.Query = FindForm.QueryValue;
                docCurrent.SearchHandler.CurrentOperator.StartingNode = (HETreeNode)MainForm.treeView1.SelectedNode;

                // Execute the query, which updates the results list.
                if (!docCurrent.SearchHandler.CurrentOperator.Execute()) MessageBox.Show("No results for search term " + findSearchKey);
                else
                {
                    // Get a reference to the Results list enumerator.
                    findEnumerator = docCurrent.SearchHandler.CurrentOperator.Results.GetEnumerator();

                    MainForm.findNextToolStripMenuItem.Enabled = true;
                    //EditFindNext(JumpToResultsSet);
                }
            }

            // Most of this code needs to be migrated to the FindHandler
            if (!(docCurrent.SearchHandler.CurrentOperator.Results.Count > 0)) MessageBox.Show("Results count was zero :(");
            else
            {
                if (findEnumerator.MoveNext())
                {
                    // There's a next record

                    if (!JumpToResultsSet) MainForm.treeView1.SelectedNode = findEnumerator.Current;
                    else
                    {
                        MainForm.treeView1.SelectedNode = docCurrent.SearchHandler.CurrentOperator.RootNode;
                        MainForm.treeView1.SelectedNode.Expand();
                    }
                }
                else
                {
                    if (!JumpToResultsSet) MessageBox.Show("End of results for search term " + findSearchKey);
                    else
                    {
                        MainForm.treeView1.SelectedNode = docCurrent.SearchHandler.CurrentOperator.RootNode;
                        MainForm.treeView1.SelectedNode.Expand();
                    }
                }
            }
        }

        /// <summary>
        /// Handles Find and Find Next simple searching of a node in the tree view control's 
        /// currently selected node's Nodes collection.
        /// </summary>
        /// <param name="next"></param>
        internal static void EditFindNext(bool JumpToResultsSet = false)
        {
            // This method is defunct and should not be called any more.
            MessageBox.Show("EditFindNext was called!!");
        }

        #endregion

        #region Tools Menu Methods

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

        #endregion

        #region Refresh Methods

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

            MainForm.Text = sb.ToString();
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

                sb.Append(nSelectedNode.FullPath);
                //sb.Append("  (");
                //sb.Append(nSelectedNode.NodeType.ToString());
                //sb.Append(")");

                MainForm.label1.Text = sb.ToString();
            }
            else
            {
                MainForm.label1.Text = ">>";
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
                MainForm.listView1.Items.Clear();

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
                            ImageIndex = HEImageList.GetIconImageIndexByNodeType(nodeParent.NodeType)
                        };
                        // Add the item
                        MainForm.listView1.Items.Add(liParentItem);
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
                            ImageIndex = HEImageList.GetIconImageIndexByNodeType(nSelectedHETNNode.NodeType)
                        };
                        // Add the item
                        MainForm.listView1.Items.Add(liCurrentItem);
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
                            arr[2] = listItem.GetNodeCount(includeSubTrees: false).ToString();
                            arr[3] = listItem.GetNodeCount(includeSubTrees: true).ToString();
                            arr[4] = listItem.Path;
                            arr[5] = ""; // listItem.GUID.ToString();
                            arr[6] = ""; // nodeChild.SceneID.ToString();

                            ListViewItem liNewItem = new ListViewItem(arr)
                            {
                                Name = listItem.Name,
                                Text = listItem.Text,
                                Tag = listItem
                            };

                            liNewItem.ImageIndex = HEImageList.GetIconImageIndexByNodeType(listItem.NodeType);

                            // Add the item
                            MainForm.listView1.Items.Add(liNewItem);
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
                        arr[2] = nodeChild.GetNodeCount(includeSubTrees: false).ToString();
                        arr[3] = nodeChild.GetNodeCount(includeSubTrees: true).ToString();
                        arr[4] = nodeChild.Path;
                        arr[5] = ""; // nodeChild.GUID.ToString();
                        arr[6] = ""; // nodeChild.SceneID.ToString();

                        ListViewItem liNewItem = new ListViewItem(arr)
                        {
                            Name = nodeChild.Text,
                            Text = nodeChild.Text,
                            Tag = nodeChild
                        };

                        liNewItem.ImageIndex = HEImageList.GetIconImageIndexByNodeType(nodeChild.NodeType);

                        // Add the item
                        MainForm.listView1.Items.Add(liNewItem);
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

            //if (nSelectedNode != null) //  && docCurrent != null && docCurrent.IsFileReady) // temp change to allow for tree use without a doc loaded
            if (false)
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
                    decimal iTotalNodeCount = docCurrent.SolarSystem.RootNode.GetNodeCount(includeSubTrees: true);
                    int iThisNodeCount = nSelectedHETNNode.GetNodeCount(includeSubTrees: false);
                    int iThisNodeAndSubsCount = nSelectedHETNNode.GetNodeCount(includeSubTrees: true);

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
            MainForm.textBox1.Text = sb1.ToString();
            MainForm.textBox2.Text = sb2.ToString();

        }

        #endregion


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
            MainForm = new MainForm();

            // Initialise the find form.
            FindForm = new FindForm(MainForm);
            FindForm.Hide();


            // Set the form's icon
            var exe = System.Reflection.Assembly.GetExecutingAssembly();
            var iconStream = exe.GetManifestResourceStream("HELLION.Explorer.HELLION.Explorer.ico");
            if (iconStream != null) MainForm.Icon = new Icon(iconStream);

            // Update the main form's title text - this adds the application name
            RefreshMainFormTitleText();

            // Disable the File/Close menu item - this is re-enabled when a file is loaded
            MainForm.closeToolStripMenuItem.Enabled = false;
            MainForm.revertToolStripMenuItem.Enabled = false;

            // Disable both the Find and FindNext menu items.
            MainForm.findToolStripMenuItem.Enabled = false;
            MainForm.findNextToolStripMenuItem.Enabled = false;

            // Disable the Save and Save As menu items.
            MainForm.saveToolStripMenuItem.Enabled = false;
            MainForm.saveAsToolStripMenuItem.Enabled = false;

            // Disable the Observed GUIDs menu item.
            MainForm.observedGUIDsToolStripMenuItem.Enabled = false;

            // Show the main form
            MainForm.Show();

            ProcessCommandLineArguments(args);

            // Start the Windows Forms message loop
            Application.Run(); // Application.Run(new MainForm());

        }

    }
}


