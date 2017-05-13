using System;
using System.IO;
using System.Collections.Generic; // for IEnumerable
using System.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HELLION.DataStructures
{
    public class HEDocumentWorkspace
    {
        // Definition for a HELLION Save (.save) JSON file
        // Includes methods for loading a save and assocaited data files in to memory, building a 
        // custom treenode tree representing the orbital objects and retrieving data from the tree
        // to populate the dynamic list and full data from the source.
        // Eventually will include modifying and saving out the data.

        //public string FileName { get; set; }
        public bool IsFileReady { get; private set; }
        public bool LoadError { get; private set; }
        public bool IsFileDirty { get; private set; }
        public bool LogToDebug { get; set; }

        // Define a custom tree node to become the root nodesfor the Nav Tree
        public HEOrbitalObjTreeNode RootNode { get; private set; }

        // Define the HEJsonFile object that holds the .save data file
        public HEGameFile MainFile { get; set; }

        // Define additional HEJsonFile objects to hold accompanying information
        public HEJsonFile DataFileCelestialBodies { get; set; }
        public HEJsonFile DataFileAsteroids { get; set; }
        public HEJsonFile DataFileStructures { get; set; }
        public HEJsonFile DataFileDynamicObjects { get; set; }
        // Other data file definitions would go here - these two are not yet implemented in game
        public HEJsonFile DataFileModules { get; set; }
        public HEJsonFile DataFileStations { get; set; }

        public HEDocumentWorkspace()
        {
            // Basic constructor
            //FileName = "";
            IsFileReady = false;
            LoadError = false;
            IsFileDirty = false;
            LogToDebug = false;

            RootNode = null;

            MainFile = new HEGameFile();
            DataFileCelestialBodies = new HEJsonFile();
            DataFileAsteroids = new HEJsonFile();
            DataFileStructures = new HEJsonFile();
            DataFileDynamicObjects = new HEJsonFile();
            DataFileModules = new HEJsonFile();
            DataFileStations = new HEJsonFile();
        }

        public void BuildSolarSystem()
        {
            // Builds the solar system
            AddCBTreeNodesRecursively(DataFileCelestialBodies.JData, nThisNode: null, lParentGUID: -1, iDepth: 10, bLogToDebug: LogToDebug);
        } // End of BuildSolarSystem()

        public void AddCBTreeNodesRecursively(
            JArray iCelestialBodies,
            HEOrbitalObjTreeNode nThisNode,
            long lParentGUID,
            int iDepth,
            bool bLogToDebug = false)
        {
            // Primary recursive function for adding Celestial Body objects as nodes to a node tree and creating one if it doesn't exist.

            if (bLogToDebug)
            {
                Debug.Print("======================================================================================");
                Debug.Print("AddCBTreeNodesRecursively entered with parentGUID: " + lParentGUID.ToString());
                Debug.Print("iDepth: " + iDepth.ToString());
            }

            // Check to see if we've reached the max depth
            if (iDepth < 1)
            {
                // Max depth reached, don't continue at this level
                if (bLogToDebug)
                {
                    Debug.Print("--------------------------------------------------------------------------------------");
                    Debug.Print("------------------ RECURSION PREVENTED DUE TO MAX DEPTH REACHED ----------------------");
                    Debug.Print("--------------------------------------------------------------------------------------");
                }
            }
            else
            {
                // Max depth not yet reached, continue

                // find the bodies that have a parentGUID of that match lParentGUID
                IOrderedEnumerable<JToken> iCBOffspring = iCelestialBodies.Where(b => (long)b["ParentGUID"] == lParentGUID)
                                                                    .OrderBy(b => (long)b["SemiMajorAxis"]);
                //.ToList<HECelestialBody>();
                if (bLogToDebug)
                {
                    Debug.Print(iCBOffspring.Count().ToString() + " children found");
                }

                if (iCBOffspring.Count() > 0)
                {
                    // The parent's GUID has matches indicating child bodies to process

                    // Loop through each child body in the iCelestialBodies list and add this node, then recurse
                    foreach (var cbChild in iCBOffspring)
                    {
                        if (bLogToDebug)
                        {
                            Debug.Print("Processing child: " + (string)cbChild["Name"] + " with GUID: " + (string)cbChild["GUID"]);
                        }

                        // Set up a new TreeNode which will be added to the TreeView control
                        HEOrbitalObjTreeNode nChildNode = new HEOrbitalObjTreeNode()
                        {
                            Name = (string)cbChild["Name"], // GUID.ToString();
                            NodeType = HETreeNodeType.CelestialBody,
                            GUID = (long)cbChild["GUID"],
                            ParentGUID = (long)cbChild["ParentGUID"],
                            SemiMajorAxis = (float)cbChild["SemiMajorAxis"],
                            Inclination = (float)cbChild["Inclination"],
                            Text = (string)cbChild["Name"],
                            Tag = cbChild,
                            ImageIndex = (int)HEObjectTypesImageList.Contrast_16x,
                            SelectedImageIndex = (int)HEObjectTypesImageList.Contrast_16x
                        };

                        //Check to see if nThisNode is null and lParentGUI is -1 representing a root node which gets handled differently
                        if (nThisNode == null && lParentGUID == -1)
                        {
                            // nThisNode was null, create root node, should only happen once
                            if (bLogToDebug)
                            {
                                Debug.Print("Creating ROOT node");
                                nChildNode.ImageIndex = (int)HEObjectTypesImageList.ButtonIcon_16x;
                                nChildNode.SelectedImageIndex = (int)HEObjectTypesImageList.ButtonIcon_16x;
                                RootNode = nChildNode;
                            }
                            // Place first node
                            //tvNavigationTree.Nodes.Add(nChildNode);
                        }
                        else if (nThisNode != null)
                        {
                            // nThisNode was not null, create child node

                            if (bLogToDebug)
                            {
                                Debug.Print("Creating CHILD node");
                            }

                            // add the node
                            nThisNode.Nodes.Add(nChildNode);

                        }

                        // Recursive call here!
                        if (bLogToDebug)
                        {
                            Debug.Print("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                            Debug.Print("Call: AddCBTreeNodesRecursively with GUID: " + (string)cbChild["GUID"]);
                            Debug.Print("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                        }
                        AddCBTreeNodesRecursively(iCelestialBodies, nChildNode, (long)cbChild["GUID"], iDepth - 1, bLogToDebug);
                        if (bLogToDebug)
                        {
                            Debug.Print("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                            Debug.Print("Return: AddCBTreeNodesRecursively with GUID: " + (string)cbChild["GUID"]);
                            Debug.Print("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                        }
                    }
                }
            }
            if (bLogToDebug)
            {
                Debug.Print("AddCBTreeNodesRecursively exiting with parentGUID: " + lParentGUID.ToString());
                Debug.Print("======================================================================================");
            }
        } // end of AddCBTreeNodesRecursively

        public void PopulateOrbitalObjects(HETreeNodeType ntAddNodesOfType)
        {
            // Populates the node tree control with ship objects from the .save file

            // Controls the maximum recursion depth to prevent runaway recursion
            int iMaxRecursionDepth = 10;

            // Used to hold the count of objects in file as loaded
            int iShipCount = 0;

            // Used to hold a string representation of the text key of the section name in the save file
            string sSectionName = "";

            switch (ntAddNodesOfType)
            {
                case HETreeNodeType.Ship:
                case HETreeNodeType.Asteroid:
                    // Process Ships and Asteroids in a similar manner
                    switch (ntAddNodesOfType)
                    {
                        case HETreeNodeType.Ship:
                            sSectionName = "Ships";
                            break;
                        case HETreeNodeType.Asteroid:
                            sSectionName = "Asteroids";
                            break;
                    }

                    // We've got a valid section name, proceed with selection of objects of specified type

                    IOrderedEnumerable<JToken> ioFilteredObjects = from s in MainFile.JData[sSectionName]
                                                                    orderby (long)s["OrbitData"]["ParentGUID"], (long)s["OrbitData"]["SemiMajorAxis"]
                                                                    select s;

                    foreach (var jtFiltObj in ioFilteredObjects)
                    {
                        iShipCount++;
                        Debug.Print("Name: " + (string)jtFiltObj["Name"] + " GUID: " + (string)jtFiltObj["GUID"] + " ParentGUID: " + (string)jtFiltObj["OrbitData"]["ParentGUID"]);
                    }


                    // start traversing the node tree recursively, starting from the root and adding children depth-first as it progresses
                    AddOrbitalObjTreeNodesRecursively(ioFilteredObjects, RootNode, ntAddNodesOfType, iMaxRecursionDepth, LogToDebug, iLogIndentLevel: 1);

                    break;

                case HETreeNodeType.Player:
                    //
                    sSectionName = "Players";

                    // We are dealing with player objects which are handled slightly differently

                    IOrderedEnumerable<JToken> ioPlayerObjects = from s in MainFile.JData[sSectionName]
                                                                    orderby (long)s["ParentGUID"] ///, (long)s["OrbitData"]["SemiMajorAxis"]
                                                                    select s;

                    foreach (var jtPlayerObject in ioPlayerObjects)
                    {
                        iShipCount++;
                        Debug.Print("Name: " + (string)jtPlayerObject["Name"] + " GUID: " + (string)jtPlayerObject["GUID"] + " ParentGUID: " + (string)jtPlayerObject["ParentGUID"]);
                    }

                    // start traversing the node tree recursively, starting from the root and adding children depth-first as it progresses
                    AddOrbitalObjTreeNodesRecursively(ioPlayerObjects, RootNode, ntAddNodesOfType, iMaxRecursionDepth, LogToDebug, iLogIndentLevel: 1);


                    break;
                default:
                    // There's a problem, not a supported node type

                    break;
            } // End of switch (ntAddNodesOfType)
        } // End of PopulateOrbitalObjects

        public void AddOrbitalObjTreeNodesRecursively(
            IOrderedEnumerable<JToken> openFileData,
            HEOrbitalObjTreeNode nThisNode,
            HETreeNodeType ntAddNodesOfType,
            int iDepth,
            bool bLogToDebug = false,
            int iLogIndentLevel = 0)
        {
            // Primary recursive function for adding game objects (but not celestial bodies) as nodes to the Nav Tree.

            // Set up indenting for this level
            string sIndent = String.Join("| ", new String[iLogIndentLevel]);

            // nThisNode prepresents the point at which this function starts from

            if (bLogToDebug)
            {
                Debug.Print(sIndent + "======================================================================================");
                Debug.Print(sIndent + "AddOrbitalObjTreeNodesRecursively entered, started processing node type {0} for HECelestialBody {1} GUID:{2} ParentGUID:{3}",
                    nThisNode.NodeType.ToString(), nThisNode.Name, nThisNode.GUID.ToString(), nThisNode.ParentGUID.ToString());
                Debug.Print(sIndent + "iDepth: " + iDepth.ToString());
                //Debug.Print()
            }

            // Check to see if we've reached the max depth - used to prevent runaway recursion on deep structures
            if (!(iDepth > 0))
            {
                // Max depth reached, don't continue at this level
                if (bLogToDebug)
                {
                    Debug.Print(sIndent + "------------------ RECURSION PREVENTED DUE TO MAX DEPTH REACHED ----------------------");
                }
            }
            else
            {
                // Max depth not yet reached, continue

                if (bLogToDebug)
                {
                    Debug.Print(sIndent + "Processing " + nThisNode.Nodes.Count.ToString() + " child nodes...");
                }

                // Define an IEnumerable object to hold this node's children
                IEnumerable<HEOrbitalObjTreeNode> IThisNodesChildren = null;

                if (ntAddNodesOfType == HETreeNodeType.Player)
                {
                    // Get all nodes as we're adding a player object
                    IThisNodesChildren = nThisNode.Nodes.Cast<HEOrbitalObjTreeNode>();
                }
                else
                {
                    // Get the child nodes that are Celestial Bodies
                    IThisNodesChildren = nThisNode.Nodes.Cast<HEOrbitalObjTreeNode>().Where(p => p.NodeType == HETreeNodeType.CelestialBody);
                }


                // Check for child nodes and recurse to each in turn - this is done before creating objects at this level
                if (IThisNodesChildren.Count() > 0)
                {
                    // There are child nodes (celcetial bodies) to process

                    // Loop through each child body in the iCelestialBodies list and recurse
                    foreach (HEOrbitalObjTreeNode nChildNode in IThisNodesChildren)
                    {
                        if (bLogToDebug)
                        {
                            Debug.Print(sIndent + "Preparing to recurse using Orbital Body: " + nChildNode.Name);
                        }

                        // Find the correct node in the Nav Tree for this cbChild

                        if (bLogToDebug)
                        {
                            Debug.Print(sIndent + "nChildNode GetNodeCount: " + nChildNode.GetNodeCount(includeSubTrees: false).ToString());
                        }

                        // Recursive call here!
                        if (bLogToDebug)
                        {
                            Debug.Print(sIndent + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                            Debug.Print(sIndent + ">> Call: AddOrbitalObjTreeNodesRecursively with GUID: " + nChildNode.GUID);
                            Debug.Print(sIndent + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                        }
                        AddOrbitalObjTreeNodesRecursively(openFileData, nChildNode, ntAddNodesOfType, iDepth - 1, bLogToDebug, iLogIndentLevel +1);

                        if (bLogToDebug)
                        {
                            Debug.Print(sIndent + "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                            Debug.Print(sIndent + "<< Return: AddOrbitalObjTreeNodesRecursively with GUID: " + nChildNode.GUID);
                            Debug.Print(sIndent + "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                        }
                    } // end of foreach (TreeNode nChildNode in nThisNode.Nodes)
                }
                else
                {
                    // Node Count was zero or less
                }

                // Branch to node creation code based on type
                switch (ntAddNodesOfType)
                {
                    case HETreeNodeType.Asteroid:
                    case HETreeNodeType.Ship:
                        // Process Ships and Asteroids similarly
                        // Find the OrbitalObjects for this ParentGUID
                        IOrderedEnumerable<JToken> ioOrbitalObjects = from s in openFileData
                                                                        where (long)s["OrbitData"]["ParentGUID"] == nThisNode.GUID
                                                                        orderby (long)s["OrbitData"]["SemiMajorAxis"]
                                                                        select s;

                        foreach (var jtOrbitalObject in ioOrbitalObjects)
                        {

                            if (bLogToDebug)
                            {
                                Debug.Print(sIndent + "Processing Orbital Body: {0} GUID[{1}] ParentGUID[{2}]", (string)jtOrbitalObject["Name"], (string)jtOrbitalObject["GUID"], (string)jtOrbitalObject["OrbitData"]["ParentGUID"]);
                            }

                            // Set up a new TreeNode which will be added to the TreeView control
                            HEOrbitalObjTreeNode nodeOrbitalObject = new HEOrbitalObjTreeNode()
                            {
                                Name = (string)jtOrbitalObject["Name"],
                                NodeType = ntAddNodesOfType,
                                Text = (string)jtOrbitalObject["Name"],
                                GUID = (long)jtOrbitalObject["GUID"],
                                ParentGUID = (long)jtOrbitalObject["OrbitData"]["ParentGUID"],
                                SemiMajorAxis = (float)jtOrbitalObject["OrbitData"]["SemiMajorAxis"],
                                Inclination = (float)jtOrbitalObject["OrbitData"]["Inclination"],
                                Tag = jtOrbitalObject
                            };

                            switch (ntAddNodesOfType)
                            {
                                case HETreeNodeType.Asteroid:
                                    nodeOrbitalObject.ImageIndex = (int)HEObjectTypesImageList.CheckDot_16x;
                                    nodeOrbitalObject.SelectedImageIndex = (int)HEObjectTypesImageList.CheckDot_16x;
                                    break;
                                case HETreeNodeType.Ship:
                                    nodeOrbitalObject.ImageIndex = (int)HEObjectTypesImageList.AzureLogicApp_16x;
                                    nodeOrbitalObject.SelectedImageIndex = (int)HEObjectTypesImageList.AzureLogicApp_16x;
                                    break;
                               
                            };

                            //Check and only continue if nThisNode is not null
                            if (nThisNode != null)
                            {
                                // nThisNode was not null, create child node

                                if (bLogToDebug)
                                {
                                    Debug.Print(sIndent + "Creating {0} node", ntAddNodesOfType.GetType());
                                }

                                // add the node
                                nThisNode.Nodes.Add(nodeOrbitalObject);
                            }
                            else
                            {
                                if (bLogToDebug)
                                {
                                    Debug.Print(sIndent + "!! NodeParent was NULL !!");
                                }
                            }
                        } // end of foreach (var jtOrbitalObject in ioOrbitalObjects)

                        break;

                    case HETreeNodeType.Player:
                        // Players get handled differently

                        // Find the OrbitalObjects for this ParentGUID
                        IOrderedEnumerable<JToken> ioPlayerObjects = from s in openFileData
                                                                        where (long)s["ParentGUID"] == nThisNode.GUID
                                                                        orderby (long)s["GUID"]
                                                                        select s;

                        foreach (var jtPlayerObject in ioPlayerObjects)
                        {

                            if (bLogToDebug)
                            {
                                Debug.Print(sIndent + "Processing Orbital Body: {0} GUID[{1}] ParentGUID[{2}]", (string)jtPlayerObject["Name"], (string)jtPlayerObject["GUID"], (string)jtPlayerObject["ParentGUID"]);
                            }

                            // Set up a new TreeNode which will be added to the TreeView control
                            HEOrbitalObjTreeNode nodeOrbitalObject = new HEOrbitalObjTreeNode()
                            {
                                Name = (string)jtPlayerObject["Name"],
                                NodeType = ntAddNodesOfType, 
                                Text = (string)jtPlayerObject["Name"],
                                GUID = (long)jtPlayerObject["GUID"],
                                ParentGUID = (long)jtPlayerObject["ParentGUID"],
                                Tag = jtPlayerObject,
                                ImageIndex = (int)HEObjectTypesImageList.Actor_16x,
                                SelectedImageIndex = (int)HEObjectTypesImageList.Actor_16x
                            };

                            //Check and only continue if nThisNode is not null
                            if (nThisNode != null)
                            {
                                // nThisNode was not null, create child node

                                if (bLogToDebug)
                                {
                                    Debug.Print(sIndent + "Creating {0} node", ntAddNodesOfType.GetType());
                                }

                                // add the node
                                nThisNode.Nodes.Add(nodeOrbitalObject);
                            }
                            else
                            {
                                if (bLogToDebug)
                                {
                                    Debug.Print(sIndent + "!! NodeParent was NULL !!");
                                }
                            }
                        } // end of foreach (var jtPlayerObject in ioPlayerObjects)

                        break;

                    default:
                        break;
                }



            }
            if (bLogToDebug)
            {
                Debug.Print(sIndent + "AddOrbitalObjTreeNodesRecursively exited, finished processing node type {0} for HECelestialBody {1} GUID:{2} ParentGUID:{3}",
                    nThisNode.NodeType.ToString(), nThisNode.Name, nThisNode.GUID.ToString(), nThisNode.ParentGUID.ToString());
                Debug.Print(sIndent + "======================================================================================");
            }
        } // end of AddOrbitalObjTreeNodesRecursively


        public bool LoadFile()
        {
            // Load file data from FileName and parse to the JData JObject of type IOrderedEnumerable<JToken>
            // Returns true if there was a loading error

            if (File.Exists(MainFile.FileName))
            {
                // do stuff

                // Apply the LogToDebug setting to all HEJsonFile objects
                MainFile.LogToDebug = LogToDebug;
                DataFileCelestialBodies.LogToDebug = LogToDebug;
                DataFileAsteroids.LogToDebug = LogToDebug;
                DataFileStructures.LogToDebug = LogToDebug;
                DataFileDynamicObjects.LogToDebug = LogToDebug;
                DataFileStructures.LogToDebug = LogToDebug;
                DataFileStations.LogToDebug = LogToDebug;



                //Load Main File
                MainFile.LoadFile();

                // Load supplementary data files. Each decides whether to skip loading based on it's own setting
                // The Celestial Bodies file is CRITICAL - it provides the framework for the all orbital objects in the save file
                DataFileCelestialBodies.LoadFile();
                DataFileAsteroids.LoadFile();
                DataFileStructures.LoadFile();
                DataFileDynamicObjects.LoadFile();
                DataFileModules.LoadFile();
                DataFileStations.LoadFile();

                // Boolean OR the load errors to catch if a LoadError was detected
                LoadError = LoadError || DataFileCelestialBodies.LoadError || DataFileAsteroids.LoadError || DataFileStructures.LoadError
                    || DataFileDynamicObjects.LoadError || DataFileModules.LoadError || DataFileStations.LoadError;

                if (LoadError && LogToDebug)
                {
                    // Report on errors if LogToDebug is true
                    if (MainFile.LoadError) Debug.Print("MainFile.LoadError");
                    if (DataFileCelestialBodies.LoadError) Debug.Print("DataFileCelestialBodies.LoadError");
                    if (DataFileAsteroids.LoadError) Debug.Print("DataFileAsteroids.LoadError");
                    if (DataFileStructures.LoadError) Debug.Print("DataFileStructures.LoadError");
                    if (DataFileDynamicObjects.LoadError) Debug.Print("DataFileDynamicObjects.LoadError");
                    if (DataFileModules.LoadError) Debug.Print("DataFileModules.LoadError");
                    if (DataFileStations.LoadError) Debug.Print("DataFileStations.LoadError");
                }

                // Build master node tree
                BuildSolarSystem();

                // Add other orbital objects
                PopulateOrbitalObjects(ntAddNodesOfType: HETreeNodeType.Asteroid);
                PopulateOrbitalObjects(ntAddNodesOfType: HETreeNodeType.Ship);
                PopulateOrbitalObjects(ntAddNodesOfType: HETreeNodeType.Player);

                IsFileReady = true;

            }
            else
            {
                // Invalid file name
                LoadError = true;

                if (LogToDebug) Debug.Print("Invalid file name passed to HEDocumentWorkspace");

            }

            // Return the value of LoadError
            return LoadError;
        } // End of LoadFile()


        public void CloseFile()
        {
            // Not yet implemented
        }

    } // End of class HEDocumentWorkspace
} // End of namespace HELLION
