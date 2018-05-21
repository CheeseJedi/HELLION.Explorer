using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    public class HEJsonFile
    {
        /// <summary>
        /// Default constructor, not used directly but used by derived classes.
        /// </summary>
        public HEJsonFile(object ownerObject)
        {
            OwnerObject = ownerObject;
            if (OwnerObject == null) Debug.Print("OwnerObject was null.");
        }

        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="PassedFileInfo">The FileInfo representing the file to be loaded.</param>
        public HEJsonFile(object ownerObject, FileInfo passedFileInfo) : this(ownerObject) // , int populateNodeTreeDepth
        {
            File = passedFileInfo ?? throw new NullReferenceException("passedFileInfo was null.");

            if (File.Exists)
            {
                LoadFile();
            }
        }

        /// <summary>
        /// Stores a reference to the parent object.
        /// </summary>
        /// <remarks>
        /// Set by the constructor.
        /// </remarks>
        public object OwnerObject { get; protected set; } = null;

        /// <summary>
        /// FileInfo object that represents the file on disk that is to be worked with.
        /// </summary>
        public FileInfo File { get; protected set; } = null;
        
        /// <summary>
        /// Public property to get and set the JToken that was loaded from the file.
        /// </summary>
        public JToken JData
        {
            get
            {
                // Check the file is loaded
                if (!IsLoaded) return null;
                // Check there wasn't a load error
                if (LoadError) return null;
                return jData;
            }
            /*
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
            */
        }

        /// <summary>
        /// Used to determine whether the file is loaded.
        /// </summary>
        public bool IsLoaded { get; protected set; } = false;

        /// <summary>
        /// Used to determine whether there was an error on load.
        /// </summary>
        public virtual bool LoadError
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
                        //RootNode.NodeType = HETreeNodeType.DataFileError;
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
                if (!IsLoaded) return true;
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
                if (value && IsLoaded && !LoadError && !File.IsReadOnly)
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
                    isDirty = false;
                }

            }
        }

        /// <summary>
        /// The JToken that was loaded from the file, if load was successful.
        /// </summary>
        protected JToken jData = null;

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
                            Console.WriteLine(File.Name + " loaded; detected as " + jData.Type.ToString() + ", " + numObj.ToString() + " JToken(s) detected.");
                        }
                        else
                        {
                            LoadError = true;
                            Debug.Print("ERROR: JData is detected as neither an ARRAY or OBJECT!");
                        }
                    }
                    // Set the IsLoaded flag to true
                    IsLoaded = true;
                }
            }
            else
            {
                // File does not exist.
                LoadError = true;
                throw new FileNotFoundException();

            }

            // Return the value of LoadError
            return LoadError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="existingToken"></param>
        /// <param name="newToken"></param>
        public void ReplaceJToken(JToken existingToken, JToken newToken)
        {
            if (IsReadOnly) throw new InvalidOperationException("Attempted JToken change on non-modifiable file. (IsReadOnly=true)");
            else
            {
                Debug.Print("Replacing token " + existingToken.ToString());
                Debug.Print("With token " + newToken.ToString());

                existingToken.Replace(newToken);
                isDirty = true;
            }
        }

        /// <summary>
        /// Save the file data.
        /// </summary>
        /// <returns></returns>
        public bool SaveFile(bool CreateBackup = true)
        {
            Debug.Print("SaveFile Called, CreateBackup="+CreateBackup.ToString());

            if (File.Exists && CreateBackup)
            {
                // Check to see if the backup file already exists
                if (System.IO.File.Exists(File.FullName + ".bak"))
                {
                    // It does, so remove it.
                    Debug.Print("Deleting " + File.FullName + ".bak");
                    System.IO.File.Delete(File.FullName + ".bak");
                }
                // MainFile already exists, create a backup copy (.save.bak)
                System.IO.File.Move(File.FullName, File.FullName + ".bak");
            }
            else
            {
                // Remove the existing file
                File.Delete();
            }

            //try
            {
                using (StreamWriter sw = new StreamWriter(File.FullName))
                {
                    // Process the stream with the JSON Text Reader in to a JToken
                    using (JsonTextWriter jtw = new JsonTextWriter(sw))
                    {
                        jtw.Formatting = Formatting.Indented;
                        jData.WriteTo(jtw);
                    }
                }
            }
            //catch (IOException)
            {
                // Some error handling to be implemented here
            }

            // We should have some data in the array
            IsDirty = false;
            
            return false;
        }

        /// <summary>
        /// Handles closing of this file, and de-allocation of it's objects
        /// </summary>
        /// <returns></returns>
        public virtual bool Close()
        {
            if (IsDirty)
            {
                return false; // indicates a problem and can't close
            }
            else
            {
                // Not dirty, OK to close everything
                IsLoaded = false;
                File = null;
                jData = null;
                //RootNode = null;
                return true;
            }
        }

    }
}
