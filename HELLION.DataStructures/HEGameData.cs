using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HELLION.DataStructures
{
    public class HEGameData
    {
        // Implements the Game Data view node tree
        public HETreeNode RootNode { get; set; }
        public HEJsonGameFile SaveFile { get; set; }
        public HEStaticDataFileCollection StaticData { get; set; }

        public HEGameData(FileInfo SaveFileInfo, DirectoryInfo StaticDataFolderInfo)
        {
            // Constructor that takes a FileInfo representing the save file
            RootNode = new HETreeNode("GAMEDATAVIEW", HETreeNodeType.DataView, "Game Data");
            if (SaveFileInfo != null && SaveFileInfo.Exists)
            {
                SaveFile = new HEJsonGameFile(SaveFileInfo);
                SaveFile.PopulateNodeTree();

                if (SaveFile.RootNode != null)
                {
                    RootNode.Nodes.Add(SaveFile.RootNode);
                }
                else
                    throw new Exception("SaveFile RootNode was null");
            }

            if (StaticDataFolderInfo != null && StaticDataFolderInfo.Exists)
            {
                StaticData = new HEStaticDataFileCollection(StaticDataFolderInfo, autoPopulateTree: true);
                if (StaticData.RootNode != null)
                {
                    RootNode.Nodes.Add(StaticData.RootNode);
                }
                else
                    throw new Exception("StaticData RootNode was null");

            }
        }
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
