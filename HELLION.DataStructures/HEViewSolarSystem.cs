using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HELLION.DataStructures
{
    class HEViewSolarSystem
    {
        // Implements the Game Data view node tree
        public HETreeNode rootNode;

        public HEViewSolarSystem()
        {
            // Basic constructor

            rootNode = new HETreeNode("SOLARSYSTEMVIEW", HETreeNodeType.DataView, "Solar System");

        }

    }
}
