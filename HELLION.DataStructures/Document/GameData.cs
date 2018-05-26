using System;
using System.Diagnostics;
using System.IO;

namespace HELLION.DataStructures.Document
{
    /// <summary>
    /// Implements the Game Data view node tree, comprised of the sub-trees
    /// of the Save File and each of the .json files in the Data folder (the Static Data).
    /// </summary>
    public class GameData : Json_File_Parent
    {
        /// <summary>
        /// Public property to get the root node of the Game Data tree.
        /// </summary>
        public HETreeNode RootNode => rootNode;

        /// <summary>
        /// Public property to get the SaveFile sub-object as this is not settable outside
        /// of the GameData class. Set at object creation through the constructor.
        /// </summary>
        public GameSave_Json_File SaveFile => saveFile;

        /// <summary>
        /// Public property to get the StaticData object as this is not settable outside
        /// of the GameData class. Set at object creation through the constructor.
        /// </summary>
        public Json_FileCollection StaticData => staticData;

        /// <summary>
        /// The root node of the Game Data tree.
        /// </summary>
        private HETreeNode rootNode = null;

        /// <summary>
        /// The SaveFile object.
        /// </summary>
        private GameSave_Json_File saveFile = null;

        /// <summary>
        /// the StaticDataFileCollection object which enumerates the Data folder and builds  
        /// node trees to a preconfigured depth of each of the .json files in that folder.
        /// </summary>
        private Json_FileCollection staticData = null;

        /// <summary>
        /// Constructor that takes a FileInfo and a DirectoryInfo representing the save file
        /// and the Data folder.
        /// </summary>
        /// <param name="SaveFileInfo">The FileInfo representing the save file.</param>
        /// <param name="StaticDataFolderInfo">The DirectoryInfo representing the Data folder.</param>
        public GameData(FileInfo SaveFileInfo, DirectoryInfo StaticDataFolderInfo)
        {
            rootNode = new HETreeNode(ownerObject: this, nodeName: "Game Data", newNodeType: HETreeNodeType.DataView);
            if (SaveFileInfo != null && SaveFileInfo.Exists)
            {
                Debug.Print("File evaluated {0}", SaveFileInfo.Name);

                // FINDME
                saveFile = new GameSave_Json_File(this, SaveFileInfo, populateNodeTreeDepth: 5);

                // Pre-load in several levels of node.
                saveFile.RootNode.CreateChildNodesFromjData(3);

                if (saveFile.RootNode == null) throw new Exception("SaveFile rootNode was null");
                else RootNode.Nodes.Add(saveFile.RootNode);
            }

            if (StaticDataFolderInfo != null && StaticDataFolderInfo.Exists)
            {
                staticData = new Json_FileCollection(this, StaticDataFolderInfo, autoPopulateTreeDepth: 1);
                if (staticData.RootNode == null) throw new Exception("StaticData rootNode was null");
                else RootNode.Nodes.Add(staticData.RootNode);
            }
        }

        /// <summary>
        /// Handles closing this object and sub-objects that support being closed.
        /// </summary>
        /// <returns>Returns true if close was successful.</returns>
        public bool Close()
        {
            bool closeSuccess = true;
            rootNode = null;

            if (saveFile.Close())
                saveFile = null;
            else
                closeSuccess = false;

            if (staticData.Close())
                staticData = null;
            else
                closeSuccess = false;

            return closeSuccess;
        }

    }
}
