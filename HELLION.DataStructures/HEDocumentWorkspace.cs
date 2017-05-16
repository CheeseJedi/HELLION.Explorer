using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic; // for IEnumerable
using System.Linq;
using System.Diagnostics;
//using System.Windows.Forms;
//using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.HEUtilities;

namespace HELLION.DataStructures
{
    public class HEDocumentWorkspace
    {
        // Definition for a HELLION Save (.save) JSON file
        // Includes methods for loading a save and assocaited data files in to memory, building a 
        // custom treenode tree representing the orbital objects and retrieving data from the tree
        // to populate the dynamic list and full data from the source.
        // Eventually will include modifying and saving out the data.

        public bool IsFileReady { get; private set; }
        public bool LoadError { get; private set; }
        public bool IsFileDirty { get; private set; }
        public bool LogToDebug { get; set; }
        public bool UseScenes { get; set; }


        // Define a custom tree node to become the root node for the Nav Tree
        public HEOrbitalObjTreeNode NavRootNode { get; private set; }

        // Define a custom tree node to become the root node for the Solar System tree
        public HEOrbitalObjTreeNode SolarSystemRootNode { get; private set; }
        
        // Define a custom tree node to become the root node for the Search Results tree
        public HEOrbitalObjTreeNode SearchResultsRootNode { get; private set; }


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
            UseScenes = false;

            SolarSystemRootNode = null;

            MainFile = new HEGameFile();
            DataFileCelestialBodies = new HEJsonFile();
            DataFileAsteroids = new HEJsonFile();
            DataFileStructures = new HEJsonFile();
            DataFileDynamicObjects = new HEJsonFile();
            DataFileModules = new HEJsonFile();
            DataFileStations = new HEJsonFile();
        }

        public Color ConvertStringToColor(string sInputString)
        {
            // Returns a System.Drawing.Color object for a given string, computed from the hash of the string

            int iHue = (Math.Abs(sInputString.GetHashCode()) % 24) * 10;
            // Debug.Print("iHue: {0}", iHue.ToString());

            HSLColor hslColor = new HSLColor(hue: iHue, saturation: 200.0, luminosity: 80.0);
            return hslColor;

        }

