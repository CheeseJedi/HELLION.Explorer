using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms; // for the TreeNodeCollection

using System.Diagnostics;

namespace HELLION.DataStructures
{
    // Define a custom class to hold data from a JSON file and associated metadata

    public class HEJsonFile
    {
        // Base class for a generic JSON data file

        public string FileName { get; set; }
        public JArray JData { get; set; } // Data files use JArrays (master save is loaded in to a JObject instead)

        // Externally read-only properties
        public bool IsFileLoaded { get; set; }
        public bool LoadError { get; set; }
        public bool SkipLoading { get; set; }

        public bool LogToDebug { get; set; }


        public HEJsonFile()
        {
            // Basic Constructor
            FileName = "";
            JData = null;
            IsFileLoaded = false;
            LoadError = false;
            SkipLoading = false;
            LogToDebug = false;
        }

        public HEJsonFile(string sFileName)
        {
            // Constructor that allows the file name to be set
            FileName = sFileName;
            JData = null;
            IsFileLoaded = false;
            LoadError = false;
            SkipLoading = false;
            LogToDebug = false;
        }

        public bool LoadFile()
        {
            // Load file data from FileName and parse to the JData JObject of type IOrderedEnumerable<JToken>
            // Returns true if there was a loading error
            if (!SkipLoading)
            {
                if (System.IO.File.Exists(FileName))
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(FileName))
                        {
                            // Process the stream with the JSON Text Reader in to a JArray; was previously an IOrderedEnumerable<JToken> JObject
                            using (JsonTextReader jtr = new JsonTextReader(sr))
                            {
                                JData = (JArray)JToken.ReadFrom(jtr);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // Some error handling to be implemented here
                        LoadError = true;
                        if (LogToDebug) Debug.Print("Exception caught during StreamReader or JsonTextReader while processing " + FileName
                            + Environment.NewLine + e.ToString());
                    }

                    if (JData == null || JData.Count == 0)
                    {
                        // The data didn't load
                        LoadError = true;
                        if (LogToDebug) Debug.Print("JData is null or empty: " + FileName);
                    }
                    else
                    {
                        // We should have some data in the array
                        IsFileLoaded = true;
                        if (LogToDebug) Debug.Print("Processed " + JData.Count + " objects from: " + FileName);
                    }
                }
                else
                {
                    // Invalid file name
                    LoadError = true;
                    if (LogToDebug) Debug.Print("Invalid file name passed: " + FileName);
                }
            }
            else
            {
                if (LogToDebug) Debug.Print("Skipping file: " + FileName);
            } // End of SkipLoading check

            // Return the value of LoadError
            return LoadError;
        } // End of LoadFile()

        
        public HETreeNode BuildNodeCollection(HETreeNodeType nodeType)
        {
            // Builds and returns a HETreeNode whose Node Collection contains HETreeNode type nodes
            // based on the data stored in this JSON file - only for use on Data files currently
            // Returns null instead of an empty collection

            // Define an HETreeNode node to serve as the parent for this collection of data objects
            HETreeNode nodeRoot = new HETreeNode();

            if (IsFileLoaded && !LoadError)
            {

                // Get the index of the image associated with this node type
                int iImageIndex = HEUtilities.GetImageIndexByNodeType(nodeType);

                foreach (JToken dataItem in JData)
                {
                    // Set up a new HETreeNode with data from this object and the type as passed in
                    // via nodeType

                    HETreeNode nodeDataItem = new HETreeNode()
                    {
                        Name = (string)dataItem["Name"],
                        NodeType = nodeType,
                        Text = (string)dataItem["ItemID"] + " " + (string)dataItem["Name"] + (string)dataItem["GameName"],
                        Tag = dataItem,
                        ImageIndex = iImageIndex,
                        SelectedImageIndex = iImageIndex,
                    };

                    // Add nodeDataItem to the nodeRoot's Nodes collection
                    nodeRoot.Nodes.Add(nodeDataItem);

                }
                // Update the count of objects for the root node
                nodeRoot.UpdateCounts();

            }
            // Return the node with child objects in the Nodes collection
            return nodeRoot;
        }
        
    } // End of class HEjsonFile
} // End of namespace HELLION.DataStructures
