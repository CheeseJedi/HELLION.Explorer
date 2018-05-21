using System;
using System.IO;

namespace HELLION.DataStructures
{
    /// <summary>
    /// A derived JsonFile class that has a HETreeNode for UI display purposes.
    /// </summary>
    /// <remarks>
    /// Used directly in the HEJsonFileCollection and is also inherited by the HEJsonGameFile class.
    /// This is a re-write intended to encapsulate more of the functionality of building node trees
    /// of the correct type and enabling lazy population of node tree branches.
    /// </remarks>
    public class HEUIJsonFile : HEJsonFile
    {
        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="ownerObject"></param>
        public HEUIJsonFile(object ownerObject) : base(ownerObject)
        {

        }

        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="PassedFileInfo">The FileInfo representing the file to be loaded.</param>
        public HEUIJsonFile(object ownerObject, FileInfo passedFileInfo, int populateNodeTreeDepth) 
            : base(ownerObject, passedFileInfo)
        {
            File = passedFileInfo ?? throw new NullReferenceException();
            RootNode = new HEGameDataTreeNode(ownerObject: this, nodeName: File.Name, newNodeType: HETreeNodeType.DataFile, nodeToolTipText: File.FullName);

            if (!File.Exists) throw new FileNotFoundException();
            else
            {
                LoadFile();
                RootNode.JData = jData;
                RootNode.CreateChildNodesFromjData(populateNodeTreeDepth);
            }
        }

        /// <summary>
        /// Public property for read-only access to the root node of the tree.
        /// </summary>
        /// <remarks>
        /// Casts the RootNode to an HEGameDataTreeNode.
        /// </remarks>
        public HEGameDataTreeNode RootNode { get; protected set; } = null;

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
