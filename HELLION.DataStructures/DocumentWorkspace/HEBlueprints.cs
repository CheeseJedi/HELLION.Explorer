﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HELLION.DataStructures
{
    public class HEBlueprints : IHENotificationReceiver
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
        public HEStaticDataFileCollection Blueprintcollection => blueprintCollection;

        /// <summary>
        /// the StaticDataFileCollection object which enumerates the Blueprints folder and builds  
        /// node trees to a preconfigured depth of each of the .json files in that folder.
        /// </summary>
        private HEStaticDataFileCollection blueprintCollection = null;

        /// <summary>
        /// 
        /// </summary>
        private DirectoryInfo StaticDataFolderInfo = null;

        public HEBlueprints()
        {
            rootNode = new HEBlueprintTreeNode("BLUEPRINTSVIEW", HETreeNodeType.DataFolderError, "Blueprints (not set up)", "You must set a blueprints directory first");


            // var a = new HEStaticDataFileCollection(StaticDataFolderInfo, this, autoPopulateTree: true);

        }







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


    }
}
