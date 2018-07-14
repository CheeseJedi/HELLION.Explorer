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
            if (RootNode == null) ReGenerateRootNode();
            RootNode.OwnerObject = this;

        }

        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="PassedFileInfo">The FileInfo representing the file to be loaded.</param>
        public Json_File_UI(IParent_Json_File ownerObject, FileInfo passedFileInfo, int populateDepth = 1) 
            : base(ownerObject, passedFileInfo, autoDeserialise: false)
        {
            // Set the auto-population depth.
            _populateDepth = populateDepth;

            // Create the root node for the file
            if (RootNode == null) ReGenerateRootNode();
            else RootNode.Name = File.Name;
            RootNode.OwnerObject = this;

        }


        #endregion

        #region Properties

        /// <summary>
        /// Public property for read-only access to the root node of the tree.
        /// </summary>
        /// <remarks>
        /// RootNode is defined as a Base_TN however is likely to actually be a derived class.
        /// </remarks>
        public Base_TN RootNode
        {
            get
            {
                if (_rootNode == null)
                {
                    ReGenerateRootNode();
                }
                return _rootNode;
            }
            protected set => _rootNode = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public Json_TN JsonRootNode { get; protected set; } = null;

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
                if (RootNode != null && value) RootNode.NodeType = Base_TN_NodeType.DataFileError;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles changes to the JData object and additionally triggers the Json node
        /// tree to regenerate.
        /// </summary>
        protected override void ProcessChangedJData()
        {
            // Call the base class function.
            base.ProcessChangedJData();

            // Trigger regeneration of nodes based on the new jdata.
            if (JData != null) RegenerateJsonNodeTree();

        }

        protected void ReGenerateRootNode(string name = null)
        {
            string nodeName = name == null ? (File != null ? File.Name : "unknown") : name;
            RootNode = new Base_TN(this, Base_TN_NodeType.DataFile, name);

        }

        protected void RegenerateJsonNodeTree()
        {



            // Create the root node for the Json tree and feed in the loaded JData.
            JsonRootNode = new Json_TN(this, JData, File.Name, _populateDepth);

            if (JsonRootNode != null && RootNode != null) RootNode.Nodes.Add(JsonRootNode);
                        
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

        //public override void Deserialise()
        //{
        //    Debug.Print("Json_File_UI.Deserialise() called - AND SHOULDN'T HAVE!");
        //}

        //public override void Serialise()
        //{
        //    Debug.Print("Json_File_UI.Serialise() called - AND SHOULDN'T HAVE!");
        //}

        #endregion

        #region Fields

        private int _populateDepth;
        private Base_TN _rootNode;

        #endregion
    }
}
