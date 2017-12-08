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
using System.Reflection;

namespace HELLION.DataStructures
{
    /// <summary>
    /// A class to hold utility functions.
    /// </summary>
    public static class HEUtilities
    {

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

        public static ImageList BuildObjectTypesImageList()
        {
            // Create a new ImageList to hold images used as icons in the tree and list views
            ImageList ilObjectTypesImageList = new ImageList();

            // there must be a better way of doing this
            string[] sListOfImages1 = new string[] {
                "3DCameraOrbit_16x.png",
                "3DExtrude_16x.png",
                "3DScene_16x.png",
                "Actor_16x.png",
                "Add_grey_16x.png",
                "Alert_16x.png",
                "Aserif_16x.png",
                "Assembly_16x.png",
                "Attribute_16x.png",
                "AzureDefaultResource_16x.png",
                "AzureLogicApp_16x.png",
                "AzureLogicApp_color_16x.png",
                "AzureResourceGroup_16x.png",
                "AzureResourceTypeView_16x.png",
                "AzureVirtualMachineExtension_16x.png",
                "BalanceBrace_16x.png",
                "BatchFile_16x.png",
                "BehaviorAction_16x.png",
                "Binary_16x.png",
                "Bios_16x.png",
                "BlankFile_16x.png",
                "Bolt_16x.png",
                "BranchRelationshipChild_16x.png",
                "BranchRelationshipCousin_16x.png",
                "BranchRelationshipGroup_16x.png",
                "BranchRelationshipParent_16x.png",
                "BranchRelationshipSibling_16x.png",
                "BranchRelationship_16x.png",
                "Branch_16x.png",
                "Brightness_16x.png",
                "BubbleChart_16x.png",
                "Bug_16x.png",
                "Builder_16x.png",
                "BulletList_16x.png",
                "ButtonIcon_16x.png",
                "Callout_16x.png",
                "CheckDot_16x.png",
                "Checkerboard_16x.png",
                "Collection_16x.png",
                "ComponentDiagram_16x.png",
                "Component_16x.png",
                "Contrast_16x.png",
                "CordovaMultidevice_16x.png",
                "CSWorkflowDiagram_16x.png",
                "DarkTheme_16x.png",
                "DateTimeAxis_16x.png",
                "Diagnose_16x.png",
                "Dictionary_16x.png",
                "Document_16x.png",
                "DomainType_16x.png",
                "Driver_16x.png",
                "Ellipsis_16x.png",
                "EndpointComponent_16x.png",
                "Event_16x.png",
                "Expander_16x.png",
                "ExplodedPieChart_16x.png",
                "FeedbackBubble_16x.png",
                "FeedbackSad_16x.png",
                "FeedbackSmile_16x.png",
                "FileCollection_16x.png",
                "FileError_16x.png",
                "FileGroupError_16x.png",
                "FileGroupWarning_16x.png",
                "FileGroup_16x.png",
                "FileOK_16x.png",
                "FileWarning_16x.png",
                "Filter_16x.png",
                "FindResults_16x.png",
                "Flag_16x.png",
                "FolderError_16x.png",
                "older_16x.png",
                "Gauge_16x.png",
                "HotSpot_16x.png",
                "Hub_16x.png",
                "JS_16x.png",
                "Label_16x.png",
                "ListFolder_16x.png",
                "Marquee_16x.png",
                "Numeric_16x.png",
                "PermissionFile_16x.png",
                "PieChart_16x.png",
                "Property_16x.png",
                "Rename_16x.png",
                "SemanticZoom_16x.png",
                "Settings_16x.png",
                "Shader_16x.png",
                "Share_16x.png",
                "String_16x.png",
                "Toolbox_16x.png",
                "TreeView_16x.png",
            };

            // Use System.Reflection to get a list of all resource names
            string[] embeddedResources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            
            // Get the currently executing assembly name
            string sEntryAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            // Process string array of resource names (this includes the namespace name)
            foreach (string embeddedResource in embeddedResources)
            {
                if (embeddedResource.Contains(sEntryAssemblyName + ".Images."))
                {
                    // Adds ANY file in the Images folder to the image list
                    ilObjectTypesImageList.Images.Add(Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource)));
                }
                else
                {
                    // not an image reference
                    throw new InvalidDataException();
                }
            }
                
                    

