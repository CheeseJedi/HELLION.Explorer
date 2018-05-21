using System;
using System.IO;

namespace HELLION.DataStructures
{
    public class HEBlueprintsHandler
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public HEBlueprintsHandler()
        {
            RootNode = new HEBlueprintsViewTreeNode(passedOwner: this);

            #region Structure Definitions File
            structureDefinitionsFileInfo = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + @"\HELLION.Explorer\StructureDefinitions.json") ?? throw new NullReferenceException("structureDefinitionsFileInfo was null."); ;

            if (!structureDefinitionsFileInfo.Exists) throw new FileNotFoundException("structureDefinitionsFileInfo doesn't exist.");

            // Create the object.
            StructureDefinitionsFile = new HEBlueprintStructureDefinitionsFile(this, structureDefinitionsFileInfo, populateNodeTreeDepth: 8);
            // StructureDefinitionsFile = new HEStationBlueprintFile(this, structureDefinitionsFileInfo, populateNodeTreeDepth: 8);

            if (StructureDefinitionsFile.RootNode == null)
                throw new NullReferenceException("StructureDefinitionsFile rootNode was null.");
            RootNode.Nodes.Add(StructureDefinitionsFile.RootNode);
            #endregion

            #region Blueprints Collection
            blueprintCollectionFolderInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + @"\HELLION.Explorer\Blueprints") ?? throw new NullReferenceException("blueprintCollectionFolderInfo was null."); ;

            if (!blueprintCollectionFolderInfo.Exists) throw new DirectoryNotFoundException("blueprintCollectionFolderInfo doesn't exist.");

            // Create the object.
            BlueprintCollection = new HEBlueprintCollection(this, blueprintCollectionFolderInfo, autoPopulateTreeDepth: 8);

            if (BlueprintCollection == null) throw new NullReferenceException("BlueprintCollection was null.");
            if (BlueprintCollection.RootNode == null) throw new NullReferenceException("BlueprintCollection RootNode was null.");
            RootNode.Nodes.Add(BlueprintCollection.RootNode);
            #endregion

        }

        /// <summary>
        /// Public property for the root node of the Search Handler tree.
        /// </summary>
        public HEBlueprintsViewTreeNode RootNode { get; protected set; } = null;

        /// <summary>
        /// the StaticDataFileCollection object which enumerates the Blueprints folder and builds  
        /// node trees to a preconfigured depth of each of the .json files in that folder.
        /// </summary>
        public HEBlueprintCollection BlueprintCollection { get; protected set; } = null;

        /// <summary>
        /// 
        /// </summary>
        private DirectoryInfo blueprintCollectionFolderInfo = null;

        /// <summary>
        /// 
        /// </summary>
        private FileInfo structureDefinitionsFileInfo = null;

        /// <summary>
        /// The StructureDefinitions.json file used in assembling blueprints.
        /// </summary>
        public HEBlueprintStructureDefinitionsFile StructureDefinitionsFile { get; protected set; } = null;

        //public HEStationBlueprintFile StructureDefinitionsFile { get; protected set; } = null;


    }
}
