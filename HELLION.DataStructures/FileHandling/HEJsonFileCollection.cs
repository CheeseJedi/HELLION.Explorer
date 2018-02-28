using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Policy;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines an enum to determine the type of folder - used by the HEJsonFileCollection
    /// </summary>
    public enum HEJsonFileCollectionType
    {
        Unknown = 0,
        StaticDataFolder,
        BlueprintsFolder,
        SnippetsFolder
    }

    /// <summary>
    /// Defines an object that contains a dictionary of HEJsonBaseFiles representing the 
    /// json files in a specified folder.
    /// </summary>
    public class HEJsonFileCollection : IHENotificationReceiver, IHENotificationSender
    {
        /// <summary>
        /// Public property to access the parent object, if set.
        /// </summary>
        public IHENotificationReceiver Parent => parent;

        /// <summary>
        /// Stores a reference to the parent object, if set using the constructor.
        /// </summary>
        protected IHENotificationReceiver parent = null;

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

        /// <summary>
        /// Public property to read the type of the collection.
        /// </summary>
        public HEJsonFileCollectionType CollectionType => collectionType;

        /// <summary>
        /// 
        /// </summary>
        protected HEJsonFileCollectionType collectionType = HEJsonFileCollectionType.Unknown;

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

        /// <summary>
        /// Constructor that takes a DirectoryInfo and if valid, triggers the load
        /// </summary>
        /// <param name="passedDirectoryInfo"></param>
        /// <param name="autoPopulateTree"></param>
        public HEJsonFileCollection(DirectoryInfo passedDirectoryInfo, HEJsonFileCollectionType passedCollectionType, object passedParentNotificationObject, int autoPopulateTreeDepth = 0)
        {

            collectionType = passedCollectionType;

            // Set up the data dictionary
            dataDictionary = new Dictionary<string, HEJsonBaseFile>();

            // Check validity and if good load the data set
            if (passedDirectoryInfo != null && passedDirectoryInfo.Exists)
            {
                if (passedParentNotificationObject == null) throw new InvalidOperationException();
                else parent = (IHENotificationReceiver)passedParentNotificationObject;

                dataDirectoryInfo = passedDirectoryInfo;

                switch (passedCollectionType)
                {
                    case HEJsonFileCollectionType.StaticDataFolder:
                        rootNode = new HETreeNode("DATAFOLDER", HETreeNodeType.DataFolder, nodeText: dataDirectoryInfo.Name, nodeToolTipText: dataDirectoryInfo.FullName);
                        break;

                    case HEJsonFileCollectionType.BlueprintsFolder:
                        rootNode = new HETreeNode("BLUEPRINTSFOLDER", HETreeNodeType.DataFolder, nodeText: "Blueprints", nodeToolTipText: dataDirectoryInfo.FullName);
                        targetFileExtension = "*.hsbp.json";
                        break;

                    default:
                        rootNode = new HETreeNode("UNKNOWN", HETreeNodeType.DataFolderError, nodeText: dataDirectoryInfo.Name, nodeToolTipText: dataDirectoryInfo.FullName);
                        break;
                }
                Load(PopulateNodeTreeDepth: autoPopulateTreeDepth);
            }
            else
            {
                rootNode = new HETreeNode("DATAFOLDER", HETreeNodeType.DataFolderError, nodeText: dataDirectoryInfo.Name + " [ERROR]", nodeToolTipText: dataDirectoryInfo.FullName);
            }
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

                    switch (collectionType)
                    {
                        case HEJsonFileCollectionType.StaticDataFolder:
                            // Create a new HEJsonBaseFile and populate the path.
                            HEJsonBaseFile tempJsonFile = new HEJsonBaseFile(dataFile, this, PopulateNodeTreeDepth);
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
                            break;

                        case HEJsonFileCollectionType.BlueprintsFolder:
                            // Create a new HEJsonBlueprintFile and populate the path.
                            HEJsonBlueprintFile tempBlueprintFile = new HEJsonBlueprintFile(dataFile, this, PopulateNodeTreeDepth);
                            // Add the file to the Data Dictionary 
                            DataDictionary.Add(dataFile.Name, tempBlueprintFile);

                            if (tempBlueprintFile.IsLoaded && !LoadError)
                            {
                                /*
                                if (PopulateNodeTreeDepth > 0)
                                {
                                    tempBlueprintFile.DataViewRootNode.CreateChildNodesFromjData(PopulateNodeTreeDepth);
                                }
                                */
                                if (tempBlueprintFile.RootNode == null) throw new Exception();
                                else RootNode.Nodes.Add(tempBlueprintFile.RootNode);
                            }
                            break;

                        default:

                            break;
                    }
                    
                    
                    /*
                    // Subscribe to the event using C# 2.0 syntax
                    tempFile.RaiseCustomEvent += HandleCustomEvent;
                    */


                        // Add the task to the list so it can be monitored
                        //tasks.Add(t);
                }

                //Task.WaitAll(tasks.ToArray());
                //foreach (Task t in tasks)
                //Debug.Print("Task {0} Status: {1}", t.Id, t.Status);
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
    /*
    //public delegate void HandleCustomEvent<HEJsonBaseFileEventArgs>(object sender, HEJsonBaseFileEventArgs e);

    // Define what actions to take when the event is raised.
    public void HandleCustomEvent(object sender, HEJsonBaseFileEventArgs e)
    {
        Debug.Print("###### " + sender.ToString()+ " sent this message: {0}", e.Message);
    }
    */

    /*
    //Class that subscribes to an event
    class Subscriber
    {
        private string id;
        public Subscriber(string ID, Publisher pub)
        {
            id = ID;
            // Subscribe to the event using C# 2.0 syntax
            pub.RaiseCustomEvent += HandleCustomEvent;
        }

        // Define what actions to take when the event is raised.
        void HandleCustomEvent(object sender, HEJsonBaseFileEventArgs e)
        {
            Console.WriteLine(id + " received this message: {0}", e.Message);
        }
    }
    */

}
