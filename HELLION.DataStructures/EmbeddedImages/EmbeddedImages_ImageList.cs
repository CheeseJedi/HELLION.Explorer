using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using HELLION.DataStructures.UI;
using static HELLION.DataStructures.StaticDataHelper;

namespace HELLION.DataStructures.EmbeddedImages
{
    /// <summary>
    /// A class to implement loading and enumeration of icon images embedded in this library.
    /// </summary>
    /// <remarks>
    /// Currently incomplete and requires manual re-generation of multiple lists if the contents
    /// of the Images folder changes in any way.
    /// </remarks>
    public class EmbeddedImages_ImageList
    {
        #region Enumerations

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

        #endregion

        /// <summary>
        /// Returns the defined image list index for the node type.
        /// </summary>
        /// <param name="NodeType">Specifies the HETreeNode type to get the image index of.</param>
        /// <returns>Returns an integer representing the image index.</returns>
        public static int GetIconImageIndexByNodeType(Base_TN_NodeType NodeType)
        {
            switch (NodeType)
            {
                case Base_TN_NodeType.SolarSystemView:
                    return (int)HEIconsImageNames.Share_16x;

                case Base_TN_NodeType.DataView:
                    return (int)HEIconsImageNames.ListFolder_16x;

                case Base_TN_NodeType.SearchResultsView:
                case Base_TN_NodeType.SearchHandler:
                case Base_TN_NodeType.SearchResultsSet:
                    return (int)HEIconsImageNames.FindResults_16x;

                //case Base_TN_NodeType.CelestialBody:
                //case Base_TN_NodeType.DefCelestialBody:
                //    return (int)HEIconsImageNames.Shader_16x;

                case Base_TN_NodeType.Star:
                    return (int)HEIconsImageNames.Brightness_16x;

                case Base_TN_NodeType.Planet:
                    return (int)HEIconsImageNames.Contrast_16x;

                case Base_TN_NodeType.Moon:
                    return (int)HEIconsImageNames.DarkTheme_16x;

                case Base_TN_NodeType.Asteroid:
                    //case Base_TN_NodeType.DefAsteroid:
                    return (int)HEIconsImageNames.CheckDot_16x;

                case Base_TN_NodeType.Ship:
                    return (int)HEIconsImageNames.AzureLogicApp_16x;

                case Base_TN_NodeType.Player:
                    return (int)HEIconsImageNames.Actor_16x;

                //case Base_TN_NodeType.DynamicObject:
                //case Base_TN_NodeType.DefDynamicObject:
                //    return (int)HEIconsImageNames.Driver_16x;

                //case Base_TN_NodeType.Scene:
                //    return (int)HEIconsImageNames.a3DScene_16x;

                //case Base_TN_NodeType.DefStructure:
                //    return (int)HEIconsImageNames.Component_16x;

                //case Base_TN_NodeType.SpawnPoint:
                //case Base_TN_NodeType.DoomControllerData:
                //case Base_TN_NodeType.SpawnManagerData:
                //    return (int)HEIconsImageNames.a3DCameraOrbit_16x;


                case Base_TN_NodeType.JsonArray:
                    return (int)HEIconsImageNames.Assembly_16x;

                case Base_TN_NodeType.JsonObject:
                    return (int)HEIconsImageNames.Settings_16x;

                case Base_TN_NodeType.JsonProperty:
                    return (int)HEIconsImageNames.Property_16x;

                case Base_TN_NodeType.JsonValue:
                    return (int)HEIconsImageNames.DomainType_16x;

                case Base_TN_NodeType.JsonBoolean:
                    return (int)HEIconsImageNames.CheckDot_16x;

                case Base_TN_NodeType.JsonBytes:
                    return (int)HEIconsImageNames.Binary_16x;

                case Base_TN_NodeType.JsonString:
                case Base_TN_NodeType.JsonUri:
                case Base_TN_NodeType.JsonComment:
                    return (int)HEIconsImageNames.String_16x;

                case Base_TN_NodeType.JsonInteger:
                case Base_TN_NodeType.JsonFloat:
                case Base_TN_NodeType.JsonGuid:
                    return (int)HEIconsImageNames.DomainType_16x;

                case Base_TN_NodeType.JsonDate:
                case Base_TN_NodeType.JsonTimeSpan:
                    return (int)HEIconsImageNames.DateTimeAxis_16x;

                case Base_TN_NodeType.JsonNull:
                    return (int)HEIconsImageNames.Checkerboard_16x;
                    
                case Base_TN_NodeType.SaveFile:
                case Base_TN_NodeType.DataFile:
                    return (int)HEIconsImageNames.Document_16x;

                case Base_TN_NodeType.SaveFileError:
                case Base_TN_NodeType.DataFileError:
                    return (int)HEIconsImageNames.FileError_16x;

                case Base_TN_NodeType.DataFolder:
                    return (int)HEIconsImageNames.Folder_16x;

                case Base_TN_NodeType.DataFolderError:
                    return (int)HEIconsImageNames.FolderError_16x;

                case Base_TN_NodeType.BlueprintsView:
                    return (int)HEIconsImageNames.CordovaMultidevice_16x;

                case Base_TN_NodeType.Blueprint:
                    return (int)HEIconsImageNames.CSWorkflowDiagram_16x;

                case Base_TN_NodeType.BlueprintCollection:
                    return (int)HEIconsImageNames.BlueprintFolder_16x;

                case Base_TN_NodeType.BlueprintHierarchyView:
                    return (int)HEIconsImageNames.TreeView_16x;

                case Base_TN_NodeType.BlueprintDataView:
                    return (int)HEIconsImageNames.BalanceBrace_16x;

                case Base_TN_NodeType.BlueprintStructureDefinitionView:
                    return (int)HEIconsImageNames.Bios_16x;

                case Base_TN_NodeType.BlueprintStructure:
                case Base_TN_NodeType.BlueprintStructureDefinition:
                    return (int)HEIconsImageNames.Component_16x;

                case Base_TN_NodeType.BlueprintRootStructure:
                    return (int)HEIconsImageNames.Hub_16x;

                case Base_TN_NodeType.BlueprintDockingPort:
                case Base_TN_NodeType.BlueprintDockingPortDefinition:
                    return (int)HEIconsImageNames.Bolt_16x;

                default:
                    return (int)HEIconsImageNames.Checkerboard_16x;
            }
        }

