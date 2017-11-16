using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HELLION.DataStructures
{
    public static class HEUtilities
    {
        // A class to hold utility functions

        public static string FindLatestRelease(string sGithubUsername, string sRepositoryName)
        {
            // http request GET /repos/owner/repo/releases
            JArray JData = FindAllGitHubReleases(sGithubUsername, sRepositoryName);

            IOrderedEnumerable<JToken> ioOrderedReleases = from s in JData //[""]
                                                           orderby (string)s["published_at"] descending
                                                           select s;
            string sLatestVersion = "";
            //StringBuilder sb = new StringBuilder();
            if (ioOrderedReleases.Count() > 0)
            {

                foreach (var item in ioOrderedReleases)
                {
                    if (sLatestVersion == "") sLatestVersion = (string)item["tag_name"];
                    //sb.Append((string)item["tag_name"] + " - " + (string)item["published_at"] + Environment.NewLine);
                }
            }
            //MessageBox.Show(sb.ToString());
            return sLatestVersion;
        }

        private static JArray FindAllGitHubReleases(string sGithubUsername, string sRepositoryName)
        {

            // Set the URL for the request
            string sURL = "https://api.github.com/repos/" + sGithubUsername + "/" + sRepositoryName + "/releases";

            JArray JData = null;

            try
            {
                // Create a new WebClient object to handle the HTTP request
                using (WebClient webClient = new WebClient())
                {
                    // Add a user-agent header
                    webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");

                    // Define up a stream object and assign the webClient's OpenRead
                    using (Stream stm = webClient.OpenRead(sURL))
                    {
                        // Create a StreamReader from the stream
                        using (StreamReader sr = new StreamReader(stm))
                        {
                            // Process the stream with the JSON Text Reader in to a JArray; was previously an IOrderedEnumerable<JToken> JObject
                            using (JsonTextReader jtr = new JsonTextReader(sr))
                            {
                                JData = (JArray)JToken.ReadFrom(jtr);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Some error handling to be implemented here
                MessageBox.Show("Exception caught during StreamReader or JsonTextReader while processing FindAllGitHubReleases" + Environment.NewLine + e.ToString());
            }

            // return the JArray
            return JData;

        }

        public static int GetImageIndexByNodeType(HETreeNodeType NodeType)
        {
            // Returns the defined image list index for the node type 

            switch (NodeType)
            {
                case HETreeNodeType.CelestialBody:
                case HETreeNodeType.DefCelestialBody:
                    return (int)HEObjectTypesImageList.Contrast_16x;
                    //break;
                case HETreeNodeType.Asteroid:
                case HETreeNodeType.DefAsteroid:
                    return (int)HEObjectTypesImageList.CheckDot_16x;
                    //break;
                case HETreeNodeType.Ship:
                    return (int)HEObjectTypesImageList.AzureLogicApp_16x;
                    //break;
                case HETreeNodeType.Player:
                    return (int)HEObjectTypesImageList.Actor_16x;
                    //break;
                case HETreeNodeType.DynamicObject:
                case HETreeNodeType.DefDynamicObject:
                    return (int)HEObjectTypesImageList.Property_16x;
                    //break;
                case HETreeNodeType.Scene:
                    return (int)HEObjectTypesImageList.a3DScene_16x;
                //break;
                case HETreeNodeType.DefStructure:
                    return (int)HEObjectTypesImageList.Component_16x;
                default:
                    return (int)HEObjectTypesImageList.Checkerboard_16x;
                    //break;
            }
        }

        // Not currently used, will need to have similar version that find nodes by other parameters
        public static TreeNode GetNodeByName(TreeNode nCurrentNode, string key)
        {
            TreeNode[] nodes = nCurrentNode.Nodes.Find(key, true);
            return nodes.Length > 0 ? nodes[0] : null;
        }


        public static class EnumUtil
        {
            // Returns the values of an enum of given type T
            // usage: var values = EnumUtil.GetValues<Foos>();

            public static IEnumerable<T> GetValues<T>()
            {
                return Enum.GetValues(typeof(T)).Cast<T>();
            }
        }

        // HSLColor Class by Rich Newman
        // https://richnewman.wordpress.com/about/code-listings-and-diagrams/hslcolor-class/
        public class HSLColor
        {
            // Private data members below are on scale 0-1
            // They are scaled for use externally based on scale
            private double hue = 1.0;
            private double saturation = 1.0;
            private double luminosity = 1.0;

            private const double scale = 240.0;

            public double Hue
            {
                get { return hue * scale; }
                set { hue = CheckRange(value / scale); }
            }
            public double Saturation
            {
                get { return saturation * scale; }
                set { saturation = CheckRange(value / scale); }
            }
            public double Luminosity
            {
                get { return luminosity * scale; }
                set { luminosity = CheckRange(value / scale); }
            }

            private double CheckRange(double value)
            {
                if (value < 0.0)
                    value = 0.0;
                else if (value > 1.0)
                    value = 1.0;
                return value;
            }

            public override string ToString()
            {
                return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", Hue, Saturation, Luminosity);
            }

            public string ToRGBString()
            {
                Color color = (Color)this;
                return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
            }

            #region Casts to/from System.Drawing.Color
            public static implicit operator Color(HSLColor hslColor)
            {
                double r = 0, g = 0, b = 0;
                if (hslColor.luminosity != 0)
                {
                    if (hslColor.saturation == 0)
                        r = g = b = hslColor.luminosity;
                    else
                    {
                        double temp2 = GetTemp2(hslColor);
                        double temp1 = 2.0 * hslColor.luminosity - temp2;

                        r = GetColorComponent(temp1, temp2, hslColor.hue + 1.0 / 3.0);
                        g = GetColorComponent(temp1, temp2, hslColor.hue);
                        b = GetColorComponent(temp1, temp2, hslColor.hue - 1.0 / 3.0);
                    }
                }
                return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
            }

            private static double GetColorComponent(double temp1, double temp2, double temp3)
            {
                temp3 = MoveIntoRange(temp3);
                if (temp3 < 1.0 / 6.0)
                    return temp1 + (temp2 - temp1) * 6.0 * temp3;
                else if (temp3 < 0.5)
                    return temp2;
                else if (temp3 < 2.0 / 3.0)
                    return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
                else
                    return temp1;
            }
            private static double MoveIntoRange(double temp3)
            {
                if (temp3 < 0.0)
                    temp3 += 1.0;
                else if (temp3 > 1.0)
                    temp3 -= 1.0;
                return temp3;
            }
            private static double GetTemp2(HSLColor hslColor)
            {
                double temp2;
                if (hslColor.luminosity < 0.5)  //<=??
                    temp2 = hslColor.luminosity * (1.0 + hslColor.saturation);
                else
                    temp2 = hslColor.luminosity + hslColor.saturation - (hslColor.luminosity * hslColor.saturation);
                return temp2;
            }

            public static implicit operator HSLColor(Color color)
            {
                HSLColor hslColor = new HSLColor()
                {
                    hue = color.GetHue() / 360.0, // we store hue as 0-1 as opposed to 0-360 
                    luminosity = color.GetBrightness(),
                    saturation = color.GetSaturation()
                };
                return hslColor;
            }
            #endregion

            public void SetRGB(int red, int green, int blue)
            {
                HSLColor hslColor = (HSLColor)Color.FromArgb(red, green, blue);
                this.hue = hslColor.hue;
                this.saturation = hslColor.saturation;
                this.luminosity = hslColor.luminosity;
            }

            public HSLColor() { }
            public HSLColor(Color color)
            {
                SetRGB(color.R, color.G, color.B);
            }
            public HSLColor(int red, int green, int blue)
            {
                SetRGB(red, green, blue);
            }
            public HSLColor(double hue, double saturation, double luminosity)
            {
                this.Hue = hue;
                this.Saturation = saturation;
                this.Luminosity = luminosity;
            }

        } // End of HSLColor

        // from http://joelabrahamsson.com/syntax-highlighting-json-with-c/
        public static string SyntaxHighlightJson(string original)
        {
            return Regex.Replace(
              original,
              @"(¤(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\¤])*¤(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)".Replace('¤', '"'),
              match => {
                  var cls = "number";
                  if (Regex.IsMatch(match.Value, @"^¤".Replace('¤', '"')))
                  {
                      if (Regex.IsMatch(match.Value, ":$"))
                      {
                          cls = "key";
                      }
                      else
                      {
                          cls = "string";
                      }
                  }
                  else if (Regex.IsMatch(match.Value, "true|false"))
                  {
                      cls = "boolean";
                  }
                  else if (Regex.IsMatch(match.Value, "null"))
                  {
                      cls = "null";
                  }
                  return "<span class=\"" + cls + "\">" + match + "</span>";
              });
        }





        // adapted from https://stackoverflow.com/questions/18769634/creating-tree-view-dynamically-according-to-json-text-in-winforms
        public static TreeNode Json2Tree(JArray root, string rootName = "", string nodeName = "")
        {
            TreeNode parent = new TreeNode(rootName);
            int index = 0;

            foreach (JToken obj in root)
            {
                TreeNode child = new TreeNode(string.Format("{0}[{1}]", nodeName, index++));
                foreach (KeyValuePair<string, JToken> token in (JObject)obj)
                {
                    switch (token.Value.Type)
                    {
                        case JTokenType.Array:
                        case JTokenType.Object:
                            child.Nodes.Add(Json2Tree((JObject)token.Value, token.Key));
                            break;
                        default:
                            child.Nodes.Add(GetChild(token));
                            break;
                    }
                }
                parent.Nodes.Add(child);
            }

            return parent;
        }

        public static TreeNode Json2Tree(JObject root, string text = "")
        {
            TreeNode parent = new TreeNode(text);

            foreach (KeyValuePair<string, JToken> token in root)
            {

                switch (token.Value.Type)
                {
                    case JTokenType.Object:
                        parent.Nodes.Add(Json2Tree((JObject)token.Value, token.Key));
                        break;
                    case JTokenType.Array:
                        int index = 0;
                        foreach (JToken element in (JArray)token.Value)
                        {
                            parent.Nodes.Add(Json2Tree((JObject)element, string.Format("{0}[{1}]", token.Key, index++)));
                        }

                        if (index == 0) parent.Nodes.Add(string.Format("{0}[ ]", token.Key)); //to handle empty arrays
                        break;
                    default:
                        parent.Nodes.Add(GetChild(token));
                        break;
                }
            }

            return parent;
        }

        private static TreeNode GetChild(KeyValuePair<string, JToken> token)
        {
            TreeNode child = new TreeNode(token.Key);
            child.Nodes.Add(string.IsNullOrEmpty(token.Value.ToString()) ? "n/a" : token.Value.ToString());
            return child;
        }

    }
}
