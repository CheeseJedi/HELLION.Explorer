using System;
using System.IO;
using System.Windows.Forms;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Definition for a workspace for a HELLION Save (.save) JSON file.
    /// </summary>
    /// <remarks>
    /// Includes methods for loading a save and associated data files in to memory, building a 
    /// custom node tree representing the orbital objects and retrieving data from the tree
    /// to populate the dynamic list and full data from the source.
    /// </remarks>
    public class DocumentWorkspace
    {
        /// <summary>
        /// Constructor that takes a FileInfo and a DirectoryInfo.
        /// </summary>
        /// <param name="passedFileInfo">The FileInfo that represents the .save file.</param>
        /// <param name="passedDirectoryInfo">The DirectoryInfo that represents the Static Data folder.</param>
        /// <param name="autoLoad"></param>
        /// <param name="passedListView"></param>
        /// <param name="passedTreeView"></param>
        /// <param name="passedImageList"></param>
        public DocumentWorkspace(FileInfo passedFileInfo, DirectoryInfo passedDirectoryInfo, TreeView passedTreeView, ListView passedListView, EmbeddedImages_ImageList passedHEImageList)
        {
            // Initialise the GameData, SolarSystem and SearchHandler objects.
            if (passedFileInfo == null || passedDirectoryInfo == null
                || !passedFileInfo.Exists || !passedDirectoryInfo.Exists)
                throw new InvalidOperationException("DocumentWorkspace Constructor: A problem occurred with a passed parameter - something doesn't exist.");
            else
            {
                GameData = new GameData(passedFileInfo, passedDirectoryInfo);
                SolarSystem = new SolarSystem(GameData);
                SearchHandler = new SearchHandler(GameData, SolarSystem);
                Blueprints = new BlueprintsHandler_UI();

                // Add the parameters related to the MainForm controls.
                mainFormTreeView = passedTreeView ?? throw new NullReferenceException("passedTreeView was null.");
                mainFormListView = passedListView ?? throw new NullReferenceException("passedListView was null.");
                mainProgramHEImageList = passedHEImageList ?? throw new NullReferenceException("passedHEImageList was null.");

                InitialiseTreeView(mainFormTreeView, mainProgramHEImageList.IconImageList);
                InitialiseListView(mainFormListView, mainProgramHEImageList.IconImageList);

                IsWorkspaceReady = true;
            }
        }

        /// <summary>
        /// The GameData object that is responsible for loading the save file and all the static data files and then
        /// building node trees representing the raw data.
        /// </summary>
        public GameData GameData { get; private set; } = null;

        /// <summary>
        /// The SolarSystem object that is responsible for building the Solar System node tree, made up of celestial
        /// bodies, asteroids, ships (which includes modules at this point) and players.
        /// </summary>
        public SolarSystem SolarSystem { get; private set; } = null;

        /// <summary>
        /// The SearchHandler object that implements search.
        /// </summary>
        public SearchHandler SearchHandler { get; private set; } = null;

        /// <summary>
        /// The BlueprintsHandler_UI object that handles loading and displaying blueprints.
        /// </summary>
        public BlueprintsHandler_UI Blueprints { get; private set; } = null;

        /// <summary>
        /// The .save file that is being opened.
        /// </summary>
        public FileInfo SaveFileInfo { get; private set; } = null;

        /// <summary>
        /// The DirectoryInfo that represents the Static Data folder.
        /// </summary>
        public DirectoryInfo DataDirectoryInfo { get; private set; } = null;

        /// <summary>
        /// Determines whether the set of files are ready for use.
        /// </summary>
        public bool IsWorkspaceReady { get; private set; } = false;

        /// <summary>
        /// Determines whether there was a load error.
        /// </summary>
        public bool LoadError { get; private set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public bool IsDirty { get; private set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public bool LogToDebug { get; set; } = false;

        /// <summary>
        /// Stores a reference to the Main Form's tree view control.
        /// </summary>
        private TreeView mainFormTreeView = null;

        /// <summary>
        /// Stores a reference to the Main Form's list view control.
        /// </summary>
        private ListView mainFormListView = null;

        /// <summary>
        /// Stores a reference to the main program's ImageList for icon use.
        /// </summary>
        private EmbeddedImages_ImageList mainProgramHEImageList = null;

        /// <summary>
        /// Handles closing of the document workspace.
        /// </summary>
        /// <remarks>
        /// Calls the close method on any sub-objects that support it.
        /// </remarks>
        /// <returns>Returns true if the close was successful.</returns>
        public bool Close()
        {
            if (IsDirty)
            {
                // Eventually - will figure out what's dirty
                return false; // indicates a problem
            }
            else
            {
                // Not dirty, OK to close everything
                if (SolarSystem != null)
                {
                    SolarSystem.Close();
                    SolarSystem = null;
                }
                if (GameData != null)
                {
                    GameData.Close();
                    GameData = null;
                }
                SaveFileInfo = null;
                DataDirectoryInfo = null;

                return true;
            }
        }

        /// <summary>
        /// Initialises the tree view control and sets the ImageList and default images.
        /// </summary>
        /// <param name="passedTreeView">The TreeView control to initialise.</param>
        /// <param name="passedImageList"></param>
        private void InitialiseTreeView(TreeView passedTreeView, ImageList passedImageList)
        {
            passedTreeView.ImageList = passedImageList;
            passedTreeView.ImageIndex = (int)EmbeddedImages_ImageList.HEIconsImageNames.Flag_16x;
            passedTreeView.SelectedImageIndex = (int)EmbeddedImages_ImageList.HEIconsImageNames.Flag_16x;
            passedTreeView.TreeViewNodeSorter = new SolarSystem_TreeNode_Sorter();
            passedTreeView.ShowNodeToolTips = true;
        }

        /// <summary>
        /// Initialises the list view control, sets the ImageList and some default columns.
        /// </summary>
        /// <param name="passedListView"></param>
        /// <param name="passedImageList"></param>
        private void InitialiseListView(ListView passedListView, ImageList passedImageList)
        {
            passedListView.SmallImageList = passedImageList;
            // Add some columns appropriate to the data we intend to add. This will likely be revised soon.
            passedListView.Columns.Add("Name", 180, HorizontalAlignment.Left);
            passedListView.Columns.Add("Type", 120, HorizontalAlignment.Left);
            passedListView.Columns.Add("Count", 50, HorizontalAlignment.Left);
            passedListView.Columns.Add("TotalCount", 60, HorizontalAlignment.Left);
            passedListView.Columns.Add("FullPath", 120, HorizontalAlignment.Left);
            passedListView.Columns.Add("GUID", 50, HorizontalAlignment.Right);
            passedListView.Columns.Add("SceneID", 30, HorizontalAlignment.Right);
        }

    }
}
