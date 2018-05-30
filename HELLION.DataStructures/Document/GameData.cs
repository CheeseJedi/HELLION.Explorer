using System;
using System.Diagnostics;
using System.IO;
using HELLION.DataStructures.UI;

namespace HELLION.DataStructures.Document
{
    /// <summary>
    /// Implements the Game Data view node tree, comprised of the sub-trees
    /// of the Save File and each of the .json files in the Data folder (the Static Data).
    /// </summary>
    public class GameData : Json_File_Parent, Iparent_Base_TN
    {
        /// <summary>
        /// Constructor that takes a FileInfo and a DirectoryInfo representing the save file
        /// and the Data folder.
        /// </summary>
        /// <param name="SaveFileInfo">The FileInfo representing the save file.</param>
        /// <param name="StaticDataFolderInfo">The DirectoryInfo representing the Data folder.</param>
        public GameData(FileInfo SaveFileInfo, DirectoryInfo StaticDataFolderInfo)
        {
            RootNode = new HETreeNode(ownerObject: this, nodeName: "Game Data", newNodeType: HETreeNodeType.DataView);
            if (SaveFileInfo != null && SaveFileInfo.Exists)
            {
                Debug.Print("File evaluated {0}", SaveFileInfo.Name);

                // FINDME
                SaveFile = new GameSave_Json_File(this, SaveFileInfo, populateNodeTreeDepth: 5);

                // Pre-load in several levels of node.
                Json_TreeNode rootNode = (Json_TreeNode)SaveFile.RootNode;
                rootNode.CreateChildNodesFromjData(3);

                if (SaveFile.RootNode == null) throw new Exception("SaveFile rootNode was null");
                else RootNode.Nodes.Add(SaveFile.RootNode);
            }

            if (StaticDataFolderInfo != null && StaticDataFolderInfo.Exists)
            {
                StaticData = new Json_FileCollection(this, StaticDataFolderInfo, autoPopulateTreeDepth: 1);
                if (StaticData.RootNode == null) throw new Exception("StaticData rootNode was null");
                else RootNode.Nodes.Add(StaticData.RootNode);
            }
        }

        /// <summary>
        /// Public property to get the root node of the Game Data tree.
        /// </summary>
        public HETreeNode RootNode { get; private set; } = null;

        /// <summary>
        /// Public property to get the SaveFile sub-object as this is not settable outside
        /// of the GameData class. Set at object creation through the constructor.
        /// </summary>
        public GameSave_Json_File SaveFile { get; private set; } = null;

        /// <summary>
        /// Public property to get the StaticData object as this is not settable outside
        /// of the GameData class. Set at object creation through the constructor.
        /// </summary>
        public Json_FileCollection StaticData { get; private set; } = null;



        /// <summary>
        /// Handles closing this object and sub-objects that support being closed.
        /// </summary>
        /// <returns>Returns true if close was successful.</returns>
        public bool Close()
        {
            bool closeSuccess = true;
            RootNode = null;

            if (SaveFile.Close())
                SaveFile = null;
            else
                closeSuccess = false;

            if (StaticData.Close())
                StaticData = null;
            else
                closeSuccess = false;

            return closeSuccess;
        }

    }
}
