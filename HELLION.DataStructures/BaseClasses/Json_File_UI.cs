﻿using System;
using System.IO;
using HELLION.DataStructures.Document;
using HELLION.DataStructures.UI;

namespace HELLION.DataStructures
{
    /// <summary>
    /// A derived Json_File class that has a HETreeNode for UI display purposes.
    /// </summary>
    /// <remarks>
    /// Used directly in the Json_FileCollection and is also inherited by the HEJsonGameFile class.
    /// This is a re-write intended to encapsulate more of the functionality of building node trees
    /// of the correct type and enabling lazy population of node tree branches.
    /// </remarks>
    public class Json_File_UI : Json_File, Iparent_Base_TN
    {
        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="ownerObject"></param>
        public Json_File_UI(Json_File_Parent ownerObject) : base(ownerObject)
        {

        }

        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="PassedFileInfo">The FileInfo representing the file to be loaded.</param>
        public Json_File_UI(Json_File_Parent ownerObject, FileInfo passedFileInfo, int populateNodeTreeDepth) 
            : base(ownerObject, passedFileInfo)
        {
            File = passedFileInfo ?? throw new NullReferenceException();
            RootNode = new Json_TreeNode(ownerObject: this, nodeName: File.Name, newNodeType: HETreeNodeType.DataFile);
                //, nodeToolTipText: File.FullName);

            if (!File.Exists) throw new FileNotFoundException();
            else
            {
                LoadFile();

                // Cast the root node as the appropriate type to use it's methods.
                Json_TreeNode tmpNode = (Json_TreeNode)RootNode;
                tmpNode.JData = jData;
                tmpNode.CreateChildNodesFromjData(populateNodeTreeDepth);
            }
        }

        /// <summary>
        /// Public property for read-only access to the root node of the tree.
        /// </summary>
        /// <remarks>
        /// Casts the RootNode to an Json_TreeNode.
        /// </remarks>
        public Base_TN RootNode { get; set; } = null;

        /// <summary>
        /// Used to determine whether there was an error on load.
        /// </summary>
        public override bool LoadError
        {
            get
            {
                return loadError;
            }
            protected set
            {
                if (value)
                {
                    if (!loadError)
                    {
                        // Set the load error flag
                        loadError = true;
                        // Change the node type so that the icon changes to the error type
                        RootNode.NodeType = HETreeNodeType.DataFileError;
                        /*
                        // Fire the event
                        OnRaiseCustomEvent(new HEJsonBaseFileEventArgs(String.Format("Load Error in file {0}", File.FullName)));
                        */
                    }
                }
                else
                {
                    loadError = value;
                }
            }
        }
        
        /// <summary>
        /// Handles closing of this file, and de-allocation of it's objects
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            if (IsDirty)
            {
                return false; // indicates a problem and can't close
            }
            else
            {
                if (!base.Close()) return false;

                // Not dirty, OK to close everything
                RootNode = null;
                return true;
            }
        }

    }
}
