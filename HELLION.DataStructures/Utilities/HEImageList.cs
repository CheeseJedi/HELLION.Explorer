using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace HELLION.DataStructures
{
    /// <summary>
    /// A class to implement loading and enumeration of icon images embedded in this library.
    /// </summary>
    /// <remarks>
    /// Currently incomplete and requires manual re-generation of multiple lists if the contents
    /// of the Images folder changes in any way.
    /// </remarks>
    public class HEImageList
    {
        /// <summary>
        /// This enum is used to look up the images that are embedded resources in this library
        /// and determine the index of a known image name.
        /// </summary>
        /// <remarks>
        /// CAUTION! This list needs to *exactly* match the directory listing of the _EmbeddedImages
        /// folder, and all images in that folder must have the VS Build Action set to Embedded Resource.
        /// In an ideal situation this list would be built dynamically, perhaps something for the future.
        /// </remarks>
        public enum HEObjectTypesImageList
        {
            a3DCameraOrbit_16x = 0,
            a3DExtrude_16x,
            a3DScene_16x,
            Actor_16x,
            Add_grey_16x,
            Alert_16x,
            Aserif_16x,
            Assembly_16x,
            Attribute_16x,
            AzureDefaultResource_16x,
            AzureLogicApp_16x,
            AzureLogicApp_color_16x,
            AzureResourceGroup_16x,
            AzureResourceTypeView_16x,
            AzureVirtualMachineExtension_16x,
            BalanceBrace_16x,
            BatchFile_16x,
            BehaviorAction_16x,
            Binary_16x,
            Bios_16x,
            BlankFile_16x,
            Bolt_16x,
            BranchRelationshipChild_16x,
            BranchRelationshipCousin_16x,
            BranchRelationshipGroup_16x,
            BranchRelationshipParent_16x,
            BranchRelationshipSibling_16x,
            BranchRelationship_16x,
            Branch_16x,
            Brightness_16x,
            BubbleChart_16x,
            Bug_16x,
            Builder_16x,
            BulletList_16x,
            ButtonIcon_16x,
            Callout_16x,
            CheckDot_16x,
            Checkerboard_16x,
            Collection_16x,
            ComponentDiagram_16x,
            Component_16x,
            Contrast_16x,
            CordovaMultidevice_16x,
            CSWorkflowDiagram_16x,
            DarkTheme_16x,
            DateTimeAxis_16x,
            Diagnose_16x,
            Dictionary_16x,
            Document_16x,
            DomainType_16x,
            Driver_16x,
            Ellipsis_16x,
            EndpointComponent_16x,
            Event_16x,
            Expander_16x,
            ExplodedPieChart_16x,
            FeedbackBubble_16x,
            FeedbackSad_16x,
            FeedbackSmile_16x,
            FileCollection_16x,
            FileError_16x,
            FileGroupError_16x,
            FileGroupWarning_16x,
            FileGroup_16x,
            FileOK_16x,
            FileWarning_16x,
            Filter_16x,
            FindResults_16x,
            Flag_16x,
            FolderError_16x,
            Folder_16x,
            Gauge_16x,
            HotSpot_16x,
            Hub_16x,
            JS_16x,
            Label_16x,
            ListFolder_16x,
            Marquee_16x,
            Numeric_16x,
            PermissionFile_16x,
            PieChart_16x,
            Property_16x,
            Rename_16x,
            SemanticZoom_16x,
            Settings_16x,
            Shader_16x,
            Share_16x,
            String_16x,
            Toolbox_16x,
            TreeView_16x,
        };

        /// <summary>
        /// Returns the defined image list index for the node type.
        /// </summary>
        /// <param name="NodeType">Specifies the HETreeNode type to get the image index of.</param>
        /// <returns>Returns an integer representing the image index.</returns>
        public static int GetImageIndexByNodeType(HETreeNodeType NodeType)
        {
            switch (NodeType)
            {
                case HETreeNodeType.SolarSystemView:
                    return (int)HEObjectTypesImageList.Share_16x;

                case HETreeNodeType.DataView:
                    return (int)HEObjectTypesImageList.ListFolder_16x;

                case HETreeNodeType.SearchResultsView:
                case HETreeNodeType.SearchHandler:
                case HETreeNodeType.SearchResultsSet:
                    return (int)HEObjectTypesImageList.FindResults_16x;

                //case HETreeNodeType.CelestialBody:
                //case HETreeNodeType.DefCelestialBody:
                //    return (int)HEObjectTypesImageList.Shader_16x;

                case HETreeNodeType.Star:
                    return (int)HEObjectTypesImageList.Brightness_16x;

                case HETreeNodeType.Planet:
                    return (int)HEObjectTypesImageList.Contrast_16x;

                case HETreeNodeType.Moon:
                    return (int)HEObjectTypesImageList.DarkTheme_16x;

                case HETreeNodeType.Asteroid:
                    //case HETreeNodeType.DefAsteroid:
                    return (int)HEObjectTypesImageList.CheckDot_16x;

                case HETreeNodeType.Ship:
                    return (int)HEObjectTypesImageList.AzureLogicApp_16x;

                case HETreeNodeType.Player:
                    return (int)HEObjectTypesImageList.Actor_16x;

                //case HETreeNodeType.DynamicObject:
                //case HETreeNodeType.DefDynamicObject:
                //    return (int)HEObjectTypesImageList.Driver_16x;

                //case HETreeNodeType.Scene:
                //    return (int)HEObjectTypesImageList.a3DScene_16x;

                //case HETreeNodeType.DefStructure:
                //    return (int)HEObjectTypesImageList.Component_16x;

                //case HETreeNodeType.SpawnPoint:
                //case HETreeNodeType.DoomControllerData:
                //case HETreeNodeType.SpawnManagerData:
                //    return (int)HEObjectTypesImageList.a3DCameraOrbit_16x;

                //case HETreeNodeType.ExpansionAvailable:
                //    return (int)HEObjectTypesImageList.Expander_16x;

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

                case HETreeNodeType.BlueprintsView:
                    return (int)HEObjectTypesImageList.ListFolder_16x;

                case HETreeNodeType.Blueprint:
                    return (int)HEObjectTypesImageList.CSWorkflowDiagram_16x;

                default:
                    return (int)HEObjectTypesImageList.Checkerboard_16x;
            }
        }

        /// <summary>
        /// Public read-only property to get the ImageList so it can be bound to WinForms controls etc.
        /// </summary>
        /// <remarks>Triggers de-streaming of the images population on first request.</remarks>
        public ImageList ImageList
        {
            get
            {
                if (imageList == null)
                {
                    // Populate the image list
                    BuildImageList();
                }
                return imageList;
            }
        }

        /// <summary>
        /// Private field for the list of images.
        /// </summary>
        private ImageList imageList = null;

        /// <summary>
        /// Builds an ImageList from the embedded resources.
        /// </summary>
        /// <remarks>
        /// It is *CRITICAL* that the load order matches the HEObjectTypesImageList enum.
        /// </remarks>
        /// <returns></returns>
        private void BuildImageList()
        {
            // Create a new ImageList to hold images used as icons in the tree and list views
            imageList = new ImageList();

            // Use System.Reflection to get a list of all resource names
            string[] embeddedResources = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            // Get the currently executing assembly name
            string sEntryAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            // Process string array of resource names (this includes the namespace name)
            foreach (string embeddedResource in embeddedResources)
            {
                if (embeddedResource.Contains(sEntryAssemblyName + "._EmbeddedImages."))
                {
                    // Caution! Adds ANY file in the _EmbeddedImages folder to the image list! Don't put non-images in this folder!
                    imageList.Images.Add(Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource)));
                }
                else
                {
                    // not an image reference
                    throw new InvalidOperationException();
                }
            }
        } // End of BuildImageList()
    }
}
