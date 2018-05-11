using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.HEImageList;

namespace HELLION.DataStructures
{
    /// <summary>
    /// Derives an HETreeNode for use in the Game Data that can self-build child nodes
    /// based on the JToken stored in the node's Tag field.
    /// </summary>
    public partial class HEGameDataTreeNode : HETreeNode
    {
        /// <summary>
        /// Constructor that takes a minimum of a name, but also optionally a type and text (display name).
        /// </summary>
        /// <param name="nodeName">Name of the new node.</param>
        /// <param name="nodeType">Type of the new node (HETreeNodeType enum)</param>
        /// <param name="nodeText">Text of the new node (Display Name). If not specified this defaults to the node's name.</param>
        /// <param name="nodeToolTipText">Tool tip text of the new node. If not specified this defaults to the node's text.</param>
        public HEGameDataTreeNode(object ownerObject, string nodeName = null, HETreeNodeType newNodeType = HETreeNodeType.Unknown,
             string nodeText = null, string nodeToolTipText = null)
            : base(ownerObject, nodeName, newNodeType, nodeText, nodeToolTipText)
        {

        }

        /// <summary>
        /// Test new constructor that takes raw Json and formats the HEGameDataTreeNode appropriately.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="nodeName">Accepts a passed Name, if not provided a name will be auto-generated.</param>
        public HEGameDataTreeNode(object ownerObject, JToken passedJson, string nodeName = "") : base(ownerObject)
        {
            JData = passedJson ?? throw new NullReferenceException("Passed Json data was null.");

            // Count child tokens
            numChildTokens = JData.Count<JToken>();

            switch (JData.Type)
            {
                case JTokenType.Object:
                    JObject tmpJObject = (JObject)JData;
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
                    // Tag = tmpJObject;

                    break;

                case JTokenType.Array:
                    JArray tmpJArray = (JArray)JData;
                    if (tmpJArray == null) throw new InvalidOperationException("tmpJArray was null");

                    if (nodeName != "")
                        Name = nodeName;
                    else
                        Name = "Array";

                    Text = Name;

                    // Set the node's type
                    NodeType = HETreeNodeType.JsonArray;

                    // Set the node's tag to the JArray
                    // Tag = tmpJArray;

                    break;

                case JTokenType.Property:
                    // It's a JProperty, similar to a key value pair
                    JProperty tmpJProperty = (JProperty)JData;
                    if (tmpJProperty == null) throw new InvalidOperationException("tmpJProperty was null");

                    // If the Name hint has been provided, use that
                    if (nodeName != "") Name = nodeName;
                    else Name = tmpJProperty.Name;

                    Text = Name;

                    // Set the node's type
                    NodeType = HETreeNodeType.JsonProperty;

                    // Set the node's tag to the JProperty
                    // Tag = tmpJProperty;


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
                    JValue tmpJValue = (JValue)JData;
                    if (tmpJValue == null) throw new InvalidOperationException("tmpJValue was null");
                    int newNodeImageIndex = 0;

                    switch (JData.Type)
                    {
                        case JTokenType.Boolean:
                            // Bool (CheckDot)
                            newNodeImageIndex = (int)HEIconsImageNames.CheckDot_16x;
                            break;
                        case JTokenType.Bytes:
                            // Binary
                            newNodeImageIndex = (int)HEIconsImageNames.Binary_16x;
                            break;
                        case JTokenType.Integer:
                        case JTokenType.Float:
                            // Number
                            newNodeImageIndex = (int)HEIconsImageNames.DomainType_16x;
                            break;
                        case JTokenType.String:
                        case JTokenType.Comment:
                        case JTokenType.Guid:
                        case JTokenType.Uri:
                            // Text
                            newNodeImageIndex = (int)HEIconsImageNames.String_16x;
                            break;
                        case JTokenType.Date:
                        case JTokenType.TimeSpan:
                            // Time/Date
                            newNodeImageIndex = (int)HEIconsImageNames.DateTimeAxis_16x;
                            break;
                        default:
                            // Default (checker board)
                            newNodeImageIndex = (int)HEIconsImageNames.Checkerboard_16x;
                            break;
                    }

                    // Set the Name based on the value of the JValue
                    Name = tmpJValue.Value == null ? "null" : tmpJValue.Value.ToString();
                    Text = Name;

                    NodeType = HETreeNodeType.JsonValue;

                    // Update the ImageIndex and SelectedImageIndex directly - we're overriding the standard icon for a JsonValue
                    ImageIndex = newNodeImageIndex;
                    SelectedImageIndex = newNodeImageIndex;

                    break;

                default:
                    throw new InvalidOperationException("Unexpected json.Type: " + JData.Type.ToString());
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
        /// Tracks whether child nodes have been loaded for this node.
        /// </summary>
        public bool ChildNodesLoaded { get; protected set; } = false;

        /// <summary>
        /// A count of the number of child tokens in the JData.
        /// </summary>
        private int numChildTokens = 0;

        /// <summary>
        /// Holds a reference to the JToken object this node represents.
        /// </summary>
        public JToken JData { get; set; } = null;

        /// <summary>
        /// Creates child nodes appropriate to the child tokens in the JData.
        /// </summary>
        /// <param name="maxDepth">Defaults to 1</param>
        public void CreateChildNodesFromjData(int maxDepth = 1)
        {
            //JToken JData = (JToken)Tag;
            if (JData != null && JData.Count() > 0)
            {
                if (!ChildNodesLoaded) // <-- check this
                {
                    foreach (JToken childToken in JData.Reverse<JToken>())
                    {
                        HEGameDataTreeNode newNode = new HEGameDataTreeNode(this, childToken);
                        Nodes.Add(newNode);
                    }
                    ChildNodesLoaded = true;
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

            // The following three are to support the Hellion Station Blueprint Format
            sb.Append((string)obj["DisplayName"]);
            sb.Append((string)obj["StructureType"]);
            sb.Append((string)obj["PortName"]);

            if (sb.Length > 0) sb.Append(" ");
            sb.Append((string)obj["ItemID"]);
            return sb.ToString().Trim();
        }

    }
}