        /// <summary>
        /// Returns the ImageIndex for a particular blueprint structure type.
        /// </summary>
        /// <param name="sceneID"></param>
        /// <returns></returns>
        public static int GetStructureImageIndexBySceneID(HEStructureSceneID sceneID)
        {
            switch (sceneID)
            {
                case HEStructureSceneID.AltCorp_CorridorModule:
                    return (int)HEStructuresImageNames.STRUCT_CIM;

                case HEStructureSceneID.AltCorp_CorridorIntersectionModule:
                    return (int)HEStructuresImageNames.STRUCT_CTM;

                case HEStructureSceneID.AltCorp_Corridor45TurnModule:
                    return (int)HEStructuresImageNames.STRUCT_CLM;

                case HEStructureSceneID.AltCorp_Shuttle_SARA:
                    return (int)HEStructuresImageNames.STRUCT_ARG;

                case HEStructureSceneID.ALtCorp_PowerSupply_Module:
                    return (int)HEStructuresImageNames.STRUCT_PSM;

                case HEStructureSceneID.AltCorp_LifeSupportModule:
                    return (int)HEStructuresImageNames.STRUCT_LSM;

                case HEStructureSceneID.AltCorp_Cargo_Module:
                    return (int)HEStructuresImageNames.STRUCT_CBM;

                case HEStructureSceneID.AltCorp_CorridorVertical:
                    return (int)HEStructuresImageNames.STRUCT_CSM;

                case HEStructureSceneID.AltCorp_Command_Module:
                    return (int)HEStructuresImageNames.STRUCT_CM;

                case HEStructureSceneID.AltCorp_Corridor45TurnRightModule:
                    return (int)HEStructuresImageNames.STRUCT_CRM;

                case HEStructureSceneID.AltCorp_StartingModule:
                    return (int)HEStructuresImageNames.STRUCT_OUTPOST;

                case HEStructureSceneID.AltCorp_AirLock:
                    return (int)HEStructuresImageNames.STRUCT_AM;

                case HEStructureSceneID.Generic_Debris_JuncRoom001:
                case HEStructureSceneID.Generic_Debris_JuncRoom002:
                case HEStructureSceneID.Generic_Debris_Corridor001:
                case HEStructureSceneID.Generic_Debris_Corridor002:
                    return (int)HEStructuresImageNames.STRUCT_Unspecified;

                case HEStructureSceneID.AltCorp_DockableContainer:
                    return (int)HEStructuresImageNames.STRUCT_IC;

                case HEStructureSceneID.AltCorp_CrewQuarters_Module:
                    return (int)HEStructuresImageNames.STRUCT_CQM;

                case HEStructureSceneID.AltCorp_SolarPowerModule:
                    return (int)HEStructuresImageNames.STRUCT_SPM;

                case HEStructureSceneID.AltCorp_Shuttle_CECA:
                    return (int)HEStructuresImageNames.STRUCT_Unspecified;

                case HEStructureSceneID.AltCorp_FabricatorModule:
                    return (int)HEStructuresImageNames.STRUCT_FM;

                case HEStructureSceneID.Unspecified:
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
                if (_structureImageList == null)
                {
                    // Populate the image list
                    BuildImageList();
                }
                return _structureImageList;
            }
        }

        private ImageList _structureImageList = null;

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
            _structureImageList = new ImageList
            {
                // Set the ImageSize property to a larger size 
                // (the default is 16 x 16).
                ImageSize = new Size(180, 180)
            };

            // Process string array of resource names (this includes the namespace name)
            foreach (string embeddedResource in embeddedResourceNames)
            {
                if (embeddedResource.Contains(entryAssemblyName + ".EmbeddedImages.Icons16x."))
                {
                    // Caution! Adds ANY file in the _EmbeddedImages folder to the image list! Don't put non-images in this folder!
                    iconImageList.Images.Add(Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource)));
                }
                else if (embeddedResource.Contains(entryAssemblyName + ".EmbeddedImages.Structures180x."))
                {
                    // Caution! Adds ANY file in the _EmbeddedImages folder to the image list! Don't put non-images in this folder!
                    _structureImageList.Images.Add(Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResource)));
                }
                else
                {
                    // not an image reference
                    Debug.Print("Embedded resource {0}", embeddedResource);
                }
            }
        }
    }
}
