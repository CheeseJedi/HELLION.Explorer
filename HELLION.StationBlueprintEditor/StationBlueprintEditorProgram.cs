using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HELLION.DataStructures.Blueprints;
using HELLION.DataStructures.EmbeddedImages;
using HELLION.DataStructures.Utilities;
using Newtonsoft.Json.Linq;

namespace HELLION.StationBlueprintEditor
{
    public static class StationBlueprintEditorProgram
    {

        #region Logging Objects

        /// <summary>
        /// Logging handler object.
        /// </summary>
        internal static LogFileHandler Logging = new LogFileHandler();

        /// <summary>
        /// The suffix appended to the current time date to create a unique log file name.
        /// </summary>
        internal const string LogFileNameSuffix = "_HELLION.StationBlueprintEditor";

        #endregion

        #region Form Related Objects

        /// <summary>
        /// Defines the main form object.
        /// </summary>
        internal static StationBlueprintEditorForm MainForm { get; private set; }

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

        internal static StructureDefinitions_File structureDefinitionsFile = null;

        /// <summary>
        /// Defines an object to hold the current open document
        /// </summary>
        internal static StationBlueprint_File docCurrent = null;

        #endregion

        #region Misc Objects

        /// <summary>
        /// Initialise an UpdateChecker object and specify the GitHub user name and repository name.
        /// </summary>
        internal static UpdateChecker hEUpdateChecker = new UpdateChecker
            ("CheeseJedi", "HELLION.StationBlueprintEditor");

        /// <summary>
        /// Initialises an EmbeddedImages_ImageList object to supply the image list to the tree view and
        /// list view controls, plus anywhere else the images may be used.
        /// </summary>
        internal static EmbeddedImages_ImageList hEImageList = new EmbeddedImages_ImageList();

        /// <summary>
        /// Defines an ImageList and set it to the EmbeddedImages_ImageList
        /// </summary>
        internal static ImageList ilObjectTypesImageList = hEImageList.IconImageList;

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
                    //SetGameDataFolder(dataFolderPath);
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
            var anHELLIONDataStructures = System.Reflection.Assembly.GetAssembly(typeof(General)).GetName();
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

        internal static void FileNew()
        {

            FileClose();



        }


