using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace HELLION.DataStructures.UI
{
    /// <summary>
    /// Derives an HETreeNode for use in the Game Data that can self-build child nodes
    /// based on the JToken stored in the node's Tag field.
    /// </summary>
    public class Json_TN : Base_TN
    {
        private JToken _jData = null;
        #region Constructors

        /// <summary>
        /// Constructor that takes a minimum of a name, but also optionally a type and text (display name).
        /// </summary>
        /// <param name="nodeName">Name of the new node.</param>
        /// <param name="nodeType">Type of the new node (Base_TN_NodeType enum)</param>
        /// <param name="nodeText">Text of the new node (Display Name). If not specified this defaults to the node's name.</param>
        /// <param name="nodeToolTipText">Tool tip text of the new node. If not specified this defaults to the node's text.</param>
        public Json_TN(Iparent_Base_TN ownerObject, string nodeName = null, Base_TN_NodeType newNodeType = Base_TN_NodeType.Unknown)
            : base(ownerObject, nodeName, newNodeType)
        { }

        /// <summary>
        /// Test new constructor that takes raw Json and formats the Json_TN appropriately.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="nodeName">Accepts a passed Name, if not provided a name will be auto-generated.</param>
        public Json_TN(Iparent_Base_TN ownerObject, JToken passedJson, string nodeName = null) : base(ownerObject)
        {
            // Enable auto-name generation.
            AutoGenerateName = true;

            // Set the JData.
            JData = passedJson ?? throw new NullReferenceException("Unable to construct Json_TN: Passed JToken was null.");

            // Set the node type.
            NodeType = DetectNodeTypeFromJToken();

            // Trigger name generation.
            RefreshName();

        }

        #endregion

        #region Properties

        /// <summary>
        /// Holds a reference to the JToken object this node represents.
        /// </summary>
        public JToken JData
        {
            get => _jData;
            set
            {
                if (_jData != value)
                {
                    _jData = value;

                    // Trigger name update.

                }
            }
        }

        /// <summary>
        /// Tracks whether child nodes have been loaded for this node.
        /// </summary>
        public bool ChildNodesLoaded => VerifyAllJTokensHaveNodes();

        /// <summary>
        /// Returns a count of the number of child tokens in the JData JToken.
        /// </summary>
        private int numChildTokens => JData.Count();

        /// <summary>
        /// Used to store a reference to the tree node in the Solar System that was created
        /// from this node and is cross-linked to. 
        /// Only used by nodes that have a Solar System representation.
        /// </summary>
        public SolarSystem_TN LinkedSolarSystemNode { get; protected set; } = null;

        /// <summary>
        /// Redefine the default object name for use with JObjects.
        /// </summary>
        protected override string DefaultObjectName => "unnamed Object";

        /// <summary>
        /// Define a default name for arrays - these aren't tokens that have names or
        /// other parameters.
        /// </summary>
        protected string DefaultArrayName => "Array";

        #endregion

        #region Overridden Methods

        /// <summary>
        /// Generates a name based on the JData type or contents.
        /// </summary>
        /// <returns></returns>
        protected override string GenerateName()
        {

            const string noJData = "No JData";

            if (JData == null) return noJData;

            switch (JData.Type)
            {
                case JTokenType.Object:
                    string _generatedName = GenerateNameFromJObject((JObject)JData);
                    return (_generatedName == String.Empty) ? DefaultObjectName : _generatedName;

                case JTokenType.Array:
                    return DefaultArrayName;

                case JTokenType.Property:
                    // It's a JProperty, similar to a key value pair
                    return ((JProperty)JData).Name;

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
                    // Set the Name based on the value of the JValue
                    return ((JValue)JData).Value == null ? "null" : ((JValue)JData).Value.ToString();

                default:
                    return "Unexpected Json Type: " + JData.Type.ToString();
            }
        }

        /// <summary>
        /// Generates a fresh ToolTipText.
        /// </summary>
        /// <returns></returns>
        protected override string GenerateToolTipText()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Name: " + Name + Environment.NewLine);
            sb.Append("Text: " + Text + Environment.NewLine);
            sb.Append("Type: " + NodeType.ToString() + Environment.NewLine);
            sb.Append("Tokens: " + numChildTokens.ToString() + Environment.NewLine);
            sb.Append("Nodes: " + Nodes.Count.ToString());
            // sb.Append("NodeType: " + NodeType + Environment.NewLine);
            // sb.Append("FullPath: " + FullPath + Environment.NewLine);

            return sb.ToString();
        }

        #endregion

        #region Methods

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
                        Json_TN newNode = new Json_TN(this, childToken);
                        Nodes.Add(newNode);
                    }
                    // ChildNodesLoaded = true;
                }
                if (maxDepth > 1)
                {
                    foreach (Json_TN childNode in Nodes)
                    {
                        childNode.CreateChildNodesFromjData(maxDepth - 1);
                    }
                }
            }
        }

        /// <summary>
        /// Determines the appropriate node type for the JToken's content type.
        /// </summary>
        /// <returns></returns>
        private Base_TN_NodeType DetectNodeTypeFromJToken()
        {
            if (JData != null)
            {
                switch (JData.Type)
                {
                    case JTokenType.Object:
                        return Base_TN_NodeType.JsonObject;
                    case JTokenType.Array:
                        return Base_TN_NodeType.JsonArray;
                    case JTokenType.Property:
                        return Base_TN_NodeType.JsonProperty;

                    // Json Value types.
                    case JTokenType.Boolean:
                        return Base_TN_NodeType.JsonBoolean;
                    case JTokenType.Bytes:
                        return Base_TN_NodeType.JsonBytes;
                    case JTokenType.Comment:
                        return Base_TN_NodeType.JsonComment;
                    case JTokenType.Date:
                        return Base_TN_NodeType.JsonDate;
                    case JTokenType.Integer:
                        return Base_TN_NodeType.JsonInteger;
                    case JTokenType.Float:
                        return Base_TN_NodeType.JsonFloat;
                    case JTokenType.Guid:
                        return Base_TN_NodeType.JsonGuid;
                    case JTokenType.String:
                        return Base_TN_NodeType.JsonString;
                    case JTokenType.TimeSpan:
                        return Base_TN_NodeType.JsonTimeSpan;
                    case JTokenType.Uri:
                        return Base_TN_NodeType.JsonUri;
                    case JTokenType.Null:
                        return Base_TN_NodeType.JsonNull;

                    default:
                        return Base_TN_NodeType.Unknown;
                }
            }
            return Base_TN_NodeType.Unknown;
        }

        /// <summary>
        /// Determines whether the number of child tokens matches the number of child nodes.
        /// </summary>
        /// <returns></returns>
        private bool VerifyAllJTokensHaveNodes()
        {
            if (JData == null || numChildTokens != Nodes.Count) return false;
            return true;
        }

        /// <summary>
        /// Attempts to build a user-friendly name from available data in a JObject
        /// </summary>
        /// <param name="obj">Takes a JObject and attempts to generate a name from expected fields</param>
        /// <returns></returns>
        private string GenerateNameFromJObject(JObject obj)
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

        /// <summary>
        /// Creates and returns a cross-linked SolarSystem_TN from this Json_TN.
        /// </summary>
        /// <param name="nodeType"></param>
        /// <remarks>
        /// Utilises a SolarSystem_TN constructor that takes a reference to an Json_TN
        /// and uses the JToken within it's Tag field to set various properties but MUST be passed a valid
        /// Base_TN_NodeType.
        /// </remarks>
        /// <returns></returns>
        public SolarSystem_TN CreateLinkedSolarSystemNode(Base_TN_NodeType nodeType)
        {
            LinkedSolarSystemNode = new SolarSystem_TN(this, nodeType);
            return LinkedSolarSystemNode;
        }


        #endregion

        //private void NewNameMethod()
        //{
        //    const string DefaultObjectName = "unnamed Object";
        //    const string defaultArrayName = "unnamed Array";

        //    switch (JData.Type)
        //    {
        //        case JTokenType.Object:
        //            JObject tmpJObject = (JObject)JData;
        //            if (tmpJObject == null) throw new InvalidOperationException("tmpJObject was null");

        //            // name lookup/generation used
        //            string newNodeName = GenerateNameFromJObject(tmpJObject).Trim();
        //            if (newNodeName != String.Empty)
        //                Name = newNodeName;
        //            else
        //                Name = DefaultObjectName;
        //            break;

        //        case JTokenType.Array:
        //            Name = defaultArrayName;
        //            break;

        //        case JTokenType.Property:
        //            // It's a JProperty, similar to a key value pair
        //            JProperty tmpJProperty = (JProperty)JData;
        //            if (tmpJProperty == null) throw new InvalidOperationException("tmpJProperty was null");

        //            Name = tmpJProperty.Name;
        //            break;

        //        case JTokenType.Boolean:
        //        case JTokenType.Bytes:
        //        case JTokenType.Comment:
        //        case JTokenType.Date:
        //        case JTokenType.Float:
        //        case JTokenType.Guid:
        //        case JTokenType.Integer:
        //        case JTokenType.String:
        //        case JTokenType.TimeSpan:
        //        case JTokenType.Uri:
        //        case JTokenType.Null:
        //            JValue tmpJValue = (JValue)JData;
        //            if (tmpJValue == null) throw new InvalidOperationException("tmpJValue was null");
        //            //int newNodeImageIndex = 0;

        //            //newNodeImageIndex = NewMethod();

        //            // Set the Name based on the value of the JValue
        //            Name = tmpJValue.Value == null ? "null" : tmpJValue.Value.ToString();
        //            //Text = Name;  // No longer necessary as the Base_TN updates it's text based on the name.

        //            NodeType = Base_TN_NodeType.JsonValue;

        //            // Update the ImageIndex and SelectedImageIndex directly - we're overriding the standard icon for a JsonValue
        //            //ImageIndex = newNodeImageIndex;
        //            //SelectedImageIndex = ImageIndex;

        //            break;

        //        default:
        //            throw new InvalidOperationException("Unexpected json.Type: " + JData.Type.ToString());
        //    }
        //}

        //private int aNewMethod()
        //{
        //    int newNodeImageIndex;
        //    switch (JData.Type)
        //    {
        //        case JTokenType.Boolean:
        //            // Bool (CheckDot)
        //            newNodeImageIndex = (int)HEIconsImageNames.CheckDot_16x;
        //            break;
        //        case JTokenType.Bytes:
        //            // Binary
        //            newNodeImageIndex = (int)HEIconsImageNames.Binary_16x;
        //            break;
        //        case JTokenType.Integer:
        //        case JTokenType.Float:
        //            // Number
        //            newNodeImageIndex = (int)HEIconsImageNames.DomainType_16x;
        //            break;
        //        case JTokenType.String:
        //        case JTokenType.Comment:
        //        case JTokenType.Guid:
        //        case JTokenType.Uri:
        //            // Text
        //            newNodeImageIndex = (int)HEIconsImageNames.String_16x;
        //            break;
        //        case JTokenType.Date:
        //        case JTokenType.TimeSpan:
        //            // Time/Date
        //            newNodeImageIndex = (int)HEIconsImageNames.DateTimeAxis_16x;
        //            break;
        //        default:
        //            // Default (checker board)
        //            newNodeImageIndex = (int)HEIconsImageNames.Checkerboard_16x;
        //            break;
        //    }

        //    return newNodeImageIndex;
        //}


    }
}
