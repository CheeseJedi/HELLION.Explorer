using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using System.Runtime.CompilerServices;

namespace HELLION.DataStructures
{
    /// <summary>
    /// 
    /// </summary>
    public class HEBlueprintStructureDefinitionsFile : HEJsonBaseFile
    {
        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="passedFileInfo">The FileInfo representing the file to be loaded.</param>
        public HEBlueprintStructureDefinitionsFile(FileInfo passedFileInfo, object passedParentObject, int populateNodeTreeDepth)
        {
            // Blueprint files

            if (passedParentObject == null) throw new NullReferenceException();
            else parent = (IHENotificationReceiver)passedParentObject;

            if (passedFileInfo == null) throw new NullReferenceException();
            else
            {
                fileInfo = passedFileInfo;

                rootNode = new HEBlueprintTreeNode(File.Name, HETreeNodeType.Blueprint, nodeToolTipText: File.FullName);

                dataViewRootNode = new HEGameDataTreeNode("Data View", HETreeNodeType.DataView,
                    nodeToolTipText: "Shows a representation of the Json data that makes up this blueprint.");

                //hierarchyViewRootNode = new HESolarSystemTreeNode("Hierarchy View", HETreeNodeType.BlueprintHierarchyView,
                //    nodeToolTipText: "Shows a tree-based view of the modules and their docking hierarchy.");

                rootNode.Nodes.Add(dataViewRootNode);
                // rootNode.Nodes.Add(hierarchyViewRootNode);

                if (!File.Exists) throw new FileNotFoundException();
                else
                {
                    LoadFile();
                    // Populate the blueprint object.
                    DeserialiseToBlueprintObject();
                    // Populate the data view.
                    dataViewRootNode.Tag = jData;
                    dataViewRootNode.CreateChildNodesFromjData(populateNodeTreeDepth);
                }
                // Populate the hierarchy view.
                //BuildHierarchyView();
            }
        }

        public new HETreeNode RootNode => rootNode;

        /// <summary>
        /// This class overrides the type of root node to represent a blueprint.
        /// </summary>
        protected new HETreeNode rootNode;

        public HEGameDataTreeNode DataViewRootNode => dataViewRootNode;

        protected HEGameDataTreeNode dataViewRootNode = null;

        public HEBlueprintStructureDefinitions BlueprintStructureDefinitionsObject => blueprintStructureDefinitionsObject;

        /// <summary>
        /// This is the actual blueprint - serialised and de-serialised from here.
        /// </summary>
        protected HEBlueprintStructureDefinitions blueprintStructureDefinitionsObject = null;



        public void DeserialiseToBlueprintObject()
        {
            blueprintStructureDefinitionsObject = jData.ToObject<HEBlueprintStructureDefinitions>();
            //blueprintStructureDefinitionsObject.ReconnectChildParentStructure();
        }

        public void SerialiseFromBlueprintObject()
        {
            throw new NotImplementedException("Not implemented yet.");
        }
    }


}
