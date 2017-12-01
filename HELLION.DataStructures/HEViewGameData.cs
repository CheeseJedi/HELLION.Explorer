using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HELLION.DataStructures
{
    public class HEViewGameData
    {
        // Implements the Game Data view node tree
        public HETreeNode RootNode { get; set; }
        public HEJsonBaseFile SaveFile { get; set; }
        public HEStaticDataFileCollection StaticData { get; set; }

        public HEViewGameData(FileInfo SaveFileInfo, DirectoryInfo StaticDataFolderInfo)
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
                    MessageBox.Show("prob here");
            }

            if (StaticDataFolderInfo != null && StaticDataFolderInfo.Exists)
            {
                StaticData = new HEStaticDataFileCollection(StaticDataFolderInfo);
                if (StaticData.RootNode != null)
                {
                    RootNode.Nodes.Add(StaticData.RootNode);
                }
                else
                    MessageBox.Show("prob here 2");

            }


        }



    }
}
