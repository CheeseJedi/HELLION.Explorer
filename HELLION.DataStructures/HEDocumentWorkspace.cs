using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic; // for IEnumerable
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.HEUtilities;

namespace HELLION.DataStructures
{
    public class HEDocumentWorkspace
    {
        // Definition for a workspace for a HELLION Save (.save) JSON file
        // Generates the game data view and solar system view
        // Includes methods for loading a save and assocaited data files in to memory, building a 
        // custom node tree representing the orbital objects and retrieving data from the tree
        // to populate the dynamic list and full data from the source.

        public HEGameData GameData { get; set; } = null;
        public HESolarSystem SolarSystem { get; set; } = null;
        //public HESearchResults SearchResults { get; set; } = null; // this doesn't exist yet but will hook in here

        public FileInfo SaveFileInfo { get; set; } = null;
        public DirectoryInfo DataDirectoryInfo { get; set; } = null;

        public bool IsFileReady { get; private set; } = false;
        public bool LoadError { get; private set; } = false;
        public bool IsDirty { get; private set; } = false;
        public bool LogToDebug { get; set; } = false;

        public HEDocumentWorkspace(FileInfo fileInfo, DirectoryInfo directoryInfo, bool autoLoad = false)
        {
            // Constructor that takes a FileInfo and a DirectoryInfo
            
            // Initialise the GameData and SolarSystem
            if (fileInfo != null && directoryInfo != null && fileInfo.Exists && directoryInfo.Exists)
            {
                GameData = new HEGameData(fileInfo, directoryInfo);
                SolarSystem = new HESolarSystem(GameData);
            }
            else
            {
                throw new Exception();
            }

        }


        public bool Close()
        {
            // Not yet implemented

            if (IsDirty)
            {
                // Eventually - will figure out what's dirty
                return false; // indicates a problem
            }
            else
            {
                // Not dirty, ok to close everything

                if (SolarSystem != null)
                {
                    SolarSystem.Close();
                    SolarSystem = null;
                }
                if (GameData != null)
                {
                    GameData.Close();
                    GameData = null;
                }
                SaveFileInfo = null;
                DataDirectoryInfo = null;

                return true;
            }


        }

    } // End of class HEDocumentWorkspace
} // End of namespace HELLION
