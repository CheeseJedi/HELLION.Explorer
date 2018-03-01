using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HELLION.DataStructures
{
    public class HEBlueprints : IHENotificationReceiver
    {
        /// <summary>
        /// Public property for the root node of the Search Handler tree.
        /// </summary>
        public HEBlueprintTreeNode RootNode => rootNode;

        /// <summary>
        /// Field for root node of the Game Data tree.
        /// </summary>
        private HEBlueprintTreeNode rootNode = null;

        /// <summary>
        /// Public getter for the Blueprint Collection
        /// </summary>
        public HEJsonFileCollection Blueprintcollection => blueprintCollection;

        /// <summary>
        /// the StaticDataFileCollection object which enumerates the Blueprints folder and builds  
        /// node trees to a preconfigured depth of each of the .json files in that folder.
        /// </summary>
        private HEJsonFileCollection blueprintCollection = null;

        /// <summary>
        /// 
        /// </summary>
        private DirectoryInfo blueprintCollectionFolderInfo = null;

        public HEBlueprints()
        {
            rootNode = new HEBlueprintTreeNode("BLUEPRINTSVIEW", HETreeNodeType.Blueprint, "Blueprints", "Hellion Station Blueprints");
            blueprintCollectionFolderInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\HELLION.Explorer\Blueprints");
            Initialise();
        }

        public void Initialise()
        {

            if (blueprintCollectionFolderInfo != null && blueprintCollectionFolderInfo.Exists)
            {
                blueprintCollection = new HEJsonFileCollection(blueprintCollectionFolderInfo, HEJsonFileCollectionType.BlueprintsFolder, this, autoPopulateTreeDepth: 8);
                if (blueprintCollection.RootNode == null) throw new Exception("StaticData rootNode was null");
                else RootNode.Nodes.Add(blueprintCollection.RootNode);
            }



        }

        /// <summary>
        /// Implements receiving of simple child-to-parent messages.
        /// </summary>
        /// <param name="sender">The child object that sent the message.</param>
        /// <param name="type">The type of message.</param>
        /// <param name="msg">Message text (optional).</param>
        void IHENotificationReceiver.ReceiveNotification(IHENotificationSender sender, HENotificationType type, string msg)
        {
            Debug.Print("Message received from {0} of type {1} :: {2}", sender.ToString(), type.ToString(), msg);
        }

    }


    public enum HEDockingPortNames
    {
        Unknown = 0,
        StandardDockingPortA,
        StandardDockingPortB,
        StandardDockingPortC,
        StandardDockingPortD,
        AirlockDockingPort,
        Grapple,
        IndustrialContainerPortA,
        IndustrialContainerPortB,
        IndustrialContainerPortC,
        IndustrialContainerPortD,
        CargoDockingPortA,
        CargoDockingPortB,
        CargoDock  // Dockable Cargo module
    }



    public class HEBlueprint
    {
        public string __ObjectType = null;
        public decimal? Version = null;
        public string Name = null;
        public Uri LinkURI = null;
        public List<HEBlueprintStructure> Structures = null;
        public object Parent = null;

        public HEBlueprint(object passedParent = null)
        {
            Parent = passedParent;
            Structures = new List<HEBlueprintStructure>();
            Debug.Print("New HEBlueprint Created");
        }

        public void ConnectTheDots()
        {
            Debug.Print("Connecting blueprint " + Name);

            foreach (HEBlueprintStructure structure in Structures)
            {
                Debug.Print("Connecting structure " + structure.StructureType);
                structure.Parent = this;
                foreach (HEBlueprintDockingPort port in structure.DockingPorts)
                {
                    Debug.Print("Connecting port " + port.PortName);
                    port.Parent = structure;
                }

            }
        }


        public class HEBlueprintStructure
        {
            public int? StructureID = null;
            public string StructureType = null;
            public List<HEBlueprintDockingPort> DockingPorts = null;
            public HEBlueprint Parent = null;

            public HEBlueprintStructure(HEBlueprint passedParent = null)
            {
                Parent = passedParent;
                DockingPorts = new List<HEBlueprintDockingPort>();
                Debug.Print("New HEBlueprintStructure Created");
            }

        }

        public class HEBlueprintDockingPort
        {
            public string PortName = null;
            public int? OrderID = null;
            public int? DockedStructureID = null;
            public string DockedPortName = null;
            public HEBlueprintStructure Parent = null;

            public HEBlueprintDockingPort(HEBlueprintStructure passedParent = null)
            {
                Parent = passedParent;
                Debug.Print("New HEBlueprintDockingPort Created");
            }
        }



    }

    public class HEbpStructureDefinitions
    {
        public string __ObjectType = null;
        public decimal? Version = null;
        public List<HEbpStructureDefinition> StructureDefinitions = null;

        public HEbpStructureDefinitions()
        {
            StructureDefinitions = new List<HEbpStructureDefinition>();
        }

        public class HEbpStructureDefinition
        {
            public string SanitisedName = null;
            public int? ItemID = null;
            public string SceneName = null;
            public List<HEbpStructureDefinitionDockingPort> DockingPorts = null;

            public HEbpStructureDefinition()
            {
                DockingPorts = new List<HEbpStructureDefinitionDockingPort>();
            }

            public class HEbpStructureDefinitionDockingPort
            {
                public string PortName = null;
                public int? PortID = null;
                public int? OrderID = null;
            }
        }
    }



}
