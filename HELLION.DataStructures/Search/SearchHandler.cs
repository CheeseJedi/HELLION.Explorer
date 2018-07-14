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
    public class SearchHandler : IParent_Base_TN
    {
        #region Constructors

        /// <summary>
        /// Constructor that takes a GameData and SolarSystem objects.
        /// </summary>
        /// <param name="passedGameData"></param>
        /// <param name="passedSolarSystem"></param>
        public SearchHandler(GameData passedGameData, SolarSystem passedSolarSystem)
        {
            _gameData = passedGameData ?? throw new NullReferenceException("passedGameData was null.");
            _solarSystem = passedSolarSystem ?? throw new NullReferenceException("passedSolarSystem was null.");

            RootNode = new SearchHandler_TN("Search", passedOwner: this, newNodeType: Base_TN_NodeType.SearchHandler);

        }

        #endregion

        #region Properties

        /// <summary>
        /// The root node of the Search Handler tree.
        /// </summary>
        public SearchHandler_TN RootNode { get; private set; } = null;

        /// <summary>
        /// The list of all search operators.
        /// </summary>
        public List<HESearchOperator> SearchOperators { get; private set; } = new List<HESearchOperator>();

        /// <summary>
        /// The current search operator.
        /// </summary>
        public HESearchOperator CurrentOperator { get; private set; } = null;

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new Search Operator of specified type and sets the currentOperator
        /// to point to it.
        /// </summary>
        /// <param name="passedOperatorType"></param>
        /// <returns></returns>
        public HESearchOperator CreateSearchOperator(HESearchOperatorFlags passedOperatorType)
        {
            CurrentOperator = new HESearchOperator(this, passedOperatorType);
            return CurrentOperator;
        }

        #endregion

        #region Fields

        private GameData _gameData = null;
        private SolarSystem _solarSystem = null;
        private HESearchOperator _currentOperator = null;

        #endregion

        #region Enumerations

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

        #endregion

        /// <summary>
        /// Implements a search operator that can execute a query and populate a list of results.
        /// </summary>
        public class HESearchOperator : IParent_Base_TN
        {
            #region Constructors

            /// <summary>
            /// Constructor that takes a SearchHandler reference to it's parent.
            /// </summary>
            /// <param name="passedParent"></param>
            public HESearchOperator(SearchHandler passedParent, HESearchOperatorFlags passedOperatorFlags)
            {
                _parentSearchHandler = passedParent ?? throw new NullReferenceException("passedParent was null.");
                OperatorFlags = passedOperatorFlags;
                RootNode = new SearchHandler_TN(this, "SEARCHOPERATORRESULTS", passedOwner: this, newNodeType: Base_TN_NodeType.SearchResultsSet);
                _parentSearchHandler.RootNode.Nodes.Insert(0, RootNode);
                _parentSearchHandler.SearchOperators.Add(this);
            }

            #endregion

            #region Properties

            /// <summary>
            /// The root node of the Search Operator tree.
            /// </summary>
            public Base_TN RootNode { get; protected set; } = null;

            /// <summary>
            /// The starting node for a search - only actually used in a simple search operation.
            /// </summary>
            public Base_TN StartingNode { get; set; } = null;

            /// <summary>
            /// The Query string.
            /// </summary>
            public string Query
            {
                get => _query;
                set
                {
                    _query = value ?? throw new NullReferenceException("Query was null.");
                }
            }


            /// <summary>
            /// Returns the results list for this search.
            /// </summary>
            public List<Base_TN> Results
            {
                get
                {
                    if (_results == null)
                    {
                        // Generate the results
                        Execute();
                    }
                    return _results;
                }
            }

            public HESearchOperatorFlags OperatorFlags { get; set; } = 0;

            /// <summary>
            /// Determines whether the results set has members.
            /// </summary>
            /// <returns></returns>
            public bool HasResults => (Results != null && Results.Count() < 0) ? true : false;

            #endregion

            #region Methods

            /// <summary>
            /// Executes the query.
            /// </summary>
            /// <returns>Returns true if the result set has more than zero members.</returns>
            public bool Execute()
            {
                if (_query == null || _query == String.Empty) return false;

                if ((OperatorFlags & HESearchOperatorFlags.ByPath) == HESearchOperatorFlags.ByPath)
                {
                    // It's a path string
                    string[] pathTokens = _query.Split('>'); // This must match the TreeView's path separator.

                    // Locating the first node outside of the recursion is important for two reasons:
                    // There are multiple root nodes in the TreeView control, and some of them are not
                    // supported for path-based searching.

                    bool atMaxDepth = pathTokens.Length <= 1 ? true : false;

                    if (pathTokens[0] == _parentSearchHandler._gameData.RootNode.Name)
                    {
                        // It's a path to a Game Data object.
                        if (atMaxDepth)
                        {

                            _results.Add(_parentSearchHandler._gameData.RootNode);
                        }
                        else
                        {
                            _results = RecursivePathSearch(pathTokens, 1, _parentSearchHandler._gameData.RootNode);
                        }
                    }
                    else if (pathTokens[0] == _parentSearchHandler._solarSystem.RootNode.Name)
                    {
                        // It's a path to a Solar System object.
                        if (atMaxDepth)
                        {
                            _results.Add(_parentSearchHandler._solarSystem.RootNode);
                        }

                        else
                        {
                            _results = RecursivePathSearch(pathTokens, 1, _parentSearchHandler._solarSystem.RootNode);
                        }
                    }
                    else
                    {
                        // Unrecognised/unsupported first token.
                        return false;
                    }
                    RootNode.Name = GenerateResultSetDisplayName();
                    return _results.Count() > 0 ? true : false;
                }
                else
                {
                    // It's a regular find operation, not a path.
                    if ((OperatorFlags & HESearchOperatorFlags.MatchCase) == HESearchOperatorFlags.MatchCase)
                    {
                        Debug.Print("Find, Case SENTITIVE");

                        _results = StartingNode.GetChildNodes(includeSubtrees: true)
                            .Where(f => f.Name.Contains(_query)
                            || f.Text.Contains(_query)
                            || f.NodeType.ToString().Contains(_query))
                            .ToList();
                    }
                    else
                    {
                        Debug.Print("Find, Case INsensitive");

                        _results = StartingNode.GetChildNodes(includeSubtrees: true)
                            .Where(f => f.Name.Contains(_query, StringComparison.OrdinalIgnoreCase)
                            || f.Text.Contains(_query, StringComparison.OrdinalIgnoreCase)
                            || f.NodeType.ToString().Contains(_query, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                    }

                    RootNode.Name = GenerateResultSetDisplayName();
                    return _results.Count() > 0 ? true : false;
                }
            }

            /// <summary>
            /// Searches a node's child nodes - used in a path-based search.
            /// </summary>
            /// <param name="pathTokens"></param>
            /// <param name="currentDepth"></param>
            /// <param name="parentNode"></param>
            /// <returns></returns>
            protected List<Base_TN> RecursivePathSearch(string[] pathTokens, int currentDepth, Base_TN parentNode)
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

            /// <summary>
            /// Generates a display name for the results set.
            /// </summary>
            /// <returns></returns>
            protected virtual string GenerateResultSetDisplayName()
            {
                string postfix = String.Empty;
                if (_query != null)
                {
                    postfix = " " + _query + " (" + _results.Count().ToString() + ")";
                }
                return baseDisplayName + postfix;
            }

            #endregion

            #region Fields

            protected List<Base_TN> _results = null;
            protected SearchHandler _parentSearchHandler = null;
            protected string _query = null;
            /// <summary>
            /// Defines the base display name of an object, to which additional info will
            /// be appended to once the operator has been executed to generate the new 
            /// display name (the node's .Text field).
            /// </summary>
            protected string baseDisplayName = "Search Results";

            #endregion

        }
    }
}