            // Return the image list
            return ilObjectTypesImageList;
        } // End of BuildObjectTypesImageList()

        public static int GetImageIndexByNodeType(HETreeNodeType NodeType)
        {
            // Returns the defined image list index for the node type 

            switch (NodeType)
            {

                case HETreeNodeType.SolarSystemView:
                    return (int)HEObjectTypesImageList.Share_16x;

                case HETreeNodeType.DataView:
                    return (int)HEObjectTypesImageList.ListFolder_16x;

                case HETreeNodeType.SearchResultsView:
                    return (int)HEObjectTypesImageList.FindResults_16x;

                case HETreeNodeType.CelestialBody:
                case HETreeNodeType.DefCelestialBody:
                    return (int)HEObjectTypesImageList.Shader_16x;

                case HETreeNodeType.SolSysStar:
                    return (int)HEObjectTypesImageList.Brightness_16x;

                case HETreeNodeType.SolSysPlanet:
                    return (int)HEObjectTypesImageList.Contrast_16x;

                case HETreeNodeType.SolSysMoon:
                    return (int)HEObjectTypesImageList.DarkTheme_16x;

                case HETreeNodeType.Asteroid:
                case HETreeNodeType.DefAsteroid:
                    return (int)HEObjectTypesImageList.CheckDot_16x;

                case HETreeNodeType.Ship:
                    return (int)HEObjectTypesImageList.AzureLogicApp_16x;

                case HETreeNodeType.Player:
                    return (int)HEObjectTypesImageList.Actor_16x;

                case HETreeNodeType.DynamicObject:
                case HETreeNodeType.DefDynamicObject:
                    return (int)HEObjectTypesImageList.Driver_16x;

                case HETreeNodeType.Scene:
                    return (int)HEObjectTypesImageList.a3DScene_16x;

                case HETreeNodeType.DefStructure:
                    return (int)HEObjectTypesImageList.Component_16x;

                case HETreeNodeType.SpawnPoint:
                case HETreeNodeType.DoomControllerData:
                case HETreeNodeType.SpawnManagerData:
                    return (int)HEObjectTypesImageList.a3DCameraOrbit_16x;

                case HETreeNodeType.ExpansionAvailable:
                    return (int)HEObjectTypesImageList.Expander_16x;

                case HETreeNodeType.JsonArray:
                    return (int)HEObjectTypesImageList.Assembly_16x;

                case HETreeNodeType.JsonObject:
                    return (int)HEObjectTypesImageList.Settings_16x;

                case HETreeNodeType.JsonProperty:
                    return (int)HEObjectTypesImageList.Property_16x;

                case HETreeNodeType.JsonValue:
                    return (int)HEObjectTypesImageList.DomainType_16x;

                case HETreeNodeType.SaveFile:
                case HETreeNodeType.DataFile:
                    return (int)HEObjectTypesImageList.Document_16x;

                case HETreeNodeType.SaveFileError:
                case HETreeNodeType.DataFileError:
                    return (int)HEObjectTypesImageList.FileError_16x;


                case HETreeNodeType.DataFolder:
                    return (int)HEObjectTypesImageList.Folder_16x;

                case HETreeNodeType.DataFolderError:
                    return (int)HEObjectTypesImageList.FolderError_16x;

                default:
                    return (int)HEObjectTypesImageList.Checkerboard_16x;
                    //break;
            }
        }

        public static TreeNode GetChildNodeByName(TreeNode nCurrentNode, string key)
        {
            // Gets the first node that matches the given key in the current nodes children
            TreeNode[] nodes = nCurrentNode.Nodes.Find(key, searchAllChildren: true);
            return nodes.Length > 0 ? nodes[0] : null;
        }

        /// <summary>
        /// Enum utility class.
        /// </summary>
        public static class EnumUtil
        {
            /// <summary>
            /// Returns the values of an enum of given type T
            /// usage: var values = EnumUtil.GetValues<Foos>();
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public static IEnumerable<T> GetValues<T>()
            {
                return Enum.GetValues(typeof(T)).Cast<T>();
            }
        }

        /// <summary>
        /// Returns a System.Drawing.Color object for a given string, computed from the hash of the string.
        /// </summary>
        /// <param name="sInputString">String to generate a colour for</param>
        /// <returns></returns>
        public static Color ConvertStringToColor(string sInputString)
        {
            int iHue = (Math.Abs(sInputString.GetHashCode()) % 24) * 10;
            // Debug.Print("iHue: {0}", iHue.ToString());

            HSLColor hslColor = new HSLColor(hue: iHue, saturation: 200.0, luminosity: 80.0);
            return hslColor;
        }

        
        // 3rd party code

        /// <summary>
        /// Based on an example from https://stackoverflow.com/questions/5427020/prompt-dialog-in-windows-forms
        /// </summary>
        public static class Prompt
        {
            public static string ShowDialog(string text, string caption)
            {
                Form prompt = new Form()
                {
                    Width = 500,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedSingle,
                    MinimizeBox = false,
                    MaximizeBox = false,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen
                };
                Label textLabel = new Label() { Left = 50, Top = 20, Width = 400, Text = text };
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
                Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(textLabel);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
        }

        /// <summary>
        /// HSLColor Class by Rich Newman
        /// https://richnewman.wordpress.com/about/code-listings-and-diagrams/hslcolor-class/
        /// </summary>
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

        /// <summary>
        /// SyntaxHighlightJson class by Joel Abrahmsson.
        /// from http://joelabrahamsson.com/syntax-highlighting-json-with-c/
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
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

    }
}
