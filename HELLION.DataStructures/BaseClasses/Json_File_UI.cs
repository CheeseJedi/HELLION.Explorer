using System.IO;
using HELLION.DataStructures.UI;

namespace HELLION.DataStructures
{
    /// <summary>
    /// A derived Json_File class that has a Base_TN or derived class for UI display purposes.
    /// </summary>
    /// <remarks>
    /// Used directly in the Json_FileCollection and is also inherited by the HEJsonGameFile class.
    /// This is a re-write intended to encapsulate more of the functionality of building node trees
    /// of the correct type and enabling lazy population of node tree branches.
    /// </remarks>
    public class Json_File_UI : Json_File, IParent_Base_TN
    {
        #region Constructors

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="ownerObject"></param>
        public Json_File_UI(IParent_Json_File ownerObject) : base(ownerObject)
        {

        }

        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="PassedFileInfo">The FileInfo representing the file to be loaded.</param>
        public Json_File_UI(IParent_Json_File ownerObject, FileInfo passedFileInfo, int populateNodeTreeDepth) 
            : base(ownerObject, passedFileInfo, autoDeserialise: true)
        {
            RootNode = new Json_TN(ownerObject: this, newNodeType: Base_TN_NodeType.DataFile, nodeName: File.Name);

            if (!File.Exists) throw new FileNotFoundException();
            else
            {
                // Cast the root node as the appropriate type to use it's methods.
                Json_TN tmpNode = (Json_TN)RootNode;
                tmpNode.JData = _jData;
                tmpNode.CreateChildNodesFromjData(populateNodeTreeDepth);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Public property for read-only access to the root node of the tree.
        /// </summary>
        /// <remarks>
        /// Casts the RootNode to an Json_TN.
        /// </remarks>
        public Base_TN RootNode { get; protected set; } = null;

        /// <summary>
        /// Used to determine whether there was an error on load.
        /// </summary>
        public override bool LoadError
        {
            get => base.LoadError;
            protected set
            {
                // Set the load error flag
                base.LoadError = value;
                // Change the node type so that the icon changes to the error type
                RootNode.NodeType = Base_TN_NodeType.DataFileError;
            }
        }

        #endregion

        #region Methods

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
                // Not dirty, OK to close everything
                if (RootNode?.Parent != null) RootNode.Parent.Nodes.Remove(RootNode);

                if (base.Close())
                {
                    RootNode = null;
                    return true;
                }
                return false;
            }
        }

        #endregion

    }
}