        public void BuildSolarSystem(HEOrbitalObjTreeNode nStartingPoint)
        {
            // Builds a tree of nodes representing the solar system and attaches it to the Starting Point on an existing node tree


            AddCBTreeNodesRecursively(DataFileCelestialBodies.JData, nThisNode: nStartingPoint, lParentGUID: -1, iDepth: 10, bLogToDebug: LogToDebug);
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

                        // Set up a new HEOrbitalData object to hold whatever orbital data we have available
                        HEOrbitalData Data = new HEOrbitalData()
                        {
                            ParentGUID = (long)cbChild["ParentGUID"],
                            SemiMajorAxis = (float)cbChild["SemiMajorAxis"],
                            Inclination = (float)cbChild["Inclination"],
                            Eccentricity = (float)cbChild["Eccentricity"],
                            ArgumentOfPeriapsis = (float)cbChild["ArgumentOfPeriapsis"],
                            LongitudeOfAscendingNode = (float)cbChild["LongitudeOfAscendingNode"]
                        };

                        // Set up a new custom TreeNode which will be added to the node tree
                        HEOrbitalObjTreeNode nChildNode = new HEOrbitalObjTreeNode()
                        {
                            Name = (string)cbChild["Name"], // GUID.ToString();
                            NodeType = HETreeNodeType.CelestialBody,
                            GUID = (long)cbChild["GUID"],


                            ParentGUID = (long)cbChild["ParentGUID"],
                            SemiMajorAxis = (float)cbChild["SemiMajorAxis"],
                            Inclination = (float)cbChild["Inclination"],
                            OrbitData = Data,


                            //OrbitData
                            Text = (string)cbChild["Name"],
                            Tag = cbChild,
                            ImageIndex = HEUtilities.GetImageIndexByOrbitalObjectType(HETreeNodeType.CelestialBody),
                            SelectedImageIndex = HEUtilities.GetImageIndexByOrbitalObjectType(HETreeNodeType.CelestialBody)
                        };


                        // maybe more changes needed here \/  \/  \/


                        //Check to see if nThisNode is null and lParentGUI is -1 representing a root node which gets handled differently
                        if (nThisNode == null && lParentGUID == -1) // <<<<< Possibly defunct
                        {
                            // nThisNode was null, create root node, should only happen once  
                            if (bLogToDebug)
                            {
                                Debug.Print("Creating ROOT Star node");
                                nChildNode.ImageIndex = (int)HEObjectTypesImageList.ButtonIcon_16x;
                                nChildNode.SelectedImageIndex = (int)HEObjectTypesImageList.ButtonIcon_16x;
                                SolarSystemRootNode = nChildNode;
                            }
                        }                        
                        else if (nThisNode != null)
                        {
                            // nThisNode was not null, create child node, normal method of operation

                            // Check if it's the star rather than other planets
                            if (lParentGUID == -1)
                            {
                                // The HELLION solar system root is the star, Hellion, and has a ParentGUID of -1
                                // Create Star with different icon
                                if (bLogToDebug)
                                {
                                    Debug.Print("Creating Star node");
                                    nChildNode.ImageIndex = (int)HEObjectTypesImageList.ButtonIcon_16x;
                                    nChildNode.SelectedImageIndex = (int)HEObjectTypesImageList.ButtonIcon_16x;
                                }
                            }
                            else if (bLogToDebug)
                            {
                                Debug.Print("Creating CHILD node");
                            }

                            // Add the node
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

        public void PopulateOrbitalObjects(HETreeNodeType ntAddNodesOfType, bool bAddScenes)
        {
            // Populates a given node tree with objects of a given type from the .save file

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
                    AddOrbitalObjTreeNodesRecursively(ioFilteredObjects, SolarSystemRootNode, ntAddNodesOfType, bAddScenes ,iMaxRecursionDepth, LogToDebug, iLogIndentLevel: 1);

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
                    AddOrbitalObjTreeNodesRecursively(ioPlayerObjects, SolarSystemRootNode, ntAddNodesOfType, bAddScenes, iMaxRecursionDepth, LogToDebug, iLogIndentLevel: 1);


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
            bool bAddScenes,
            int iDepth,
            bool bLogToDebug = false,
            int iLogIndentLevel = 0)
        {
            // Primary recursive function for adding game objects (but not celestial bodies) as nodes to the Nav Tree.

            // Set up indenting for this level
            string sIndent = String.Join("| ", new String[iLogIndentLevel]);

            // Get the index of the image associated with this node type
            int iImageIndex = HEUtilities.GetImageIndexByOrbitalObjectType(ntAddNodesOfType);

            // nThisNode prepresents the point at which this function starts from
            //Check and only continue if nThisNode is not null
            if (nThisNode != null)
            {
                // nThisNode was not null, continue processing

                if (bLogToDebug)
                {
                    Debug.Print(sIndent + "======================================================================================");
                    Debug.Print(sIndent + "AddOrbitalObjTreeNodesRecursively entered, started processing node type {0} for HECelestialBody {1} GUID:{2} ParentGUID:{3}",
                        ntAddNodesOfType.ToString(), nThisNode.Name, nThisNode.GUID.ToString(), nThisNode.OrbitData.ParentGUID.ToString());
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

                        // Loop through each child body in the iCelestialBodies list and RECURSE
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
                            AddOrbitalObjTreeNodesRecursively(openFileData, nChildNode, ntAddNodesOfType, bAddScenes, iDepth - 1, bLogToDebug, iLogIndentLevel + 1);

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
                        {
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
                                    Debug.Print(sIndent + "Processing object type {0} for Orbital Body: {1} GUID[{2}] ParentGUID[{3}]", ntAddNodesOfType.ToString(), (string)jtOrbitalObject["Name"], (string)jtOrbitalObject["GUID"], (string)jtOrbitalObject["OrbitData"]["ParentGUID"]);
                                }

                                // Set up a new HEOrbitalData object to hold whatever orbital data we have available
                                // and will be cloned if we're creating the Scene node also
                                HEOrbitalData OrbitalObjectData = new HEOrbitalData()
                                {
                                    ParentGUID = (long)jtOrbitalObject["OrbitData"]["ParentGUID"],
                                    SemiMajorAxis = (float)jtOrbitalObject["OrbitData"]["SemiMajorAxis"],
                                    Inclination = (float)jtOrbitalObject["OrbitData"]["Inclination"],
                                    Eccentricity = (float)jtOrbitalObject["OrbitData"]["Eccentricity"],
                                    ArgumentOfPeriapsis = (float)jtOrbitalObject["OrbitData"]["ArgumentOfPeriapsis"],
                                    LongitudeOfAscendingNode = (float)jtOrbitalObject["OrbitData"]["LongitudeOfAscendingNode"]
                                };

                                // Create a new TreeNode representing the object we're adding
                                HEOrbitalObjTreeNode nodeOrbitalObject = new HEOrbitalObjTreeNode()
                                {
                                    Name = (string)jtOrbitalObject["Name"],
                                    NodeType = ntAddNodesOfType,
                                    Text = (string)jtOrbitalObject["Name"],
                                    GUID = (long)jtOrbitalObject["GUID"],
                                    ParentGUID = (long)jtOrbitalObject["OrbitData"]["ParentGUID"], // to be removed
                                    SemiMajorAxis = (float)jtOrbitalObject["OrbitData"]["SemiMajorAxis"], // to be removed
                                    Inclination = (float)jtOrbitalObject["OrbitData"]["Inclination"], // to be removed
                                    // Generate the foreground colour
                                    //ForeColor = ConvertStringToColor((string)jtOrbitalObject["Name"]),
                                    ImageIndex = iImageIndex,
                                    SelectedImageIndex = iImageIndex,
                                    SceneID = (int)jtOrbitalObject["SceneID"],
                                    Type = (int)jtOrbitalObject["Type"],
                                    OrbitData = OrbitalObjectData,
                                    Tag = jtOrbitalObject
                                };

                                // Set Image index for the node we're adding
                                switch (ntAddNodesOfType)
                                {
                                    //case HETreeNodeType.Asteroid:
                                        //nodeOrbitalObject.ImageIndex = (int)HEObjectTypesImageList.CheckDot_16x;
                                        //nodeOrbitalObject.SelectedImageIndex = (int)HEObjectTypesImageList.CheckDot_16x;
                                        //break;
                                    case HETreeNodeType.Ship:
                                        //nodeOrbitalObject.ImageIndex = (int)HEObjectTypesImageList.AzureLogicApp_16x;
                                        //nodeOrbitalObject.SelectedImageIndex = (int)HEObjectTypesImageList.AzureLogicApp_16x;

                                        if ((string)jtOrbitalObject["OrbitData"]["VesselID"] != "")
                                        {
                                            //OrbitalObjectData.VesselID = (long)jtOrbitalObject["OrbitData"]["VesselID"];
                                        }

                                        if ((string)jtOrbitalObject["OrbitData"]["VesselType"] != "")
                                        {
                                            //OrbitalObjectData.VesselType = (long)jtOrbitalObject["OrbitData"]["VesselType"];
                                        }

                                        break;

                                };

                                // Define a node to represent where we're adding the new node to
                                // This also becomes the Scene node if we're creating one
                                HEOrbitalObjTreeNode nodeMountPoint = null;

                                // Branching logic determining whether we're adding a node to a scene or a celestial body
                                if (bAddScenes)
                                {

                                    //Find out if a Scene node exists for this jtOrbitalObject

                                    // Get the child nodes that are Scenes, if any, and filter by SceneID
                                    IEnumerable<HEOrbitalObjTreeNode> IThisNodesScenes = nThisNode.Nodes.Cast<HEOrbitalObjTreeNode>()
                                        .Where(p => (p.NodeType == HETreeNodeType.Scene) & (p.SceneID == (int)jtOrbitalObject["SceneID"]));

                                    if (IThisNodesScenes.Count() > 0)
                                    {
                                        // We found a match, scene node already exists, select it
                                        nodeMountPoint = IThisNodesScenes.First();

                                        if (bLogToDebug)
                                        {
                                            Debug.Print(sIndent + "Using existing scene node SceneID {0}", nodeMountPoint.Text);
                                        }
                                    }
                                    else
                                    {
                                        // No match found, create a Scene node to hold the jtOrbitalObject

                                        int iSceneImageIndex = HEUtilities.GetImageIndexByOrbitalObjectType(HETreeNodeType.Scene);


                                        // Set up a new TreeNode for the scene which will be added to the node tree ahead of the object node
                                        nodeMountPoint = new HEOrbitalObjTreeNode()
                                        {
                                            Name = "SceneID_" + (string)jtOrbitalObject["SceneID"],
                                            NodeType = HETreeNodeType.Scene,
                                            Text = "Scene " + (string)jtOrbitalObject["SceneID"],
                                            //GUID = (long)jtOrbitalObject["GUID"],
                                            ParentGUID = (long)jtOrbitalObject["OrbitData"]["ParentGUID"],
                                            SemiMajorAxis = (float)jtOrbitalObject["OrbitData"]["SemiMajorAxis"],
                                            Inclination = (float)jtOrbitalObject["OrbitData"]["Inclination"],
                                            // Generate the foreground colour
                                            ForeColor = ConvertStringToColor("SceneID_" + (string)jtOrbitalObject["Name"]),
                                            SceneID = (int)jtOrbitalObject["SceneID"],
                                            Type = (int)jtOrbitalObject["Type"],
                                            OrbitData = OrbitalObjectData.Clone(), // use the custom Clone method to clone the OrbitalData object instead of redefining
                                            ImageIndex = iSceneImageIndex,
                                            SelectedImageIndex = iSceneImageIndex

                                            //Tag = jtOrbitalObject
                                        };

                                        if (bLogToDebug)
                                        {
                                            Debug.Print(sIndent + "Creating new scene node SceneID {0}", nodeMountPoint.Text);
                                        }

                                        // Add the nodeScene node to the nThisNode.Nodes collection
                                        nThisNode.Nodes.Add(nodeMountPoint);
                                        
                                    }
                                }
                                else
                                {
                                    // We're not adding objects by scenes so set the mount point to the parent
                                    nodeMountPoint = nThisNode;
                                }


                                // Continue processing the Orbital Object
                                // add the node
                                nodeMountPoint.Nodes.Add(nodeOrbitalObject);

                            } // End of foreach (var jtOrbitalObject in ioOrbitalObjects)

                            break;
                        }
                        //case HETreeNodeType.DynamicObject:
                        case HETreeNodeType.Player:
                        {
                            // Players get handled differently

                            // We (currently) only add players to Ship objects
                            if (nThisNode.NodeType == HETreeNodeType.Ship)
                            {
                                //

                                // Find the OrbitalObjects for this ParentGUID
                                IOrderedEnumerable<JToken> ioPlayerObjects = from s in openFileData
                                                                                where (long)s["ParentGUID"] == nThisNode.GUID
                                                                                orderby (long)s["GUID"]
                                                                                select s;

                                foreach (var jtPlayerObject in ioPlayerObjects)
                                {

                                    if (bLogToDebug)
                                    {
                                        Debug.Print(sIndent + "Processing Player {0} GUID[{1}] ParentGUID[{2}]", (string)jtPlayerObject["Name"], (string)jtPlayerObject["GUID"], (string)jtPlayerObject["ParentGUID"]);
                                    }

                                    // Set up a new TreeNode which will be added to the node tree
                                    HEOrbitalObjTreeNode nodeOrbitalObject = new HEOrbitalObjTreeNode()
                                    {
                                        Name = (string)jtPlayerObject["Name"],
                                        NodeType = ntAddNodesOfType,
                                        Text = (string)jtPlayerObject["Name"],
                                        GUID = (long)jtPlayerObject["GUID"],
                                        //ParentGUID = (long)jtPlayerObject["ParentGUID"],
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
                                            Debug.Print(sIndent + "!!!!FINDME!!!! Creating {0} node", ntAddNodesOfType.GetType());
                                        }

                                        // add the node
                                        nThisNode.Nodes.Add(nodeOrbitalObject);
                                    }
                                    else
                                    {
                                        if (bLogToDebug)
                                        {
                                            Debug.Print(sIndent + "!!!!FINDME!!!! NodeParent was NULL !!");
                                        }
                                    }
                                } // end of foreach (var jtPlayerObject in ioPlayerObjects)

                            }
                                else
                                {
                                    Debug.Print(sIndent + "!!!!FINDME!!!! Incorrect type for Player add, skipping Orbital Body: {0} GUID[{1}] ParentGUID[{2}]", nThisNode.Text, nThisNode.GUID.ToString(), nThisNode.OrbitData.ParentGUID.ToString());
                                }
                                break;
                        }

                        default:
                            break;
                    }
                }
                if (bLogToDebug)
                {
                    Debug.Print(sIndent + "AddOrbitalObjTreeNodesRecursively exited, finished processing node type {0} for HECelestialBody {1} GUID:{2} ParentGUID:{3}",
                        ntAddNodesOfType.ToString(), nThisNode.Name, nThisNode.GUID.ToString(), nThisNode.OrbitData.ParentGUID.ToString());
                    Debug.Print(sIndent + "======================================================================================");
                }
            }
            else
            {
                if (bLogToDebug)
                {
                    Debug.Print(sIndent + "!! NodeParent was NULL !!");
                }
            }

        } // end of AddOrbitalObjTreeNodesRecursively

        public bool LoadFile()
        {
            // Load file data from FileName and parse to the JData JObject of type IOrderedEnumerable<JToken>
            // Returns true if there was a loading error

            if (File.Exists(MainFile.FileName))
            {
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

                // Report on errors if LogToDebug is true
                if (LoadError && LogToDebug)
                {
                    if (MainFile.LoadError) Debug.Print("MainFile.LoadError");
                    if (DataFileCelestialBodies.LoadError) Debug.Print("DataFileCelestialBodies.LoadError");
                    if (DataFileAsteroids.LoadError) Debug.Print("DataFileAsteroids.LoadError");
                    if (DataFileStructures.LoadError) Debug.Print("DataFileStructures.LoadError");
                    if (DataFileDynamicObjects.LoadError) Debug.Print("DataFileDynamicObjects.LoadError");
                    if (DataFileModules.LoadError) Debug.Print("DataFileModules.LoadError");
                    if (DataFileStations.LoadError) Debug.Print("DataFileStations.LoadError");
                }

                IsFileReady = true;

                // need to change mouse cursor here


                // Set up a new custom TreeNode which will be added to the node tree
                SolarSystemRootNode = new HEOrbitalObjTreeNode()
                {
                    Name = "NAV_SolarSystem",
                    NodeType = HETreeNodeType.SystemNAV,
                    //GUID = (long)cbChild["GUID"],

                    Text = "Solar System",
                    Tag = "Please select a game object within the Solar System to see JSON data",
                    ImageIndex = (int)HEObjectTypesImageList.Share_16x,
                    SelectedImageIndex = (int)HEObjectTypesImageList.Share_16x
                };






                // Build master node tree
                BuildSolarSystem(SolarSystemRootNode);

                // Add other orbital objects
                PopulateOrbitalObjects(HETreeNodeType.Asteroid, false);
                PopulateOrbitalObjects(HETreeNodeType.Ship, false);
                PopulateOrbitalObjects(HETreeNodeType.Player, false);

                // Update counts of nodes
                SolarSystemRootNode.UpdateCounts();


                // Add the entry for search results

                // Set up a new custom TreeNode which will be added to the node tree
                SearchResultsRootNode = new HEOrbitalObjTreeNode() // <-- WRONG CLASS!!
                {
                    Name = "NAV_SearchResults",
                    NodeType = HETreeNodeType.SystemNAV,
                    //GUID = (long)cbChild["GUID"],


                    //ParentGUID = (long)cbChild["ParentGUID"],
                    //SemiMajorAxis = (float)cbChild["SemiMajorAxis"],
                    //Inclination = (float)cbChild["Inclination"],
                    //OrbitData = Data,


                    //OrbitData
                    Text = "Search Results",
                    Tag = "No data available for this view",
                    ImageIndex = (int)HEObjectTypesImageList.FindResults_16x,
                    SelectedImageIndex = (int)HEObjectTypesImageList.FindResults_16x
                };


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
