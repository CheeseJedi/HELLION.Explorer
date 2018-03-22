using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HELLION.DataStructures
{
    public class HEBlueprints //: IHENotificationReceiver
    {
        /// <summary>
        /// Public property for the root node of the Search Handler tree.
        /// </summary>
        public HEBlueprintTreeNode RootNode => rootNode;

        /// <summary>
        /// Field for root node of the Game Data tree.
        /// </summary>
        private HEBlueprintTreeNode rootNode = null;

        /// <summary>
        /// Public getter for the Blueprint Collection
        /// </summary>
        public HEJsonFileCollection Blueprintcollection => blueprintCollection;

        /// <summary>
        /// the StaticDataFileCollection object which enumerates the Blueprints folder and builds  
        /// node trees to a preconfigured depth of each of the .json files in that folder.
        /// </summary>
        private HEJsonFileCollection blueprintCollection = null;

        /// <summary>
        /// 
        /// </summary>
        private DirectoryInfo blueprintCollectionFolderInfo = null;

        private FileInfo structureDefinitionsFileInfo = null;

        public HEBlueprintStructureDefinitionsFile StructureDefinitionsFile => structureDefinitionsFile;

        private HEBlueprintStructureDefinitionsFile structureDefinitionsFile = null;

        public HEBlueprints()
        {
            rootNode = new HEBlueprintTreeNode("BLUEPRINTSVIEW", HETreeNodeType.Blueprint, "Blueprints", "Hellion Station Blueprints", passedOwner: this);
            blueprintCollectionFolderInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\HELLION.Explorer\Blueprints");
            structureDefinitionsFileInfo = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\HELLION.Explorer\StructureDefinitions.json");
            Initialise();
        }

        public void Initialise()
        {
            if (blueprintCollectionFolderInfo != null && blueprintCollectionFolderInfo.Exists)
            {
                structureDefinitionsFile = new HEBlueprintStructureDefinitionsFile(this, structureDefinitionsFileInfo, populateNodeTreeDepth: 8);
                if (structureDefinitionsFile.RootNode == null) throw new NullReferenceException("structureDefinitionsFile rootNode was null");
                else RootNode.Nodes.Add(structureDefinitionsFile.RootNode);

                blueprintCollection = new HEBlueprintCollection(this, blueprintCollectionFolderInfo, autoPopulateTreeDepth: 8);
                if (blueprintCollection.RootNode == null) throw new NullReferenceException("StaticData rootNode was null");
                else RootNode.Nodes.Add(blueprintCollection.RootNode);
            }
        }

        /*
        /// <summary>
        /// Implements receiving of simple child-to-parent messages.
        /// </summary>
        /// <param name="sender">The child object that sent the message.</param>
        /// <param name="type">The type of message.</param>
        /// <param name="msg">Message text (optional).</param>
        void IHENotificationReceiver.ReceiveNotification(IHENotificationSender sender, HENotificationType type, string msg)
        {
            Debug.Print("Message received from {0} of type {1} :: {2}", sender.ToString(), type.ToString(), msg);
        }
        */
    }




}
