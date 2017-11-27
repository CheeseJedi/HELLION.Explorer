using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace HELLION.DataStructures
{
    // Defines a class to load and hold data from a JSON file and associated metadata

    // This is a re-write intended to encapsulate more of the functionality of building
    // node trees of the correct type and enabling (delayable) expansion of objects

    public class HEJsonBaseFile
    {
        // Base class for a generic JSON data file

        //public string FileName { get; set; }
        public FileInfo File { get; set; }
        public JToken JData { get; set; }
        public HETreeNode dataFileRoot { get; set; }
        public bool IsFileLoaded { get; set; }
        public bool IsFileWritable { get; set; }
        public bool LoadError { get; set; }
        public bool SkipLoading { get; set; }
        public bool LogToDebug { get; set; }


        public HEJsonBaseFile()
        {
            // Basic Constructor
            //FileName = "";
            File = null;
            JData = null;
            IsFileLoaded = false;
            IsFileWritable = false;
            LoadError = false;
            SkipLoading = false;
            LogToDebug = false;
            dataFileRoot = new HETreeNode("DATAFILE", HETreeNodeType.DataFile);


        }

        public HEJsonBaseFile(string PassedFileName)
        {
            // Constructor that allows the file name to be set and triggers load
            //FileName = sFileName;
            File = null;
            JData = null;
            IsFileLoaded = false;
            LoadError = false;
            SkipLoading = false;
            LogToDebug = false;
            dataFileRoot = null;

            if (System.IO.File.Exists(PassedFileName))
            {
                File = new FileInfo(PassedFileName);
                if (File.Exists)
                {
                    dataFileRoot = new HETreeNode("DATAFILE", HETreeNodeType.DataFile, nodeText: File.Name, nodeToolTipText: File.FullName);
                    LoadFile();
                }
            }
        }

        public HEJsonBaseFile(FileInfo PassedFileInfo)
        {
            // Constructor that allows the FileInfo to be passed
            //FileName = sFileName;
            JData = null;
            IsFileLoaded = false;
            LoadError = false;
            SkipLoading = false;
            LogToDebug = false;

            if (PassedFileInfo != null)
            {
                File = PassedFileInfo;
                if (File.Exists)
                    LoadFile();
            }
        }




        public bool LoadFile()
        {
            // Load file data from FileName and parse to the JData JObject of type IOrderedEnumerable<JToken>
            // Returns true if there was a loading error
            if (!SkipLoading)
            {
                if (File.Exists)
                {
                    try
                    {
                        using (StreamReader sr = File.OpenText())
                        {
                            // Process the stream with the JSON Text Reader in to a JToken
                            // this was previously using Array; was previously an IOrderedEnumerable<JToken> JObject
                            using (JsonTextReader jtr = new JsonTextReader(sr))
                            {
                                JData = JToken.ReadFrom(jtr);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // Some error handling to be implemented here
                        LoadError = true;
                        if (LogToDebug) Debug.Print("Exception caught during StreamReader or JsonTextReader while processing " + File.Name
                            + Environment.NewLine + e.ToString());
                    }

                    if (JData == null)                   
                    {
                        // The data didn't load
                        LoadError = true;
                        if (LogToDebug) Debug.Print("JData is null or empty: " + File.Name);
                    }
                    else
                    {
                        // We should have some data

                        if (LogToDebug)
                        {
                            int numObj = 0;
                            Debug.Print("Token Type: " + JData.Type.ToString());
                            if (JData.Type == JTokenType.Array || JData.Type == JTokenType.Object)
                            {
                                numObj = JData.Count();
                                Debug.Print(File.Name + " is detected as an " + JData.Type.ToString() + ", " + numObj.ToString() + " JTokens loaded.");
                            }
                            else
                                Debug.Print("ERROR: JData is detected as neither an ARRAY or OBJECT!");
                        }
                        // Set the IsFileLoaded flag to true
                        IsFileLoaded = true;
                    }
                }
                else
                {
                    // Invalid file name
                    LoadError = true;
                    if (LogToDebug) Debug.Print("Invalid file name passed: " + File.Name);
                }
            }
            else
            {
                if (LogToDebug) Debug.Print("Skipping file: " + File.Name);
            } // End of SkipLoading check

            // Return the value of LoadError
            return LoadError;
        } // End of LoadFile()

        public void SaveFile()
        {
            // Will save this file (not yet implemented)

            if (false)
            {
                //
            }
            /*
            // Check to see if this fle already exists
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
                IsFileDirty = false;
            }

            */

        } // End of SaveFile()

        public void MakeReadWrite()
        {
            // Changes the IsFileWritable to true - may need some additional checks here though
            if (IsFileLoaded && !LoadError)
            {
                IsFileWritable = true;
            }
            else
            {
                IsFileWritable = false;
            }
        } // End of MakeReadWrite()

        public void MakeReadOnly()
        {
            // Changes the IsFileWritable to false
            IsFileWritable = false;
        } // End of MakeReadOnly()



        public HETreeNode BuildNodeCollection(HETreeNodeType dummy, [CallerMemberName] string callerName = "")
        {
            // Builds and returns a HETreeNode whose Node Collection contains HETreeNode type nodes
            // based on the data stored in this JSON file - only for use on Data files currently
            // Returns null instead of an empty collection
        

            if (LogToDebug)
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

            if (IsFileLoaded && !LoadError)
            {

                int iImageIndex = 0;

                if (LogToDebug)
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
                        if (LogToDebug)
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
                            if (LogToDebug)
                                Debug.Print("The property value contains a value");

                            nodeDataItem.NodeType = HETreeNodeType.JsonValue;
                            nodeDataItem.ImageIndex = nodeDataItem.SelectedImageIndex = HEUtilities.GetImageIndexByNodeType(nodeDataItem.NodeType);

                        }



                        if (PropertyValue.Type == JTokenType.Array)
                        {
                            if (LogToDebug)
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
                            if (LogToDebug)
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
                                if (LogToDebug)
                                    Debug.Print("Property name " + PropertyName + " added");






                                //JProperty athisProperty = (JProperty)thing;
                                //subPropertyName = (string)thing["Name"];
                                /*
                                JToken token = thing["Registration"];
                                if (token != null)
                                {
                                    aPropertyName += " " + (string)thing["Registration"];
                                }
                                */
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
                        if (LogToDebug)
                            Debug.Print("It's an object");

                    }

                    else
                    {
                        if (LogToDebug)
                            Debug.Print("Not a property or an object!");
                    }



                }
                // Update the count of objects for the root node
                nodeRoot.UpdateCounts();

            }
            // Return the node with child objects in the Nodes collection
            return nodeRoot;
        }

        public void BuildBasicNodeTreeFromJson(JToken json, HETreeNode nodeParent, int maxDepth = 1, string nameHint = "", bool collapseJArrays = false , bool logToDebug = false)
        {
            // Builds a section of node tree and recurses if necessary
            if (logToDebug)
            {
                Debug.Print("BNTFJ {0} ENTERED with type: {1}", maxDepth, json.Type);
            }


            if (json != null) // prob needs more tests that this
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
                                        if (logToDebug) Debug.Print("BNTFJ {0} Recursing with type: {1}", maxDepth, token.Type);
                                        BuildBasicNodeTreeFromJson(token, newNode, maxDepth - 1, logToDebug: logToDebug);
                                        if (logToDebug) Debug.Print("BNTFJ {0} Back with type: {1}", maxDepth, token.Type);
                                        // Add the node
                                        nodeParent.Nodes.Add(newNode);
                                    }
                                }
                            }
                             
                        }
                        else
                        {
                            // We shouldn't be here!
                            if (logToDebug) Debug.Print("tmpJObject was null");
                        }
                        break;

                    case JTokenType.Array: // It's a JArray
                        // Depth and null check, if valid, create this node
                        JArray tmpJArray = (JArray)json;
                        if (tmpJArray != null)
                        {
                            if (maxDepth <= 0) // maxDepth (zero) has been reached, offer expansion if this has child tokens
                            {
                                // Count children
                                int numChildTokens = 0;
                                foreach (JToken token in tmpJArray) // .Children<JToken>()
                                    numChildTokens++;
                                // If it has child tokens
                                if (numChildTokens > 0)
                                {
                                    // attach a single expansion node to the tree - this will be removed when the actual tree is generated
                                    if (logToDebug) Debug.Print("BNTFJ {0} Adding expansion node", maxDepth);
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
                                foreach (JToken token in tmpJArray) // .Children<JToken>()
                                {
                                    if (logToDebug) Debug.Print("BNTFJ {0} Recursing with type: {1}", maxDepth, token.Type);
                                    // collapsearrays check to go here
                                    if (collapseJArrays)
                                    {
                                        // Adjust parent node instead of using the generated node
                                        // Changes the nodetype to represent an array
                                        //nodeParent.NodeType = HETreeNodeType.JsonArray; // HETreeNodeType.JsonArray;
                                        //nodeParent.UpdateImageIndeces();
                                        // Recursive call, but don't decrease the maxDepth as collapseArrays is true
                                        BuildBasicNodeTreeFromJson(token, nodeParent, maxDepth, logToDebug: logToDebug);
                                    }
                                    else
                                    {
                                        BuildBasicNodeTreeFromJson(token, newNode, maxDepth - 1, logToDebug: logToDebug);
                                    }
                                    if (logToDebug) Debug.Print("BNTFJ {0} Back with type: {1}", maxDepth, token.Type);
                                    // Add the node
                                    nodeParent.Nodes.Add(newNode);
                                }
                            }
                        }
                        else
                        {
                            // We shouldn't be here!
                            if (logToDebug) Debug.Print("tmpJArray was null");
                        }
                        break;

                    case JTokenType.Property: // Ity's a JProperty, similar to a key value pair
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

                                if (logToDebug) Debug.Print("BNTFJ {0} Recursing with type: {1}", maxDepth, tmpJProperty.Type);

                                BuildBasicNodeTreeFromJson(tmpJProperty.Value, newNode, maxDepth - 1, logToDebug: logToDebug);

                                if (logToDebug) Debug.Print("BNTFJ {0} Back with type: {1}", maxDepth, tmpJProperty.Type);

                                // Add the node
                                nodeParent.Nodes.Add(newNode);
                            }
                            else
                            {
                                Debug.Print("### JProperty maxDepth {0}", maxDepth);
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
                                    // Bool (checkdot)
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
                                // Update the imageindex and selectedimageindex directly - we're overriding the standard icons
                                ImageIndex = newNodeImageIndex,
                                SelectedImageIndex = newNodeImageIndex,
                            };
                            nodeParent.Nodes.Add(newNode);

                        }
                        break;

                    default:
                        //
                        if (logToDebug)
                        {
                            Debug.Print("BNTFJ {0} VALUE, type: {1} - NO ACTION TAKEN!", maxDepth, json.Type);
                            Debug.Print(json.ToString());
                        }
                        break;
                }
            }
            //}
            /*
            else
            {
                if (logToDebug)
                    Debug.Print("BNTFJ {0} RECURSION LIMIT with type: {1}", maxDepth, json.Type);

            }
            */

            if (logToDebug)
                Debug.Print("BNTFJ {0} EXITING with type: {1}", maxDepth, json.Type);
        }


    } // End of class HEjsonFile
} // End of namespace HELLION.DataStructures
