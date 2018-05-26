using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HELLION.DataStructures.Blueprints
{
    public class Blueprint_FileCollection : Json_FileCollection
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="passedParent"></param>
        /// <param name="passedDirectoryInfo"></param>
        /// <param name="passedCollectionType"></param>
        /// <param name="autoPopulateTreeDepth"></param>
        public Blueprint_FileCollection(BlueprintsHandler_UI passedParent, DirectoryInfo passedDirectoryInfo,
             int autoPopulateTreeDepth = 0) : base()
        {
            // Set up the data dictionary
            DataDictionary = new Dictionary<string, StationBlueprint_File>();

            OwnerObject = passedParent ?? throw new NullReferenceException("passedParent was null.");
            DataDirectoryInfo = passedDirectoryInfo ?? throw new NullReferenceException("passedDirectoryInfo was null.");
            if (!DataDirectoryInfo.Exists) throw new DirectoryNotFoundException("DataDirectoryInfo reports the passed folder doesn't exist.");

            RootNode = new Blueprint_FileCollection_TreeNode(passedOwner: this, nodeName: DataDirectoryInfo.Name,
                nodeToolTipText: DataDirectoryInfo.FullName);
                
            if (RootNode == null ) throw new InvalidOperationException("RootNode failed to create.");

            Load(PopulateNodeTreeDepth: autoPopulateTreeDepth);

        }

        /// <summary>
        /// Public property to access the parent object.
        /// </summary>
        public new BlueprintsHandler_UI OwnerObject { get; protected set; } = null;
            
        /// <summary>
        /// The Data Dictionary holds Json_File_UI objects, with the file name as the key.
        /// </summary>
        public new Dictionary<string, StationBlueprint_File> DataDictionary { get; protected set; } = null;

        /// <summary>
        /// The root node of the Blueprint file collection - each data file will have it's
        /// own tree attached as child nodes to this node.
        /// </summary>
        public new Blueprint_FileCollection_TreeNode RootNode { get; set; } = null;

        /// <summary>
        /// The load routine for the static data file collection
        /// </summary>
        /// <param name="PopulateNodeTrees"></param>
        /// <returns></returns>
        public new bool Load(int PopulateNodeTreeDepth = 0)
        {
            // Loads the static data and builds the trees representing the data files
            if (!DataDirectoryInfo.Exists) return false;
            else
            {
                foreach (FileInfo dataFile in DataDirectoryInfo.GetFiles(targetFileExtension).Reverse())
                {
                    Debug.Print("File evaluated {0}", dataFile.Name);

                    // Create a new HEStationonBlueprintFile and populate the path.
                    StationBlueprint_File tempBlueprintFile = new StationBlueprint_File(this, dataFile, PopulateNodeTreeDepth);
                    // Add the file to the Data Dictionary 
                    DataDictionary.Add(dataFile.Name, tempBlueprintFile);

                    if (tempBlueprintFile.IsLoaded && !LoadError)
                    {
                        // if (PopulateNodeTreeDepth > 0) tempBlueprintFile.DataViewRootNode.CreateChildNodesFromjData(PopulateNodeTreeDepth);

                        if (tempBlueprintFile.RootNode == null) throw new NullReferenceException();
                        else RootNode.Nodes.Add(tempBlueprintFile.RootNode);
                    }
                }
                return true;
            }
        }
    }
}
