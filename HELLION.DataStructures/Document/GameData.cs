using System;
using System.Diagnostics;
using System.IO;
using HELLION.DataStructures.UI;
using Newtonsoft.Json.Linq;

namespace HELLION.DataStructures.Document
{
    /// <summary>
    /// Implements the Game Data view node tree, comprised of the sub-trees
    /// of the Save File and each of the .json files in the Data folder (the Static Data).
    /// </summary>
    public class GameData : IParent_Json_File, IParent_Base_TN
    {
        #region Constructors

        /// <summary>
        /// Constructor that takes a FileInfo and a DirectoryInfo representing the save file
        /// and the Data folder.
        /// </summary>
        /// <param name="SaveFileInfo">The FileInfo representing the save file.</param>
        /// <param name="StaticDataFolderInfo">The DirectoryInfo representing the Data folder.</param>
        public GameData(FileInfo SaveFileInfo, DirectoryInfo StaticDataFolderInfo)
        {
            RootNode = new Base_TN(ownerObject: this, nodeType: Base_TN_NodeType.DataView, nodeName: "Game Data");
            if (SaveFileInfo != null && SaveFileInfo.Exists)
            {
                // Create new save file object.
                SaveFile = new GameSave_Json_File(this, SaveFileInfo, populateNodeTreeDepth: 5);

                // Pre-load in several levels of node.
                ((Json_TN)SaveFile.RootNode).CreateChildNodesFromjData(3);

                if (SaveFile.RootNode == null) throw new Exception("SaveFile rootNode was null");
                else RootNode.Nodes.Insert(0, SaveFile.RootNode);
            }

            if (StaticDataFolderInfo != null && StaticDataFolderInfo.Exists)
            {
                StaticData = new Json_FileCollection(this, StaticDataFolderInfo, autoPopulateTreeDepth: 1);
                if (StaticData.RootNode == null) throw new Exception("StaticData rootNode was null");
                else RootNode.Nodes.Insert(0, StaticData.RootNode);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Public property to get the root node of the Game Data tree.
        /// </summary>
        public Base_TN RootNode { get; private set; } = null;

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

        #endregion

        #region Methods


        /// <summary>
        /// Attempts to find a TreeNode in the save file or data files
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        public Json_File FindOwningFile(Json_TN treeNode)
        {

            if (treeNode == null) return null;

            // Check the Save File first.
            Debug.Print("Searching " + SaveFile.File.FullName);

            if (SaveFile.RootNode.AllNodes.Contains(treeNode)) return SaveFile;

            foreach (Json_File_UI file in StaticData.DataDictionary.Values)
            {
                Debug.Print("Searching " + file.File.FullName);
                if (file.RootNode.AllNodes.Contains(treeNode)) return file;

            }


            return null;     //FindOwningFile(token);
        }



        //public Json_File FindOwningFile(JToken token)
        //{
        //    if (token == null) return null;
        //    if (!token.HasValues) return null;

        //    Json_File file = null;

        //    // Search the open save file first.

        //    if (SaveFile.JData != null)
        //    {
        //        if (  SaveFile.JData)
        //    }

        //    return file;
        //}





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

        #endregion
    }
}
