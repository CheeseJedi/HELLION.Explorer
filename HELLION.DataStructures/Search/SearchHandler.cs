using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using HELLION.DataStructures.Document;
using HELLION.DataStructures.UI;
using HELLION.DataStructures.Utilities;

namespace HELLION.DataStructures.Search
{
    /// <summary>
    /// A class to handle searches and present search results.
    /// </summary>
    public class SearchHandler : Iparent_Base_TN
    {
        /// <summary>
        /// Defines the available search operator types.
        /// </summary>
        /// <remarks>
        /// The _CI postfix represents Case Insensitivity.
        /// </remarks>
        [Flags]
        public enum HESearchOperatorFlags
        {
            MatchCase = 0x1,
            ByPath = 0x2,
        }

        /// <summary>
        /// Public property for the root node of the Search Handler tree.
        /// </summary>
        public SearchHandler_TN RootNode => rootNode;

        /// <summary>
        /// Field for root node of the Game Data tree.
        /// </summary>
        private SearchHandler_TN rootNode = null;

        /// <summary>
        /// Stores a reference to the GameData object.
        /// </summary>
        private GameData gameData = null;

        /// <summary>
        /// Stores a reference to the Solar System object.
        /// </summary>
        private SolarSystem solarSystem = null;

        /// <summary>
        /// Public property to access to access the current search operator.
        /// </summary>
        public HESearchOperator CurrentOperator => currentOperator;

        /// <summary>
        /// The current search operator.
        /// </summary>
        protected HESearchOperator currentOperator = null;


        /// <summary>
        /// Public read-only access to FindOperator object.
        /// </summary>
        //public HEFindOperator FindOperator => findOperator;

        /// <summary>
        /// Field for storing the FindOperator object.
        /// </summary>
        //private HEFindOperator findOperator = null;

        //public HEFindNodesByPathOperator FindNodeByPathOperator => findNodeByPathOperator;
        //private HEFindNodesByPathOperator findNodeByPathOperator = null;

        private List<HESearchOperator> searchOperators = null;

        /// <summary>
        /// Constructor that takes a GameData and SolarSystem objects.
        /// </summary>
        /// <param name="passedGameData"></param>
        /// <param name="passedSolarSystem"></param>
        public SearchHandler(GameData passedGameData, SolarSystem passedSolarSystem)
        {
            gameData = passedGameData ?? throw new NullReferenceException("passedGameData was null.");
            solarSystem = passedSolarSystem ?? throw new NullReferenceException("passedSolarSystem was null.");
            
            rootNode = new SearchHandler_TN("Search", passedOwner: this, newNodeType: Base_TN_NodeType.SearchHandler);
            
            searchOperators = new List<HESearchOperator>();

            // Initialise the FindNodeByPathOperator
            //findNodeByPathOperator = new HEFindNodesByPathOperator(this);

            // Initialise the FindOperator.
            //findOperator = new HEFindOperator(this);

        }

        /// <summary>
        /// Creates a new Search Operator of specified type and sets the currentOperator
        /// to point to it.
        /// </summary>
        /// <param name="passedOperatorType"></param>
        /// <returns></returns>
        public HESearchOperator CreateSearchOperator(HESearchOperatorFlags passedOperatorType)
        {
            currentOperator = new HESearchOperator(this, passedOperatorType);
            return currentOperator;
        }

        /// <summary>
        /// Implements a search operator that can execute a query and populate a list of results.
        /// </summary>
        public class HESearchOperator : Iparent_Base_TN
        {
            /// <summary>
            /// Constructor that takes a SearchHandler reference to it's parent.
            /// </summary>
            /// <param name="passedParent"></param>
            public HESearchOperator(SearchHandler passedParent, HESearchOperatorFlags passedOperatorFlags)
            {
                parent = passedParent ?? throw new NullReferenceException("passedParent was null.");
                OperatorFlags = passedOperatorFlags;
                _rootNode = new SearchHandler_TN(this, "SEARCHOPERATORRESULTS", passedOwner: this, newNodeType: Base_TN_NodeType.SearchResultsSet);
                parent.rootNode.Nodes.Add(_rootNode);
                parent.searchOperators.Add(this);
            }

            /// <summary>
            /// Defines the base display name of an object, to which additional info will
            /// be appended to once the operator has been executed to generate the new 
            /// display name (the node's .Text field).
            /// </summary>
            protected string baseDisplayName = "Search Results";

            /// <summary>
            /// Public property for the root node of the Search Handler tree.
            /// </summary>
            public Base_TN RootNode => _rootNode;

            /// <summary>
            /// The root node of the Game Data tree.
            /// </summary>
            protected Base_TN _rootNode = null;

            /// <summary>
            /// Stores a reference to this object's parent, the SearchHandler
            /// </summary>
            protected SearchHandler parent = null;

            /// <summary>
            /// Public property to get the results list.
            /// </summary>
            public List<Base_TN> Results
            {
                get
                {
                    if (results == null)
                    {
                        // Generate the results
                        Execute();
                    }
                    return results;
                }
            }
            
