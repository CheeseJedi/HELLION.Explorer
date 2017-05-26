/* HETreeNodeSorter.cs
 * CheeseJedi 2017
 * Defines a custom comparer class to sort HEOrbitalObjTreeNodes
 */

using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms; // required for the base TreeNode class

namespace HELLION.DataStructures
{
    // Create a node sorter that implements the IComparer interface to sort HEOrbitalObjTreeNodes
    public class HETreeNodeSorter : IComparer
    {
        // Compare the values of SemiMajorAxis, or the vaule of Inclination if they are the same
        public int Compare(object x, object y)
        {
            HEOrbitalObjTreeNode nodeX = x as HEOrbitalObjTreeNode;
            HEOrbitalObjTreeNode nodeY = y as HEOrbitalObjTreeNode;

            // int iResult = Comparer<double>.Default.Compare(nodeX.SemiMajorAxis, nodeY.SemiMajorAxis);

            if (nodeX != null && nodeY != null)
            {
                // Compare the values of SemiMajorAxis, returning the result.
                return Comparer<double>.Default.Compare(nodeX.SemiMajorAxis, nodeY.SemiMajorAxis);
            }
            else
            {
                // One of the two nodes was null, cannot compare so resturn 0 indicating equivalency
                return 0;
            }
        }
    } // End of HETreeNodeSorter
} // End of namespace HELLION
