using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HELLION.DataStructures
{
    /// <summary>
    /// A class to handle searches and present search results.
    /// </summary>
    public class HESearchHandler
    {

        /// <summary>
        /// Public property for the root node of the Search Handler tree.
        /// </summary>
        public HESearchHandlerTreeNode RootNode { get { return rootNode; } }

        /// <summary>
        /// Field for root node of the Game Data tree.
        /// </summary>
        private HESearchHandlerTreeNode rootNode = null;

        /// <summary>
        /// Stores a reference to the GameData object.
        /// </summary>
        private HEGameData gameData = null;

        /// <summary>
        /// Stores a reference to the Solar System object.
        /// </summary>
        private HESolarSystem solarSystem = null;

        /// <summary>
        /// Public read-only access to FindOperator object.
        /// </summary>
        public HEFindOperator FindOperator => findOperator;

        /// <summary>
        /// Field for storing the FindOperator object.
        /// </summary>
        private HEFindOperator findOperator = null;

        private List<HESearchOperator> searchOperators = null;

        /// <summary>
        /// Constructor that takes a GameData and SolarSystem objects.
        /// </summary>
        /// <param name="passedGameData"></param>
        /// <param name="passedSolarSystem"></param>
        public HESearchHandler(HEGameData passedGameData, HESolarSystem passedSolarSystem)
        {
            gameData = passedGameData ?? throw new NullReferenceException("passedGameData was null.");
            solarSystem = passedSolarSystem ?? throw new NullReferenceException("passedSolarSystem was null.");
            
            rootNode = new HESearchHandlerTreeNode("SEARCHHANDLERVIEW", HETreeNodeType.SearchHandler, "Search");

            searchOperators = new List<HESearchOperator>();


            // Initialise the FindOperator.
            findOperator = new HEFindOperator(this);
            
            // searchOperators.Add(findOperator);

        }

        /*
        /// <summary>
        /// Handles a very basic find-by-name of a node in the tree view control's currently
        /// selected node's Nodes collection.
        /// </summary>
        public static void FindNodeByName(TreeView passedTreeView)
        {
            string searchKey = HEUtilities.Prompt.ShowDialog("Enter exact name of node to find (case insensitive):", "Find node by name", frmMainForm.Icon);

            TreeNode result = HEUtilities.GetChildNodeByName(passedTreeView.SelectedNode, searchKey);

            if (result != null)
            {
                // Select the node in the tree
                passedTreeView.SelectedNode = result;
            }
            else
            {
                MessageBox.Show("No results for search term " + searchKey);
            }
        }
        */

        /// <summary>
        /// Implements a search operator that can execute a query and populate a results set.
        /// </summary>
        public class HESearchOperator
        {
            /// <summary>
            /// Constructor that takes a HESearchHandler reference to it's parent.
            /// </summary>
            /// <param name="passedParent"></param>
            public HESearchOperator(HESearchHandler passedParent)
            {
                parent = passedParent ?? throw new NullReferenceException("passedParent was null.");
                rootNode = new HESearchHandlerTreeNode(this, "SEARCHOPERATORRESULTS", HETreeNodeType.SearchResultsSet, baseDisplayName);
                parent.rootNode.Nodes.Add(rootNode);
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
            public HETreeNode RootNode => rootNode;

            /// <summary>
            /// The root node of the Game Data tree.
            /// </summary>
            protected HETreeNode rootNode = null;

            /// <summary>
            /// Stores a reference to this object's parent, the HESearchHandler
            /// </summary>
            protected HESearchHandler parent = null;

            /// <summary>
            /// Public property to get the results list.
            /// </summary>
            public List<HETreeNode> Results
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
            protected List<HETreeNode> results = null;

            /// <summary>
            /// Determines whether the results set has members.
            /// </summary>
            /// <returns></returns>
            public bool HasResults()
            {
                if (results == null || results.Count() < 1 ) return false;
                else return true;
            }

            protected virtual string GenerateResultSetDisplayName()
            {
                string postfix = "";
                if (query != null)
                {
                    postfix = " " + query + " (" + results.Count().ToString() + ")";
                }
                return baseDisplayName + postfix;
            }

            /// <summary>
            /// Public property to access the Query.
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

            public HETreeNode StartingNode
            {
                get => startingNode;
                set => startingNode = value ?? throw new NullReferenceException("StartingNode was null.");
            }


            protected HETreeNode startingNode = null;

            /// <summary>
            /// Executes the query.
            /// </summary>
            /// <returns>Returns true if the result set has more than zero members.</returns>
            public bool Execute()
            {
                if (query == null) return false;
                else
                {
                    results = startingNode.ListOfAllChildNodes
                        .Where<HETreeNode>(f => f.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                            || f.Text.Contains(query, StringComparison.OrdinalIgnoreCase)
                            || f.NodeType.ToString().Contains(query, StringComparison.OrdinalIgnoreCase))
                        .ToList<HETreeNode>();
                    rootNode.Text = GenerateResultSetDisplayName();
                    return results.Count() > 0 ? true : false;
                }
            }
        }

        public class HEFindOperator : HESearchOperator
        {
            public HEFindOperator(HESearchHandler passedParent) : base(passedParent)
            {
                rootNode.NodeType = HETreeNodeType.SearchResultsSet;
                rootNode.Name = "FINDOPERATORRESULTS";
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
                string postfix = "";
                if (query != null)
                {
                    postfix = " '" + query + "' (" + results.Count().ToString() + ")";
                }
                return baseDisplayName + postfix;
            }





}
    }
}
