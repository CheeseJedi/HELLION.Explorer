using System;
using System.Windows.Forms;
using System.Text;
using Newtonsoft.Json.Linq;
using HELLION.DataStructures;
using System.Drawing;
using System.Reflection;
//using System.IO;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Diagnostics;
//using Newtonsoft.Json;

namespace HELLION.Explorer
{
    static class Program
    {
        // This is the main class that implements the HELLION Explorer program.
            
        // main object definitions
            
        // Define the main form object
        public static MainForm frmMainForm { get; private set; }

        // Define an object to hold the current open document
        public static HEDocumentWorkspace docCurrent = null;

        // Define an ImageList and fill it
        public static ImageList ilObjectTypesImageList = BuildObjectTypesImageList();

        private static bool bLogToDebug = true;
        public static bool bViewShowNavigationPane = true;
        public static bool bViewShowDynamicList = true;
        public static bool bViewShowInfoPane = true;

        public static void ControlledExit()
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
        }

        public static ImageList BuildObjectTypesImageList()
        {
            // Create a new ImageList to hold images used as icons in the tree and list views
            ImageList ilObjectTypesImageList = new ImageList();

            // there must be a better way of doing this, perhaps read from JSON file ;) 
            string[] sListOfImages1 = new string[] {
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
            };

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
                    //MessageBox.Show(sImageName);

                    ilObjectTypesImageList.Images.Add(Image.FromStream(Assembly.GetEntryAssembly().GetManifestResourceStream(sImageName)));
    
                   }
            }

            // Return the image list
            return ilObjectTypesImageList;
        } // End of BuildObjectTypesImageList()

        public static void FileOpen(string sFileName = "")
        {
            // Loads a save file in to memory and processes it

            // Check that the Data folder path has been defined, if not trigger the DefineGameFolder then return
            if (!IsGameDataFolderDefined())
            {
                DefineGameFolder();
                return;
            }

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
                    MessageBox.Show(String.Format("Error opening file:{0}{1}{0] from command line - file doesn't exist.", Environment.NewLine, sFileName));
                    return;
                }


            }


            { 

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
                    //docCurrent.CloseFile();
                    //docCurrent = null;

                    FileClose();

                }




                // Create a new DocumentWorkspace
                docCurrent = new HEDocumentWorkspace()
                {
                    // Activates LogToDebug for docCurrent
                    LogToDebug = bLogToDebug
                };

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
                    frmMainForm.treeView1.Nodes.Add(docCurrent.SearchResultsRootNode);

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

        public static void FileClose()
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
        }

        public static string GenerateAboutBoxText()
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
            sb.Append("HELLION trademarks, content and materials are property of Zero Gravity games or it's licensors. http://http://www.zerogravitygames.com");
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

        }

        public static bool IsGameDataFolderDefined()
        {
            // Checks that the GameDataFolder in settings is not empty, if it is empty offer the folder browser and store the location

            // Properties.Program.Default.sGameDataFolder

            if (Properties.HELLIONExplorer.Default.sGameDataFolder.Trim() == "")  // Other conditions to test should be added here
            {
                return false;
            }
            else
            {
                return true;
            }
        } // End of IsGameDataFolderDefined()

        public static void DefineGameFolder()
        {
            MessageBox.Show("Please use the following folder browser window to select the location of the game Data folder." + Environment.NewLine + Environment.NewLine +
                "The Data folder and the files within can be obtained as part of the HELLION Dedicated Server installation and are required to load a .save file."
                , "Please set the Data folder location", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Create a new OpenFileDialog box and set some parameters
            var folderBrowserDialog1 = new FolderBrowserDialog()
            {
                Description = "Select location of Data folder",
                RootFolder = Environment.SpecialFolder.Desktop,
                SelectedPath = Properties.HELLIONExplorer.Default.sGameDataFolder,
            };

            // Hacky workaround for the Folder Browser Dialog not scrolling to folder passed to it :(
            SendKeys.Send("{TAB}{TAB}{RIGHT}");

            // If the user clicked ok then set the game data path on the settings
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) // && folderBrowserDialog1. CheckFolderExists)
            {
                //MessageBox.Show("You selected: " + folderBrowserDialog1.SelectedPath, "Folder selection cofirmed");
                Properties.HELLIONExplorer.Default.sGameDataFolder = folderBrowserDialog1.SelectedPath;
                Properties.HELLIONExplorer.Default.Save();
            }
        } // End of DefineGameFilder()

        public static void RefreshMainFormTitleText()
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

        public static void RefreshSelectedOjectPathBarText(HEOrbitalObjTreeNode nSelectedNode)
        {

            if (docCurrent != null && docCurrent.IsFileReady)
            {
                // Update the object path + name + Tag in the object summary bar
                StringBuilder sb = new StringBuilder();

                sb.Append(">> ");
                sb.Append(nSelectedNode.FullPath);
                sb.Append("  (");
                sb.Append(nSelectedNode.NodeType.ToString());
                sb.Append(")");

                /*
                if (bLogToDebug)
                {
                    sb.Append(" GUID: [" + nSelectedNode.GUID + "]");
                    sb.Append(" ParentGUID: [" + nSelectedNode.ParentGUID + "]");
                }
                */

                frmMainForm.label1.Text = sb.ToString();
            }
            else
            {
                frmMainForm.label1.Text = ">>";
            }
        } // End of RefreshSelectedOjectPathBarText()

        internal static void RefreshListView(HEOrbitalObjTreeNode nSelectedNode)
        {
            //throw new NotImplementedException();

            if (nSelectedNode != null && docCurrent != null && docCurrent.IsFileReady)
            {

                frmMainForm.listView1.Items.Clear();


                // Test to see if we're drawing a <PARENT> and <THIS> item in the list view (option not yet implemented, on by defualt)
                const bool bFakeTestResult = true;
                if (bFakeTestResult)
                {
                    // Only draw the <PARENT> node if it's not null
                    if (nSelectedNode.Parent != null)
                    {

                        HEOrbitalObjTreeNode nodeParent = (HEOrbitalObjTreeNode)nSelectedNode.Parent;

                        string[] arrParentItem = new string[2];
                        arrParentItem[0] = "<" + nodeParent.Text + ">";
                        arrParentItem[1] = "<PARENT>";

                        ListViewItem liParentItem = new ListViewItem(arrParentItem)
                        {
                            Name = "<PARENT>",
                            Text = "<" + nSelectedNode.Parent.Text + ">",
                            Tag = nSelectedNode.Parent,
                            ImageIndex = HEUtilities.GetImageIndexByOrbitalObjectType(nodeParent.NodeType)
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
                            ImageIndex = HEUtilities.GetImageIndexByOrbitalObjectType(nSelectedNode.NodeType)
                        };
                        // Add the item
                        frmMainForm.listView1.Items.Add(liCurrentItem);
                    }
                }

                foreach (HEOrbitalObjTreeNode nodeChild in nSelectedNode.Nodes)
                {
                    string[] arr = new string[7];
                    arr[0] = nodeChild.Text;
                    arr[1] = nodeChild.NodeType.ToString();
                    arr[2] = nodeChild.CountOfChildNodes.ToString();
                    arr[3] = nodeChild.CountOfAllChildNodes.ToString();
                    arr[4] = nodeChild.OrbitData.SemiMajorAxis.ToString();
                    arr[5] = nodeChild.GUID.ToString();
                    arr[6] = nodeChild.SceneID.ToString();

                    ListViewItem liNewItem = new ListViewItem(arr)
                    {
                        Name = nodeChild.Text,
                        Text = nodeChild.Text,
                        Tag = nodeChild
                    };

                    if ((nodeChild.OrbitData.ParentGUID == -1) && (nodeChild.NodeType == HETreeNodeType.CelestialBody))
                    {
                        // It's the star, a special case
                        liNewItem.ImageIndex = (int)HEObjectTypesImageList.ButtonIcon_16x;
                    }
                    else
                    {
                        liNewItem.ImageIndex = HEUtilities.GetImageIndexByOrbitalObjectType(nodeChild.NodeType);
                    }


                    // Add the item
                    frmMainForm.listView1.Items.Add(liNewItem);
                }
            }
            else if (nSelectedNode == null)
            {
                //MessageBox.Show("RefreshListView was passed a null nSelectedNode");
            }
        }

        public static void RefreshSelectedObjectSummaryText(HEOrbitalObjTreeNode nSelectedNode)
        {
            // Updates the Object Information panel

            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            StringBuilder sb3 = new StringBuilder();


            if (nSelectedNode != null && docCurrent != null && docCurrent.IsFileReady)
            {

                sb1.Append("Node Tree Data");
                sb1.Append(Environment.NewLine);
                sb1.Append("Name: " + nSelectedNode.Name);
                sb1.Append(Environment.NewLine);
                sb1.Append("NodeType: " + nSelectedNode.NodeType.ToString());
                sb1.Append(Environment.NewLine);
                sb1.Append("GUID: " + nSelectedNode.GUID.ToString());
                sb1.Append(Environment.NewLine);
                sb1.Append("ParentGUID: " + nSelectedNode.ParentGUID.ToString());
                sb1.Append(Environment.NewLine);

                sb1.Append("SceneID: " + nSelectedNode.SceneID.ToString());
                sb1.Append(Environment.NewLine);
                sb1.Append("Type: " + nSelectedNode.Type.ToString());
                sb1.Append(Environment.NewLine);
                sb1.Append(Environment.NewLine);


                if (nSelectedNode.NodeType == HETreeNodeType.CelestialBody || nSelectedNode.NodeType == HETreeNodeType.Ship || nSelectedNode.NodeType == HETreeNodeType.Asteroid)
                {
                    sb1.Append(Environment.NewLine);
                    sb1.Append("ORBITAL DATA");
                    sb1.Append(Environment.NewLine);

                    sb1.Append("ParentGUID: " + nSelectedNode.OrbitData.ParentGUID.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("VesselID: " + nSelectedNode.OrbitData.VesselID.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("VesselType: " + nSelectedNode.OrbitData.VesselType.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("SemiMajorAxis: " + nSelectedNode.SemiMajorAxis.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("Inclination: " + nSelectedNode.Inclination.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("Eccentricity: " + nSelectedNode.OrbitData.Eccentricity.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("LongitudeOfAscendingNode: " + nSelectedNode.OrbitData.LongitudeOfAscendingNode.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("ArgumentOfPeriapsis: " + nSelectedNode.OrbitData.ArgumentOfPeriapsis.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append(Environment.NewLine);

                    sb1.Append("OrbitData.TimeSincePeriapsis: " + nSelectedNode.OrbitData.TimeSincePeriapsis.ToString());
                    sb1.Append(Environment.NewLine);
                    sb1.Append("OrbitData.SolarSystemPeriapsisTime: " + nSelectedNode.OrbitData.SolarSystemPeriapsisTime.ToString());
                    sb1.Append(Environment.NewLine);
                }

                if (nSelectedNode.NodeType != HETreeNodeType.SystemNAV)
                {
                    // Get the count of the child nodes contained in the selected node
                    decimal iTotalNodeCount = docCurrent.SolarSystemRootNode.CountOfAllChildNodes;
                    int iThisNodeCount = nSelectedNode.CountOfChildNodes;
                    int iThisNodeAndSubsCount = nSelectedNode.CountOfAllChildNodes;

                    decimal dThisNodeCountAsPercentage = ((decimal)iThisNodeCount / iTotalNodeCount) * 100;
                    decimal dThisNodeAndSubsCountAsPercentage = ((decimal)iThisNodeAndSubsCount / iTotalNodeCount) * 100;

                    sb2.Append("Node object counts for object " + nSelectedNode.Name + " of type " + nSelectedNode.NodeType.ToString());
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

                // Create JSON data for selected node
                if (nSelectedNode.Tag != null) sb3.Append((JValue)nSelectedNode.Tag.ToString());


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

            // Pass results to textBox1.Text
            frmMainForm.textBox1.Text = sb1.ToString();
            frmMainForm.textBox2.Text = sb2.ToString();
            frmMainForm.textBox3.Text = sb3.ToString();

        } // End of RefreshObjectSummaryText()

        public static void InitialiseTreeView(TreeView tvCurrent)
        {
            // Set the specified TreeView control's ImageLists to  
            // ilObjectTypesImageList and set the default icons
            tvCurrent.ImageList = ilObjectTypesImageList;
            tvCurrent.ImageIndex = (int)HEObjectTypesImageList.Flag_16x;
            tvCurrent.SelectedImageIndex = (int)HEObjectTypesImageList.Flag_16x;
            tvCurrent.TreeViewNodeSorter = new HEOrbitalObjTreeNodeSorter();

        }

        public static void InitialiseListView(ListView lvCurrent)
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

        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            frmMainForm = new MainForm();

            // Update the main form's title text - this adds the application name
            RefreshMainFormTitleText();

            // Prepare the TreeView control
            InitialiseTreeView(frmMainForm.treeView1);

            // Prepare the ListView control
            InitialiseListView(frmMainForm.listView1);

            // Disable the File/Close menu item - this is renabled when a file is loaded
            frmMainForm.closeToolStripMenuItem.Enabled = false;



            frmMainForm.Show();

            MessageBox.Show("Testing!");
            //frmMainForm.ShowDialog();

            // Start the Windows Forms message loop
            Application.Run(); // Application.Run(new MainForm());

        }
    } // end class Program
} // end namepsace HELLION


