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
    public class StationBlueprintEditorProgram
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

        #region Properties

        /// <summary>
        /// Defines the main form object.
        /// </summary>
        internal static StationBlueprintEditorForm MainForm { get; private set; }

        /// <summary>
        /// Defines an object to hold the current open document
        /// </summary>
        internal static StationBlueprint_File DocCurrent
        {
            get => _docCurrent;
            set
            {
                if (_docCurrent != value)
                {
                    _docCurrent = value;
                    if (_docCurrent?.File != null)
                    {
                        Debug.Print("DocCurrent change triggering potential SaveFileInfo change.");
                        //SaveFileInfo = _docCurrent.File;
                    }
                }
            }
        }

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
        /// <param name="arguments"></param>
        internal static bool ProcessCommandLineArguments(string[] arguments)
        {
            if (arguments.Length > 0)
            {
                // There are arguments 

                string helpText = Application.ProductName
                    + ".exe <full file name of station blueprint .json file to open> "
                    + "[/logfolder <path to log file directory>] "
                    + "[/nobackup] [/verbose] ";

                for (int i = 0; i < arguments.Length; i++)
                {
                    // Try to figure out what's in this argument
                    if (arguments[i].EndsWith(".json", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's a .json file
                        arg_blueprintFileInfo = new FileInfo(arguments[i]);
                        Logging.WriteLine("Argument: Station Blueprint File " + arg_blueprintFileInfo.FullName);

                        if (!arg_blueprintFileInfo.Exists)
                        {
                            Logging.WriteLine("Specified Station Blueprint File does not exist.");
                            return false;
                        }
                    }
                    else if (arguments[i].Equals("/logfolder", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Increment i by one to prevent the next element being processed in case there are other(?) arguments.
                        i++;
                        DirectoryInfo _loggingPath = new DirectoryInfo(arguments[i]);
                        Logging.WriteLine("Argument: Log File Path " + _loggingPath.FullName);
                        if (!(_loggingPath.Exists))
                        {
                            Logging.WriteLine("Invalid logging path specified.");
                            return false;
                        }
                        Logging.LogFile = new FileInfo(Path.Combine(_loggingPath.FullName, Logging.GenerateLogFileName(LogFileNameSuffix)));
                        Logging.WriteLine("Logging to: " + Logging.LogFile.FullName);

                        Logging.Mode = LogFileHandler.LoggingOperationType.ConsoleAndLogFile;

                    }
                    else if (arguments[i].Equals("/nobackup", StringComparison.CurrentCultureIgnoreCase))
                    {
                        arg_createBackup = false;
                        Logging.WriteLine("Argument: Backup file will NOT be created.");
                    }
                    else if (arguments[i].Equals("/verbose", StringComparison.CurrentCultureIgnoreCase))
                    {
                        arg_verboseOutput = true;
                        Logging.WriteLine("Argument: Verbose output ON.");
                    }
                    else if (arguments[i].Equals("/?") || arguments[i].ToLower().Contains("help"))
                    {
                        Logging.WriteLine(helpText);
                        return false;
                    }
                    else
                    {
                        Logging.WriteLine("Unexpected Argument: " + arguments[i]);
                        Logging.WriteLine("Use /? or /help to show available arguments.");
                        return false;
                    }
                }

                // We got here so everything checked out so far.

                return true;
            }
            else
            {
                //Logging.WriteLine("No parameters specified.");
                //Logging.WriteLine("Use /? or /help to show available arguments.");
                return false;
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
            sb.Append(sNL);
            sb.Append("Part of HELLION.Explorer - https://github.com/CheeseJedi/HELLION.Explorer");
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
            sb.Append(string.Format("Approx. memory usage (bytes): {0:N0}", GC.GetTotalMemory(false)));
            sb.Append(sNL2);
            sb.Append(sNL);

            // Credit
            sb.Append("Uses the Newtonsoft JSON library. http://www.newtonsoft.com/json");
            sb.Append(sNL2);

            // Credit
            //sb.Append("Uses the FastColoredTextBox library. https://github.com/PavelTorgashov/FastColoredTextBox");
            //sb.Append(sNL2);

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
        public static void ControlledExit()
        {
            // Check the current document isn't null
            if (DocCurrent != null)
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
        /// Gets the application path.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This process may be better off using reflection.</remarks>
        public string GetApplicationPath() => Environment.CurrentDirectory;


        #endregion

        #region File Menu Methods

        /// <summary>
        /// Creates a new empty blueprint object and blueprint file object.
        /// </summary>
        internal static void FileNew(JToken jdata = null)
        {
            Debug.Print("FileNew() called, jdata == null {0}", (jdata == null).ToString());
            // Call the file close to handle unsaved changes
            if (DocCurrent != null) FileClose();

            // Create a new StationBlueprint file, passing the JToken.
            DocCurrent = new StationBlueprint_File(null, jdata);

            if (DocCurrent.BlueprintObject == null) Debug.Print("Newly created StationBlueprintFile doesn't contain a blueprint object.");

            MainForm.RefreshEverything();

        }

        /// <summary>
        /// Opens a file and displays a OpenFileDialog if no file path specified.
        /// </summary>
        /// <remarks>Intended to be called by a UI action rather than a command line argument.</remarks>
        /// <param name="fileName"></param>
        /// <returns>Returns true on error.</returns>
        internal static bool FileOpen(string fileName = null)
        {

            // If the fileName is set, check the file exists otherwise prompt the user to select a file
            if (string.IsNullOrEmpty(fileName))
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
                if (dialogResult == DialogResult.Cancel) return true;

                // Check that the file exists when the user clicked OK
                if (dialogResult == DialogResult.OK)
                {
                    // Check for an existing document and close it if necessary
                    if (DocCurrent != null)
                    {
                        FileClose();
                    }
                    // Set the file name from the dialog selection.
                    fileName = openFileDialog1.FileName;
                }
            }
            else
            {
                // We were passed a file name from the command line, check to see if it's actually there
                if (!System.IO.File.Exists(fileName))
                {
                    // The file name passed doesn't exist
                    MessageBox.Show(string.Format("Error opening file:{1}{0}from command line - file doesn't exist.", Environment.NewLine, fileName));

                    return true;
                }
            }

            return FileOpen(new FileInfo(fileName));


        }

        /// <summary>
        /// Opens a file from a specified FileInfo object.
        /// </summary>
        /// <param name="saveFileInfo"></param>
        /// <returns>Returns true on error.</returns>
        internal static bool FileOpen(FileInfo saveFileInfo)
        {
            // Take care of any existing open (including modified) file.
            if (DocCurrent != null) FileClose();

            Debug.Print("FileOpen(FileInfo) called.");
            // Make a note of the starting time
            DateTime startingTime = DateTime.Now;


            if (!saveFileInfo.Exists) throw new FileNotFoundException("File not found: {0}", saveFileInfo.FullName);
            // Set the status strip message
            //MainForm.toolStripStatusLabel1.Text = ("Loading file: " + _saveFileInfo.FullName);

            // Update the main window's title text to reflect the filename selected
            //RefreshMainFormTitleText(_saveFileInfo.FullName);

            //Application.UseWaitCursor = true;
            MainForm.Cursor = Cursors.WaitCursor;

            // Suppress repainting the TreeView until all the objects have been created.
            //MainForm.treeView1.BeginUpdate();

            DocCurrent = new StationBlueprint_File(null, saveFileInfo);


            //MainForm.JsonBlueprintFile = DocCurrent;
            //MainForm.Blueprint = DocCurrent.BlueprintObject;

            Debug.Print(DocCurrent.BlueprintObject.Name);

                

            // Enable the Save and Save As menu items.
            MainForm.saveToolStripMenuItem.Enabled = true;
            MainForm.saveAsToolStripMenuItem.Enabled = true;


            // Begin repainting the TreeView.
            //MainForm.treeView1.EndUpdate();

            //Application.UseWaitCursor = false;
            MainForm.Cursor = Cursors.Default;

            //RefreshMainFormTitleText();

            MainForm.RefreshEverything();


            MainForm.closeToolStripMenuItem.Enabled = true;
            MainForm.revertToolStripMenuItem.Enabled = true;

            MainForm.toolStripStatusLabel1.Text = string.Format("File load and processing completed in {0:mm}m{0:ss}s", DateTime.Now - startingTime);

            return false;
        }

        /// <summary>
        /// Performs a save operation of the current document, or if a string file name is
        /// passed it will perform a SaveAs operation to the new desired file name.
        /// </summary>
        /// <param name="passedFileName"></param>
        internal static void FileSave(string passedFileName = null)
        {
            if (DocCurrent == null) throw new NullReferenceException("FileSave: DocCurrent was null.");
            else
            {
                DocCurrent.Serialise();

                if (!string.IsNullOrEmpty(passedFileName))
                    DocCurrent.SaveFile(true, new FileInfo(passedFileName));

                else DocCurrent.SaveFile(createBackup: true);

            }
        }

        /// <summary>
        /// Prompts the user to input a new file name then calls the FileSave method.
        /// </summary>
        internal static void FileSaveAs()
        {
            if (DocCurrent == null) throw new NullReferenceException("FileSaveAs: DocCurrent was null.");
            else
            {

                // Display Save As dialog, with current file name passed to be used as the
                // default file name.
                var saveFileDialog1 = new SaveFileDialog()
                {
                    FileName = DocCurrent.File.FullName,
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
        /// Prepares the current open blueprint document for closing.
        /// </summary>
        internal static void FileClose()
        {
            if (DocCurrent != null)
            {
                Debug.Print("File Close called; Filename={0} BlueprintName={1}",
                    DocCurrent.File?.FullName, DocCurrent.BlueprintObject?.Name);

                // isFileDirty check before closing
                if (DocCurrent.IsDirty)
                {
                    // Unsaved changes, prompt user to save
                    string sMessage = DocCurrent.File?.FullName + " has been modified." + Environment.NewLine 
                        + "Do you want to save changes to this Station Blueprint File before exiting?";
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
                            break;

                        case DialogResult.No:
                        default:
                            break;
                    }

                }


                // Close the file

                // Set the mouse cursor to the animated wait cursor.
                MainForm.Cursor = Cursors.WaitCursor;

                // Suppress repainting the TreeView until all the objects have been created.
                MainForm.treeViewPrimaryStructure.BeginUpdate();
                MainForm.treeViewSecondaryStructures.BeginUpdate();

                // Clear both primary and secondary selections.
                MainForm.SelectedPrimaryStructureNode = null;
                MainForm.SelectedSecondaryStructureNode = null;

                // Disable the Save and Save As menu items.
                MainForm.saveToolStripMenuItem.Enabled = false;
                MainForm.saveAsToolStripMenuItem.Enabled = false;
                MainForm.closeToolStripMenuItem.Enabled = false;
                MainForm.revertToolStripMenuItem.Enabled = false;

                // Clear the existing document
                DocCurrent.Close();
                DocCurrent = null;
                
                // Trigger refresh
                MainForm.RefreshEverything();

                // Begin repainting the TreeViews.
                MainForm.treeViewPrimaryStructure.EndUpdate();
                MainForm.treeViewSecondaryStructures.EndUpdate();




                // Initiate Garbage Collection
                GC.Collect();

                // Restore default mouse cursor.
                MainForm.Cursor = Cursors.Default;

            }
        }

        /// <summary>
        /// Reverts the current document to the on-disk version.
        /// </summary>
        /// <remarks>
        /// Achieves this by closing the current file 
        /// </remarks>
        internal static void FileRevert()
        {
            string currentFileName = DocCurrent.File.FullName;
            FileClose();
            FileOpen(currentFileName);
        }

        #endregion

        #region Fields

        internal static StationBlueprint_File _docCurrent = null;
        internal static FileInfo arg_blueprintFileInfo = null;
        internal static bool arg_createBackup = true;
        internal static bool arg_verboseOutput = false;

        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <remarks>
        /// Public access modifier added to allow calling from the Explorer.
        /// </remarks>
        [STAThread]
        public static void Main(string[] args)
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
            // StructureDefinitionsFile = new StructureDefinitions_File(null, new FileInfo(@"E:\HELLION\TestArea\Output.json"));

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


            // Process the command line arguments.
            // If a file is specified to open, it will replace the just created new (empty) file.
            ProcessCommandLineArguments(args);

            if (arg_blueprintFileInfo != null && arg_blueprintFileInfo.Exists)
            {

                Logging.WriteLine("Loading station blueprint file...");
                if (FileOpen(arg_blueprintFileInfo))
                {
                    Logging.WriteLine("Problem loading save file.");
                }
                Logging.WriteLine("Complete.");



            }
            else
            {
                // Generate a new file.
                FileNew((JToken)null);
            }




            // Start the Windows Forms message loop
            Application.Run(); // Application.Run(new MainForm());



        }
    }
}
