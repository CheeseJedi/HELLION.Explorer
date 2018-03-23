using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HELLION.DataStructures
{
    public class HEBlueprintCollection : HEJsonFileCollection
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="passedParent"></param>
        /// <param name="passedDirectoryInfo"></param>
        /// <param name="passedCollectionType"></param>
        /// <param name="autoPopulateTreeDepth"></param>
        public HEBlueprintCollection(HEBlueprints passedParent, DirectoryInfo passedDirectoryInfo,
             int autoPopulateTreeDepth = 0)
        {
            // Set up the data dictionary
            dataDictionary = new Dictionary<string, HEJsonBlueprintFile>();

            parent = passedParent ?? throw new InvalidOperationException("passedParent was null.");

            // Check validity and if good load the data set
            if (passedDirectoryInfo != null && passedDirectoryInfo.Exists)
            {
                dataDirectoryInfo = passedDirectoryInfo;

                rootNode = new HETreeNode(dataDirectoryInfo.Name, HETreeNodeType.DataFolder,
                    nodeToolTipText: dataDirectoryInfo.FullName, passedOwner: this);

                Load(PopulateNodeTreeDepth: autoPopulateTreeDepth);

            }
        }

        /// <summary>
        /// Public property to access the parent object.
        /// </summary>
        public new HEBlueprints Parent => parent;

        /// <summary>
        /// Stores a reference to the parent object.
        /// </summary>
        protected new HEBlueprints parent = null;

        /// <summary>
        /// Public property for the data dictionary object.
        /// </summary>
        public new Dictionary<string, HEJsonBlueprintFile> DataDictionary => dataDictionary;

        /// <summary>
        /// The Data Dictionary holds HEJsonBaseFile objects, with the file name as the key.
        /// </summary>
        protected new Dictionary<string, HEJsonBlueprintFile> dataDictionary = null;



        /// <summary>
        /// The load routine for the static data file collection
        /// </summary>
        /// <param name="PopulateNodeTrees"></param>
        /// <returns></returns>
        public new bool Load(int PopulateNodeTreeDepth = 0)
        {
            // Loads the static data and builds the trees representing the data files
            if (!dataDirectoryInfo.Exists) return false;
            else
            {
                foreach (FileInfo dataFile in dataDirectoryInfo.GetFiles(targetFileExtension).Reverse())
                {
                    Debug.Print("File evaluated {0}", dataFile.Name);

                    // Create a new HEJsonBlueprintFile and populate the path.
                    HEJsonBlueprintFile tempBlueprintFile = new HEJsonBlueprintFile(this, dataFile, PopulateNodeTreeDepth);
                    // Add the file to the Data Dictionary 
                    DataDictionary.Add(dataFile.Name, tempBlueprintFile);

                    if (tempBlueprintFile.IsLoaded && !LoadError)
                    {
                        
                        // if (PopulateNodeTreeDepth > 0) tempBlueprintFile.DataViewRootNode.CreateChildNodesFromjData(PopulateNodeTreeDepth);

                        if (tempBlueprintFile.RootNode == null) throw new Exception();
                        else RootNode.Nodes.Add(tempBlueprintFile.RootNode);
                    }
                }

                return true;
            }
        }
    }
}
