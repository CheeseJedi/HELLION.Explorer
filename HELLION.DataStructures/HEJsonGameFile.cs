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
            // Basic Constructor, calls the base class constructor
            File = null;
            JData = null;
            IsFileLoaded = false;
            IsFileWritable = false;
            LoadError = false;
            LogToDebug = false;
            SkipLoading = false;
            RootNode = new HETreeNode("DATAFILE", HETreeNodeType.SaveFile);
        }

        public HEJsonGameFile(FileInfo PassedFileInfo)
        {
            // Constructor that allows the file name to be set and triggers the load            
            File = null;
            JData = null;
            IsFileLoaded = false;
            IsFileWritable = false;
            LoadError = false;
            LogToDebug = false;
            SkipLoading = false;
            RootNode = null;
            if (PassedFileInfo != null)
            {
                File = PassedFileInfo;
                RootNode = new HETreeNode("DATAFILE", HETreeNodeType.SaveFile, nodeText: File.Name, nodeToolTipText: File.FullName);

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
            base.SaveFile();
            // Run POST operations after here
        }
        

        public new void PopulateNodeTree()
        {
            // Populates the RootNode using the build function
            HETreeNode tn = BuildHETreeNodeTreeFromJson(JData, maxDepth: 5, collapseJArrays: false);
            RootNode.Nodes.Add(tn ?? new HETreeNode("LOADING ERROR!", HETreeNodeType.DataFileError));
        }




    } // End of class HEGameFile

} // End of namespace HELLION.DataStructures
