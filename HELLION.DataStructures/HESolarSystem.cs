using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Windows.Forms;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Implements the Solar System view node tree.
    /// </summary>
    public class HESolarSystem
    {
        public HEOrbitalObjTreeNode RootNode { get; set; } = null;
        public HEGameData GameData { get; set; } = null;

        /// <summary>
        /// Constructor that takes an HEGameData object and uses this as it's data source.
        /// </summary>
        /// <param name="gameData"></param>
        public HESolarSystem(HEGameData gameData)
        {
            // Basic constructor

            RootNode = new HEOrbitalObjTreeNode("SOLARSYSTEMVIEW", HETreeNodeType.SolarSystemView, "Solar System")
            {
                GUID = -1 // Hellion, the star, has a ParentGUID of -1, so we utilise this to attach it to the Solar System root node
            };

            if (gameData != null)
            {
                // Store a reference to the GameData object
                GameData = gameData;

                // Create Solar System hierarchical structure from the Celestial Bodies
                BuildSolarSystem();

            }
            else throw new Exception();

        }

        public void Close()
        {
            RootNode = null;
        }
        
        /// <summary>
        /// Builds a tree of nodes representing the solar system and attaches it to the RootNode, then
        /// calls the PopulateOrbitalObjects helper with types Asteroid, Ship(includes modules), and players.
        /// </summary>
        public void BuildSolarSystem()
        {

            // Get the CelestialBodies.json data

            if (GameData.StaticData.DataDictionary.TryGetValue("CelestialBodies.json", out HEJsonBaseFile tempJsonBaseFile))
            {
                AddCBTreeNodesRecursively((JArray)tempJsonBaseFile.JData, nThisNode: RootNode, lParentGUID: -1, iDepth: 4);

                PopulateOrbitalObjects(HETreeNodeType.Asteroid, bLogToDebug: true);
                PopulateOrbitalObjects(HETreeNodeType.Ship);
                RehydrateDockedShips();
                PopulateOrbitalObjects(HETreeNodeType.Player);
                RootNode.UpdateCounts();

            }
            else
            {
                throw new Exception();
            }

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
                            SemiMajorAxis = (double)cbChild["SemiMajorAxis"],
                            Inclination = (double)cbChild["Inclination"],
                            Eccentricity = (double)cbChild["Eccentricity"],
                            ArgumentOfPeriapsis = (double)cbChild["ArgumentOfPeriapsis"],
                            LongitudeOfAscendingNode = (double)cbChild["LongitudeOfAscendingNode"]
                        };

                        // Figure out whether this is the star, a planet, or a moon

                        HETreeNodeType newNodeType = HETreeNodeType.SolSysMoon; // Default to a moon
                        if (Data.ParentGUID == -1) // It's the star
                            newNodeType = HETreeNodeType.SolSysStar;
                        else if (Data.ParentGUID == 1) // It's parent is the star, it's a planet
                            newNodeType = HETreeNodeType.SolSysPlanet;

                        // Set up a new custom TreeNode which will be added to the node tree
                        HEOrbitalObjTreeNode nChildNode = new HEOrbitalObjTreeNode((string)cbChild["Name"], newNodeType)
                        {
                            //Name = (string)cbChild["Name"], // GUID.ToString();
                            //NodeType = HETreeNodeType.CelestialBody,
                            GUID = (long)cbChild["GUID"],

                            ParentGUID = (long)cbChild["ParentGUID"],
                            SemiMajorAxis = (double)cbChild["SemiMajorAxis"],
                            Inclination = (double)cbChild["Inclination"],
                            OrbitData = Data,

                            Tag = cbChild,
                        };

                        // Add the node
                        if (nThisNode != null)
                            nThisNode.Nodes.Add(nChildNode);

                        // Recursive call here
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


        public void PopulateOrbitalObjects(HETreeNodeType ntAddNodesOfType, bool bAddScenes = false, bool bLogToDebug = false)
        {
            // Populates a given node tree with objects of a given type from the .save file

            // TODO add variable nStartingPoint rather than starting at an arbirtary place

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

                    IOrderedEnumerable<JToken> ioFilteredObjects = from s in GameData.SaveFile.JData[sSectionName]
                                                                   orderby (long)s["OrbitData"]["ParentGUID"], (long)s["OrbitData"]["SemiMajorAxis"]
                                                                   //(long)s["DockedToShipGUID"] != null ? (long)s["DockedToShipGUID"] : 0
                                                                   select s;

                    //iShipCount 
                    int a = ioFilteredObjects.Count<JToken>();
                    if (bLogToDebug)
                    {
                        foreach (var jtFiltObj in ioFilteredObjects)
                        {
                            StringBuilder sb = new StringBuilder();

                            sb.Append("PopOrbObj #" + iShipCount.ToString());

                            sb.Append(" Name: " + (string)jtFiltObj["Name"]);

                            JToken testToken = jtFiltObj["Registration"];
                            if (testToken != null)
                            {
                                sb.Append(" Registration: " + (string)jtFiltObj["Registration"]);
                            }

                            sb.Append(" GUID: " + (string)jtFiltObj["GUID"]);
                            sb.Append(" ParentGUID: " + (string)jtFiltObj["OrbitData"]["ParentGUID"]);

                            testToken = jtFiltObj["DockedToShipGUID"];
                            if (testToken != null)
                            {
                                sb.Append(" DockedToShipGUID: " + (long)jtFiltObj["DockedToShipGUID"]);
                            }

                            //sb.Append(Environment.NewLine);

                            Debug.Print(sb.ToString());

                        }

                    }



                    // start traversing the node tree recursively, starting from the root and adding children depth-first as it progresses
                    AddOrbitalObjTreeNodesRecursively(ioFilteredObjects, RootNode, ntAddNodesOfType, bAddScenes, iMaxRecursionDepth, bLogToDebug: bLogToDebug, iLogIndentLevel: 1);

                    // Parses the tree of nodes looking for ships/modules that have a DockedToShipGUID value that's not null/empty
                    // and re-parents them to the appropriate node

                    if (ntAddNodesOfType == HETreeNodeType.Ship)
                    {
                        // Call the routine to collapse docked stations
                        //FlattenDockedShipNodesRecursively(RootNode, iMaxRecursionDepth, false, iLogIndentLevel: 1);
                    }


                    break;

                case HETreeNodeType.Player:
                    //
                    sSectionName = "Players";

                    // We are dealing with player objects which are handled slightly differently

                    IOrderedEnumerable<JToken> ioPlayerObjects = from s in GameData.SaveFile.JData[sSectionName]
                                                                 orderby (long)s["ParentGUID"] ///, (long)s["OrbitData"]["SemiMajorAxis"]
                                                                 select s;

                    foreach (var jtPlayerObject in ioPlayerObjects)
                    {
                        iShipCount++;
                        Debug.Print("Name: " + (string)jtPlayerObject["Name"] + " GUID: " + (string)jtPlayerObject["GUID"] + " ParentGUID: " + (string)jtPlayerObject["ParentGUID"]);
                    }

                    // start traversing the node tree recursively, starting from the root and adding children depth-first as it progresses
                    AddOrbitalObjTreeNodesRecursively(ioPlayerObjects, RootNode, ntAddNodesOfType, bAddScenes, iMaxRecursionDepth, bLogToDebug: bLogToDebug);


                    break;
                default:
                    // There's a problem, not a supported node type

                    break;
            } // End of switch (ntAddNodesOfType)
        } // End of PopulateOrbitalObjects

        public void AddOrbitalObjTreeNodesRecursively(
        IOrderedEnumerable<JToken> JData,
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
            //int iImageIndex = HEUtilities.GetImageIndexByNodeType(ntAddNodesOfType);

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
                    IEnumerable<HEOrbitalObjTreeNode> thisNodesChildren = null;

                    if (ntAddNodesOfType == HETreeNodeType.Player)
                    {
                        // Get all nodes as we're adding a player object
                        thisNodesChildren = nThisNode.Nodes.Cast<HEOrbitalObjTreeNode>();
                    }
                    else
                    {
                        // Get the child nodes that are Celestial Bodies
                        thisNodesChildren = nThisNode.Nodes
                            .Cast<HEOrbitalObjTreeNode>().Where(p =>
                            p.NodeType == HETreeNodeType.SolarSystemView ||
                            p.NodeType == HETreeNodeType.SolSysStar ||
                            p.NodeType == HETreeNodeType.SolSysPlanet ||
                            p.NodeType == HETreeNodeType.SolSysMoon);
                    }

                    if (bLogToDebug)
                    {
                        Debug.Print(sIndent + "{0} child nodes were celestial bodies", thisNodesChildren.Count());
                    }

                    // Check for child nodes and recurse to each in turn - this is done before creating objects at this level
                    if (thisNodesChildren.Count() > 0)
                    {
                        // There are child nodes (celcetial bodies) to process

                        // Loop through each child body in the iCelestialBodies list and RECURSE
                        foreach (HEOrbitalObjTreeNode nChildNode in thisNodesChildren)
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
                            AddOrbitalObjTreeNodesRecursively(JData, nChildNode, ntAddNodesOfType, bAddScenes, iDepth - 1, bLogToDebug, iLogIndentLevel + 1);

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
                                Debug.Print("GOT HERE 1");

                                // Find the OrbitalObjects for this ParentGUID
                                IOrderedEnumerable<JToken> ioOrbitalObjects = from s in JData
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
                                        SemiMajorAxis = (double)jtOrbitalObject["OrbitData"]["SemiMajorAxis"],
                                        Inclination = (double)jtOrbitalObject["OrbitData"]["Inclination"],
                                        Eccentricity = (double)jtOrbitalObject["OrbitData"]["Eccentricity"],
                                        ArgumentOfPeriapsis = (double)jtOrbitalObject["OrbitData"]["ArgumentOfPeriapsis"],
                                        LongitudeOfAscendingNode = (double)jtOrbitalObject["OrbitData"]["LongitudeOfAscendingNode"]
                                    };

                                    string sObjectName; // = HEJsonBaseFile.GenerateDisplayName((JObject)jtOrbitalObject);
                                    // Build the node names for ships differently
                                    if (ntAddNodesOfType == HETreeNodeType.Ship)
                                    {
                                        sObjectName = (string)jtOrbitalObject["Registration"] + " " + (string)jtOrbitalObject["Name"];
                                    }
                                    else
                                    {
                                        sObjectName = (string)jtOrbitalObject["Name"];
                                    }

                                    Debug.Print("GOT HERE 2 " + sObjectName);

                                    // Create a new TreeNode representing the object we're adding
                                    HEOrbitalObjTreeNode nodeOrbitalObject = new HEOrbitalObjTreeNode(sObjectName, ntAddNodesOfType)
                                    {
                                        GUID = (long)jtOrbitalObject["GUID"],
                                        ParentGUID = (long)jtOrbitalObject["OrbitData"]["ParentGUID"],
                                        SemiMajorAxis = (double)jtOrbitalObject["OrbitData"]["SemiMajorAxis"],
                                        Inclination = (double)jtOrbitalObject["OrbitData"]["Inclination"],
                                        // Generate the foreground colour
                                        //ForeColor = ConvertStringToColor((string)jtOrbitalObject["Name"]),
                                        SceneID = (int)jtOrbitalObject["SceneID"],
                                        Type = (int)jtOrbitalObject["Type"],
                                        OrbitData = OrbitalObjectData,
                                        Tag = jtOrbitalObject
                                    };

                                    JToken testToken = jtOrbitalObject["DockedToShipGUID"];
                                    if (testToken != null)
                                        nodeOrbitalObject.DockedToShipGUID = (long)jtOrbitalObject["DockedToShipGUID"];

                                    testToken = jtOrbitalObject["DockedPortID"];
                                    if (testToken != null)
                                        nodeOrbitalObject.DockedPortID = (int)jtOrbitalObject["DockedPortID"];

                                    testToken = jtOrbitalObject["DockedToPortID"];
                                    if (testToken != null)
                                        nodeOrbitalObject.DockedToPortID = (int)jtOrbitalObject["DockedToPortID"];

                                    Debug.Print("GOT HERE 3 {0}-{1}-{2}", nodeOrbitalObject.Name, nodeOrbitalObject.GUID, nodeOrbitalObject.ParentGUID);

                                    // add the node
                                    nThisNode.Nodes.Add(nodeOrbitalObject);

                                } // End of foreach (var jtOrbitalObject in ioOrbitalObjects)

                                break;
                            }
                        //case HETreeNodeType.DynamicObject:
                        case HETreeNodeType.Player:
                            {
                                // Players get handled differently

                                // We (currently) only add players to Ship objects
                                if (true) //(nThisNode.NodeType == HETreeNodeType.Ship)
                                {
                                    //

                                    // Find the OrbitalObjects for this ParentGUID
                                    IOrderedEnumerable<JToken> ioPlayerObjects = from s in JData
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
                                        HEOrbitalObjTreeNode nodeOrbitalObject = new HEOrbitalObjTreeNode((string)jtPlayerObject["Name"], ntAddNodesOfType)
                                        {
                                            //Name = (string)jtPlayerObject["Name"],
                                            //NodeType = ntAddNodesOfType,
                                            //Text = (string)jtPlayerObject["Name"],
                                            GUID = (long)jtPlayerObject["GUID"],
                                            //ParentGUID = (long)jtPlayerObject["ParentGUID"],
                                            Tag = jtPlayerObject,
                                            //ImageIndex = (int)HEObjectTypesImageList.Actor_16x,
                                            //SelectedImageIndex = (int)HEObjectTypesImageList.Actor_16x
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

        /// <summary>
        /// Re-arranges (rehydrates) existing ship nodes bu their DockedToShipGUID forming a tree where the
        /// root node is the parent vessel of the docked ships (and is what shows up on radar in-game).
        /// </summary>
        /// <remarks>
        /// Although this particular function is non-recursive, recursive calls are made when calling
        /// the HETreeNode.GetAllNodes() to get sub-nodes.
        /// </remarks>
        public void RehydrateDockedShips()
        {
            IEnumerable<HEOrbitalObjTreeNode> shipsToBeReparented = RootNode.ListOfAllChildNodes
                .Cast<HEOrbitalObjTreeNode>()
                .Where(p => (p.NodeType == HETreeNodeType.Ship) && (p.DockedToShipGUID > 0));

            foreach (HEOrbitalObjTreeNode node in shipsToBeReparented)
            {
                // If fhis node has a non-zero value for DockedToShipGUID, process it.
                if (node.DockedToShipGUID != 0)
                {
                    // Find the node that has the GUID matching the DockedToShipGUID of this node.
                    HEOrbitalObjTreeNode newParentNode = RootNode.ListOfAllChildNodes
                        .Cast<HEOrbitalObjTreeNode>()
                        .Where(p => p.GUID == node.DockedToShipGUID)
                        .Single();

                    // Remove the ship to be re-parented from it's current parent's node collection.
                    node.Parent.Nodes.Remove(node);
                    // Add the ship being re-parented to the new parent's node collection.
                    newParentNode.Nodes.Add(node);
                    // As the new parent's node collection has changed, clear it's cache to force regeneration.
                    newParentNode.ClearCachedData();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /*
        public void FlattenDockedShipNodesRecursively(
            HEOrbitalObjTreeNode nThisNode,
            int iDepth = 20,
            bool bLogToDebug = false,
            int iLogIndentLevel = 0)
        {
            // Breadth-first recursive function for implementing flattening of docked ship groups

            // Set up indenting for this level
            string sIndent = String.Join("| ", new String[iLogIndentLevel]);

            // nThisNode prepresents the point at which this function starts from
            //Check and only continue if nThisNode is not null
            if (nThisNode != null)
            {
                if (bLogToDebug)
                {
                    Debug.Print(sIndent + "======================================================================================");
                    Debug.Print(sIndent + "CollapseDockedShipNodesRecursively entered, started processing node for HECelestialBody {0} GUID:{1} ParentGUID:{2}",
                        nThisNode.Name, nThisNode.GUID.ToString(), nThisNode.OrbitData.ParentGUID.ToString());
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
                    IEnumerable<HEOrbitalObjTreeNode> thisNodesChildren = nThisNode.Nodes.Cast<HEOrbitalObjTreeNode>().Where(p =>
                            p.NodeType == HETreeNodeType.SolarSystemView ||
                            p.NodeType == HETreeNodeType.SolSysStar ||
                            p.NodeType == HETreeNodeType.SolSysPlanet ||
                            p.NodeType == HETreeNodeType.SolSysMoon);

                    // Check for child nodes and recurse to each in turn - this is done before processing objects at this level
                    if (thisNodesChildren.Count() > 0)
                    {
                        // There are child nodes (celcetial bodies) to process

                        // Loop through each child body in the iCelestialBodies list and RECURSE
                        foreach (HEOrbitalObjTreeNode nChildNode in thisNodesChildren)
                        {
                            if (bLogToDebug)
                            {
                                Debug.Print(sIndent + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                                Debug.Print(sIndent + ">> Call: FlattenDockedShipNodesRecursively with GUID: " + nChildNode.GUID);
                                Debug.Print(sIndent + ">> : Preparing to recurse using Orbital Body: " + nChildNode.Name);
                                Debug.Print(sIndent + ">> : nChildNode GetNodeCount: " + nChildNode.GetNodeCount(includeSubTrees: false).ToString());
                                Debug.Print(sIndent + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                            }
                            // Find the correct node in the Nav Tree for this cbChild

                            // *** Recursive call here ***
                            FlattenDockedShipNodesRecursively(nChildNode, iDepth - 1, bLogToDebug, iLogIndentLevel + 1);

                            if (bLogToDebug)
                            {
                                Debug.Print(sIndent + "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                                Debug.Print(sIndent + "<< Return: FlattenDockedShipNodesRecursively with GUID: " + nChildNode.GUID);
                                Debug.Print(sIndent + "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                            }
                        } // end of foreach (TreeNode nChildNode in nThisNode.Nodes)
                    }
                    else
                    {
                        // Node Count was zero or less
                    }

                    // Process this node's ships

                    // Define an IEnumerable object to hold this node's children that are Ships and have a DockedToShipGUID
                    IEnumerable<HEOrbitalObjTreeNode> IShipsToReParent = nThisNode.Nodes.Cast<HEOrbitalObjTreeNode>().Where(p => (p.NodeType == HETreeNodeType.Ship) && (p.DockedToShipGUID > 0));

                    // Check for child nodes and recurse to each in turn - this is done before processing objects at this level
                    if (IShipsToReParent.Count() > 0)
                    {
                        // There are nodes to process

                        IEnumerable<HEOrbitalObjTreeNode> DockingParents = null;

                        // Loop through each child body in the iCelestialBodies list and RECURSE
                        foreach (HEOrbitalObjTreeNode nShipToReparent in IShipsToReParent)
                        {
                            if (nShipToReparent.DockedToShipGUID > 0)
                            {

                                DockingParents = nThisNode.Nodes.Cast<HEOrbitalObjTreeNode>().Where(n => n.GUID == nShipToReparent.DockedToShipGUID);
                                foreach (HEOrbitalObjTreeNode DockingParent in DockingParents)
                                {
                                    // Only process the first, there should only be one match anyway
                                    // Remove the ship to be re-parented from it's parent node collection
                                    nShipToReparent.Parent.Nodes.Remove(nShipToReparent);
                                    // Add the ship being re-parented to the parent's node collection
                                    DockingParent.Nodes.Add(nShipToReparent);
                                }
                            }

                        }


                    }






                }
                if (bLogToDebug)
                {
                    Debug.Print(sIndent + "FlattenDockedShipNodesRecursively exited, finished processing node type  HECelestialBody {0} GUID:{1} ParentGUID:{2}",
                        nThisNode.Name, nThisNode.GUID.ToString(), nThisNode.OrbitData.ParentGUID.ToString());
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

        } // end of FlattenDockedShipNodesRecursively
        */

    }
}
