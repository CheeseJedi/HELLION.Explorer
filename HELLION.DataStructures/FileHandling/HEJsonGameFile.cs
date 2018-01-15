using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
//using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Defines a class to load and hold data from a JSON .save file and associated metadata.
    /// </summary>
    /// <remarks>
    /// Derived from the Base class for a generic JSON data file.
    /// This is a re-write intended to encapsulate more of the functionality of building node trees
    /// of the correct type and enabling lazy population of node tree branches.
    /// </remarks>
    public class HEJsonGameFile : HEJsonBaseFile
    {
        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="PassedFileInfo">The FileInfo representing the file to be loaded.</param>
        public HEJsonGameFile(FileInfo PassedFileInfo, object passedParentObject)
        {
            if (passedParentObject != null)
            {
                parent = (IHENotificationReceiver)passedParentObject;

            }
            else
            {
                throw new InvalidOperationException();
            }

            if (PassedFileInfo != null)
            {
                File = PassedFileInfo;
                rootNode = new HEGameDataTreeNode("SAVEFILE", HETreeNodeType.SaveFile, nodeText: File.Name, nodeToolTipText: File.FullName);

                if (File.Exists)
                {
                    LoadFile();
                    rootNode.Tag = jData;
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
        /// Populates the node tree from the jData
        /// </summary>
        public new void PopulateNodeTree()
        {
            // Populates the RootNode using the build function
            HETreeNode tn = BuildHETreeNodeTreeFromJson(jData, maxDepth: 5, collapseJArrays: false);
            rootNode.Nodes.Add(tn ?? new HETreeNode("LOADING ERROR!", HETreeNodeType.DataFileError));
        }

    } // End of class HEJsonGameFile
}