            /// <summary>
            /// 
            /// </summary>
            protected List<Base_TN> results = null;

            public HESearchOperatorFlags OperatorFlags{ get; set; } = 0;

            /// <summary>
            /// Determines whether the results set has members.
            /// </summary>
            /// <returns></returns>
            public bool HasResults()
            {
                if (results == null || results.Count() < 1 ) return false;
                else return true;
            }

            /// <summary>
            /// Generates a display name for the results set.
            /// </summary>
            /// <returns></returns>
            protected virtual string GenerateResultSetDisplayName()
            {
                string postfix = String.Empty;
                if (query != null)
                {
                    postfix = " " + query + " (" + results.Count().ToString() + ")";
                }
                return baseDisplayName + postfix;
            }

            /// <summary>
            /// Public property to access the Query string.
            /// </summary>
            /// <remarks>
            /// Currently has only null checking on set.
            /// </remarks>
            public string Query
            {
                get => query;
                set
                {
                    query = value ?? throw new NullReferenceException("Query was null.");
                }
            }

            /// <summary>
            /// Field for the query string.
            /// </summary>
            protected string query = null;

            public Base_TN StartingNode
            {
                get => startingNode;
                set => startingNode = value; // ?? throw new NullReferenceException("StartingNode was null.");
            }
            
            protected Base_TN startingNode = null;

            /// <summary>
            /// Executes the query.
            /// </summary>
            /// <returns>Returns true if the result set has more than zero members.</returns>
            public bool Execute()
            {
                if (query == null || query == String.Empty) return false;

                if ((OperatorFlags & HESearchOperatorFlags.ByPath) == HESearchOperatorFlags.ByPath)
                {
                    // It's a path string
                    string[] pathTokens = query.Split('>'); // This must match the TreeView's path separator.

                    // Locating the first node outside of the recursion is important for two reasons:
                    // There are multiple root nodes in the TreeView control, and some of them are not
                    // supported for path-based searching.

                    bool atMaxDepth = pathTokens.Length <= 1 ? true : false;

                    if (pathTokens[0] == parent.gameData.RootNode.Name)
                    {
                        // It's a path to a Game Data object.
                        if (atMaxDepth)
                        {

                            results.Add(parent.gameData.RootNode);
                        }
                        else
                        {
                            results = RecursivePathSearch(pathTokens, 1, parent.gameData.RootNode);
                        }
                    }
                    else if (pathTokens[0] == parent.solarSystem.RootNode.Name)
                    {
                        // It's a path to a Solar System object.
                        if (atMaxDepth)
                        {
                            results.Add(parent.solarSystem.RootNode);
                        }

                        else
                        {
                            results = RecursivePathSearch(pathTokens, 1, parent.solarSystem.RootNode);
                        }
                    }
                    else
                    {
                        // Unrecognised/unsupported first token.
                        return false;
                    }
                    _rootNode.Name = GenerateResultSetDisplayName();
                    return results.Count() > 0 ? true : false;
                }
                else
                {
                    // It's a regular find operation, not a path.
                    if ((OperatorFlags & HESearchOperatorFlags.MatchCase) == HESearchOperatorFlags.MatchCase)
                    {
                        Debug.Print("Find, Case SENTITIVE");

                        results = startingNode.GetChildNodes(includeSubtrees: true)
                            .Where<Base_TN>(f => f.Name.Contains(query)
                            || f.Text.Contains(query)
                            || f.NodeType.ToString().Contains(query))
                            .ToList<Base_TN>();
                    }
                    else
                    {
                        Debug.Print("Find, Case INsensitive");

                        results = startingNode.GetChildNodes(includeSubtrees: true)
                            .Where<Base_TN>(f => f.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                            || f.Text.Contains(query, StringComparison.OrdinalIgnoreCase)
                            || f.NodeType.ToString().Contains(query, StringComparison.OrdinalIgnoreCase))
                            .ToList<Base_TN>();
                    }

                    _rootNode.Name = GenerateResultSetDisplayName();
                    return results.Count() > 0 ? true : false;
                }
            }
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pathTokens"></param>
            /// <param name="currentDepth"></param>
            /// <param name="parentNode"></param>
            /// <returns></returns>
            internal List<Base_TN> RecursivePathSearch(string[] pathTokens, int currentDepth, Base_TN parentNode)
            {
                List<Base_TN> results = new List<Base_TN>();
                bool atMaxDepth = currentDepth >= pathTokens.Length - 1 ? true : false;

                if (pathTokens[currentDepth] == "*")
                {
                    // We've got a wild-card token - process all nodes in the collection.
                    foreach (Base_TN node in parentNode.Nodes)
                    {
                        if (atMaxDepth)
                        {
                            // Add the nodes to the results list
                            results.Add(node);
                        }
                        else
                        {
                            // Recurse from each node
                            results.AddRange(RecursivePathSearch(pathTokens, currentDepth + 1, node));
                        }
                    }
                }
                else
                {
                    // It's a regular (name) token we've got.
                    TreeNode[] currentNodeArray = parentNode.Nodes.Find(pathTokens[currentDepth], false);
                    if (currentNodeArray.Length > 0)
                    {
                        if (atMaxDepth)
                        {
                            // Add the node to the results list
                            results.Add((Base_TN)currentNodeArray[0]);
                        }
                        else
                        {
                            // Recurse from each node
                            results.AddRange(RecursivePathSearch(pathTokens, currentDepth + 1, (Base_TN)currentNodeArray[0]));
                        }
                    }
                }
                return results;
            }
        }

