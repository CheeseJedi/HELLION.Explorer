using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
//using System.Runtime.CompilerServices;
using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.HEImageList;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines a class to load and hold data from a JSON file and associated metadata.
    /// </summary>
    /// <remarks>
    /// Used directly in the HEStaticDataFileCollection and is also inherited by the HEJsonGameFile class.
    /// This is a re-write intended to encapsulate more of the functionality of building node trees
    /// of the correct type and enabling lazy population of node tree branches.
    /// </remarks>
    public class HEJsonBaseFile : IHENotificationSender
    {
        /// <summary>
        /// Public property to access the parent object, if set through the constructor.
        /// </summary>
        public IHENotificationReceiver Parent { get { return parent; } }
        
        /// <summary>
        /// Public property to get and set the FileInfo object that represents
        /// the file to be worked with.
        /// </summary>
        public FileInfo File { get; set; } = null;
        
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
                    // This is temporary and to detect data changes
                    throw new Exception("Attempted JData change :)");

                    IsDirty = true;
                    // jData = value;
                }
                else throw new InvalidOperationException();
            }
        }
        
        /// <summary>
        /// Public property for read-only access to the root node of the tree.
        /// </summary>
        public HETreeNode RootNode { get { return rootNode; } }
        
        /// <summary>
        /// Used to determine whether the file is loaded; read only.
        /// </summary>
        public bool IsLoaded { get { return isLoaded; } }

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
        protected HETreeNode rootNode = null;

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
        /// Default constructor, not used but required by the derived class for some reason.
        /// </summary>
        public HEJsonBaseFile()
        { }

        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="PassedFileInfo">The FileInfo representing the file to be loaded.</param>
        public HEJsonBaseFile(FileInfo passedFileInfo, object passedParentObject)
        {
            if (passedParentObject != null)
            {
                parent = (IHENotificationReceiver)passedParentObject;

            }
            else
            {
                throw new InvalidOperationException();
            }

            if (passedFileInfo != null)
            {
                File = passedFileInfo;
                if (File.Exists)
                {

                    rootNode = new HETreeNode("DATAFILE", HETreeNodeType.DataFile, nodeText: File.Name, nodeToolTipText: File.FullName);
                    LoadFile();
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            else
            {
                throw new InvalidOperationException();
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
                            jData  = JToken.ReadFrom(jtr);
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
        } // End of LoadFile()

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
        } // End of SaveFile()

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
                File = null;
                jData = null;
                rootNode = null;
                return true;
            }
        }

        /*
        public HETreeNode BuildNodeCollection(HETreeNodeType dummy, [CallerMemberName] string callerName = "")
        {
            // Builds and returns a HETreeNode whose Node Collection contains HETreeNode type nodes
            // based on the data stored in this JSON file - only for use on Data files currently
            // Returns null instead of an empty collection
        

            if (logToDebug)
            {
                foreach (var method in new StackTrace().GetFrames())
                {
                    if (method.GetMethod().Name == callerName)
                    {
                        callerName = $"{method.GetMethod().ReflectedType.Name}.{callerName}";
                        break;
                    }
                }
                Debug.Print("INFO: HEJsonFile.BuildNodeCollection in was called by " + callerName); // + " with type: " + nodeType.ToString());
            }
            // Define an HETreeNode node to serve as the parent for this collection of data objects
            HETreeNode nodeRoot = new HETreeNode();
            nodeRoot.Name = nodeRoot.Text = File.Name;

            HETreeNodeType typeOfNode = HETreeNodeType.ExpansionAvailable;

            if (IsLoaded && !LoadError)
            {

                int iImageIndex = 0;

                if (logToDebug)
                    Debug.Print("JData type: " + JData.GetType());

                foreach (JToken dataItem in JData.Reverse()) // seems to come in backwards, hence the .Reverse()
                {

                    string PropertyName;
                    JToken PropertyValue;

                    if (dataItem.Type == JTokenType.Property)
                    {
                        // Evaluate the Property
                        typeOfNode = HETreeNodeType.JsonProperty;

                        // Get the index of the image associated with this node type
                        iImageIndex = HEUtilities.GetImageIndexByNodeType(typeOfNode);

                        // Get the Property's name
                        JProperty thisProperty = (JProperty)dataItem;
                        PropertyName = thisProperty.Name;
                        PropertyValue = thisProperty.Value;
                        if (logToDebug)
                            Debug.Print("Property name " + PropertyName + " added");


                        HETreeNode nodeDataItem = new HETreeNode()
                        {
                            Name = PropertyName,
                            NodeType = typeOfNode,
                            Text = PropertyName,
                            Tag = dataItem,
                            ImageIndex = iImageIndex,
                            SelectedImageIndex = iImageIndex,
                        };

                        // Add nodeDataItem to the nodeRoot's Nodes collection
                        nodeRoot.Nodes.Add(nodeDataItem);

                        if (PropertyValue.Type == JTokenType.Boolean
                            || PropertyValue.Type == JTokenType.Bytes
                            || PropertyValue.Type == JTokenType.Comment
                            || PropertyValue.Type == JTokenType.Constructor
                            || PropertyValue.Type == JTokenType.Date
                            || PropertyValue.Type == JTokenType.Float
                            || PropertyValue.Type == JTokenType.Guid
                            || PropertyValue.Type == JTokenType.Integer
                            || PropertyValue.Type == JTokenType.None
                            || PropertyValue.Type == JTokenType.Null
                            || PropertyValue.Type == JTokenType.String
                            || PropertyValue.Type == JTokenType.TimeSpan
                            || PropertyValue.Type == JTokenType.Undefined
                            || PropertyValue.Type == JTokenType.Uri
                            )
                        {
                            if (logToDebug)
                                Debug.Print("The property value contains a value");

                            nodeDataItem.NodeType = HETreeNodeType.JsonValue;
                            nodeDataItem.ImageIndex = nodeDataItem.SelectedImageIndex = HEUtilities.GetImageIndexByNodeType(nodeDataItem.NodeType);

                        }



                        if (PropertyValue.Type == JTokenType.Array)
                        {
                            if (logToDebug)
                                Debug.Print("The property value contains an array, evaluating ");

                            nodeDataItem.NodeType = HETreeNodeType.JsonArray;
                            nodeDataItem.ImageIndex = nodeDataItem.SelectedImageIndex = HEUtilities.GetImageIndexByNodeType(nodeDataItem.NodeType);
                            string aPropertyName;

                            foreach (JToken thing in PropertyValue)
                            {
                                // Get the Property's name
                                //JProperty athisProperty = (JProperty)thing;
                                aPropertyName = (string)thing["Name"];

                                JToken token = thing["Registration"];
                                if (token != null)
                                {
                                    aPropertyName += " " + (string)thing["Registration"];
                                }

                                HETreeNode anodeDataItem = new HETreeNode()
                                {
                                    Name = aPropertyName,
                                    NodeType = HETreeNodeType.Unknown,
                                    Text = aPropertyName,
                                    Tag = dataItem,
                                    ImageIndex = HEUtilities.GetImageIndexByNodeType(HETreeNodeType.Unknown),
                                    SelectedImageIndex = HEUtilities.GetImageIndexByNodeType(HETreeNodeType.Unknown),
                                };
                                // Add anodeDataItem to the nodeRoot's Nodes collection
                                nodeDataItem.Nodes.Add(anodeDataItem);
                            }
                        }

                        if (PropertyValue.Type == JTokenType.Object)
                        {
                            if (logToDebug)
                                Debug.Print("The property value contains an object, evaluating");
                            nodeDataItem.NodeType = HETreeNodeType.JsonObject;
                            nodeDataItem.ImageIndex = nodeDataItem.SelectedImageIndex = HEUtilities.GetImageIndexByNodeType(nodeDataItem.NodeType);

                            string SubPropertyName;
                            JToken SubPropertyValue;

                            foreach (JToken thing in PropertyValue)
                            {
                                // Get the Property's name
                                JProperty thisSubProperty = (JProperty)thing;
                                SubPropertyName = thisSubProperty.Name;
                                SubPropertyValue = thisSubProperty.Value;
                                if (logToDebug)
                                    Debug.Print("Property name " + PropertyName + " added");


                                //JProperty athisProperty = (JProperty)thing;
                                //subPropertyName = (string)thing["Name"];
                                
                                //JToken token = thing["Registration"];
                                //if (token != null)
                                //{
                                //    aPropertyName += " " + (string)thing["Registration"];
                                //}
                                
                                HETreeNode anodeDataItem = new HETreeNode()
                                {
                                    Name = SubPropertyName,
                                    NodeType = HETreeNodeType.Unknown,
                                    Text = SubPropertyName,
                                    Tag = thing,
                                    ImageIndex = HEUtilities.GetImageIndexByNodeType(HETreeNodeType.Unknown),
                                    SelectedImageIndex = HEUtilities.GetImageIndexByNodeType(HETreeNodeType.Unknown),
                                };
                                // Add anodeDataItem to the nodeRoot's Nodes collection
                                nodeDataItem.Nodes.Add(anodeDataItem);
                            }
                        }



                    }

                    else if (dataItem.Type == JTokenType.Object)
                    {
                        if (logToDebug)
                            Debug.Print("It's an object");

                    }

                    else
                    {
                        if (logToDebug)
                            Debug.Print("Not a property or an object!");
                    }



                }
                // Update the count of objects for the root node
                nodeRoot.UpdateCounts();

            }
            // Return the node with child objects in the Nodes collection
            return nodeRoot;
        }

        public void BuildBasicNodeTreeFromJson(
            JToken json,
            HETreeNode nodeParent,
            int maxDepth = 1,
            int currentDepth = 0,
            string nameHint = "",
            bool collapseJArrays = false,
            [CallerMemberName] string callerName = "")
        {
            // Set up indenting for this level
            string thisLevelIndent = String.Join("| ", new String[currentDepth]);

            if (logToDebug)
            {
                foreach (var method in new StackTrace().GetFrames())
                {
                    if (method.GetMethod().Name == callerName)
                    {
                        callerName = $"{method.GetMethod().ReflectedType.Name}.{callerName}";
                        break;
                    }
                }
                Debug.Print("{0} :{1} BuildTree depth {2}({3}) ENTERED with type: {4}, {5}", DateTime.Now, thisLevelIndent, currentDepth, maxDepth, json.Type, callerName);
                Debug.Print(json.ToString());
            }

            // Builds a section of node tree and recurses if necessary
            if (json != null) // probably needs more tests that this
            {
                HETreeNode newNode;
                //JToken token = (JToken)json.Reverse();

                switch (json.Type)
                {
                    case JTokenType.Object: // It's a JObject
                        // Depth and null check, if valid, create this node
                        JObject tmpJObject = (JObject)json;
                        if (tmpJObject != null)
                        {
                            if (maxDepth < 1) // maxDepth (zero) has been reached, offer expansion if this has child tokens
                            {
                                // Count children
                                int numChildTokens = 0;
                                foreach (JToken token in tmpJObject.Children<JToken>())
                                    numChildTokens++;
                                // If it has child tokens
                                if (numChildTokens > 0)
                                {
                                    // attach a single expansion node to the tree - this will be removed when the actual tree is generated
                                    if (logToDebug)
                                        Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding (obj) expansion node", DateTime.Now, thisLevelIndent, currentDepth, maxDepth);

                                    HETreeNode tempExpansionNode = new HETreeNode("Click to expand " + numChildTokens.ToString() + "nodes...", HETreeNodeType.ExpansionAvailable);
                                    nodeParent.Nodes.Add(tempExpansionNode);
                                }
                            }
                            else
                            {
                                // Creation of this node
                                if (nameHint != "")
                                {
                                    // Name hint provided, use that
                                    newNode = new HETreeNode(nameHint, HETreeNodeType.JsonObject);
                                }
                                else
                                {
                                    if (false)
                                    {
                                        // name lookup/generation should be hooked in here, but doesn't yet exist
                                    }
                                    else
                                    {
                                        newNode = new HETreeNode("Object", HETreeNodeType.JsonObject);
                                    }
                                }
                                // Set the node's tag to the JObject
                                //newNode.Tag = tmpJObject;

                                // Process child tokens - actually JProperties in the case of a JObject
                                int numChildProperties = 0;
                                foreach (JToken token in tmpJObject.Children<JToken>())
                                {
                                    numChildProperties++;
                                    if (maxDepth >= 2)
                                    {
                                        if (logToDebug)
                                            Debug.Print("{0} :{1} BuildTree depth {2}({3}) Calling recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);
                                        BuildBasicNodeTreeFromJson(token, newNode, maxDepth: maxDepth - 1, currentDepth: currentDepth + 1);
                                        if (logToDebug)
                                            Debug.Print("{0} :{1} BuildTree depth {2}({3}) Returning from recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);
                                        // Add the node
                                        if (logToDebug)
                                            Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding Node {3}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, newNode.Text);
                                        nodeParent.Nodes.Add(newNode);
                                    }
                                }
                            }

                        }
                        else
                        {
                            // We shouldn't be here!
                            if (logToDebug)
                                Debug.Print("{0} :{1} BuildTree depth {2}({3}) tmpJObject was null", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);
                        }
                        break;

                    case JTokenType.Array: // It's a JArray
                        // Depth and null check, if valid, create this node
                        //JArray tmpJArray = (JArray)json;
                        //if (tmpJArray != null)
                        //{
                            if (maxDepth <= 0) // maxDepth (zero) has been reached, offer expansion if this has child tokens
                            {
                                // Count children
                                int numChildTokens = 0;
                                foreach (JToken token in json) // tmpJArray.Children<JToken>()
                                    numChildTokens++;
                                // If it has child tokens
                                if (numChildTokens > 0)
                                {
                                    // attach a single expansion node to the tree - this will be removed when the actual tree is generated
                                    if (logToDebug)
                                        Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding expansion node", DateTime.Now, thisLevelIndent, currentDepth, maxDepth);

                                    HETreeNode tempExpansionNode = new HETreeNode("Click to expand " + numChildTokens.ToString() + " nodes...", HETreeNodeType.ExpansionAvailable);
                                    nodeParent.Nodes.Add(tempExpansionNode);
                                }
                            }
                            else
                            {
                                // Creation of this node
                                if (nameHint != "")
                                {
                                    // Name hint provided, use that
                                    newNode = new HETreeNode(nameHint, HETreeNodeType.JsonArray);
                                }
                                else
                                {
                                    if (false)
                                    {
                                        // name lookup/generation should be hooked in here, but doesn't yet exist
                                    }
                                    else
                                    {
                                        newNode = new HETreeNode("Array", HETreeNodeType.JsonArray);
                                    }
                                }
                                // Set the node's tag to the JArray
                                //newNode.Tag = tmpJArray;

                                // Process child tokens - actually JProperties in the case of a JObject
                                foreach (JToken token in json) // .Children<JToken>()
                                {
                                    if (logToDebug)
                                        Debug.Print("{0} :{1} BuildTree depth {2}({3}) Calling recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);
                                    // collapse arrays mounts members of the array to the parent rather than creating a distinct node for the array. Defaults to off.
                                    if (collapseJArrays)
                                    {
                                        // Adjust parent node instead of using the generated node
                                        // Changes the node type to represent an array
                                        //nodeParent.NodeType = HETreeNodeType.JsonArray; // HETreeNodeType.JsonArray;
                                        //nodeParent.UpdateImageIndeces();
                                        // Recursive call
                                        BuildBasicNodeTreeFromJson(token, nodeParent, maxDepth: maxDepth - 1, currentDepth: currentDepth + 1);
                                        if (logToDebug)
                                            Debug.Print("{0} :{1} BuildTree depth{1}({2}) CollapseArrays using parent {3}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, nodeParent.Name);

                                }
                                else
                                    {
                                        BuildBasicNodeTreeFromJson(token, newNode, maxDepth: maxDepth - 1, currentDepth: currentDepth + 1);
                                        // Add the node
                                        if (logToDebug)
                                            Debug.Print("{0} :{1} BuildTree depth{1}({2}) Adding Node {3}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, newNode.Text);

                                        nodeParent.Nodes.Add(newNode);
                                        }
                                if (logToDebug)
                                    Debug.Print("{0} :{1} BuildTree depth{1}({2}) Returning from recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);

                                

                            }
                        }
                        //}
                        //else
                        //{
                            // We shouldn't be here!
                            //if (logToDebug) Debug.Print("tmpJArray was null");
                        //}
                        break;

                    case JTokenType.Property: // It's a JProperty, similar to a key value pair
                        //

                        // Depth and null check, if valid, create this node
                        JProperty tmpJProperty = (JProperty)json;
                        if (tmpJProperty != null)
                        {
                            if (maxDepth >= 0)
                            {
                                // Creation of this node
                                if (nameHint != "")
                                {
                                    // Name hint provided, use that
                                    newNode = new HETreeNode(nameHint, HETreeNodeType.JsonProperty);
                                }
                                else
                                {
                                    if (false)
                                    {
                                        // name lookup/generation should be hooked in here, but doesn't yet exist
                                    }
                                    else
                                    {
                                        newNode = new HETreeNode(tmpJProperty.Name, HETreeNodeType.JsonProperty);
                                    }
                                }
                                // Set the node's tag to the JObject
                                //newNode.Tag = tmpJProperty;

                                // Process value

                                if (logToDebug)
                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Calling recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);

                                BuildBasicNodeTreeFromJson(tmpJProperty.Value, newNode, maxDepth: maxDepth - 1, currentDepth: currentDepth + 1);

                                if (logToDebug)
                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Returning from recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);

                                if (logToDebug)
                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding Node {4}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, newNode.Text);
                                // Add the node
                                nodeParent.Nodes.Add(newNode);
                            }
                            else
                            {
                                if (logToDebug)
                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Max Depth reached", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);
                            }
                        }

                        break;

                    case JTokenType.Boolean:
                    case JTokenType.Bytes:
                    case JTokenType.Comment:
                    case JTokenType.Date:
                    case JTokenType.Float:
                    case JTokenType.Guid:
                    case JTokenType.Integer:
                    case JTokenType.String:
                    case JTokenType.TimeSpan:
                    case JTokenType.Uri:
                        JValue tmpJValue = (JValue)json;
                        if (tmpJValue != null)
                        {
                            int newNodeImageIndex = 0;

                            switch (json.Type)
                            {
                                case JTokenType.Boolean:
                                    // Bool (CheckDot)
                                    newNodeImageIndex = (int)HEObjectTypesImageList.CheckDot_16x;
                                    break;
                                case JTokenType.Bytes:
                                    // Binary
                                    newNodeImageIndex = (int)HEObjectTypesImageList.Binary_16x;
                                    break;
                                case JTokenType.Integer:
                                case JTokenType.Float:
                                    // Number
                                    newNodeImageIndex = (int)HEObjectTypesImageList.DomainType_16x;
                                    break;
                                case JTokenType.String:
                                case JTokenType.Comment:
                                case JTokenType.Guid:
                                case JTokenType.Uri:
                                    // Text
                                    newNodeImageIndex = (int)HEObjectTypesImageList.String_16x;
                                    break;
                                case JTokenType.Date:
                                case JTokenType.TimeSpan:
                                    // Time/Date
                                    newNodeImageIndex = (int)HEObjectTypesImageList.DateTimeAxis_16x;
                                    break;
                                default:
                                    //
                                    newNodeImageIndex = (int)HEObjectTypesImageList.Checkerboard_16x;
                                    break;
                            }



                            if (logToDebug) Debug.Print("::Value " + Environment.NewLine + "{0}", tmpJValue.Value);

                            newNode = new HETreeNode(tmpJValue.Value.ToString(), HETreeNodeType.JsonValue)
                            {
                                Tag = tmpJValue,
                                // Update the ImageIndex and SelectedImageIndex directly - we're overriding the standard icons
                                ImageIndex = newNodeImageIndex,
                                SelectedImageIndex = newNodeImageIndex,
                            };
                            if (logToDebug)
                                Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding Node {3}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, newNode.Text);

                            nodeParent.Nodes.Add(newNode);

                        }
                        break;

                    default:
                        //
                        if (logToDebug)
                        {
                            Debug.Print("BNTFJ {0} VALUE, type: {1} - NO ACTION TAKEN!", currentDepth, json.Type);
                            Debug.Print(json.ToString());
                        }
                        break;
                }
            }
            else
            {
                if (logToDebug)
                    Debug.Print("{0} : BuildTree depth {1}({2}) JToken was null", DateTime.Now.ToString(), currentDepth, maxDepth);
            }

            if (logToDebug)
                Debug.Print("{0} : BuildTree depth {1}({2}) EXITING", DateTime.Now.ToString(), currentDepth, maxDepth);
        } // End of BuildBasicNodeTreeFromJson
        */

        /// <summary>
        /// Builds a section of node tree and recurses if necessary - can return null
        /// </summary>
        /// <param name="json"></param>
        /// <param name="maxDepth">Defaults to 10</param>
        /// <param name="currentDepth">Used internally</param>
        /// <param name="nameHint">Used to provide a hint to name an object instead of generating it</param>
        /// <param name="collapseJArrays">Non-functional</param>
        /// <returns>Returns an HETreeNode that is the root of the built tree.</returns>
        public HETreeNode BuildHETreeNodeTreeFromJson(
            JToken json,
            int maxDepth = 10,
            int currentDepth = 0,
            string nameHint = "",
            bool collapseJArrays = false)
        {
            // Set up indenting for this level
            string thisLevelIndent = String.Join("| ", new String[currentDepth]);
            HETreeNode newNode = null; // node to represent this level of recursion - this is returned once children are populated recursively

            if (logToDebug)
            {
                Debug.Print("{0} :{1} BuildTree depth {2}({3}) ENTERED with type: {4}", DateTime.Now, thisLevelIndent, currentDepth, maxDepth, json.Type);
                Debug.Print(json.ToString());
            }

            if (json != null) // probably needs more tests that this
            {
                //JToken token = (JToken)json.Reverse();

                switch (json.Type)
                {
                    case JTokenType.Object: 
                        // It's a JObject
                        // Depth and null check, if valid, create this node
                        JObject tmpJObject = (JObject)json;
                        string newNodeName = "";
                        if (tmpJObject != null)
                        {
                            // Creation of this node
                            if (nameHint != "")
                            {
                                // Name hint provided, use that
                                newNodeName = nameHint;
                            }
                            else
                            {
                                // name lookup/generation used
                                newNodeName = GenerateDisplayName(tmpJObject).Trim();
                                if (newNodeName == "")
                                {
                                    newNodeName = "Object";
                                }
                                
                            }
                            newNode = new HETreeNode(newNodeName, HETreeNodeType.JsonObject);

                            // Set the node's tag to the JObject
                            newNode.Tag = tmpJObject;

                            // Process any child tokens - actually JProperties in the case of a JObject
                            // Count children
                            int numChildTokens = json.Count<JToken>();
                            if (numChildTokens > 0)
                            {
                                if ((currentDepth + 1) > maxDepth)
                                {
                                    // It has child tokens, but we're at the maximum depth already
                                    // attach a single expansion node to the tree - this will be removed when the actual tree is generated

                                    HETreeNode tempExpansionNode = new HETreeNode("Click to expand " + numChildTokens.ToString() + " nodes...", HETreeNodeType.ExpansionAvailable);
                                    if (logToDebug)
                                        Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding (obj) expansion node", DateTime.Now, thisLevelIndent, currentDepth, maxDepth);
                                    // Add the  expansion node
                                    newNode.Nodes.Add(tempExpansionNode);
                                }
                                else
                                {
                                    // Process child tokens 
                                    int numChildProperties = 0;
                                    foreach (JToken token in tmpJObject.Children<JToken>().Reverse<JToken>())
                                    {
                                        numChildProperties++;
                                        if (maxDepth >= 2)
                                        {
                                            if (logToDebug) Debug.Print("{0} :{1} BuildTree depth {2}({3}) Calling recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);
                                            HETreeNode temp = BuildHETreeNodeTreeFromJson(token, maxDepth: maxDepth - 1, currentDepth: currentDepth + 1);
                                            if (logToDebug) Debug.Print("{0} :{1} BuildTree depth {2}({3}) Returning from recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);

                                            if (temp != null)
                                            {
                                                // Add the node
                                                if (logToDebug)
                                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding Node {3}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, newNode.Text);
                                                newNode.Nodes.Add(temp);
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            // We shouldn't be here!
                            if (logToDebug)
                                Debug.Print("{0} :{1} BuildTree depth {2}({3}) tmpJObject was null", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);
                        }
                        break;

                    case JTokenType.Array: 
                        // It's a JArray
                        // Depth and null check, if valid, create this node
                        JArray tmpJArray = (JArray)json;
                        if (tmpJArray != null)
                        {
                            // Creation of this node
                            if (nameHint != "")
                            {
                                // Name hint provided, use that
                                newNode = new HETreeNode(nameHint, HETreeNodeType.JsonArray);
                            }
                            else
                            {
                                if (false)
                                {
                                    // name lookup/generation should be hooked in here, but doesn't yet exist
                                }
                                else
                                {
                                    newNode = new HETreeNode("Array", HETreeNodeType.JsonArray);
                                }
                            }
                            // Set the node's tag to the JArray
                            newNode.Tag = tmpJArray;

                            // Process any child tokens
                            int numChildTokens = json.Count<JToken>();
                            if (numChildTokens > 0)
                            {
                                if ((currentDepth + 1) > maxDepth)
                                {
                                    // It has child tokens, but we're at the maximum depth already
                                    // attach a single expansion node to the tree - this will be removed when the actual tree is generated

                                    HETreeNode tempExpansionNode = new HETreeNode("Click to expand " + numChildTokens.ToString() + " nodes...", HETreeNodeType.ExpansionAvailable);
                                    if (logToDebug)
                                        Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding (arr) expansion node", DateTime.Now, thisLevelIndent, currentDepth, maxDepth);
                                    // Add the  expansion node
                                    newNode.Nodes.Add(tempExpansionNode);
                                }
                                else
                                {
                                    // Process child tokens - actually JProperties in the case of a JObject
                                    foreach (JToken token in tmpJArray.Children<JToken>().Reverse<JToken>())
                                    {
                                        // Collapse Arrays mounts members of the array to the parent rather than creating a distinct node for the array. Defaults to off.
                                        if (collapseJArrays)
                                        {
                                            // Adjust parent node instead of using the generated node, or at least it used to - this needs re-doing in light of recent changes
                                            // Recursive call
                                            BuildHETreeNodeTreeFromJson(token, maxDepth: maxDepth - 1, currentDepth: currentDepth + 1);
                                            if (logToDebug)
                                            {
                                                Debug.Print("FINDME in collapseArrays!!!!");
                                                //Debug.Print("{0} :{1} BuildTree depth{1}({2}) CollapseArrays using parent {3}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, nodeParent.Name);
                                            }

                                        }
                                        else
                                        {
                                            // Add the node
                                            if (logToDebug) Debug.Print("{0} :{1} BuildTree depth {2}({3}) Calling recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);

                                            newNode.Nodes.Add(BuildHETreeNodeTreeFromJson(token, maxDepth: maxDepth, currentDepth: currentDepth + 1));

                                            if (logToDebug) Debug.Print("{0} :{1} BuildTree depth{1}({2}) Returning from recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);
                                        }
                                    } // End of foreach (token)
                                }
                            }
                        }
                        if (logToDebug)
                        Debug.Print("{0} :{1} BuildTree depth{1}({2}) Returning Node {3}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, newNode.Text);
                        break;


                    case JTokenType.Property: 
                        // It's a JProperty, similar to a key value pair
                        // Depth and null check, if valid, create this node
                        JProperty tmpJProperty = (JProperty)json;
                        if (tmpJProperty != null)
                        {
                            if (maxDepth >= 0)
                            {
                                // Creation of this node
                                if (nameHint != "")
                                {
                                    // Name hint provided, use that
                                    newNode = new HETreeNode(nameHint, HETreeNodeType.JsonProperty);
                                }
                                else
                                {
                                    if (false)
                                    {
                                        // name lookup/generation should be hooked in here, but doesn't yet exist
                                    }
                                    else
                                    {
                                        newNode = new HETreeNode(tmpJProperty.Name, HETreeNodeType.JsonProperty);
                                    }
                                }
                                // Set the node's tag to the JObject
                                newNode.Tag = tmpJProperty;

                                // Process value

                                if (logToDebug)
                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Calling recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);

                                HETreeNode temp = BuildHETreeNodeTreeFromJson(tmpJProperty.Value, maxDepth: maxDepth - 1, currentDepth: currentDepth + 1);

                                if (logToDebug)
                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Returning from recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);

                                if (logToDebug)
                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding Node {4}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, newNode.Text);
                                // Add the node
                                newNode.Nodes.Add(temp ?? new HETreeNode("null",HETreeNodeType.Unknown));
                            }
                            else
                            {
                                if (logToDebug)
                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Max Depth reached", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);
                            }
                        }

                        break;

                    case JTokenType.Boolean:
                    case JTokenType.Bytes:
                    case JTokenType.Comment:
                    case JTokenType.Date:
                    case JTokenType.Float:
                    case JTokenType.Guid:
                    case JTokenType.Integer:
                    case JTokenType.String:
                    case JTokenType.TimeSpan:
                    case JTokenType.Uri:
                        JValue tmpJValue = (JValue)json;
                        if (tmpJValue != null)
                        {
                            int newNodeImageIndex = 0;

                            switch (json.Type)
                            {
                                case JTokenType.Boolean:
                                    // Bool (CheckDot)
                                    newNodeImageIndex = (int)HEObjectTypesImageList.CheckDot_16x;
                                    break;
                                case JTokenType.Bytes:
                                    // Binary
                                    newNodeImageIndex = (int)HEObjectTypesImageList.Binary_16x;
                                    break;
                                case JTokenType.Integer:
                                case JTokenType.Float:
                                    // Number
                                    newNodeImageIndex = (int)HEObjectTypesImageList.DomainType_16x;
                                    break;
                                case JTokenType.String:
                                case JTokenType.Comment:
                                case JTokenType.Guid:
                                case JTokenType.Uri:
                                    // Text
                                    newNodeImageIndex = (int)HEObjectTypesImageList.String_16x;
                                    break;
                                case JTokenType.Date:
                                case JTokenType.TimeSpan:
                                    // Time/Date
                                    newNodeImageIndex = (int)HEObjectTypesImageList.DateTimeAxis_16x;
                                    break;
                                default:
                                    //
                                    newNodeImageIndex = (int)HEObjectTypesImageList.Checkerboard_16x;
                                    break;
                            }

                            if (logToDebug) Debug.Print("::Value " + Environment.NewLine + "{0}", tmpJValue.Value);

                            newNode = new HETreeNode(tmpJValue.Value.ToString(), HETreeNodeType.JsonValue)
                            {
                                Tag = tmpJValue,
                                // Update the ImageIndex and SelectedImageIndex directly - we're overriding the standard icons
                                ImageIndex = newNodeImageIndex,
                                SelectedImageIndex = newNodeImageIndex,
                            };
                            if (logToDebug)
                                Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding Node {3}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, newNode.Text);

                            //nodeParent.Nodes.Add(newNode);

                        }
                        break;

                    default:
                        //
                        if (logToDebug)
                        {
                            Debug.Print("BNTFJ {0} VALUE, type: {1} - NO ACTION TAKEN!", currentDepth, json.Type);
                            Debug.Print(json.ToString());
                        }
                        break;
                }
            }
            else
            {
                if (logToDebug)
                    Debug.Print("{0} : BuildTree depth {1}({2}) JToken was null", DateTime.Now.ToString(), currentDepth, maxDepth);

                return null;
            }

            if (logToDebug)
                Debug.Print("{0} : BuildTree depth {1}({2}) EXITING", DateTime.Now.ToString(), currentDepth, maxDepth);

            // Return the newNode
            if (newNode == null) Debug.Print("newNode was null");
            return newNode;
        } // End of BuildBasicNodeTreeFromJson

        /// <summary>
        /// Populates the node tree from the jData
        /// </summary>
        public void PopulateNodeTree()
        {
            // Populates the rootNode using the build function
            HETreeNode tn = BuildHETreeNodeTreeFromJson(JData, maxDepth: 1, collapseJArrays: false);
            rootNode.Nodes.Add(tn ?? new HETreeNode("LOADING ERROR!", HETreeNodeType.DataFileError));
        }

        /// <summary>
        /// Attempts to build a user-friendly name from available data in a JObject
        /// </summary>
        /// <param name="obj">Takes a JObject and attempts to generate a name from expected fields</param>
        /// <returns></returns>
        public string GenerateDisplayName (JObject obj)
        {
            // 
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
