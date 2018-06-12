﻿using System;
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
    /// Used directly in the Json_FileCollection and is also inherited by the HEJsonGameFile class.
    /// This is a re-write intended to encapsulate more of the functionality of building node trees
    /// of the correct type and enabling lazy population of node tree branches.
    /// </remarks>
    public class Json_File
    {
        #region Constructors

        /// <summary>
        /// Default constructor, not used directly but used by derived classes.
        /// </summary>
        public Json_File(IParent_Json_File ownerObject)
        {
            OwnerObject = ownerObject;
        }

        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="PassedFileInfo">The FileInfo representing the file to be loaded.</param>
        public Json_File(IParent_Json_File ownerObject, FileInfo passedFileInfo) : this(ownerObject) // , int populateNodeTreeDepth
        {
            File = passedFileInfo ?? throw new NullReferenceException("passedFileInfo was null.");

            if (File.Exists)
            {
                LoadFile();
            }
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Stores a reference to the parent object.
        /// </summary>
        /// <remarks>
        /// Set by the constructor.
        /// </remarks>
        public IParent_Json_File OwnerObject { get; protected set; } = null;

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
                return _jData;
            }
            /*
            set
            {
                // Nothing special here right now, this will need to be fleshed out
                if (value != null)
                {
                    IsDirty = true;
                    // _jData = value;

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
                return _loadError;
            }
            protected set
            {
                if (value)
                {
                    if (!_loadError)
                    {
                        // Set the load error flag
                        _loadError = true;
                        /*
                        // Fire the event
                        OnRaiseCustomEvent(new HEJsonBaseFileEventArgs(String.Format("Load Error in file {0}", File.FullName)));
                        */
                    }
                }
                else
                {
                    _loadError = value;
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
                return _readOnlyOverride;
            }
            set
            {
                // Attempts to set the file state to writeable.
                if (value && IsLoaded && !LoadError && !File.IsReadOnly)
                {
                    _readOnlyOverride = true;
                }
                else
                {
                    _readOnlyOverride = false;
                }
            }
        }
        
        /// <summary>
        /// Used to determine whether the _jData object has been modified, and will trigger a prompt to save.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }
            private set
            {
                if (value)
                {
                    if (!_isDirty)
                    {
                        // Set the _isDirty flag
                        _isDirty = true;
                        
                        /*
                        // Fire the event
                        OnRaiseCustomEvent(new HEJsonBaseFileEventArgs(String.Format("Changes detected in file {0}", File.FullName)));
                        */
                    }
                }
                else
                {
                    _isDirty = false;
                }

            }
        }

        #endregion

        #region Methods

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
                        // Process the stream with the JsonTextReader in to a JToken.
                        using (JsonTextReader jtr = new JsonTextReader(sr))
                        {
                            _jData = JToken.ReadFrom(jtr);
                        }
                    }
                }
                catch (Exception e)
                {
                    // Some (better) error handling to be implemented here.
                    LoadError = true;
                    if (_logToDebug) Debug.Print("Exception caught during StreamReader or JsonTextReader while processing " + File.Name
                        + Environment.NewLine + e);
                }

                if (_jData == null)
                {
                    // The data didn't load
                    LoadError = true;
                    if (_logToDebug) Debug.Print("JData is null or empty: " + File.Name);
                }
                else
                {
                    // We should have some data
                    Console.WriteLine("File loaded: {0} ({1}, {2} child tokens).", File.Name, _jData.Type, _jData.Count());

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
        /// Save the file data.
        /// </summary>
        /// <returns></returns>
        public bool SaveFile(bool CreateBackup = true)
        {
            //Debug.Print("SaveFile Called, CreateBackup="+CreateBackup.ToString());

            if (File.Exists && CreateBackup)
            {
                string backupFullName = File.FullName + ".backup";
                // Check to see if the backup file already exists
                if (System.IO.File.Exists(backupFullName))
                {
                    // It does, so remove it.
                    Console.WriteLine("Deleting " + backupFullName);
                    System.IO.File.Delete(backupFullName);
                }
                // MainFile already exists, create a backup copy (.save.bak)
                System.IO.File.Move(File.FullName, backupFullName);
            }
            else
            {
                // Remove the existing file
                File.Delete();
            }

            //try
            {
                Console.WriteLine("Writing to file: " + File.FullName);

                using (StreamWriter sw = new StreamWriter(File.FullName))
                {
                    // Process the stream with the JSON Text Reader in to a JToken
                    using (JsonTextWriter jtw = new JsonTextWriter(sw))
                    {
                        jtw.Formatting = Formatting.Indented;
                        _jData.WriteTo(jtw);
                    }
                }
                Console.WriteLine("File write complete.");

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
                _jData = null;


                //RootNode = null;
                return true;
            }
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
                _isDirty = true;
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// The JToken that was loaded from the file, if load was successful.
        /// </summary>
        protected JToken _jData = null;

        /// <summary>
        /// Tracks whether there was an error encountered during load - only used by the 
        /// LoadError property to prevent re-triggering events.
        /// </summary>
        protected bool _loadError = false;

        /// <summary>
        /// This flag is set when the _jData is modified - only used by the 
        /// IsDirty property to prevent re-triggering events.
        /// </summary>
        protected bool _isDirty = false;

        /// <summary>
        /// Used to determine whether the file is forced to read-only or whether the other
        /// constraints alone determine whether the file can be modified.
        /// </summary>
        protected bool _readOnlyOverride = true;

        /// <summary>
        /// Used to activate extended logging to the Debug window in VS.
        /// </summary>
        protected bool _logToDebug = true;

        #endregion

    }

    /// <summary>
    /// Defines an interface to allow different classes to be a parent of a Json_File
    /// </summary>
    public interface IParent_Json_File
    { }
}