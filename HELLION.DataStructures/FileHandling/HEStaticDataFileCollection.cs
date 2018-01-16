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
    /// Defines a class to hold a dictionary of HEJsonBaseFiles representing the Static Data
    /// </summary>
    public class HEStaticDataFileCollection : IHENotificationReceiver, IHENotificationSender
    {
        /// <summary>
        /// Public property to access the parent object, if set.
        /// </summary>
        public IHENotificationReceiver Parent { get { return parent; } }

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
        /// the Static Data Dictionary - HEJsonBaseFile objects, with the file name as the key.
        /// </summary>
        public Dictionary<string, HEJsonBaseFile> DataDictionary { get; set; } = null;

        /// <summary>
        /// The DirectoryInfo representing the Static Data directory.
        /// </summary>
        public DirectoryInfo StaticDataDirectoryInfo { get; set; } = null;// The object representing the static Data folder

        /// <summary>
        /// The root node of the Static Data file collection - each data file will have it's
        /// own tree attached as child nodes to this node.
        /// </summary>
        public HETreeNode RootNode { get; set; } = null;

        /// <summary>
        /// Determines whether the file load is complete.
        /// </summary>
        public bool IsLoaded { get; private set; } = false;

        /// <summary>
        /// Determines whether the file encountered an error while loading.
        /// </summary>
        public bool LoadError { get; private set; } = false;

        /// <summary>
        /// Determines whether the JData object has been modified - is used to alert the user
        /// if there are unsaved changes.
        /// </summary>
        public bool IsDirty { get; set; } = false;

        /// <summary>
        /// Constructor that takes a DirectoryInfo and if valid, triggers the load
        /// </summary>
        /// <param name="passedDirectoryInfo"></param>
        /// <param name="autoPopulateTree"></param>
        public HEStaticDataFileCollection(DirectoryInfo passedDirectoryInfo, object passedParentObject, bool autoPopulateTree = false)
        {
            // 
            DataDictionary = new Dictionary<string, HEJsonBaseFile>();

            // Check validity and if good load the data set
            if (passedDirectoryInfo != null && passedDirectoryInfo.Exists)
            {

                if (passedParentObject != null)
                {
                    parent = (IHENotificationReceiver)passedParentObject;

                }
                else
                {
                    throw new InvalidOperationException();
                }


                StaticDataDirectoryInfo = passedDirectoryInfo;
                RootNode = new HETreeNode("DATAFOLDER", HETreeNodeType.DataFolder, nodeText: StaticDataDirectoryInfo.Name, nodeToolTipText: StaticDataDirectoryInfo.FullName);

                Load(PopulateNodeTrees: autoPopulateTree);
            }
            else
            {
                RootNode = new HETreeNode("DATAFOLDER", HETreeNodeType.DataFolderError, nodeText: StaticDataDirectoryInfo.Name + " [ERROR]", nodeToolTipText: StaticDataDirectoryInfo.FullName);
            }
        }

        /// <summary>
        /// The load routine for the static data file collection
        /// </summary>
        /// <param name="PopulateNodeTrees"></param>
        /// <returns></returns>
        public /*async*/ bool Load(bool PopulateNodeTrees = false)
        {
            // Loads the static data and builds the trees representing the data files
            if (StaticDataDirectoryInfo.Exists)
            {
                //RootNode = new HETreeNode("DATAFOLDER", HETreeNodeType.DataFolder, nodeText: StaticDataDirectoryInfo.Name, nodeToolTipText: StaticDataDirectoryInfo.FullName);

                // Set up a list to monitor tasks running asynchronously
                //List<Task> tasks = new List<Task>();

                foreach (FileInfo dataFile in StaticDataDirectoryInfo.GetFiles("*.json").Reverse())
                {
                    Debug.Print("File evaluated {0}", dataFile.Name);

                    // Create a new HEJsonBaseFile, populate the path and check validity before creating a new task

                    //IHENotificationReceiver tmp = this;

                    HEJsonBaseFile tempFile = new HEJsonBaseFile(dataFile, this);

                    /*
                    // Subscribe to the event using C# 2.0 syntax
                    tempFile.RaiseCustomEvent += HandleCustomEvent;
                    */

                    // Add the file to the DataDictionary List
                    DataDictionary.Add(dataFile.Name, tempFile);

                    if (tempFile.IsLoaded && !LoadError)
                    {
                        // Create and run new task to build the node tree asynchronously
                        //Task t = Task.Run(() => 

                        if (PopulateNodeTrees)
                        {
                            // Populate child nodes to a depth of 1 (the default)
                            tempFile.RootNode.CreateChildNodesFromjData();
                        }

                        if (tempFile.RootNode != null)
                            RootNode.Nodes.Add(tempFile.RootNode);
                        else
                            throw new Exception();

                        //HETreeNode tn = tempFile.BuildHETreeNodeTreeFromJson(tempFile.JData, maxDepth:10, collapseJArrays: false);
                        //if (tn != null)
                        {
                            //tempNode.Nodes.Add(tn);
                        }

                        // Add the task to the list so it can be monitored
                        //tasks.Add(t);
                    }
                    //else
                    //throw new Exception();


                    // Add tree node representing this file
                    //RootNode.Nodes.Add(tempNode);

                }

                //Task.WaitAll(tasks.ToArray());
                //foreach (Task t in tasks)
                //Debug.Print("Task {0} Status: {1}", t.Id, t.Status);
                return true;

            }
            else
            {
                return false;
                throw new Exception();
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
                IsLoaded = false;
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
                    DataDictionary = null;

                StaticDataDirectoryInfo = null;
                RootNode = null;
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
