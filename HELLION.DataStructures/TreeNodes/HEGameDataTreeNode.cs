using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.HEImageList;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Derives an HETreeNode for use in the Game Data that can self-build child nodes
    /// based on the JToken stored in the node's Tag field.
    /// </summary>
    public class HEGameDataTreeNode : HETreeNode
    {
        public bool ChildNodesLoaded => childNodesLoaded;

        private bool childNodesLoaded = false;
        
        private int numChildTokens = 0;
        
        /// <summary>
        /// Constructor that takes a minimum of a name, but also optionally a type and text (display name).
        /// </summary>
        /// <param name="nodeName">Name of the new node.</param>
        /// <param name="nodeType">Type of the new node (HETreeNodeType enum)</param>
        /// <param name="nodeText">Text of the new node (Display Name). If not specified this defaults to the node's name.</param>
        /// <param name="nodeToolTipText">Tool tip text of the new node. If not specified this defaults to the node's text.</param>
        public HEGameDataTreeNode(string nodeName, HETreeNodeType newNodeType = HETreeNodeType.Unknown, string nodeText = "", string nodeToolTipText = "") 
            : base(nodeName, newNodeType, nodeText, nodeToolTipText)
        {
            childNodesLoaded = false;
        }

        /// <summary>
        /// Test new constructor that takes raw Json and formats the HEGameDataTreeNode appropriately.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="nodeName">Accepts a passed Name, if not provided a name will be auto-generated.</param>
        public HEGameDataTreeNode(JToken json, string nodeName = "")
        {
            childNodesLoaded = false;
            if (json == null) throw new InvalidOperationException("Passed Json data was null.");
            // Count child tokens
            numChildTokens = json.Count<JToken>();

            switch (json.Type)
            {
                case JTokenType.Object:
                    JObject tmpJObject = (JObject)json;
                    if (tmpJObject == null) throw new InvalidOperationException("tmpJObject was null");

                    // If Name hint provided, use that
                    if (nodeName != "") Name = nodeName;
                    else
                    {
                        // name lookup/generation used
                        string newNodeName = GenerateDisplayName(tmpJObject).Trim();
                        if (newNodeName != "")
                            Name = newNodeName;
                        else
                            Name = "Object";
                    }
                    Text = Name;
                        
                    // Set the node's type
                    NodeType = HETreeNodeType.JsonObject;
                        
                    // Set the node's tag to the JObject
                    Tag = tmpJObject;

                    break;

                case JTokenType.Array:
                    JArray tmpJArray = (JArray)json;
                    if (tmpJArray == null) throw new InvalidOperationException("tmpJArray was null");

                    if (nodeName != "")
                        Name = nodeName;
                    else
                        Name = "Array";

                    Text = Name;

                    // Set the node's type
                    NodeType = HETreeNodeType.JsonArray;
                        
                    // Set the node's tag to the JArray
                    Tag = tmpJArray;

                    break;

                case JTokenType.Property:
                    // It's a JProperty, similar to a key value pair
                    JProperty tmpJProperty = (JProperty)json;
                    if (tmpJProperty == null) throw new InvalidOperationException("tmpJProperty was null");

                    // If the Name hint has been provided, use that
                    if (nodeName != "") Name = nodeName;
                    else Name = tmpJProperty.Name;

                    Text = Name;

                    // Set the node's type
                    NodeType = HETreeNodeType.JsonProperty;

                    // Set the node's tag to the JProperty
                    Tag = tmpJProperty;

                    // Process value - this is old but may need something here
                    //HETreeNode temp = BuildHETreeNodeTreeFromJson(tmpJProperty.Value, maxDepth: maxDepth - 1, currentDepth: currentDepth + 1);

                    // Add the node
                    //newNode.Nodes.Add(temp ?? new HETreeNode("null", HETreeNodeType.Unknown));

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
                case JTokenType.Null:
                    JValue tmpJValue = (JValue)json;
                    if (tmpJValue == null) throw new InvalidOperationException("tmpJValue was null");
                    int newNodeImageIndex = 0;

                    switch (json.Type)
                    {
                        case JTokenType.Boolean:
                            // Bool (CheckDot)
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
                            // Default (checker board)
                            newNodeImageIndex = (int)HEObjectTypesImageList.Checkerboard_16x;
                            break;
                    }

                    // Set the Name based on the value of the JValue
                    if (tmpJValue.Value == null)
                    {
                        Name = "null";
                    }
                    else
                    {
                        Name = tmpJValue.Value.ToString();
                    }
                    Text = Name;

                    NodeType = HETreeNodeType.JsonValue;

                    // Update the ImageIndex and SelectedImageIndex directly - we're overriding the standard icon for a JsonValue
                    ImageIndex = newNodeImageIndex;
                    SelectedImageIndex = newNodeImageIndex;

                    break;

                default:
                    throw new InvalidOperationException("Unexpected json.Type: " + json.Type.ToString());
            }

            // Set ToolTipText
            StringBuilder sb = new StringBuilder();
            sb.Append("Name: " + Name + Environment.NewLine);
            sb.Append("Text: " + Text + Environment.NewLine);
            sb.Append("Type: " + NodeType.ToString() + Environment.NewLine);
            sb.Append("Tokens: " + numChildTokens.ToString() + Environment.NewLine);
            sb.Append("Nodes: " + Nodes.Count.ToString());
            ToolTipText = sb.ToString();

        }

        /// <summary>
        /// Creates child nodes appropriate to the child tokens in the JData.
        /// </summary>
        /// <param name="maxDepth">Defaults to 1</param>
        public void CreateChildNodesFromjData(int maxDepth = 1)
        {
            JToken jData = (JToken)Tag;
            if (jData != null && jData.Count() > 0)
            {
                if (!childNodesLoaded) // <-- check this
                {
                    foreach (JToken childToken in jData)
                    {
                        HEGameDataTreeNode newNode = new HEGameDataTreeNode(childToken);
                        Nodes.Add(newNode);
                    }
                    childNodesLoaded = true;
                }

                if (maxDepth > 1)
                {
                    foreach (HEGameDataTreeNode childNode in Nodes)
                    {
                        childNode.CreateChildNodesFromjData(maxDepth - 1);
                    }
                }

            }
        }


        /*
        /// <summary>
        /// Builds a section of node tree and recurses if necessary - can return null
        /// </summary>
        /// <param name="json"></param>
        /// <param name="maxDepth">Defaults to 10</param>
        /// <param name="currentDepth">Used internally</param>
        /// <param name="nodeName">Used to provide a hint to name an object instead of generating it</param>
        /// <param name="collapseJArrays">Non-functional</param>
        /// <param name="logToDebug"></param>
        /// <returns>Returns an HETreeNode that is the root of the built tree.</returns>
        public HETreeNode BuildHETreeNodeTreeFromJson(
            JToken json,
            int maxDepth = 10,
            int currentDepth = 0,
            string nodeName = "",
            bool collapseJArrays = false,
            bool logToDebug = false)
        {
            // Set up indenting for this level
            string thisLevelIndent = String.Join("| ", new String[currentDepth]);
            HETreeNode newNode = null; // node to represent this level of recursion - this is returned once children are populated recursively

            if (logToDebug)
            {
                Debug.Print("{0} :{1} BuildTree depth {2}({3}) ENTERED with type: {4}", DateTime.Now, thisLevelIndent, currentDepth, maxDepth, json.Type);
                Debug.Print(json.ToString());
            }

            if (json != null) // probably needs more tests that this
            {
                //JToken token = (JToken)json.Reverse();

                switch (json.Type)
                {
                    case JTokenType.Object:
                        // It's a JObject
                        // Depth and null check, if valid, create this node
                        JObject tmpJObject = (JObject)json;
                        string newNodeName = "";
                        if (tmpJObject != null)
                        {
                            // Creation of this node
                            if (nodeName != "")
                            {
                                // Name hint provided, use that
                                newNodeName = nodeName;
                            }
                            else
                            {
                                // name lookup/generation used
                                newNodeName = GenerateDisplayName(tmpJObject).Trim();
                                if (newNodeName == "")
                                {
                                    newNodeName = "Object";
                                }

                            }
                            newNode = new HETreeNode(newNodeName, HETreeNodeType.JsonObject)
                            {
                                // Set the node's tag to the JObject
                                Tag = tmpJObject
                            };

                            // Process any child tokens - actually JProperties in the case of a JObject
                            // Count children
                            int numChildTokens = json.Count<JToken>();
                            if (numChildTokens > 0)
                            {
                                if ((currentDepth + 1) > maxDepth)
                                {
                                    // It has child tokens, but we're at the maximum depth already
                                    // attach a single expansion node to the tree - this will be removed when the actual tree is generated

                                    HETreeNode tempExpansionNode = new HETreeNode("Click to expand " + numChildTokens.ToString() + " nodes...", HETreeNodeType.ExpansionAvailable);
                                    if (logToDebug)
                                        Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding (obj) expansion node", DateTime.Now, thisLevelIndent, currentDepth, maxDepth);
                                    // Add the  expansion node
                                    newNode.Nodes.Add(tempExpansionNode);
                                }
                                else
                                {
                                    // Process child tokens 
                                    int numChildProperties = 0;
                                    foreach (JToken token in tmpJObject.Children<JToken>().Reverse<JToken>())
                                    {
                                        numChildProperties++;
                                        if (maxDepth >= 2)
                                        {
                                            if (logToDebug) Debug.Print("{0} :{1} BuildTree depth {2}({3}) Calling recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);
                                            HETreeNode temp = BuildHETreeNodeTreeFromJson(token, maxDepth: maxDepth - 1, currentDepth: currentDepth + 1);
                                            if (logToDebug) Debug.Print("{0} :{1} BuildTree depth {2}({3}) Returning from recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);

                                            if (temp != null)
                                            {
                                                // Add the node
                                                if (logToDebug)
                                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding Node {3}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, newNode.Text);
                                                newNode.Nodes.Add(temp);
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            // We shouldn't be here!
                            if (logToDebug)
                                Debug.Print("{0} :{1} BuildTree depth {2}({3}) tmpJObject was null", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);
                        }
                        break;

                    case JTokenType.Array:
                        // It's a JArray
                        // Depth and null check, if valid, create this node
                        JArray tmpJArray = (JArray)json;
                        if (tmpJArray != null)
                        {
                            // Creation of this node
                            if (nodeName != "")
                            {
                                // Name hint provided, use that
                                newNode = new HETreeNode(nodeName, HETreeNodeType.JsonArray);
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
                            newNode.Tag = tmpJArray;

                            // Process any child tokens
                            int numChildTokens = json.Count<JToken>();
                            if (numChildTokens > 0)
                            {
                                if ((currentDepth + 1) > maxDepth)
                                {
                                    // It has child tokens, but we're at the maximum depth already
                                    // attach a single expansion node to the tree - this will be removed when the actual tree is generated

                                    HETreeNode tempExpansionNode = new HETreeNode("Click to expand " + numChildTokens.ToString() + " nodes...", HETreeNodeType.ExpansionAvailable);
                                    if (logToDebug)
                                        Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding (arr) expansion node", DateTime.Now, thisLevelIndent, currentDepth, maxDepth);
                                    // Add the  expansion node
                                    newNode.Nodes.Add(tempExpansionNode);
                                }
                                else
                                {
                                    // Process child tokens - actually JProperties in the case of a JObject
                                    foreach (JToken token in tmpJArray.Children<JToken>().Reverse<JToken>())
                                    {
                                        // Collapse Arrays mounts members of the array to the parent rather than creating a distinct node for the array. Defaults to off.
                                        if (collapseJArrays)
                                        {
                                            // Adjust parent node instead of using the generated node, or at least it used to - this needs re-doing in light of recent changes
                                            // Recursive call
                                            BuildHETreeNodeTreeFromJson(token, maxDepth: maxDepth - 1, currentDepth: currentDepth + 1);
                                            if (logToDebug)
                                            {
                                                Debug.Print("FINDME in collapseArrays!!!!");
                                                //Debug.Print("{0} :{1} BuildTree depth{1}({2}) CollapseArrays using parent {3}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, nodeParent.Name);
                                            }

                                        }
                                        else
                                        {
                                            // Add the node
                                            if (logToDebug) Debug.Print("{0} :{1} BuildTree depth {2}({3}) Calling recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);

                                            newNode.Nodes.Add(BuildHETreeNodeTreeFromJson(token, maxDepth: maxDepth, currentDepth: currentDepth + 1));

                                            if (logToDebug) Debug.Print("{0} :{1} BuildTree depth{1}({2}) Returning from recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);
                                        }
                                    } // End of foreach (token)
                                }
                            }
                        }
                        if (logToDebug)
                            Debug.Print("{0} :{1} BuildTree depth{1}({2}) Returning Node {3}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, newNode.Text);
                        break;


                    case JTokenType.Property:
                        // It's a JProperty, similar to a key value pair
                        // Depth and null check, if valid, create this node
                        JProperty tmpJProperty = (JProperty)json;
                        if (tmpJProperty != null)
                        {
                            if (maxDepth >= 0)
                            {
                                // Creation of this node
                                if (nodeName != "")
                                {
                                    // Name hint provided, use that
                                    newNode = new HETreeNode(nodeName, HETreeNodeType.JsonProperty);
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
                                newNode.Tag = tmpJProperty;

                                // Process value

                                if (logToDebug)
                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Calling recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);

                                HETreeNode temp = BuildHETreeNodeTreeFromJson(tmpJProperty.Value, maxDepth: maxDepth - 1, currentDepth: currentDepth + 1);

                                if (logToDebug)
                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Returning from recursion", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);

                                if (logToDebug)
                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding Node {4}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, newNode.Text);
                                // Add the node
                                newNode.Nodes.Add(temp ?? new HETreeNode("null", HETreeNodeType.Unknown));
                            }
                            else
                            {
                                if (logToDebug)
                                    Debug.Print("{0} :{1} BuildTree depth {2}({3}) Max Depth reached", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth);
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
                                    // Bool (CheckDot)
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
                                // Update the ImageIndex and SelectedImageIndex directly - we're overriding the standard icons
                                ImageIndex = newNodeImageIndex,
                                SelectedImageIndex = newNodeImageIndex,
                            };
                            if (logToDebug)
                                Debug.Print("{0} :{1} BuildTree depth {2}({3}) Adding Node {3}", DateTime.Now.ToString(), thisLevelIndent, currentDepth, maxDepth, newNode.Text);

                            //nodeParent.Nodes.Add(newNode);

                        }
                        break;

                    default:
                        //
                        if (logToDebug)
                        {
                            Debug.Print("BNTFJ {0} VALUE, type: {1} - NO ACTION TAKEN!", currentDepth, json.Type);
                            Debug.Print(json.ToString());
                        }
                        break;
                }
            }
            else
            {
                if (logToDebug)
                    Debug.Print("{0} : BuildTree depth {1}({2}) JToken was null", DateTime.Now.ToString(), currentDepth, maxDepth);

                return null;
            }

            if (logToDebug)
                Debug.Print("{0} : BuildTree depth {1}({2}) EXITING", DateTime.Now.ToString(), currentDepth, maxDepth);

            // Return the newNode
            if (newNode == null) Debug.Print("newNode was null");
            return newNode;
        }
        */

        /// <summary>
        /// Attempts to build a user-friendly name from available data in a JObject
        /// </summary>
        /// <param name="obj">Takes a JObject and attempts to generate a name from expected fields</param>
        /// <returns></returns>
        public string GenerateDisplayName(JObject obj)

        {
            StringBuilder sb = new StringBuilder();
            sb.Append(((string)obj["Registration"] + " " + (string)obj["Name"]).Trim());
            sb.Append((string)obj["GameName"]);
            sb.Append((string)obj["CategoryName"]);
            sb.Append((string)obj["name"]);
            //string[] prefabPathParts = obj["PrefabPath"].ToString().Split('\\');
            //sb.Append(prefabPathParts[prefabPathParts.Length - 1]);
            sb.Append((string)obj["PrefabPath"]);
            sb.Append((string)obj["RuleName"]);
            sb.Append((string)obj["TierName"]);
            sb.Append((string)obj["GroupName"]);
            if (sb.Length > 0) sb.Append(" ");
            sb.Append((string)obj["ItemID"]);
            return sb.ToString();
        }

    }
}
