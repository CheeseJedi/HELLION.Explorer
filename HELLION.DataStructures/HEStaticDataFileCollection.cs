using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HELLION.DataStructures
{
    public class HEStaticDataFileCollection
    {
        // Defines a class to hold a dictionary of HEJsonBaseFiles representing the Static Data
        public Dictionary<string, HEJsonBaseFile> DataDictionary { get; set; } = null;// The collection of static data files
        public DirectoryInfo StaticDataDirectoryInfo { get; set; } = null;// The object representing the static Data folder
        public HETreeNode RootNode { get; set; } = null;
        public bool IsLoaded { get; private set; } = false;
        public bool LoadError { get; private set; } = false;
        public bool IsDirty { get; set; } = false;
        /*
        public HEStaticDataFileCollection()
        {
            // Basic constructor
            DataDictionary = null;
            StaticDataDirectoryInfo = null;
            IsLoaded = false;
            RootNode = new HETreeNode("DATAFOLDER", HETreeNodeType.DataFolderError, "Data Folder");
            
        }
        */
        public HEStaticDataFileCollection(DirectoryInfo passedDirectoryInfo, bool autoPopulateTree = false)
        {
            // Constructor that takes a DirectoryInfo and loads
            DataDictionary = new Dictionary<string, HEJsonBaseFile>();

            // Check validity and if good load the data set
            if (passedDirectoryInfo != null && passedDirectoryInfo.Exists)
            {
                StaticDataDirectoryInfo = passedDirectoryInfo;
                RootNode = new HETreeNode("DATAFOLDER", HETreeNodeType.DataFolder, nodeText: StaticDataDirectoryInfo.Name, nodeToolTipText: StaticDataDirectoryInfo.FullName);

                Load(PopulateNodeTrees: autoPopulateTree);
            }
            else
            {
                RootNode = new HETreeNode("DATAFOLDER", HETreeNodeType.DataFolderError, nodeText: StaticDataDirectoryInfo.Name + " [ERROR]", nodeToolTipText: StaticDataDirectoryInfo.FullName);
            }
        }

        public /*async*/ bool Load(bool PopulateNodeTrees = false)
        {
            // Loads the static data and builds the trees representing the data files
            if (StaticDataDirectoryInfo.Exists)
            {
                //RootNode = new HETreeNode("DATAFOLDER", HETreeNodeType.DataFolder, nodeText: StaticDataDirectoryInfo.Name, nodeToolTipText: StaticDataDirectoryInfo.FullName);

                // Set up a list to monitor tasks running asynchronously
                //List<Task> tasks = new List<Task>();

                foreach (FileInfo dataFile in StaticDataDirectoryInfo.GetFiles("*.json").Reverse())
                {
                    Debug.Print("File evaluated {0}", dataFile.Name);

                    // Create a new HEJsonBaseFile, populate the path and check validity before creating a new task
                    HEJsonBaseFile tempFile = new HEJsonBaseFile(dataFile);
                    // Add the file to the DataDictionary List
                    DataDictionary.Add(dataFile.Name, tempFile);

                    if (tempFile.IsLoaded && !LoadError)
                    {
                        // Create and run new task to build the node tree asynchronously
                        //Task t = Task.Run(() => 

                        if (PopulateNodeTrees)
                            tempFile.PopulateNodeTree();

                        if (tempFile.RootNode != null)
                            RootNode.Nodes.Add(tempFile.RootNode);
                        else
                            throw new Exception();

                        //HETreeNode tn = tempFile.BuildHETreeNodeTreeFromJson(tempFile.JData, maxDepth:10, collapseJArrays: false);
                        //if (tn != null)
                        {
                            //tempNode.Nodes.Add(tn);
                        }

                        // Add the task to the list so it can be monitored
                        //tasks.Add(t);
                    }
                    else
                        throw new Exception();


                    // Add tree node representing this file
                    //RootNode.Nodes.Add(tempNode);

                }

                //Task.WaitAll(tasks.ToArray());
                //foreach (Task t in tasks)
                //Debug.Print("Task {0} Status: {1}", t.Id, t.Status);
                return true;

            }
            else
            {
                return false;
                throw new Exception();
            }


        }

        public bool Close()
        {
            // Handles closing of this collection of files, and de-allocation of it's objects

            if (IsDirty)
            {
                return false; // indicates a problem and can't close
            }
            else
            {
                // Not dirty, ok to close everything
                IsLoaded = false;
                bool subFileCloseSuccess = true;

                if (DataDictionary != null)
                {
                    foreach (KeyValuePair<string, HEJsonBaseFile> keyValuePair in DataDictionary)
                    {
                        HEJsonBaseFile jsonBaseFile = keyValuePair.Value;

                        if (jsonBaseFile != null)
                        {
                            bool resultOk = jsonBaseFile.Close();
                            if (!resultOk)
                            {
                                subFileCloseSuccess = false;
                            }
                            else
                            {
                                jsonBaseFile = null;
                                // remove the jsonBaseFile from the list
                                //DataDictionary.Remove(keyValuePair.Key);
                            }
                        }
                    }
                }
                else
                    DataDictionary = null;

                StaticDataDirectoryInfo = null;
                RootNode = null;
                return subFileCloseSuccess;
            }
        }


    }
}
