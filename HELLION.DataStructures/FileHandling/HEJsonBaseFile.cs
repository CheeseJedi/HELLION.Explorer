using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.HEImageList;
//using System.Runtime.CompilerServices;
//using System.Threading.Tasks;
//using System.Windows.Forms;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines a class to load and hold data from a JSON file and associated metadata.
    /// </summary>
    /// <remarks>
    /// Used directly in the HEJsonFileCollection and is also inherited by the HEJsonGameFile class.
    /// This is a re-write intended to encapsulate more of the functionality of building node trees
    /// of the correct type and enabling lazy population of node tree branches.
    /// </remarks>
    public class HEJsonBaseFile : IHENotificationSender
    {
        /// <summary>
        /// Public property to access the parent object, if set through the constructor.
        /// </summary>
        public IHENotificationReceiver Parent => parent;

        /// <summary>
        /// Public property to get and set the FileInfo object.
        /// </summary>
        public FileInfo File => fileInfo;

        /// <summary>
        /// FileInfo object that represents the file on disk that is to be worked with.
        /// </summary>
        protected FileInfo fileInfo = null;
        
        /// <summary>
        /// Public property to get and set the JToken that was loaded from the file.
        /// </summary>
        public JToken JData
        {
            get
            {
                // Check the file is loaded
                if (!isLoaded) return null;
                // Check there wasn't a load error
                if (LoadError) return null;
                return jData;
            }
            set
            {
                // Nothing special here right now, this will need to be fleshed out
                if (value != null)
                {
                    IsDirty = true;
                    // jData = value;

                    // This is temporary and to detect data changes
                    throw new Exception("Attempted JData change :)");
                }
                else throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Public property for read-only access to the root node of the tree.
        /// </summary>
        /// <remarks>
        /// Casts the RootNode to an HEGameDataTreeNode.
        /// </remarks>
        public HEGameDataTreeNode RootNode => rootNode;

        /// <summary>
        /// Used to determine whether the file is loaded; read only.
        /// </summary>
        public bool IsLoaded => isLoaded;

        /// <summary>
        /// Used to determine whether there was an error on load.
        /// </summary>
        public bool LoadError
        {
            get
            {
                return loadError;
            }
            private set
            {
                if (value)
                {
                    if (!loadError)
                    {
                        // Set the load error flag
                        loadError = true;
                        // Change the node type so that the icon changes to the error type
                        rootNode.NodeType = HETreeNodeType.DataFileError;
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
        /// Determines whether the file is writeable and can attempt to set it to writeable
        /// if the necessary conditions have been met.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                // Check the file is loaded
                if (!isLoaded) return true;
                // Check there wasn't a load error
                if (LoadError) return true;
                // Is the file read-only in the file system?
                if (File.IsReadOnly) return true;
                // None of the above applied, return the value of the override
                return readOnlyOverride;
            }
            set
            {

                // Attempts to set the file state to writeable.
                if (value && isLoaded && !LoadError && !File.IsReadOnly)
                {
                    readOnlyOverride = true;
                }
                else
                {
                    readOnlyOverride = false;
                }
            }
        }
        
        /// <summary>
        /// Used to determine whether the jData object has been modified, and will trigger a prompt to save.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            private set
            {
                if (value)
                {
                    if (!isDirty)
                    {
                        // Set the isDirty flag
                        isDirty = true;
                        
                        /*
                        // Fire the event
                        OnRaiseCustomEvent(new HEJsonBaseFileEventArgs(String.Format("Changes detected in file {0}", File.FullName)));
                        */
                    }
                }
                else
                {
                    isDirty = value;
                }

            }
        }

        /*
        
        /// <summary>
        /// Declare the event handler for notifying the parent object about changes using EventHandler<T>
        /// </summary>
        public event EventHandler<HEJsonBaseFileEventArgs> RaiseCustomEvent;

        /// <summary>
        /// This event invocation is wrapped inside a protected virtual method
        /// to allow derived classes to override the event invocation behaviour
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRaiseCustomEvent(HEJsonBaseFileEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<HEJsonBaseFileEventArgs> handler = RaiseCustomEvent;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                // Format the string to send inside the CustomEventArgs parameter
                e.Message += String.Format(" at {0}", DateTime.Now.ToString());

                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

        */

        /// <summary>
        /// Stores a reference to the parent object, if set using the constructor.
        /// </summary>
        protected IHENotificationReceiver parent = null;

        /// <summary>
        /// The JToken that was loaded from the file, if load was successful.
        /// </summary>
        protected JToken jData = null;

        /// <summary>
        /// The root node of the file - top level will be a node representing the file and
        /// any sub objects will be children of this.
        /// </summary>
        protected HEGameDataTreeNode rootNode = null;

        /// <summary>
        /// Determines whether the file has been loaded.
        /// </summary>
        protected bool isLoaded = false;

        /// <summary>
        /// Tracks whether there was an error encountered during load - only used by the 
        /// LoadError property to prevent re-triggering events.
        /// </summary>
        protected bool loadError = false;

        /// <summary>
        /// This flag is set when the jData is modified - only used by the 
        /// IsDirty property to prevent re-triggering events.
        /// </summary>
        protected bool isDirty = false;

        /// <summary>
        /// Used to determine whether the file is forced to read-only or whether the other
        /// constraints alone determine whether the file can be modified.
        /// </summary>
        protected bool readOnlyOverride = true;

        /// <summary>
        /// Used to activate extended logging to the Debug window in VS.
        /// </summary>
        protected bool logToDebug = false;

        /// <summary>
        /// Default constructor, not used directly but required by the derived class.
        /// </summary>
        public HEJsonBaseFile()
        { }

        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="PassedFileInfo">The FileInfo representing the file to be loaded.</param>
        public HEJsonBaseFile(FileInfo passedFileInfo, object passedParentObject, int populateNodeTreeDepth)
        {
            if (passedParentObject == null) throw new NullReferenceException();
            else parent = (IHENotificationReceiver)passedParentObject;

            if (passedFileInfo == null) throw new NullReferenceException();
            else
            {
                fileInfo = passedFileInfo;
                rootNode = new HEGameDataTreeNode(File.Name, HETreeNodeType.DataFile, nodeToolTipText: File.FullName); // nodeText: File.Name,

                if (!File.Exists) throw new FileNotFoundException();
                else
                {
                    LoadFile();
                    rootNode.Tag = jData;
                    rootNode.CreateChildNodesFromjData(populateNodeTreeDepth);
                }
            }
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
                Debug.Print("@@@ ALERT @@@ " + this.File.Name + " .SendNotification was called but parent was null.");
                //throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Load file data from FileName and parse to the JData JObject of type IOrderedEnumerable<JToken>
        /// </summary>
        /// <returns>Returns true if there was a loading error</returns>
        protected bool LoadFile()
        {           
            if (File.Exists)
            {
                try
                {
                    using (StreamReader sr = File.OpenText())
                    {
                        // Process the stream with the JSON Text Reader in to a JToken
                        using (JsonTextReader jtr = new JsonTextReader(sr))
                        {
                            jData = JToken.ReadFrom(jtr);
                        }
                    }
                }
                catch (Exception e)
                {
                    // Some error handling to be implemented here
                    LoadError = true;
                    if (logToDebug) Debug.Print("Exception caught during StreamReader or JsonTextReader while processing " + File.Name
                        + Environment.NewLine + e);
                }

                if (jData == null)                   
                {
                    // The data didn't load
                    LoadError = true;
                    if (logToDebug) Debug.Print("JData is null or empty: " + File.Name);
                }
                else
                {
                    // We should have some data

                    if (logToDebug)
                    {
                        int numObj = 0;
                        Debug.Print("Token Type: " + jData.Type.ToString());
                        if (jData.Type == JTokenType.Array || jData.Type == JTokenType.Object)
                        {
                            numObj = jData.Count();
                            Debug.Print(File.Name + " loading and is detected as an " + jData.Type.ToString() + ", " + numObj.ToString() + " JTokens loaded.");
                        }
                        else
                        {
                            LoadError = true;
                            Debug.Print("ERROR: JData is detected as neither an ARRAY or OBJECT!");
                        }
                    }
                    // Set the IsLoaded flag to true
                    isLoaded = true;
                }
            }
            else
            {
                // File does not exist.
                LoadError = true;
                throw new FileNotFoundException();

            }

            //OnRaiseCustomEvent(new HEJsonBaseFileEventArgs(String.Format("Loading Complete in file {0}", File.FullName)));

            IHENotificationSender tmp = (IHENotificationSender)this;
            if (LoadError)
            {
                tmp.SendNotification(HENotificationType.FileLoadError, "(" + File.Name + ")");
            }
            else
            {
                tmp.SendNotification(HENotificationType.FileLoadComplete, "(" + File.Name + ")");
            }


            // Return the value of LoadError
            return LoadError;
        }

        /// <summary>
        /// Save the file data.
        /// </summary>
        /// <returns></returns>
        protected bool SaveFile()
        {
            // Will save this file (not yet implemented)

            if (false)
            {
                //
            }
            /*
            // Check to see if this file already exists
            if (MainFile.Exists(FileName))
            {
                // MainFile already exists, create a backup copy (.save.bak)

                // do stuff
            }

            try
            {
                using (StreamWriter sw = new StreamWriter(FileName))
                {
                    // stuff from the load routine for reference only
                    
                    // Read the contents of the file
                    String file = sr.ReadToEnd();
                    // Parse the contents of the file in to the Data JArray
                    JData = JArray.Parse(file);
                }
            }
            catch (IOException)
            {
                // Some error handling to be implemented here
            }

            if (JData == null || JData.Count == 0)
            {
                // The data didn't load
            }
            else
            {
                // We should have some data in the array
                IsDirty = false;
            }

            */
            return false;
        }

        /// <summary>
        /// Handles closing of this file, and de-allocation of it's objects
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
                fileInfo = null;
                jData = null;
                rootNode = null;
                return true;
            }
        }

        /// <summary>
        /// Attempts to build a user-friendly name from available data in a JObject
        /// </summary>
        /// <param name="obj">Takes a JObject and attempts to generate a name from expected fields</param>
        /// <returns></returns>
        public string GenerateDisplayName (JObject obj)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(((string)obj["Registration"] + " " + (string)obj["Name"]).Trim());
            sb.Append((string)obj["GameName"]);
            sb.Append((string)obj["CategoryName"]);
            sb.Append((string)obj["name"]);
            //string[] prefabPathParts = obj["PrefabPath"].ToString().Split('\\');
            //sb.Append(prefabPathParts[prefabPathParts.Length - 1]);
            sb.Append((string)obj["PrefabPath"]);
            sb.Append((string)obj["RuleName"]);
            sb.Append((string)obj["TierName"]);
            sb.Append((string)obj["GroupName"]);
            if (sb.Length > 0) sb.Append(" ");
            sb.Append((string)obj["ItemID"]);
            return sb.ToString() ?? null;
        }

    } // End of class HEjsonFile
}
