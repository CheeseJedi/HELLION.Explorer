using System;
using System.IO;

namespace HELLION.DataStructures.Blueprints
{
    public class BlueprintsHandler
    {
        public BlueprintsHandler()
        {
            #region Structure Definitions File
            structureDefinitionsFileInfo = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                + @"\HELLION.Explorer\StructureDefinitions.json") ?? throw new NullReferenceException("structureDefinitionsFileInfo was null."); ;

            if (!structureDefinitionsFileInfo.Exists) throw new FileNotFoundException("structureDefinitionsFileInfo doesn't exist.");

            // Create the object.
            StructureDefinitionsFile = new StructureDefinitions_File(this, structureDefinitionsFileInfo);
            // StructureDefinitionsFile = new StationBlueprint_File(this, structureDefinitionsFileInfo, populateNodeTreeDepth: 8);

            if (StructureDefinitionsFile.RootNode == null)
                throw new NullReferenceException("StructureDefinitionsFile rootNode was null.");
            #endregion

        }

        /// <summary>
        /// The FileIndo for the StructureDefinitions.json file.
        /// </summary>
        protected FileInfo structureDefinitionsFileInfo = null;

        /// <summary>
        /// The StructureDefinitions.json file used in assembling blueprints.
        /// </summary>
        public StructureDefinitions_File StructureDefinitionsFile { get; protected set; } = null;
    }



}
