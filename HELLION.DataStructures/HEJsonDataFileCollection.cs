using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HELLION.DataStructures
{
    public class HEJsonDataFileCollection
    {
        // Defines a class to hold a collection of HEJsonBaseFiles representing the Static Data
        public IEnumerable<HEJsonBaseFile> StaticData { get; set; } // The collection of static data files
        public DirectoryInfo StaticDataFolder { get; set; } // The object representing the static Data folder
        public bool IsCollectionLoaded { get; private set; }
        public bool LoadError { get; private set; }
        public HETreeNode CollectionRoot { get; set; }

        public HEJsonDataFileCollection()
        {
            // Basic constructor
            StaticData = null;
            StaticDataFolder = null;
            IsCollectionLoaded = false;
            CollectionRoot = new HETreeNode("DATAFOLDER", HETreeNodeType.DataFolderError, "Data Folder");
            
        }
        public HEJsonDataFileCollection(string passedFolderName)
        {
            // Constructor that takes a folder name
            StaticData = null;
            StaticDataFolder = null;
            IsCollectionLoaded = false;
            CollectionRoot = new HETreeNode("DATAFOLDER", HETreeNodeType.DataFolder, "Data Folder");

            // Check validity and if good load the data set
            if (passedFolderName != "")
            {
                StaticDataFolder = new DirectoryInfo(passedFolderName);

                if (StaticDataFolder.Exists)
                    LoadStaticData();
            }
        }

        public /*async*/ void LoadStaticData()
        {
            //
            if (StaticDataFolder.Exists)
            {
                // Set up a list to monitor tasks running asynchronously
                List<Task> tasks = new List<Task>();




                foreach (FileInfo dataFile in StaticDataFolder.GetFiles("*.json").Reverse())
                {
                    Debug.Print("File evaluated {0}", dataFile.Name);

                    HETreeNode tempNode = new HETreeNode("DATAFILE", HETreeNodeType.DataFile, nodeText: dataFile.Name, nodeToolTipText: dataFile.FullName);

                    // Create a new HEJsonBaseFile, populate the path and check validity before creating a new task
                    HEJsonBaseFile tempFile = new HEJsonBaseFile(dataFile);
                    if (tempFile.IsFileLoaded && !LoadError)
                    {
                        // Create and run new task to build the node tree asynchronously
                        Task t = Task.Run(() => tempFile.BuildBasicNodeTreeFromJson(tempFile.JData, tempNode, maxDepth:1));
                        // Add the task to the list so it can be monitored
                        tasks.Add(t);
                    }



                    // Add tree node representing this file
                    CollectionRoot.Nodes.Add(tempNode);

                }

                Task.WaitAll(tasks.ToArray());
                foreach (Task t in tasks)
                    Debug.Print("Task {0} Status: {1}", t.Id, t.Status);

            }
        }



    }
}
