using System;
using System.IO;
//using System.Runtime.CompilerServices;

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
        /// <param name="passedFileInfo">The FileInfo representing the file to be loaded.</param>
        public HEJsonGameFile(object passedParentObject, FileInfo passedFileInfo, int populateNodeTreeDepth) : base(passedParentObject, passedFileInfo, populateNodeTreeDepth)
        {
            rootNode.Name = File.Name;
            rootNode.NodeType = HETreeNodeType.SaveFile;
            rootNode.Text = File.Name;
            rootNode.ToolTipText = File.FullName;
        }
    }
}
