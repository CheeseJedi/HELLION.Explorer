using System;
using System.IO;
using HELLION.DataStructures.UI;

namespace HELLION.DataStructures.Blueprints
{
    public class BlueprintsHandler_UI : BlueprintsHandler, IParent_Base_TN
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BlueprintsHandler_UI()
        {
            RootNode = new BlueprintsView_TN(passedOwner: this);
            RootNode.Nodes.Insert(0, StructureDefinitionsFile.RootNode);

            #region Blueprints Collection
            blueprintCollectionFolderInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + @"\HELLION.Explorer\Blueprints") ?? throw new NullReferenceException("blueprintCollectionFolderInfo was null."); ;

            if (!blueprintCollectionFolderInfo.Exists) throw new DirectoryNotFoundException("blueprintCollectionFolderInfo doesn't exist.");

            // Create the object.
            BlueprintCollection = new Blueprint_FileCollection(this, blueprintCollectionFolderInfo, autoPopulateTreeDepth: 8);

            if (BlueprintCollection == null) throw new NullReferenceException("BlueprintCollection was null.");
            if (BlueprintCollection.RootNode == null) throw new NullReferenceException("BlueprintCollection RootNode was null.");
            RootNode.Nodes.Insert(0, BlueprintCollection.RootNode);
            #endregion
        }

        /// <summary>
        /// Public property for the root node of the Search Handler tree.
        /// </summary>
        public BlueprintsView_TN RootNode { get; protected set; } = null;

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
