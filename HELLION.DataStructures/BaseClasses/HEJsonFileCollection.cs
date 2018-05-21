using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines an object that contains a dictionary of HEJsonBaseFiles representing the 
    /// json files in a specified folder.
    /// </summary>
    public class HEJsonFileCollection
    {
        public HEJsonFileCollection()
        {

        }

        /// <summary>
        /// Constructor that takes a DirectoryInfo and if valid, triggers the load
        /// </summary>
        /// <param name="passedDirectoryInfo"></param>
        /// <param name="autoPopulateTree"></param>
        public HEJsonFileCollection(HEGameData passedParent, DirectoryInfo passedDirectoryInfo, int autoPopulateTreeDepth = 0)
        {

            // Set up the data dictionary
            DataDictionary = new Dictionary<string, HEUIJsonFile>();

            // Check validity - if good load the data set
            OwnerObject = passedParent ?? throw new InvalidOperationException("passedParent was null.");
            DataDirectoryInfo = passedDirectoryInfo ?? throw new NullReferenceException("passedDirectoryInfo was null.");
            if (!DataDirectoryInfo.Exists) throw new DirectoryNotFoundException("DataDirectoryInfo reports the passed folder doesn't exist.");

            RootNode = new HETreeNode(ownerObject: this, nodeName: DataDirectoryInfo.Name,
                newNodeType: HETreeNodeType.DataFolder, nodeToolTipText: DataDirectoryInfo.FullName);

            Load(PopulateNodeTreeDepth: autoPopulateTreeDepth);
        }

        /// <summary>
        /// A reference to the 'Owning' object.
        /// </summary>
        public HEGameData OwnerObject { get; protected set; } = null;
            
        /// <summary>
        /// The Data Dictionary holds HEUIJsonFile objects, with the file name as the key.
        /// </summary>
        public Dictionary<string, HEUIJsonFile> DataDictionary { get; protected set; } = null;

        /// <summary>
        /// Public property to access the DirectoryInfo used to build the file collection.
        /// </summary>
        public DirectoryInfo DataDirectoryInfo { get; protected set; } = null;

        /// <summary>
        /// The root node of the Static Data file collection - each data file will have it's
        /// own tree attached as child nodes to this node.
        /// </summary>
        public HETreeNode RootNode { get; set; } = null;

        /// <summary>
        /// Public property to read the isLoaded bool.
        /// </summary>
        public bool IsLoaded { get; protected set; } = false;

        /// <summary>
        /// Determines whether the file encountered an error while loading.
        /// </summary>
        public bool LoadError { get; protected set; } = false;

        /// <summary>
        /// Property to read the isDirty field.
        /// </summary>
        public bool IsDirty { get; protected set; } = false;

        /// <summary>
        /// Specifies the target file extension for included files - default on *.json.
        /// </summary>
        protected string targetFileExtension = "*.json";

        /// <summary>
        /// The load routine for the static data file collection
        /// </summary>
        /// <param name="PopulateNodeTrees"></param>
        /// <returns></returns>
        public bool Load(int PopulateNodeTreeDepth = 0)
        {
            // Loads the static data and builds the trees representing the data files
            if (!DataDirectoryInfo.Exists) return false;
            else
            {
                // Set up a list to monitor tasks running asynchronously
                //List<Task> tasks = new List<Task>();

                //HEUIJsonFile tempFile = null;

                foreach (FileInfo dataFile in DataDirectoryInfo.GetFiles(targetFileExtension).Reverse())
                {
                    Debug.Print("File evaluated {0}", dataFile.Name);

                    
                    // Create a new HEUIJsonFile and populate the path.
                    HEUIJsonFile tempJsonFile = new HEUIJsonFile(this, dataFile, PopulateNodeTreeDepth);
                    // Add the file to the Data Dictionary 
                    DataDictionary.Add(dataFile.Name, tempJsonFile);

                    if (tempJsonFile.IsLoaded && !LoadError)
                    {
                        /*
                        if (PopulateNodeTreeDepth > 0)
                        {
                            tempJsonFile.RootNode.CreateChildNodesFromjData(PopulateNodeTreeDepth);
                        }
                        */
                        if (tempJsonFile.RootNode == null) throw new Exception();
                        else RootNode.Nodes.Add(tempJsonFile.RootNode);
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Handles closing of this collection of files, and de-allocation of it's objects
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (IsDirty)
            {
                return false; // indicates a problem and can't close
            }
            else
            {
                // Not dirty, OK to close everything
                IsLoaded = false;
                bool subFileCloseSuccess = true;

                if (DataDictionary != null)
                {
                    foreach (KeyValuePair<string, HEUIJsonFile> keyValuePair in DataDictionary)
                    {
                        HEUIJsonFile jsonBaseFile = keyValuePair.Value;

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

                DataDirectoryInfo = null;
                RootNode = null;
                return subFileCloseSuccess;
            }
        }
    }

}