        /*

        public class HEFindOperator : HESearchOperator
        {
            public HEFindOperator(SearchHandler passedParent) : base(passedParent)
            {
                rootNode.NodeType = Base_TN_NodeType.SearchResultsSet;
                rootNode.Name = "Find Results";
                rootNode.Text =  "Find Results";

                // Will need to set itself to undeletable.
            }

            /// <summary>
            /// Defines the base display name of an object, to which additional info will
            /// be appended to once the operator has been executed to generate the new 
            /// display name (the node's .Text field).
            /// </summary>
            protected new string baseDisplayName = "Find: ";

            protected override string GenerateResultSetDisplayName()

            {
                string postfix = String.Empty;
                if (query != null)
                {
                    postfix = " '" + query + "' (" + results.Count().ToString() + ")";
                }
                return baseDisplayName + postfix;
            }

        }

        public class HEFindNodesByPathOperator : HESearchOperator
        {
            public HEFindNodesByPathOperator(SearchHandler passedParent) : base(passedParent)
            {
                rootNode.NodeType = Base_TN_NodeType.SearchResultsSet;
                rootNode.Name = "Find Path Results";
                rootNode.Text = "Find Path Results";

                // Will need to set itself to undeletable.
            }

            /// <summary>
            /// Defines the base display name of an object, to which additional info will
            /// be appended to once the operator has been executed to generate the new 
            /// display name (the node's .Text field).
            /// </summary>
            protected new string baseDisplayName = "Path: ";

            /// <summary>
            /// Executes the query.
            /// </summary>
            /// <returns>Returns true if the result set has more than zero members.</returns>
            public new bool Execute()
            {
                if (query == null || query == String.Empty) return false;
                else
                {
                    string[] pathTokens = query.Split('>');

                    // Locating the first node outside of the recursion is important for two reasons:
                    // There are multiple root nodes in the TreeView control, and some of them are not
                    // supported for path-based searching.

                    bool atMaxDepth = pathTokens.Length <= 1 ? true : false;

                    //Debug.Print(String.Empty);
                    //Debug.Print("Execute - atMaxDepth: " + atMaxDepth);
                    //Debug.Print(query);
                    //Debug.Print(pathTokens[0]);


                    if (pathTokens[0] == parent.gameData.RootNode.Name)
                    {
                        // It's a path to a Game Data object.
                        if (atMaxDepth)
                        {
                            // Add the nodes to the results list
                            results.Add(parent.gameData.RootNode);
                        }
                        else
                        {
                            // Recurse from each node
                            results = RecursivePathSearch(pathTokens, 1, parent.gameData.RootNode);
                        }
                    }
                    else if (pathTokens[0] == parent.solarSystem.RootNode.Name)
                    {
                        // It's a path to a Solar System object.
                        if (atMaxDepth)
                        {
                            // Add the node to the results list
                            results.Add(parent.solarSystem.RootNode);
                        }
                        else
                        {
                            // Recurse from each node
                            results = RecursivePathSearch(pathTokens, 1, parent.solarSystem.RootNode);
                        }
                    }
                    else
                    {
                        // Unrecognised/supported first token.
                        return false;
                    }
                    rootNode.Text = GenerateResultSetDisplayName();
                    return results.Count() > 0 ? true : false;
                }
            }



            /// <summary>
            /// Returns a single TreeNode with a given path - TRANSPLANTED HERE FROM HELLION.Explorer.cs
            /// </summary>
            /// <param name="tv"></param>
            /// <param name="passedPath"></param>
            /// <returns></returns>
            internal static TreeNode GetNodeByPath(TreeView tv, string passedPath)
            {
                List<string> pathTokens = new List<string>(passedPath.Split('>'));

                TreeNode previousNode = null;

                TreeNode[] currentNodeArray = tv.Nodes.Find(pathTokens[0], false);


                if (currentNodeArray.Length > 0)
                {
                    TreeNode currentNode = currentNodeArray[0];

                    // Setting the current node was successful, remove it from the list and continue.
                    // From here on in we're working with TreeNode's .Nodes collections instead of 
                    // the TreeView control itself.
                    pathTokens.RemoveAt(0);
                    foreach (string token in pathTokens)
                    {
                        previousNode = currentNode;

                        currentNodeArray = currentNode.Nodes.Find(token, false);

                        if (currentNodeArray.Length > 0) currentNode = currentNodeArray[0];
                        else
                        {
                            MessageBox.Show("Node not found: " + token);
                        }

                        if (currentNode == null) throw new NullReferenceException("currentNode is null.");
                    }
                    return currentNode;
                }
                else
                {
                    // No results, return null.
                    return null;
                }
            }

        }
        */

    }
}
