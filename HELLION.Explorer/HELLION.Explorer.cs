using System;
using System.Windows.Forms;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HELLION.DataStructures;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;

//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

namespace HELLION.Explorer
{
    static class Program
    {
        // This is the main class that implements the HELLION Explorer program.
        // Most of the work is done by the HEDocumentWorkspace object however this
        // assembly is responsible for building the output for the ListView control

        // Define the main form object
        internal static MainForm frmMainForm { get; private set; }
        // Define an object to hold the current open document
        internal static HEDocumentWorkspace docCurrent = null;
        // Define an ImageList and fill it
        internal static ImageList ilObjectTypesImageList = HEUtilities.BuildObjectTypesImageList();

        internal static bool bLogToDebug = false;
        internal static bool bViewShowNavigationPane = true;
        internal static bool bViewShowDynamicList = true;
        internal static bool bViewShowInfoPane = true;

        internal static void CheckForUpdates()
        {
            // Checks current build number against the latest release on GitHub repo
            StringBuilder sb = new StringBuilder();

            sb.Append(Environment.NewLine);

            sb.Append("You're currently running version:");
            sb.Append(Environment.NewLine);
            sb.Append("v" + Application.ProductVersion);
            sb.Append(Environment.NewLine);

            sb.Append(Environment.NewLine);
            sb.Append("Latest GitHub release version:");
            sb.Append(Environment.NewLine);
            sb.Append(HEUtilities.FindLatestRelease("CheeseJedi", "HELLION.Explorer"));

            MessageBox.Show(sb.ToString(), "Version update check", MessageBoxButtons.OK, MessageBoxIcon.Information);

        } // End of CheckForUpdates()

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
                // WinForms app
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                // Console app
                System.Environment.Exit(1);
            }
            */
        } // End of ControlledExit()
        /*
        internal static ImageList BuildObjectTypesImageList()
        {
            // Create a new ImageList to hold images used as icons in the tree and list views
            ImageList ilObjectTypesImageList = new ImageList();

            // there must be a better way of doing this, perhaps read from JSON file ;) 
            string[] sListOfImages1 = new string[] {
                "3DCameraOrbit_16x.png",
                "3DExtrude_16x.png",
                "3DScene_16x.png",
                "Actor_16x.png",
                "Add_grey_16x.png",
                "Alert_16x.png",
                "Aserif_16x.png",
                "Assembly_16x.png",
                "Attribute_16x.png",
                "AzureDefaultResource_16x.png",
                "AzureLogicApp_16x.png",
                "AzureLogicApp_color_16x.png",
                "AzureResourceGroup_16x.png",
                "AzureResourceTypeView_16x.png",
                "AzureVirtualMachineExtension_16x.png",
                "BalanceBrace_16x.png",
                "BatchFile_16x.png",
                "BehaviorAction_16x.png",
                "Binary_16x.png",
                "Bios_16x.png",
                "BlankFile_16x.png",
                "Bolt_16x.png",
                "BranchRelationshipChild_16x.png",
                "BranchRelationshipCousin_16x.png",
                "BranchRelationshipGroup_16x.png",
                "BranchRelationshipParent_16x.png",
                "BranchRelationshipSibling_16x.png",
                "BranchRelationship_16x.png",
                "Branch_16x.png",
                "Brightness_16x.png",
                "BubbleChart_16x.png",
                "Bug_16x.png",
                "Builder_16x.png",
                "BulletList_16x.png",
                "ButtonIcon_16x.png",
                "Callout_16x.png",
                "CheckDot_16x.png",
                "Checkerboard_16x.png",
                "Collection_16x.png",
                "ComponentDiagram_16x.png",
                "Component_16x.png",
                "Contrast_16x.png",
                "CordovaMultidevice_16x.png",
                "CSWorkflowDiagram_16x.png",
                "DarkTheme_16x.png",
                "DateTimeAxis_16x.png",
                "Diagnose_16x.png",
                "Dictionary_16x.png",
                "Document_16x.png",
                "DomainType_16x.png",
                "Driver_16x.png",
                "Ellipsis_16x.png",
                "EndpointComponent_16x.png",
                "Event_16x.png",
                "Expander_16x.png",
                "ExplodedPieChart_16x.png",
                "FeedbackBubble_16x.png",
                "FeedbackSad_16x.png",
                "FeedbackSmile_16x.png",
                "FileCollection_16x.png",
                "FileError_16x.png",
                "FileGroupError_16x.png",
                "FileGroupWarning_16x.png",
                "FileGroup_16x.png",
                "FileOK_16x.png",
                "FileWarning_16x.png",
                "Filter_16x.png",
                "FindResults_16x.png",
                "Flag_16x.png",
                "FolderError_16x.png",
                "older_16x.png",
                "Gauge_16x.png",
                "HotSpot_16x.png",
                "Hub_16x.png",
                "JS_16x.png",
                "Label_16x.png",
                "ListFolder_16x.png",
                "Marquee_16x.png",
                "Numeric_16x.png",
                "PermissionFile_16x.png",
                "PieChart_16x.png",
                "Property_16x.png",
                "Rename_16x.png",
                "SemanticZoom_16x.png",
                "Settings_16x.png",
                "Shader_16x.png",
                "Share_16x.png",
                "String_16x.png",
                "Toolbox_16x.png",
                "TreeView_16x.png",
            };

            /* old list
                "3DCameraOrbit_16x.png",
                "3DExtrude_16x.png",
                "3DScene_16x.png",
                "Actor_16x.png",
                "Assembly_16x.png",
                "AzureLogicApp_16x.png",
                "AzureLogicApp_color_16x.png",
                "ButtonIcon_16x.png",
                "CheckDot_16x.png",
                "Checkerboard_16x.png",
                "Component_16x.png",
                "Contrast_16x.png",
                "CSWorkflowDiagram_16x.png",
                "DarkTheme_16x.png",
                "Diagnose_16x.png",
                "Document_16x.png",
                "DomainType_16x.png",
                "Driver_16x.png",
                "Ellipsis_16x.png",
                "Event_16x.png",
                "ExplodedPieChart_16x.png",
                "Filter_16x.png",
                "FindResults_16x.png",
                "Flag_16x.png",
                "Gauge_16x.png",
                "HotSpot_16x.png",
                "Hub_16x.png",
                "PieChart_16x.png",
                "Property_16x.png",
                "Settings_16x.png",
                "Shader_16x.png",
                "Share_16x.png",
                "TreeView_16x.png"
            */
            /*
            // Use System.Reflection to get a list of all resource names
            string[] sListOfImages = Assembly.GetEntryAssembly().GetManifestResourceNames();

            // Get the currently executing assembly name
            string sEntryAssemblyName = Assembly.GetEntryAssembly().GetName().Name;
            //MessageBox.Show(sEntryAssemblyName);

            // Process string array of resource names (this includes the namespace name)
            foreach (string sImageName in sListOfImages)
            {
                if (sImageName.Contains(sEntryAssemblyName + ".Images."))
                {
                    ilObjectTypesImageList.Images.Add(Image.FromStream(Assembly.GetEntryAssembly().GetManifestResourceStream(sImageName)));   
                   }
            }

            // Return the image list
            return ilObjectTypesImageList;
        } // End of BuildObjectTypesImageList()
        */
        internal static void FileOpen(string sFileName = "")
        {
            // Loads a save file in to memory and processes it

            // If the sFileName is set, check the file exists otherwise prompt the user to select a file
            if (sFileName == "")
            {
                // Create a new OpenFileDialog box and set some parameters
                var openFileDialog1 = new OpenFileDialog()
                {
                    Filter = "HELLION DS Save Files|*.save|JSON Files|*.json|All files|*.*",
                    Title = "Open file",
                    CheckFileExists = true
                };

                // Check that the file exists when the user clicked ok
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    sFileName = openFileDialog1.FileName;
                }
            }
            else
            {
                // We were passed a file name, check to see if it's actually there
                if (!System.IO.File.Exists(sFileName))
                {
                    // The file name passed doesn't exist
                    MessageBox.Show(String.Format("Error opening file:{0}{1}{0} from command line - file doesn't exist.", Environment.NewLine, sFileName));
                    
                    return;
                }
            }

            { 
                // Update the main window's title text to reflect the filename selected
                RefreshMainFormTitleText();

                //Application.UseWaitCursor = true;
                frmMainForm.Cursor = Cursors.WaitCursor;

                // Suppress repainting the TreeView until all the objects have been created.
                frmMainForm.treeView1.BeginUpdate();

                // Clear any existing nodes
                //frmMainForm.treeView1.Nodes.Clear();

                // Check for an existing document and close it if necesary
                if (docCurrent != null)
                {
                    // Clear the existing document
                    FileClose();
                }

                // Create a new DocumentWorkspace
                docCurrent = new HEDocumentWorkspace()
                {
                    // Activates LogToDebug for docCurrent
                    LogToDebug = bLogToDebug
                };



                // Check that the Data folder path has been defined and the expected files are there
                if (!IsGameDataFolderValid())
                {
                    // The checks failed, throw up an error message and cancel the load
                    MessageBox.Show("There was a problem with the Data Folder - use Set Data Folder option in Tools menu :)"); // this needs to be massively improved!
                                                                                                                               
                    // Restore mouse cursor and return
                    frmMainForm.Cursor = Cursors.Default;
                    return;
                }


                // Grab the Game Data Folder from Properties
                string sGameDataFolder = Properties.HELLIONExplorer.Default.sGameDataFolder + "\\";

                docCurrent.MainFile.FileName = sFileName;
                frmMainForm.toolStripStatusLabel1.Text = ("Loading file: " + docCurrent.MainFile.FileName);

                if (Properties.HELLIONExplorer.Default.bLoadCelestialBodiesFile)
                {
                    docCurrent.DataFileCelestialBodies.FileName = (sGameDataFolder + Properties.HELLIONExplorer.Default.sCelestialBodiesFileName);
                }
                else
                {
                    docCurrent.DataFileCelestialBodies.SkipLoading = true;

                }

                if (Properties.HELLIONExplorer.Default.bLoadAsteroidsFile)
                {
                    docCurrent.DataFileAsteroids.FileName = (sGameDataFolder + Properties.HELLIONExplorer.Default.sAsteroidsFileName);
                }
                else
                {
                    docCurrent.DataFileAsteroids.SkipLoading = true;
                }

                if (Properties.HELLIONExplorer.Default.bLoadStructuresFile)
                {
                    docCurrent.DataFileStructures.FileName = (sGameDataFolder + Properties.HELLIONExplorer.Default.sStructuresFileName);
                }
                else
                {
                    docCurrent.DataFileStructures.SkipLoading = true;
                }

                if (Properties.HELLIONExplorer.Default.bLoadDynamicObjectsFile)
                {
                    docCurrent.DataFileDynamicObjects.FileName = (sGameDataFolder + Properties.HELLIONExplorer.Default.sDynamicObjectsFile);
                }
                else
                {
                    docCurrent.DataFileDynamicObjects.SkipLoading = true;
                }

                if (Properties.HELLIONExplorer.Default.bLoadModulesFile)
                {
                    docCurrent.DataFileModules.FileName = (sGameDataFolder + Properties.HELLIONExplorer.Default.sModulesFileName);
                }
                else
                {
                    docCurrent.DataFileModules.SkipLoading = true;
                }

                if (Properties.HELLIONExplorer.Default.bLoadStationsFile)
                { 
                    docCurrent.DataFileStations.FileName = (sGameDataFolder + Properties.HELLIONExplorer.Default.sStationsFileName);
                }
                else
                {
                    docCurrent.DataFileStations.SkipLoading = true;
                }

                docCurrent.LoadFile();

                // Attach the document's root node to the node tree
                if (docCurrent.SolarSystemRootNode != null)
                {
                    // Add the SolarSystemRoot and SearchResultsRoot nodes to the treeview 
                    frmMainForm.treeView1.Nodes.Add(docCurrent.SolarSystemRootNode);
                    frmMainForm.treeView1.Nodes.Add(docCurrent.GameDataRootNode);
                    
                    // Search results node - currently not enabled
                    //frmMainForm.treeView1.Nodes.Add(docCurrent.SearchResultsRootNode);

                    frmMainForm.treeView1.SelectedNode = docCurrent.SolarSystemRootNode;
                    frmMainForm.treeView1.SelectedNode.Expand();
                    frmMainForm.toolStripStatusLabel1.Text = ("Ready");
                }
                else
                {
                    MessageBox.Show("Error: SolarSystemRootNode was null");
                }
                    
                    
                // Begin repainting the TreeView.
                frmMainForm.treeView1.EndUpdate();

                //Application.UseWaitCursor = false;
                frmMainForm.Cursor = Cursors.Default;

                RefreshMainFormTitleText();
                RefreshSelectedObjectSummaryText(docCurrent.SolarSystemRootNode);

                frmMainForm.closeToolStripMenuItem.Enabled = true;

            }
        } // End of FileOpen()

        internal static void FileClose()
        {
            // Handles closing of files

            // isFileDirty check before exiting
            if (docCurrent.IsFileDirty)
            {
                // Unsaved changes, prompt user to save
                string sMessage = docCurrent.MainFile.FileName + Environment.NewLine + "This file has been modified. Do you want to save changes before exiting?";
                const string sCaption = "Unsaved Changes";
                var result = MessageBox.Show(sMessage, sCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // If the yes button was pressed ...
                if (result == DialogResult.Yes)
                {
                    // User selected Yes, call save routine
                    MessageBox.Show("User selected Yes to save changes", "Unsaved Changes Dialog", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Saving is not yet implemented
                }
            }




            // Clear any existing nodes from the tree view
            frmMainForm.treeView1.Nodes.Clear();

            // Clear any items from the list view
            frmMainForm.listView1.Items.Clear();

            // Check for an existing document and close it if necesary
            if (docCurrent != null)
            {
                // Clear the existing document
                docCurrent.CloseFile();
                docCurrent = null;
            }

            frmMainForm.closeToolStripMenuItem.Enabled = false;

            // Trigger refresh of UI elements
            RefreshMainFormTitleText();

            RefreshSelectedOjectPathBarText(null);
            RefreshListView(null);
            RefreshSelectedObjectSummaryText(null);

            // Initiate Grabage Collection
            GC.Collect();
        } // End of FileClose()

        internal static string GenerateAboutBoxText()
        {
            // Define a StringBuilder to hold the string to be sent to the dalog box
            StringBuilder sb = new StringBuilder();

            // Create a 'shorthand' for the new line character appropriate for this environment
            string sNL = Environment.NewLine;

            // Assemble the About dialog text
            sb.Append(sNL);

            // Add the product name and version
            sb.Append(Application.ProductName);
            sb.Append("   Version ");
            sb.Append(Application.ProductVersion);
            sb.Append(sNL);
            sb.Append(sNL);

            // Add version information for HELLION.DataStructures.dll
            var anHELLIONDataStructures = System.Reflection.Assembly.GetAssembly(typeof(HEDocumentWorkspace)).GetName();
            sb.Append(anHELLIONDataStructures.Name);
            sb.Append("   Version ");
            sb.Append(anHELLIONDataStructures.Version);
            sb.Append(sNL);

            // Add verison information for NewtonsoftJson.dll
            var anNewtonsoftJson = System.Reflection.Assembly.GetAssembly(typeof(JObject)).GetName();
            sb.Append(anNewtonsoftJson.Name);
            sb.Append("   Version ");
            sb.Append(anNewtonsoftJson.Version);
            sb.Append(sNL);
            sb.Append(sNL);

            // Add an estimate of current memory usage from the garbage collector
            sb.Append(String.Format("Memory usage (bytes): {0:N0}", GC.GetTotalMemory(false)));
            sb.Append(sNL);
            sb.Append(sNL);
            sb.Append(sNL);

            // Credit
            sb.Append("Uses the Newtonsoft JSON library. http://www.newtonsoft.com/json");
            sb.Append(sNL);
            sb.Append(sNL);

            // Credit
            sb.Append("HELLION trademarks, content and materials are property of Zero Gravity Games or it's licensors. http://www.zerogravitygames.com");
            sb.Append(sNL);
            sb.Append(sNL);
            sb.Append(sNL);

            // Cheeseware statement
            sb.Append("This product is 100% certified Cheeseware* and is not dishwasher safe.");
            sb.Append(sNL);
            sb.Append(sNL);

            // Cheeseware definition ;)
            sb.Append("* cheeseware (Noun)");
            sb.Append(sNL);
            sb.Append("  1. (computing, slang, pejorative) Exceptionally low-quality software.");
            sb.Append(sNL);

            return sb.ToString();

        } // End of GenerateAboutBoxText()

        internal static bool IsGameDataFolderValid()
        {
            // Called indirectly by menu option on the main form, and directly when opening a file, or by other means. 
            // Verifies that there's a data folder defined, and that it's got files in it with familiar names, but honours
            // the loading flags in the config file and doesn't check files that are marked as not to be loaded.
            // Does not chech the contents of the files.


            string StoredDataFolderPath = Properties.HELLIONExplorer.Default.sGameDataFolder.Trim();

            // Check GameDataFolder path in settings is not null or empty
            if (StoredDataFolderPath == null || StoredDataFolderPath == "") 
                return false;

            // Check the folder exists
            if (!Directory.Exists(StoredDataFolderPath))
                return false;

            // Check the Celestial Bodies file - this one is particularly critical
            if (Properties.HELLIONExplorer.Default.bLoadCelestialBodiesFile)
            {
                if (!File.Exists(StoredDataFolderPath + "\\" + Properties.HELLIONExplorer.Default.sCelestialBodiesFileName.Trim()))
                    return false;
            }

            // Check the Asteroids file
            if (Properties.HELLIONExplorer.Default.bLoadAsteroidsFile)
            {
                if (!File.Exists(StoredDataFolderPath + "\\" + Properties.HELLIONExplorer.Default.sAsteroidsFileName.Trim()))
                    return false;
            }

            // Check the Structures file
            if (Properties.HELLIONExplorer.Default.bLoadStructuresFile)
            {
                if (!File.Exists(StoredDataFolderPath + "\\" + Properties.HELLIONExplorer.Default.sStructuresFileName.Trim()))
                    return false;
            }

            // Check the Dynamic Objects file
            if (Properties.HELLIONExplorer.Default.bLoadStructuresFile)
            {
                if (!File.Exists(StoredDataFolderPath + "\\" + Properties.HELLIONExplorer.Default.sStructuresFileName.Trim()))
                    return false;
            }

            // Check the Modules file
            if (Properties.HELLIONExplorer.Default.bLoadModulesFile)
            {
                if (!File.Exists(StoredDataFolderPath + "\\" + Properties.HELLIONExplorer.Default.sModulesFileName.Trim()))
                    return false;
            }

            // Check the Stations file
            if (Properties.HELLIONExplorer.Default.bLoadStationsFile)
            {
                if (!File.Exists(StoredDataFolderPath + "\\" + Properties.HELLIONExplorer.Default.sStationsFileName.Trim()))
                    return false;
            }

            // Check the Loot Categories file
            if (Properties.HELLIONExplorer.Default.bLoadLootCategoriesFile)
            {
                if (!File.Exists(StoredDataFolderPath + "\\" + Properties.HELLIONExplorer.Default.sLootCategoriesFileName.Trim()))
                    return false;
            }

            // Check the Spawn Rules file
            if (Properties.HELLIONExplorer.Default.bLoadSpawnRulesFile)
            {
                if (!File.Exists(StoredDataFolderPath + "\\" + Properties.HELLIONExplorer.Default.sSpawnRulesFileName.Trim()))
                    return false;
            }

            // Check the Item Recipies file
            if (Properties.HELLIONExplorer.Default.bLoadItemRecipiesFile)
            {
                if (!File.Exists(StoredDataFolderPath + "\\" + Properties.HELLIONExplorer.Default.sItemRecipiesFileName.Trim()))
                    return false;
            }

            // Check the Glossary file
            if (Properties.HELLIONExplorer.Default.bLoadGlossaryFile)
            {
                if (!File.Exists(StoredDataFolderPath + "\\" + Properties.HELLIONExplorer.Default.sGlossaryFileName.Trim()))
                    return false;
            }

            /*
            // Some test code for enumerating the settings - potentially less code changes needed in future if this could be dynamic
            string result = "Settings ";
            foreach (SettingsProperty currentProperty in Properties.HELLIONExplorer.Default.Properties)
            {
                result += currentProperty.Name.ToString() + ": " + Properties.HELLIONExplorer.Default[currentProperty.Name].ToString() + Environment.NewLine;
            }
            MessageBox.Show(result);
            */
            

            // No checks failed, assume folder is ok
            return true;

        } // End of IsGameDataFolderValid()

        internal static void VerifyGameDataFolder()
        {

            // Called by menu option on the main form
            // Is interactive and will prompt the user to set a valid folder


            // Check that the Data folder path has been defined and there's stuff there
            if (!IsGameDataFolderValid())
            {
                // The checks failed, throw up an error message and cancel the load
                MessageBox.Show("There was a problem with the Data Folder - use Set Data Folder option in Tools menu :)"); // this needs to be massively improved!
                return;
            }
            else
            {
                MessageBox.Show("Game Data folder seems valid.");
            }


            /*  OLD CODE!
            if (false)
            {
                if (false)
                    MessageBox.Show("Game Data folder seems valid.");
            }
            else
            {
                MessageBox.Show("Game Data folder: " + Environment.NewLine + StoredDataFolderPath + Environment.NewLine + "is INVALID! Please set this in the following dialog.");
                SetGameDataFolder();
            }
            */
        }

        internal static void SetGameDataFolder()
        {
            MessageBox.Show("Please use the following folder browser window to select the location of the game Data folder." + Environment.NewLine + Environment.NewLine +
                "The Data folder and the files within can be obtained as part of the HELLION Dedicated Server installation and are required to load a .save file."
                , "Please set the Data folder location", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Create a new OpenFileDialog box and set some parameters
            var folderBrowserDialog1 = new FolderBrowserDialog()
            {
                Description = "Select location of Data folder",
                RootFolder = Environment.SpecialFolder.Desktop,
                // Pre-populate the path with whatever's stored in the Properties 
                SelectedPath = Properties.HELLIONExplorer.Default.sGameDataFolder,
            };

            // Hacky workaround for the Folder Browser Dialog not scrolling to folder passed to it :(
            //SendKeys.Send("{TAB}{TAB}{RIGHT}");

            // If the user clicked ok then set the game data path on the settings
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) // && folderBrowserDialog1. CheckFolderExists)
            {
                //MessageBox.Show("You selected: " + folderBrowserDialog1.SelectedPath, "Folder selection cofirmed");
                Properties.HELLIONExplorer.Default.sGameDataFolder = folderBrowserDialog1.SelectedPath;
                Properties.HELLIONExplorer.Default.Save();
            }
        } // End of DefineGameFilder()

        internal static void RefreshMainFormTitleText()
        {
            // Regenerates and sets the application's main window title text
            StringBuilder sb = new StringBuilder();

            // Add the product name
            sb.Append(Application.ProductName);

            if (docCurrent != null && docCurrent.IsFileReady)
            {
                sb.Append(" [");
                sb.Append(docCurrent.MainFile.FileName);
                sb.Append("] ");

                if (docCurrent.IsFileDirty) sb.Append("*");
            }

            frmMainForm.Text = sb.ToString();
                

        } // End of RefreshMainFormTitleText

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
        } // End of RefreshSelectedOjectPathBarText()

        internal static void RefreshListView(TreeNode nSelectedNode)
        {
            //throw new NotImplementedException();

            if (nSelectedNode != null) // && docCurrent != null && docCurrent.IsFileReady) // temp change to allow unloaded document tree display
            {


                HETreeNode nSelectedHETNNode = (HETreeNode)nSelectedNode;


                // Clear the list view's items
                frmMainForm.listView1.Items.Clear();


                // Test to see if we're drawing a <PARENT> and <THIS> item in the list view (option not yet implemented, on by defualt)
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
                            ImageIndex = HEUtilities.GetImageIndexByNodeType(nodeParent.NodeType)
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
                            ImageIndex = HEUtilities.GetImageIndexByNodeType(nSelectedHETNNode.NodeType)
                        };
                        // Add the item
                        frmMainForm.listView1.Items.Add(liCurrentItem);
                    }
                }

                foreach (HETreeNode nodeChild in nSelectedNode.Nodes)
                {
                    string[] arr = new string[7];
                    arr[0] = nodeChild.Text;
                    arr[1] = nodeChild.NodeType.ToString();
                    arr[2] = nodeChild.CountOfChildNodes.ToString();
                    arr[3] = nodeChild.CountOfAllChildNodes.ToString();
                    arr[4] = ""; // nodeChild.OrbitData.SemiMajorAxis.ToString();
                    arr[5] = ""; // nodeChild.GUID.ToString();
                    arr[6] = ""; // nodeChild.SceneID.ToString();

                    ListViewItem liNewItem = new ListViewItem(arr)
                    {
                        Name = nodeChild.Text,
                        Text = nodeChild.Text,
                        Tag = nodeChild
                    };
                    /*
                    if ((nodeChild.OrbitData.ParentGUID == -1) && (nodeChild.NodeType == HETreeNodeType.CelestialBody))
                    {
                        // It's the star, a special case
                        liNewItem.ImageIndex = (int)HEObjectTypesImageList.ButtonIcon_16x;
                    }
                    else */
                    {
                        liNewItem.ImageIndex = HEUtilities.GetImageIndexByNodeType(nodeChild.NodeType);
                    }
                    

                    // Add the item
                    frmMainForm.listView1.Items.Add(liNewItem);
                }
            }
            else if (nSelectedNode == null)
            {
                //MessageBox.Show("RefreshListView was passed a null nSelectedNode");
            }
        } // End of RefreshListView

        internal static void RefreshSelectedObjectSummaryText(TreeNode nSelectedNode)
        {
            // Updates the Object Information panel

            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            StringBuilder sb3 = new StringBuilder();
            StringBuilder sb4 = new StringBuilder();


            if (nSelectedNode != null) //  && docCurrent != null && docCurrent.IsFileReady) // temp change to allow for tree use without a doc loaded
            {

                HETreeNode nSelectedHETNNode = (HETreeNode)nSelectedNode;

                sb1.Append("Node Tree Data");
                sb1.Append(Environment.NewLine);
                sb1.Append("Name: " + nSelectedHETNNode.Name);
                sb1.Append(Environment.NewLine);
                sb1.Append("NodeType: " + nSelectedHETNNode.NodeType.ToString());
                sb1.Append(Environment.NewLine);
                //sb1.Append("GUID: " + nSelectedHETNNode.GUID.ToString());
                sb1.Append(Environment.NewLine);
                //sb1.Append("ParentGUID: " + nSelectedHETNNode.ParentGUID.ToString());
                sb1.Append(Environment.NewLine);

                //sb1.Append("SceneID: " + nSelectedNode.SceneID.ToString());
                sb1.Append(Environment.NewLine);
                //sb1.Append("Type: " + nSelectedNode.Type.ToString());
                sb1.Append(Environment.NewLine);
                sb1.Append(Environment.NewLine);

/*
                if (false) //nSelectedHETNNode.NodeType == HETreeNodeType.CelestialBody || nSelectedHETNNode.NodeType == HETreeNodeType.Ship || nSelectedHETNNode.NodeType == HETreeNodeType.Asteroid)
                {

                    HEOrbitalObjTreeNode nSelectedOrbitalObjNode = (HEOrbitalObjTreeNode)nSelectedNode;



                    sb1.Append(Environment.NewLine);
                    sb1.Append("ORBITAL DATA");
                    sb1.Append(Environment.NewLine);

                    sb1.Append("ParentGUID: " + nSelectedOrbitalObjNode.OrbitData.ParentGUID.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("VesselID: " + nSelectedOrbitalObjNode.OrbitData.VesselID.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("VesselType: " + nSelectedOrbitalObjNode.OrbitData.VesselType.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("SemiMajorAxis: " + nSelectedOrbitalObjNode.SemiMajorAxis.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("Inclination: " + nSelectedOrbitalObjNode.Inclination.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("Eccentricity: " + nSelectedOrbitalObjNode.OrbitData.Eccentricity.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("LongitudeOfAscendingNode: " + nSelectedOrbitalObjNode.OrbitData.LongitudeOfAscendingNode.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("ArgumentOfPeriapsis: " + nSelectedOrbitalObjNode.OrbitData.ArgumentOfPeriapsis.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append(Environment.NewLine);

                    sb1.Append("OrbitData.TimeSincePeriapsis: " + nSelectedOrbitalObjNode.OrbitData.TimeSincePeriapsis.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("OrbitData.SolarSystemPeriapsisTime: " + nSelectedOrbitalObjNode.OrbitData.SolarSystemPeriapsisTime.ToString());
                    sb1.Append(Environment.NewLine);
                }
*/

                if (false) //nSelectedNode.NodeType != HETreeNodeType.SystemNAV) // temp addition
                {
                    // Get the count of the child nodes contained in the selected node
                    decimal iTotalNodeCount = docCurrent.SolarSystemRootNode.CountOfAllChildNodes;
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

                sb4.Append("<!DOCTYPE html><html><head><style>"
                    + "body { margin: 0em; padding: 0em; } "
                    + "body div { white-space: pre-wrap; font-family:Courier; font-size: 0.9em } " 
                    + ".container { counter-reset: line; "
                    + "  .ln:before { counter-increment: line; content: counter(line); display: inline - block; border - right: 1px solid #ddd; padding: 0 .5em; margin - right: .5em; color: #888 } "
                    + "} "                    
                    + ".number { color: orange; } "
                    + ".key { color: crimson; } "
                    + ".string { color: darkgreen; } "
                    + ".boolean { color: blue; } "
                    + ".null { color: black; } "
                    + "</style></head><body><div class='container'><div class='ln'>");

                // Create JSON data for selected node
                if (nSelectedNode.Tag != null)
                {
                    sb3.Append((JValue)nSelectedNode.Tag.ToString());

                    sb4.Append(HEUtilities.SyntaxHighlightJson(sb3.ToString().Replace("  ", "&emsp;").Replace("\n", "</div><div class='ln'>")));
                    //MessageBox.Show(sb4.ToString());

                }
                sb4.Append("</div></div></body></html>");

                /*
                if (nSelectedHETNNode.NodeType == HETreeNodeType.CelestialBody) // || nSelectedHETNNode.NodeType == HETreeNodeType.SystemNAV)
                {
                    DataTable myObjectDT = JsonConvert.DeserializeObject<DataTable>("[" + (nSelectedNode.Tag).ToString() + "]"); // (JValue)
                    frmMainForm.dataGridView1.DataSource = myObjectDT;


                    //frmMainForm.dataGridView1.DataBindings.
                }
                else
                {
                    frmMainForm.dataGridView1.DataSource = null;
                }
                string jsonString = sb3.ToString();

                frmMainForm.treeView2.Nodes.Clear();
                if (jsonString != "")
                {

                    string rootName = "root", nodeName = "node";
                    JContainer json;
                    try
                    {
                        if (jsonString.StartsWith("["))
                        {
                            json = JArray.Parse(jsonString);
                            frmMainForm.treeView2.Nodes.Add(HEUtilities.Json2Tree((JArray)json, rootName, nodeName));
                        }
                        else
                        {
                            json = JObject.Parse(jsonString);
                            frmMainForm.treeView2.Nodes.Add(HEUtilities.Json2Tree((JObject)json, "text"));
                        }
                    }
                    catch (JsonReaderException jre)
                    {
                        MessageBox.Show("Invalid Json." + Environment.NewLine + jre.ToString() + Environment.NewLine + jsonString);
                    }
                }
                */
                /*
                switch (nSelectedNode.NodeType)
                {
                    case HETreeNodeType.CelestialBody:
                        // It's a Celestial Body!

                        break;
                    case HETreeNodeType.Asteroid:
                        // It's an Asteroid!

                        break;
                    case HETreeNodeType.Ship:
                        // It's a ship!

                        // Filter down the ships list from openFileData by matching GUID and list them all
                        IOrderedEnumerable<JToken> ioShips = from s in openFileData["Ships"]
                                                                where (long)s["GUID"] == lGUID
                                                                orderby (long)s["GUID"]
                                                                select s;
                        int iIndex = 0;
                        foreach (var jtShip in ioShips)
                        {
                            iIndex++;
                            sb.Append(Environment.NewLine);
                            sb.Append("IndexNo: " + iIndex.ToString());
                            sb.Append(Environment.NewLine);
                            sb.Append("Ship data from file");
                            sb.Append(Environment.NewLine);
                            sb.Append("Name: " + (string)jtShip["Name"]);
                            sb.Append(Environment.NewLine);
                            sb.Append("GUID: " + (string)jtShip["GUID"]);
                            sb.Append(Environment.NewLine);
                            sb.Append("ParentGUID: " + (string)jtShip["OrbitData"]["ParentGUID"]);
                            sb.Append(Environment.NewLine);
                            sb.Append("SemiMajorAxis: " + (string)jtShip["OrbitData"]["SemiMajorAxis"]);
                            sb.Append(Environment.NewLine);
                            sb.Append(Environment.NewLine);
                            sb.Append(Environment.NewLine);
                        }
                        break;
                    default:
                        break;
                }
                */
            }

            // Pass results to various textboxes
            frmMainForm.textBox1.Text = sb1.ToString();
            frmMainForm.textBox2.Text = sb2.ToString();
            frmMainForm.textBox3.Text = sb3.ToString();

            //frmMainForm.webBrowser1.DocumentText = sb4.ToString();
            //frmMainForm.webBrowser1.Refresh();

            frmMainForm.webBrowser1.Navigate("about:blank");
            if (frmMainForm.webBrowser1.Document != null)
            {
                frmMainForm.webBrowser1.Document.Write(string.Empty);
            }
            frmMainForm.webBrowser1.DocumentText = sb4.ToString();




            // 



        } // End of RefreshObjectSummaryText()

        internal static void InitialiseTreeView(TreeView tvCurrent)
        {
            // Set the specified TreeView control's ImageLists to  
            // ilObjectTypesImageList and set the default icons
            tvCurrent.ImageList = ilObjectTypesImageList;
            tvCurrent.ImageIndex = (int)HEObjectTypesImageList.Flag_16x;
            tvCurrent.SelectedImageIndex = (int)HEObjectTypesImageList.Flag_16x;
            tvCurrent.TreeViewNodeSorter = new HETreeNodeSorter();
            tvCurrent.ShowNodeToolTips = true;

        } // End of InitialiseTreeView

        internal static void InitialiseListView(ListView lvCurrent)
        {
            // Set the specified ListView control's ImageLists to  
            // ilObjectTypesImageList and set the default icons

            frmMainForm.listView1.SmallImageList = ilObjectTypesImageList;
            // Add some colums appropriate to the data we intend to add
            frmMainForm.listView1.Columns.Add("Name", 180, HorizontalAlignment.Left);
            frmMainForm.listView1.Columns.Add("Type", 120, HorizontalAlignment.Left);
            frmMainForm.listView1.Columns.Add("Count", 50, HorizontalAlignment.Left);
            frmMainForm.listView1.Columns.Add("TotalCount", 60, HorizontalAlignment.Left);
            frmMainForm.listView1.Columns.Add("SemiMajorAxis", 80, HorizontalAlignment.Left);
            frmMainForm.listView1.Columns.Add("GUID", 50, HorizontalAlignment.Right);
            frmMainForm.listView1.Columns.Add("SceneID", 30, HorizontalAlignment.Right);

        } // End of InitialiseListView

        internal static void TestOption1()
        {
            // Scratchpad area for testing new stuff out - has corresponding menu item
            // Make a note of the starting time
            DateTime StartingTime = DateTime.Now;
            HETreeNode tempLoadingIndicatorNode = new HETreeNode("Loading...", HETreeNodeType.ExpansionAvailable);
            HEViewGameData GameDataView = null;

            frmMainForm.treeView1.Nodes.Add(tempLoadingIndicatorNode);

            FileInfo fileInfo = new FileInfo(@"C:\Users\James\Downloads\MegaBaseSave\ServerSave_2017-11-23-22-21-56.save");

            DirectoryInfo directoryInfo = new DirectoryInfo(Properties.HELLIONExplorer.Default.sGameDataFolder);

            if (fileInfo != null && fileInfo.Exists && directoryInfo != null && directoryInfo.Exists)
            {
                GameDataView = new HEViewGameData(fileInfo, directoryInfo);
            }

            if (GameDataView != null)
            {
                frmMainForm.treeView1.Nodes.Add(GameDataView.RootNode);
            }
            else
            {
                MessageBox.Show("Loading error");
            }





            // Grab a data file path Properties

            //HEJsonBaseFile testDataFile = new HEJsonBaseFile(@"C:\Users\James\Downloads\MegaBaseSave\ServerSave_2017-11-23-22-21-56.save");
            //HEJsonGameFile testDataFile = new HEJsonGameFile(@"C:\Users\James\Downloads\ServerSave_2017-11-15-18-00-57\ServerSave_2017-11-15-18-00-57.save");
            //HEJsonBaseFile testDataFile = new HEJsonBaseFile(@"C:\Users\James\Desktop\Data\CelestialBodies.json");
            //HEJsonBaseFile testDataFile = new HEJsonBaseFile(Properties.HELLIONExplorer.Default.sGameDataFolder + "\\" + Properties.HELLIONExplorer.Default.sCelestialBodiesFileName);
            //HEJsonBaseFile testDataFile = new HEJsonBaseFile(@"C:\Users\James\Downloads\ServerSave_2017-11-15-18-00-57\ServerSave_2017-11-15-18-00-57.save");
            //HEJsonBaseFile testDataFile = new HEJsonBaseFile(@"C:\Users\James\Desktop\Data\New folder\arraywith2objects.json");

            //testDataFile.LogToDebug = false;
            //testDataFile.LoadFile();

            //int numRuns = 0;

            // Some async test stuff

            //HETreeNode nodeSaveFile = new HETreeNode("SAVEFILE", HETreeNodeType.SaveFile, nodeText: testDataFile.File.Name, nodeToolTipText: testDataFile.File.FullName);



            // Task to run asynchronously
            //List<Task> tasks = new List<Task>();
            //Task t1 = Task.Run(() => 

            //testDataFile.BuildBasicNodeTreeFromJson(testDataFile.JData, nodeSaveFile, maxDepth: 2, logToDebug: true);

            //tempParent.Nodes.Add(testDataFile.BuildHETreeNodeTreeFromJson(json: testDataFile.JData, maxDepth: 6) ?? new HETreeNode("LOADING ERROR!",HETreeNodeType.DataFileError));


            //tasks.Add(t1);

            //testDataFile.BuildNodeTreesFromJson(testDataFile.JData, tempParent, numRuns);

            //HEStaticDataFileCollection testDataFileCollection = null;

            //if (Properties.HELLIONExplorer.Default.sGameDataFolder != "")
            {

                //Task t2 = Task.Run(() => 
                //testDataFileCollection = new HEStaticDataFileCollection(Properties.HELLIONExplorer.Default.sGameDataFolder);
                //tasks.Add(t2);

            }


            // Wait for tasks to complete
            //Task.WaitAll(tasks.ToArray());

            //tempParent.Nodes.Add(testDataFileCollection.RootNode ?? new HETreeNode("DATAFOLDER", HETreeNodeType.DataFolderError, "Data Folder - ERROR"));

            //tempParent.Nodes.Add(nodeSaveFile);

            frmMainForm.treeView1.Nodes.Remove(tempLoadingIndicatorNode);

            //foreach (Task t in tasks)
                //Debug.Print("Task {0} Status: {1}", t.Id, t.Status);
            Debug.Print("Process completed in {0}", DateTime.Now - StartingTime);

            
            //tempParent.UpdateCounts();
            GC.Collect();
    }


    /// <summary>
    /// The main entry point for the application.
    /// </summary>

    [STAThread]
        static void Main(string[] args)
        {
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

            // Prepare the TreeView control
            InitialiseTreeView(frmMainForm.treeView1);

            // Prepare the ListView control
            InitialiseListView(frmMainForm.listView1);

            // Disable the File/Close menu item - this is renabled when a file is loaded
            frmMainForm.closeToolStripMenuItem.Enabled = false;

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

            if (args.Length > 0)
            {
                FileOpen(args[0]);
            }

            // Start the Windows Forms message loop
            Application.Run(); // Application.Run(new MainForm());

        } // End of Main()
    } // End of class Program
} // End of namepsace HELLION.Explorer


