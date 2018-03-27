using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

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
        public enum HEIconsImageNames
        {
            a3DCameraOrbit_16x = 0,
            a3DExtrude_16x,
            a3DScene_16x,
            Actor_16x,
            Add_grey_16x,
            Alert_16x,
            Aserif_16x,
            ASPXFile_16x,
            AssemblyInfoFile_16x,
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
            BinaryFile_16x,
            Binary_16x,
            Bios_16x,
            BlankFile_16x,
            BlueprintFolder_16x,
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
            ComponentFile_16x,
            Component_16x,
            ConfigurationEditor_16x,
            ConfigurationFile_16x,
            Contrast_16x,
            CordovaMultidevice_16x,
            CSWorkflowDiagram_16x,
            DarkTheme_16x,
            DateTimeAxis_16x,
            Diagnose_16x,
            Dictionary_16x,
            DocumentLibraryFolder_16x,
            Document_16x,
            DomainType_16x,
            Driver_16x,
            EditPage_16x,
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
            FindinFiles_16x,
            FindNext_16x,
            FindPrevious_16x,
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
            LocalServer_16x,
            Marquee_16x,
            Numeric_16x,
            PermissionFile_16x,
            PieChart_16x,
            Property_16x,
            Rename_16x,
            SearchFolder_16x,
            SemanticZoom_16x,
            Settings_16x,
            Shader_16x,
            Share_16x,
            String_16x,
            Toolbox_16x,
            TreeView_16x,

        };

        public enum HEStructuresImageNames
        {
            STRUCT_AM = 0,
            STRUCT_ARG,
            STRUCT_CBM,
            STRUCT_CIM,
            STRUCT_CLM,
            STRUCT_CM,
            STRUCT_CQM,
            STRUCT_CRM,
            STRUCT_CSM,
            STRUCT_CTM,
            STRUCT_FM,
            STRUCT_IC,
            STRUCT_LSM,
            STRUCT_OUTPOST,
            STRUCT_PSM,
            STRUCT_SPM,
            STRUCT_Unspecified,
        }


        /// <summary>
        /// Returns the defined image list index for the node type.
        /// </summary>
        /// <param name="NodeType">Specifies the HETreeNode type to get the image index of.</param>
        /// <returns>Returns an integer representing the image index.</returns>
        public static int GetIconImageIndexByNodeType(HETreeNodeType NodeType)
        {
            switch (NodeType)
            {
                case HETreeNodeType.SolarSystemView:
                    return (int)HEIconsImageNames.Share_16x;

                case HETreeNodeType.DataView:
                    return (int)HEIconsImageNames.ListFolder_16x;

                case HETreeNodeType.SearchResultsView:
                case HETreeNodeType.SearchHandler:
                case HETreeNodeType.SearchResultsSet:
                    return (int)HEIconsImageNames.FindResults_16x;

                //case HETreeNodeType.CelestialBody:
                //case HETreeNodeType.DefCelestialBody:
                //    return (int)HEIconsImageNames.Shader_16x;

                case HETreeNodeType.Star:
                    return (int)HEIconsImageNames.Brightness_16x;

                case HETreeNodeType.Planet:
                    return (int)HEIconsImageNames.Contrast_16x;

                case HETreeNodeType.Moon:
                    return (int)HEIconsImageNames.DarkTheme_16x;

                case HETreeNodeType.Asteroid:
                    //case HETreeNodeType.DefAsteroid:
                    return (int)HEIconsImageNames.CheckDot_16x;

                case HETreeNodeType.Ship:
                    return (int)HEIconsImageNames.AzureLogicApp_16x;

                case HETreeNodeType.Player:
                    return (int)HEIconsImageNames.Actor_16x;

                //case HETreeNodeType.DynamicObject:
                //case HETreeNodeType.DefDynamicObject:
                //    return (int)HEIconsImageNames.Driver_16x;

                //case HETreeNodeType.Scene:
                //    return (int)HEIconsImageNames.a3DScene_16x;

                //case HETreeNodeType.DefStructure:
                //    return (int)HEIconsImageNames.Component_16x;

                //case HETreeNodeType.SpawnPoint:
                //case HETreeNodeType.DoomControllerData:
                //case HETreeNodeType.SpawnManagerData:
                //    return (int)HEIconsImageNames.a3DCameraOrbit_16x;


                case HETreeNodeType.JsonArray:
                    return (int)HEIconsImageNames.Assembly_16x;

                case HETreeNodeType.JsonObject:
                    return (int)HEIconsImageNames.Settings_16x;

                case HETreeNodeType.JsonProperty:
                    return (int)HEIconsImageNames.Property_16x;

                case HETreeNodeType.JsonValue:
                    return (int)HEIconsImageNames.DomainType_16x;

                case HETreeNodeType.SaveFile:
                case HETreeNodeType.DataFile:
                    return (int)HEIconsImageNames.Document_16x;

                case HETreeNodeType.SaveFileError:
                case HETreeNodeType.DataFileError:
                    return (int)HEIconsImageNames.FileError_16x;

                case HETreeNodeType.DataFolder:
                    return (int)HEIconsImageNames.Folder_16x;

                case HETreeNodeType.DataFolderError:
                    return (int)HEIconsImageNames.FolderError_16x;

                case HETreeNodeType.BlueprintsView:
                    return (int)HEIconsImageNames.CordovaMultidevice_16x;

                case HETreeNodeType.Blueprint:
                    return (int)HEIconsImageNames.CSWorkflowDiagram_16x;

                case HETreeNodeType.BlueprintCollection:
                    return (int)HEIconsImageNames.BlueprintFolder_16x;

                case HETreeNodeType.BlueprintHierarchyView:
                    return (int)HEIconsImageNames.TreeView_16x;

                case HETreeNodeType.BlueprintDataView:
                    return (int)HEIconsImageNames.BalanceBrace_16x;

                case HETreeNodeType.BlueprintStructureDefinitionView:
                    return (int)HEIconsImageNames.Bios_16x;

                case HETreeNodeType.BlueprintStructure:
                case HETreeNodeType.BlueprintStructureDefinition:
                    return (int)HEIconsImageNames.Component_16x;

                case HETreeNodeType.BlueprintRootStructure:
                    return (int)HEIconsImageNames.Hub_16x;

                case HETreeNodeType.BlueprintDockingPort:
                case HETreeNodeType.BlueprintDockingPortDefinition:
                    return (int)HEIconsImageNames.Bolt_16x;

                default:
                    return (int)HEIconsImageNames.Checkerboard_16x;
            }
        }


        public static int GetStructureImageIndexByStructureType(HEBlueprintStructureTypes StructureType)
        {
            switch (StructureType)
            {
                case HEBlueprintStructureTypes.AM:
                    return (int)HEStructuresImageNames.STRUCT_AM;

                case HEBlueprintStructureTypes.ARG:
                    return (int)HEStructuresImageNames.STRUCT_ARG;

                case HEBlueprintStructureTypes.CBM:
                    return (int)HEStructuresImageNames.STRUCT_CBM;

                case HEBlueprintStructureTypes.CIM:
                    return (int)HEStructuresImageNames.STRUCT_CIM;

                case HEBlueprintStructureTypes.CLM:
                    return (int)HEStructuresImageNames.STRUCT_CLM;

                case HEBlueprintStructureTypes.CM:
                    return (int)HEStructuresImageNames.STRUCT_CM;

                case HEBlueprintStructureTypes.CQM:
                    return (int)HEStructuresImageNames.STRUCT_CQM;

                case HEBlueprintStructureTypes.CRM:
                    return (int)HEStructuresImageNames.STRUCT_CRM;

                case HEBlueprintStructureTypes.CSM:
                    return (int)HEStructuresImageNames.STRUCT_CSM;

                case HEBlueprintStructureTypes.CTM:
                    return (int)HEStructuresImageNames.STRUCT_CTM;

                case HEBlueprintStructureTypes.FM:
                    return (int)HEStructuresImageNames.STRUCT_FM;

                case HEBlueprintStructureTypes.IC:
                    return (int)HEStructuresImageNames.STRUCT_IC;

                case HEBlueprintStructureTypes.LSM:
                    return (int)HEStructuresImageNames.STRUCT_LSM;

                case HEBlueprintStructureTypes.OUTPOST:
                    return (int)HEStructuresImageNames.STRUCT_OUTPOST;

                case HEBlueprintStructureTypes.PSM:
                    return (int)HEStructuresImageNames.STRUCT_PSM;

                case HEBlueprintStructureTypes.SPM:
                    return (int)HEStructuresImageNames.STRUCT_SPM;

                case HEBlueprintStructureTypes.Unspecified:
                default:
                    return (int)HEStructuresImageNames.STRUCT_Unspecified;

            }
        }


        /// <summary>
        /// Public read-only property to get the IconImageList so it can be bound to WinForms controls etc.
        /// </summary>
        /// <remarks>Triggers de-streaming of the images population on first request.</remarks>
        public ImageList IconImageList
        {
            get
            {
                if (iconImageList == null)
                {
                    // Populate the image list
                    BuildImageList();
                }
                return iconImageList;
            }
        }

        /// <summary>
        /// Private field for the list of images.
        /// </summary>
        private ImageList iconImageList = null;

        /// <summary>
        /// Public read-only property to get the IconImageList so it can be bound to WinForms controls etc.
        /// </summary>
        /// <remarks>Triggers de-streaming of the images population on first request.</remarks>
        public ImageList StructureImageList
        {
            get
            {
                if (structureImageList == null)
                {
                    // Populate the image list
                    BuildImageList();
                }
                return structureImageList;
            }
        }

        private ImageList structureImageList = null;


        /// <summary>
        /// Builds an ImageList from the embedded resources.
        /// </summary>
        /// <remarks>
        /// It is *CRITICAL* that the load order matches the HEIconsImageNames enum.
        /// </remarks>
        /// <returns></returns>
        private void BuildImageList()
        {
            // Use System.Reflection to get a list of all resource names
            string[] embeddedResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            // Get the currently executing assembly name
            string entryAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            // Create ImageList to hold images used as icons in the tree and list views.
            iconImageList = new ImageList();

            // Create ImageList to hold structure (ship/module) images for identification use.
            structureImageList = new ImageList();
            // Set the ImageSize property to a larger size 
            // (the default is 16 x 16).
            structureImageList.ImageSize = new Size(180, 180);

            // Process string array of resource names (this includes the namespace name)
            foreach (string embeddedResource in embeddedResourceNames)
            {
                Debug.Print(embeddedResource);
                
                if (embeddedResource.Contains(entryAssemblyName + "._EmbeddedImages.Icons16x."))
                {
                    Debug.Print("Icon");
                    // Caution! Adds ANY file in the _EmbeddedImages folder to the image list! Don't put non-images in this folder!
                    iconImageList.Images.Add(Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource)));
                }
                
                else if (embeddedResource.Contains(entryAssemblyName + "._EmbeddedImages.Structures180x."))
                {
                    Debug.Print("Structure");

                    // Caution! Adds ANY file in the _EmbeddedImages folder to the image list! Don't put non-images in this folder!
                    structureImageList.Images.Add(Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource)));
                }
                
                else
                {
                    // not an image reference
                    throw new InvalidOperationException();
                }
                
            }
        }
    }
}
