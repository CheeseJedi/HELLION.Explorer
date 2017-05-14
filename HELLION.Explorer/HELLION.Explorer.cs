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

        public static void ControlledExit()
        {
            // Check the current document isn't null
            if (docCurrent != null)
            {
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
                    }
                }
            }
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

        public static void FileOpen()
        {
            // Loads a save file in to memory and processes it

            // Check that the Data folder path has been defined, if not trigger the DefineGameFolder then return
            if (!IsGameDataFolderDefined())
            {
                DefineGameFolder();
                return;
            }

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

                RefreshMainFormTitleText();

                //Application.UseWaitCursor = true;
                frmMainForm.Cursor = Cursors.WaitCursor;

                // Suppress repainting the TreeView until all the objects have been created.
                frmMainForm.tvNavigationTree.BeginUpdate();

                // Clear any existing nodes
                frmMainForm.tvNavigationTree.Nodes.Clear();

                docCurrent = new HEDocumentWorkspace()
                {
                    // Activates LogToDebug for docCurrent
                    LogToDebug = bLogToDebug
                };




                // Grab the Game Data Folder from Properties
                string sGameDataFolder = Properties.HELLIONExplorer.Default.sGameDataFolder + "\\";

                docCurrent.MainFile.FileName = openFileDialog1.FileName;
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
                if (docCurrent.RootNode != null)
                {
                    // This feels wrong calling frmMainForm 
                    frmMainForm.tvNavigationTree.Nodes.Add(docCurrent.RootNode);
                    frmMainForm.tvNavigationTree.SelectedNode = docCurrent.RootNode;
                    frmMainForm.tvNavigationTree.SelectedNode.Expand();
                    frmMainForm.toolStripStatusLabel1.Text = ("Ready");
                }
                else
                {
                    MessageBox.Show("Error: RootNode was null");
                }
                    
                    
                // Begin repainting the TreeView.
                frmMainForm.tvNavigationTree.EndUpdate();

                //Application.UseWaitCursor = false;
                frmMainForm.Cursor = Cursors.Default;

                RefreshMainFormTitleText();
                RefreshSelectedObjectSummaryText(docCurrent.RootNode);

                    

            }
        } // End of FileOpen()

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
            // Update the object path + name + Tag in the object summary bar
            StringBuilder sb = new StringBuilder();

            sb.Append(">>");
            sb.Append(nSelectedNode.FullPath);
            sb.Append("  (");
            sb.Append(nSelectedNode.NodeType.ToString());
            sb.Append(")");

            if (bLogToDebug)
            {
                sb.Append(" GUID: [" + nSelectedNode.GUID + "]");
                sb.Append(" ParentGUID: [" + nSelectedNode.ParentGUID + "]");
            }

            frmMainForm.label1.Text = sb.ToString();
        } // End of RefreshSelectedOjectPathBarText()

        internal static void RefreshListView(HEOrbitalObjTreeNode nSelectedNode)
        {
            //throw new NotImplementedException();
            frmMainForm.listView1.Items.Clear();

            foreach (HEOrbitalObjTreeNode nodeChild in nSelectedNode.Nodes)
            {

                string[] arr = new string[2];
                arr[0] = nodeChild.Text;
                arr[1] = nodeChild.NodeType.ToString();

                ListViewItem liNewItem = new ListViewItem(arr)
                {
                    Name = nodeChild.Text,
                    Text = nodeChild.Text,
                    Tag = nodeChild.NodeType.ToString()
                };


                frmMainForm.listView1.Items.Add(liNewItem);
            }
        }

        public static void RefreshSelectedObjectSummaryText(HEOrbitalObjTreeNode nSelectedNode)
        {
            // Updates the Object Information panel
                
            StringBuilder sb1 = new StringBuilder();

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

            sb1.Append(Environment.NewLine);
            sb1.Append("ORBITAL DATA");
            sb1.Append(Environment.NewLine);

            sb1.Append("OrbitData.ParentGUID: " + nSelectedNode.OrbitData.ParentGUID.ToString());
            sb1.Append(Environment.NewLine);
            sb1.Append("OrbitData.VesselID: " + nSelectedNode.OrbitData.VesselID.ToString());
            sb1.Append(Environment.NewLine);
            sb1.Append("OrbitData.VesselType: " + nSelectedNode.OrbitData.VesselType.ToString());
            sb1.Append(Environment.NewLine);



            sb1.Append("OrbitData.SemiMajorAxis: " + nSelectedNode.SemiMajorAxis.ToString());
            sb1.Append(Environment.NewLine);
            sb1.Append("OrbitData.Inclination: " + nSelectedNode.Inclination.ToString());
            sb1.Append(Environment.NewLine);
            sb1.Append(Environment.NewLine);

            sb1.Append("OrbitData.Eccentricity: " + nSelectedNode.OrbitData.Eccentricity.ToString());
            sb1.Append(Environment.NewLine);

            sb1.Append("OrbitData.LongitudeOfAscendingNode: " + nSelectedNode.OrbitData.LongitudeOfAscendingNode.ToString());
            sb1.Append(Environment.NewLine);
            sb1.Append("OrbitData.ArgumentOfPeriapsis: " + nSelectedNode.OrbitData.ArgumentOfPeriapsis.ToString());
            sb1.Append(Environment.NewLine);
            sb1.Append(Environment.NewLine);



            sb1.Append("OrbitData.TimeSincePeriapsis: " + nSelectedNode.OrbitData.TimeSincePeriapsis.ToString());
            sb1.Append(Environment.NewLine);
            sb1.Append("OrbitData.SolarSystemPeriapsisTime: " + nSelectedNode.OrbitData.SolarSystemPeriapsisTime.ToString());
            sb1.Append(Environment.NewLine);






            sb1.Append(Environment.NewLine);
            sb1.Append("Immediate SubNodes (all types): ");
            sb1.Append(nSelectedNode.GetNodeCount(includeSubTrees: false).ToString());
            sb1.Append(Environment.NewLine);
            sb1.Append("All SubNodes (all types): ");
            sb1.Append(nSelectedNode.GetNodeCount(includeSubTrees: true).ToString());
            sb1.Append(Environment.NewLine);
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

            // Pass results to textBox1.Text
            frmMainForm.textBox1.Text = sb1.ToString();

            StringBuilder sb2 = new StringBuilder();

            sb2.Append("JSON Data");
            sb2.Append(Environment.NewLine);
            sb2.Append(Environment.NewLine);
            if (nSelectedNode.Tag != null) sb2.Append((JValue)nSelectedNode.Tag.ToString());
                



        } // End of RefreshObjectSummaryText()

        /// <summary>
        /// The main entry point for the application.
        /// </summary>



        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new frmMainForm());
            frmMainForm = new MainForm();
            RefreshMainFormTitleText();
            // Set the tvNavigationTree and listView1 ImageLists to  
            // ilObjectTypesImageList and set the default icons
            frmMainForm.tvNavigationTree.ImageList = ilObjectTypesImageList;
            frmMainForm.tvNavigationTree.ImageIndex = (int)HEObjectTypesImageList.Flag_16x;
            frmMainForm.tvNavigationTree.SelectedImageIndex = (int)HEObjectTypesImageList.Flag_16x;
            frmMainForm.tvNavigationTree.TreeViewNodeSorter = new HEOrbitalObjTreeNodeSorter();

            frmMainForm.listView1.SmallImageList = ilObjectTypesImageList;
            







            frmMainForm.ShowDialog();
        }
    } // end class Program
} // end namepsace HELLION


