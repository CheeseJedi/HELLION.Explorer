using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines an object that contains a dictionary of HEJsonBaseFiles representing the 
    /// json files in a specified folder.
    /// </summary>
    public class HEJsonFileCollection //: IHENotificationReceiver, IHENotificationSender
    {
        
        
        /// <summary>
        /// Public property to access the parent object, if set.
        /// </summary>
        public Object Parent => parent;

        /// <summary>
        /// Stores a reference to the parent object, if set using the constructor.
        /// </summary>
        protected Object parent = null;

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


            IHENotificationSender tmp = (IHENotificationSender)this;
            tmp.SendNotification(type, "Relayed message from " + sender.ToString() + " " + msg);


        }

        /// <summary>
        /// Implements sending of simple child-to-parent messages.
        /// </summary>
        /// <param name="type">The type of message.</param>
        /// <param name="msg">Message text (optional).</param>
        void IHENotificationSender.SendNotification(HENotificationType type, string msg)
        {
            if (Parent != null)
            {
                Parent.ReceiveNotification((IHENotificationSender)this, type, msg);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        */


        /// <summary>
        /// 
        /// </summary>
        protected string targetFileExtension = "*.json";


        /// <summary>
        /// Public property for the data dictionary object.
        /// </summary>
        public Dictionary<string, HEJsonBaseFile> DataDictionary => dataDictionary;

        /// <summary>
        /// The Data Dictionary holds HEJsonBaseFile objects, with the file name as the key.
        /// </summary>
        protected Dictionary<string, HEJsonBaseFile> dataDictionary = null;

        /// <summary>
        /// Public property to access the DirectoryInfo used to build the file collection.
        /// </summary>
        public DirectoryInfo DataDirectoryInfo => dataDirectoryInfo;

        /// <summary>
        /// The DirectoryInfo used to build the file collection.
        /// </summary>
        protected DirectoryInfo dataDirectoryInfo = null;

        /// <summary>
        /// Public property to access the rootNode field
        /// </summary>
        public HETreeNode RootNode => rootNode;

        /// <summary>
        /// The root node of the Static Data file collection - each data file will have it's
        /// own tree attached as child nodes to this node.
        /// </summary>
        protected HETreeNode rootNode = null;

        /// <summary>
        /// Public property to read the isLoaded bool.
        /// </summary>
        public bool IsLoaded => isLoaded;

        /// <summary>
        /// Determines whether the file load is complete.
        /// </summary>
        protected bool isLoaded = false;

        /// <summary>
        /// Public property to read the loadError bool.
        /// </summary>
        public bool LoadError => loadError;

        /// <summary>
        /// Determines whether the file encountered an error while loading.
        /// </summary>
        protected bool loadError = false;

        /// <summary>
        /// Property to read the isDirty field.
        /// </summary>
        public bool IsDirty => isDirty;

        /// <summary>
        /// Used to track whether any of the child objects have been modified.
        /// </summary>
        protected bool isDirty  = false;


        public HEJsonFileCollection()
        {
        }

        /// <summary>
        /// Constructor that takes a DirectoryInfo and if valid, triggers the load
        /// </summary>
        /// <param name="passedDirectoryInfo"></param>
        /// <param name="autoPopulateTree"></param>
        public HEJsonFileCollection(DirectoryInfo passedDirectoryInfo, int autoPopulateTreeDepth = 0)
        {

            // Set up the data dictionary
            dataDictionary = new Dictionary<string, HEJsonBaseFile>();

            // Check validity and if good load the data set
            if (passedDirectoryInfo != null && passedDirectoryInfo.Exists)
            {
                dataDirectoryInfo = passedDirectoryInfo;

                rootNode = new HETreeNode(dataDirectoryInfo.Name, HETreeNodeType.DataFolder, 
                    nodeToolTipText: dataDirectoryInfo.FullName, passedOwner: this);

                Load(PopulateNodeTreeDepth: autoPopulateTreeDepth);
            }
            else
            {
                rootNode = new HETreeNode(dataDirectoryInfo.Name + " [ERROR]", HETreeNodeType.DataFolderError, 
                    nodeToolTipText: dataDirectoryInfo.FullName, passedOwner: this);
            }
        }

        public HEJsonFileCollection(object passedParent, DirectoryInfo passedDirectoryInfo, int autoPopulateTreeDepth = 0) : this(passedDirectoryInfo, autoPopulateTreeDepth)
        {
            parent = passedParent ?? throw new InvalidOperationException();

        }




        /// <summary>
        /// The load routine for the static data file collection
        /// </summary>
        /// <param name="PopulateNodeTrees"></param>
        /// <returns></returns>
        public /*async*/ bool Load(int PopulateNodeTreeDepth = 0)
        {
            // Loads the static data and builds the trees representing the data files
            if (!dataDirectoryInfo.Exists) return false;
            else
            {
                // Set up a list to monitor tasks running asynchronously
                //List<Task> tasks = new List<Task>();

                //HEJsonBaseFile tempFile = null;

                foreach (FileInfo dataFile in dataDirectoryInfo.GetFiles(targetFileExtension).Reverse())
                {
                    Debug.Print("File evaluated {0}", dataFile.Name);

                    
                    // Create a new HEJsonBaseFile and populate the path.
                    HEJsonBaseFile tempJsonFile = new HEJsonBaseFile(this, dataFile, PopulateNodeTreeDepth);
                    // Add the file to the Data Dictionary 
                    DataDictionary.Add(dataFile.Name, tempJsonFile);

                    if (tempJsonFile.IsLoaded && !LoadError)
                    {
                        /*
                        if (PopulateNodeTreeDepth > 0)
                        {
                            tempJsonFile.RootNode.CreateChildNodesFromjData(PopulateNodeTreeDepth);
                        }
                        */
                        if (tempJsonFile.RootNode == null) throw new Exception();
                        else RootNode.Nodes.Add(tempJsonFile.RootNode);
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Handles closing of this collection of files, and de-allocation of it's objects
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (IsDirty)
            {
                return false; // indicates a problem and can't close
            }
            else
            {
                // Not dirty, OK to close everything
                isLoaded = false;
                bool subFileCloseSuccess = true;

                if (DataDictionary != null)
                {
                    foreach (KeyValuePair<string, HEJsonBaseFile> keyValuePair in DataDictionary)
                    {
                        HEJsonBaseFile jsonBaseFile = keyValuePair.Value;

                        if (jsonBaseFile != null)
                        {
                            bool resultOk = jsonBaseFile.Close();
                            if (!resultOk)
                            {
                                subFileCloseSuccess = false;
                            }
                            else
                            {
                                jsonBaseFile = null;
                                // remove the jsonBaseFile from the list
                                //DataDictionary.Remove(keyValuePair.Key);
                            }
                        }
                    }
                }
                else
                    dataDictionary = null;

                dataDirectoryInfo = null;
                rootNode = null;
                return subFileCloseSuccess;
            }
        }
    }

}
