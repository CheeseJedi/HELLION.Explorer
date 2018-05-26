using System;
using System.IO;

namespace HELLION.DataStructures.Blueprints
{
    public class BlueprintsHandler_UI : BlueprintsHandler
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BlueprintsHandler_UI()
        {
            RootNode = new HEBlueprintsViewTreeNode(passedOwner: this);
            RootNode.Nodes.Add(StructureDefinitionsFile.RootNode);

            #region Blueprints Collection
            blueprintCollectionFolderInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + @"\HELLION.Explorer\Blueprints") ?? throw new NullReferenceException("blueprintCollectionFolderInfo was null."); ;

            if (!blueprintCollectionFolderInfo.Exists) throw new DirectoryNotFoundException("blueprintCollectionFolderInfo doesn't exist.");

            // Create the object.
            BlueprintCollection = new Blueprint_FileCollection(this, blueprintCollectionFolderInfo, autoPopulateTreeDepth: 8);

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
        public Blueprint_FileCollection BlueprintCollection { get; protected set; } = null;

        /// <summary>
        /// the DirectoryInfo for the Blueprints Folder.
        /// </summary>
        private DirectoryInfo blueprintCollectionFolderInfo = null;
    }



}
