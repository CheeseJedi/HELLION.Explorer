using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Diagnostics;
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace HELLION.DataStructures
{
    // Derive a class for the .save game data file from the HEJsonBaseFile class

    public class HEJsonGameFile : HEJsonBaseFile
    {
        // Derived from the Base class for a generic JSON data file, but extended to encompass the functionality of
        // the save file, plus the Static Data stored in the Data folder


        public HEJsonGameFile()
        {
            // Basic Constructor
            File = null;
            JData = null;
            IsFileLoaded = false;
            IsFileWritable = false;
            LoadError = false;
            LogToDebug = false;
            SkipLoading = false;
            //StaticData = null;
            //StaticDataFolder = null;
        }

        public HEJsonGameFile(string PassedFileName)
        {
            // Constructor that allows the file name to be set and triggers the load
            File = null;
            JData = null;
            IsFileLoaded = false;
            IsFileWritable = false;
            LoadError = false;
            LogToDebug = false;
            SkipLoading = false;
            //StaticData = null;
            //StaticDataFolder = null;

            if (System.IO.File.Exists(PassedFileName))
            {
                File = new FileInfo(PassedFileName);
                if (File.Exists)
                    LoadFile();
            }
        }

        public new void LoadFile()
        {
            // Placeholder load routine for the this class

            // Run PRE operations before here
            // Call the base class' version of this
            base.LoadFile();
            // Run POST operations after here
        }

        public new void SaveFile()
        {
            // Placeholder save routine for the this class

            // Run PRE operations before here
            // Call the base class' version of this
            base.LoadFile();
            // Run POST operations after here
        }

    } // End of class HEGameFile

} // End of namespace HELLION.DataStructures
