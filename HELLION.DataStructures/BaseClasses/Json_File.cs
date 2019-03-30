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
        protected Json_File(IParent_Json_File ownerObject) => OwnerObject = ownerObject;

        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="PassedFileInfo">The FileInfo representing the file to be loaded.</param>
        public Json_File(IParent_Json_File ownerObject, FileInfo passedFileInfo, bool autoDeserialise = false)
            : this(ownerObject)
        {
            //Debug.Print("Json_File.ctor(FileInfo) called {0}", passedFileInfo.FullName);

            AutoLoadOnFileInfoSet = true;
            AutoDeserialiseOnJdataModification = autoDeserialise;

            // Set the FileInfo object and ensure it's not null.
            File = passedFileInfo ?? throw new NullReferenceException("Json_File.ctor(FileInfo) - passedFileInfo was null.");

            // Load the file.
            //if (LoadFile()) Debug.Print("File failed to load from constructor - LoadFile() returned true.");
            //else
            //{
            //    // Trigger the first de-serialisation.
            //    if (autoDeserialise) Deserialise();

            //    // Switch on automatic de-serialisation of the JData when it changes, if specified.
            //    // This is done _after_ the de-serialisation to prevent excessive 
            //    // triggered de-serialisation.
            //    AutoDeserialiseOnJdataModification = autoDeserialise;

            //}
        }

        /// <summary>
        /// Constructor for creating a Json_File in memory from a JToken.
        /// </summary>
        /// <param name="ownerObject"></param>
        /// <param name="jdata"></param>
        public Json_File(IParent_Json_File ownerObject, JToken jdata, bool autoDeserialise = false)
            : this(ownerObject)
        {
            //Debug.Print("Json_File.ctor(FileInfo) called - HasValues [{0}]",
            //    jdata != null ? jdata.HasValues.ToString() : "null");

            AutoLoadOnFileInfoSet = false;

            // Switch on automatic de-serialisation of the JData when it changes.
            AutoDeserialiseOnJdataModification = autoDeserialise;

            // Set the JData, triggering de-serialisation.
            JData = jdata;

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
        public FileInfo File
        {
            get => _file;
            protected set
            {
                if (_file != value)
                {
                    _file = value;

                    ProcessChangedFileInfo();

                }
            }
        }


        /// <summary>
        /// Public property to get and set the JToken that was loaded from the file.
        /// </summary>
        public JToken JData
        {
            get => _jData;
            set
            {
                if (_jData != value)
                {
                    _jData = value;

                    ProcessChangedJData();
                }
            }
        }

        /// <summary>
        /// Determines whether a change to the jData will trigger de-serialisation.
        /// </summary>
        public bool AutoDeserialiseOnJdataModification { get; set; } = false;

        /// <summary>
        /// Determines whether a new FileInfo being set will trigger a LoadFile() operation.
        /// </summary>
        public bool AutoLoadOnFileInfoSet { get; set; } = false;

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
                        OnRaiseCustomEvent(new HEJsonBaseFileEventArgs(string.Format("Load Error in file {0}", File.FullName)));
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
            get => _isDirty;
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;

                    // Potential trigger mount point.

                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Responsible for triggering load and de-serialisation when the FileInfo changes.
        /// </summary>
        private void ProcessChangedFileInfo()
        {
            if (AutoLoadOnFileInfoSet)
            {
                bool autoDeserialise = AutoDeserialiseOnJdataModification;

                // The JData object changes a lot during a load so the auto-deserialise is deactivated.
                AutoDeserialiseOnJdataModification = false;

                // Load the file.
                if (LoadFile())
                {
                    Debug.Print("File failed to load from constructor - LoadFile() returned true.");
                    LoadError = true;
                }
                else
                {

                    // Trigger the first de-serialisation.
                    if (autoDeserialise) Deserialise();

                    // Switch on automatic de-serialisation of the JData when it changes, if specified.
                    // This is done _after_ the de-serialisation to prevent excessive 
                    // triggered de-serialisation.
                    AutoDeserialiseOnJdataModification = autoDeserialise;

                }
            }
        }



        /// <summary>
        /// Load file data from FileName and parse to the JData JObject of type IOrderedEnumerable<JToken>
        /// </summary>
        /// <returns>Returns true if there was a loading error</returns>
        public virtual bool LoadFile()
        {
            if (File != null && File.Exists)
            {
                try
                {
                    using (StreamReader sr = File.OpenText())
                    {
                        // Process the stream with the JsonTextReader in to a JToken.
                        using (JsonTextReader jtr = new JsonTextReader(sr))
                        {
                            JData = JToken.ReadFrom(jtr);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Some (better) error handling to be implemented here.
                    LoadError = true;
                    Debug.Print("LoadFile() - Exception caught during StreamReader or JsonTextReader while processing "
                        + File.Name + Environment.NewLine + ex);
                    return true;
                }

                if (JData == null)
                {
                    // The data didn't load
                    LoadError = true;
                    Debug.Print("LoadFile() - JData is null or empty: " + File.Name);
                    return true;
                }
                else
                {
                    // We should have some data - success!
                    Console.WriteLine("File loaded: {0} ({1}, {2} child tokens).", File.Name, JData.Type, JData.Count());

                    // Set the IsLoaded flag to true
                    IsLoaded = true;
                    LoadError = false;
                    return false;
                }
            }

            // FileInfo was null or file does not exist.

            if (File != null && !File.Exists) Debug.Print("File does not exist");
            else Debug.Print("FileInfo was null.");

            LoadError = true;
            return true;
        }

        /// <summary>
        /// Handles changes to the JData object.
        /// </summary>
        protected virtual void ProcessChangedJData()
        {
            if (AutoDeserialiseOnJdataModification)
            {
                // Triggered de-serialisation.
                Deserialise();
            }
        }

        /// <summary>
        /// Does nothing - This method is a stub to be overridden by derived classes.
        /// </summary>
        public virtual void Deserialise() => throw new NotImplementedException
            ("This method is a stub to be overridden by derived classes.");

        /// <summary>
        /// Does nothing - This method is a stub to be overridden by derived classes.
        /// </summary>
        public virtual void Serialise() => throw new NotImplementedException
            ("This method is a stub to be overridden by derived classes.");

        /// <summary>
        /// Save the file data.
        /// </summary>
        /// <returns></returns>
        public bool SaveFile(bool createBackup = true, FileInfo file = null)
        {
            //Debug.Print("SaveFile Called, CreateBackup="+CreateBackup.ToString());

            // Unless a new FileInfo was specified (SaveAs), use the existing File (Save)
            if (file == null) file = File;


            if (file.Exists && createBackup)
            {
                string backupFullName = file.FullName + ".backup";
                // Check to see if the backup file already exists
                if (System.IO.File.Exists(backupFullName))
                {
                    // It does, so remove it.
                    Console.WriteLine("Deleting {0}", backupFullName);
                    System.IO.File.Delete(backupFullName);
                }
                // The file already exists, create a backup copy (.save.bak)
                Console.WriteLine("Backing up {0} to {1}", file.FullName, backupFullName);
                System.IO.File.Move(file.FullName, backupFullName);
            }
            else
            {
                // Remove the existing file
                file.Delete();
            }

            //try
            {
                Console.WriteLine("Writing to file: {0}...", file.FullName);

                using (StreamWriter sw = new StreamWriter(file.FullName))
                {
                    // Process the stream with the Json Text Reader in to a JToken
                    using (JsonTextWriter jtw = new JsonTextWriter(sw))
                    {
                        jtw.Formatting = Formatting.Indented;
                        JData.WriteTo(jtw);
                    }
                }
                Console.WriteLine("File write complete.");

            }
            //catch (IOException)
            {
                // Some error handling to be implemented here
            }

            // Clear the IsDirty flag.
            IsDirty = false;

            // Set the FileInfo to the new file
            File = file;


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
                // the following may trigger unwanted refreshing.
                JData = null;

                //RootNode = null;
                return true;
            }
        }








        /// <summary>
        /// 
        /// </summary>
        /// <param name="existingToken"></param>
        /// <param name="newToken"></param>
        //public void ReplaceJToken(JToken existingToken, JToken newToken)
        //{
        //    if (IsReadOnly) throw new InvalidOperationException("Attempted JToken change on non-modifiable file. (IsReadOnly=true)");
        //    else
        //    {
        //        Debug.Print("Replacing token " + existingToken.ToString());
        //        Debug.Print("With token " + newToken.ToString());

        //        existingToken.Replace(newToken);
        //        _isDirty = true;
        //    }
        //}

        #endregion

        #region Fields

        protected JToken _jData = null;
        protected bool _loadError = false;
        protected bool _isDirty = false;
        protected bool _readOnlyOverride = true;
        protected bool _logToDebug = true;
        private FileInfo _file = null;

        #endregion

    }

    /// <summary>
    /// Defines an interface to allow different classes to be a parent of a Json_File
    /// </summary>
    public interface IParent_Json_File
    { }
}