        /// <summary>
        /// Loads a .save file in to memory - passes details on to the DocumentWorkspace and
        /// tells it to load.
        /// </summary>
        /// <param name="sFileName"></param>
        internal static void FileOpen(string sFileName = "")
        {
            // Make a note of the starting time
            DateTime startingTime = DateTime.Now;

            // If the sFileName is set, check the file exists otherwise prompt the user to select a file
            if (sFileName == "")
            {
                // Create a new OpenFileDialog box and set some parameters
                var openFileDialog1 = new OpenFileDialog()
                {
                    Filter = "Hellion Station Blueprint Files (Json)|*.json|All files|*.*",
                    Title = "Open Station Blueprint file",
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
            //dataDirectoryInfo = new DirectoryInfo(Properties.HELLION.Explorer.Default.sGameDataFolder);

            if (saveFileInfo.Exists) // && dataDirectoryInfo.Exists)
            {
                // Set the status strip message
                //MainForm.toolStripStatusLabel1.Text = ("Loading file: " + saveFileInfo.FullName);

                // Update the main window's title text to reflect the filename selected
                //RefreshMainFormTitleText(saveFileInfo.FullName);

                //Application.UseWaitCursor = true;
                MainForm.Cursor = Cursors.WaitCursor;

                // Suppress repainting the TreeView until all the objects have been created.
                //MainForm.treeView1.BeginUpdate();

                // Create a new DocumentWorkspace
                //docCurrent = new DocumentWorkspace(saveFileInfo, dataDirectoryInfo, MainForm.treeView1, MainForm.listView1, hEImageList);

                docCurrent = new StationBlueprint_File(null, saveFileInfo);


                MainForm.JsonBlueprintFile = docCurrent;
                MainForm.Blueprint = docCurrent.BlueprintObject;

                Debug.Print(docCurrent.BlueprintObject.Name);

                MainForm.RefreshEverything();

                // Enable the Save and Save As menu items.
                MainForm.saveToolStripMenuItem.Enabled = true;
                MainForm.saveAsToolStripMenuItem.Enabled = true;


                // Begin repainting the TreeView.
                //MainForm.treeView1.EndUpdate();

                //Application.UseWaitCursor = false;
                MainForm.Cursor = Cursors.Default;

                //RefreshMainFormTitleText();

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
                string newFileName = passedFileName ?? docCurrent.File.FullName;
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
                    FileName = docCurrent.File.FullName,
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
            if (docCurrent != null && docCurrent.IsDirty)
            {
                // Unsaved changes, prompt user to save
                string sMessage = docCurrent.File.FullName + " has been modified." + Environment.NewLine + "Do you want to save changes to this Blueprint before exiting?";
                const string sCaption = "Unsaved Changes";
                DialogResult result = MessageBox.Show(sMessage, sCaption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Cancel) return;


                switch (result)
                {
                    case DialogResult.Cancel:
                        //e.Cancel = true;
                        return;
                    // If the yes button was pressed ...
                    case DialogResult.Yes:
                        MessageBox.Show("User selected to save changes.", "NonImplemented Notice",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Save operation to be triggered here.



                        break;

                }

                // Close the file



            }

            // TODO: More work to be done here to handle cleanup, and calling the save

            //GraftTreeOutboundToMainForm();

            // Remove the current JsonDataViewForm from the jsonDataViews list
            //StationBlueprintEditorProgram.blueprintEditorForms.Remove(this);

            MainForm.Cursor = Cursors.WaitCursor;

            // Suppress repainting the TreeView until all the objects have been created.
            //MainForm.treeView1.BeginUpdate();

            // Create a new DocumentWorkspace
            //docCurrent = new DocumentWorkspace(saveFileInfo, dataDirectoryInfo, MainForm.treeView1, MainForm.listView1, hEImageList);

            //Blueprint = JsonBlueprintFile.BlueprintObject ?? throw new NullReferenceException("JsonBlueprintFile.BlueprintObject was null.");

            // Trigger a refresh on each of the node trees.
            //docCurrent.SolarSystem.RootNode.RefreshToolTipText(includeSubtrees: true);
            //docCurrent.GameData.RootNode.RefreshToolTipText(includeSubtrees: true);
            //docCurrent.Blueprints.RootNode.RefreshToolTipText(includeSubtrees: true);
            //docCurrent.SearchHandler.RootNode.RefreshToolTipText(includeSubtrees: true);

            Debug.Print(docCurrent.BlueprintObject.Name);

            MainForm.FormTitleText = String.Empty;
            MainForm.RefreshEverything();

            // Enable the Save and Save As menu items.
            //MainForm.saveToolStripMenuItem.Enabled = true;
            //MainForm.saveAsToolStripMenuItem.Enabled = true;

            // Begin repainting the TreeView.
            //MainForm.treeView1.EndUpdate();

            //Application.UseWaitCursor = false;
            MainForm.Cursor = Cursors.Default;

            // Disable the Save and Save As menu items.
            //MainForm.saveToolStripMenuItem.Enabled = false;
            //MainForm.saveAsToolStripMenuItem.Enabled = false;

            // Disable the Observed GUIDs menu item.
            //MainForm.observedGUIDsToolStripMenuItem.Enabled = false;

            // Clear any existing nodes from the tree view
            //MainForm.treeView1.Nodes.Clear();

            // Clear any items from the list view
            //MainForm.listView1.Items.Clear();

            // Check for an existing document and close it if necessary
            if (docCurrent != null)
            {
                // Clear the existing document
                docCurrent.Close();
                docCurrent = null;
            }

            MainForm.Blueprint = null;
            MainForm.JsonBlueprintFile = null;




            MainForm.RefreshEverything();


            MainForm.closeToolStripMenuItem.Enabled = false;
            MainForm.revertToolStripMenuItem.Enabled = false;

            // Trigger refresh of UI elements
            //RefreshMainFormTitleText();

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
            string currentFileName = docCurrent.File.FullName;
            FileClose();
            FileOpen(currentFileName);
        }

        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Make a record of the starting time.
            DateTime operationStartTime = DateTime.Now;
            Logging.WriteLine(Application.ProductName + " - " + Application.ProductVersion);
#if DEBUG
            Logging.WriteLine("Mode=Debug");
#else
            Logging.WriteLine("Mode=Release"); 
#endif
            Logging.WriteLine("Part of HELLION.Explorer - https://github.com/CheeseJedi/HELLION.Explorer");


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            // Initialise the StructureDefinitions file - temporary setup!
            structureDefinitionsFile = new StructureDefinitions_File(null, new FileInfo(@"E:\HELLION\TestArea\Output.json"));

            // Initialise the main form
            MainForm = new StationBlueprintEditorForm();


            // Set the form's icon
            var exe = System.Reflection.Assembly.GetExecutingAssembly();
            var iconStream = exe.GetManifestResourceStream("HELLION.StationBlueprintEditor.HELLION.StationBlueprintEditor.ico");
            if (iconStream != null) MainForm.Icon = new Icon(iconStream);

            // Update the main form's title text - this adds the application name
            //RefreshMainFormTitleText();

            // Show the main form
            MainForm.Show();

            ProcessCommandLineArguments(args);

            // Start the Windows Forms message loop
            Application.Run(); // Application.Run(new MainForm());



        }
    }
}
