/* HEOrbitalObjTreeNodeComparer.cs
 * CheeseJedi 2017
 * Defines a custom comparer class to sort HEOrbitalObjTreeNodes
 */

using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms; // required for the base TreeNode class

namespace HELLION.DataStructures
{
    // Create a node sorter that implements the IComparer interface to sort HEOrbitalObjTreeNodes
    public class HEOrbitalObjTreeNodeComparer : IComparer
    {
        // Compare the values of SemiMajorAxis, or the vaule of Inclination if they are the same
        public int Compare(object x, object y)
        {
            HEOrbitalObjTreeNode nodeX = x as HEOrbitalObjTreeNode;
            HEOrbitalObjTreeNode nodeY = y as HEOrbitalObjTreeNode;

            // Compare the values of SemiMajorAxis, returning the difference.
            int iResult = Comparer<float>.Default.Compare(nodeX.SemiMajorAxis, nodeY.SemiMajorAxis);

            if (iResult == 0)
            {
                // They were the same value, call Compare on the Inclination
                return Comparer<float>.Default.Compare(nodeX.Inclination, nodeY.Inclination);
            }
            else
            {
                return iResult;
            }
        }
    } // End of HEOrbitalObjTreeNodeComparer
} // End of namespace HELLION
