﻿using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Defines a derived HETreeNode to handle objects in the Solar System view.
/// Also defines a node sorter that sorts by Semi-Major axis instead of by name which is
/// the default on a TreeView control, and a class to hold the orbital data.
/// </summary>
namespace HELLION.DataStructures.UI
{
    /// <summary>
    /// Create a node sorter that implements the IComparer interface to sort HEOrbitalObjTreeNodes
    /// by Semi-Major axis.
    /// </summary>
    public class SolarSystem_TN_Sorter : IComparer
    {
        /// <summary>
        /// Compare the values of SemiMajorAxis.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            SolarSystem_TN nodeX = x as SolarSystem_TN;
            SolarSystem_TN nodeY = y as SolarSystem_TN;

            if (nodeX != null && nodeY != null)
            {
                // Compare the values of SemiMajorAxis, returning the result.
                return Comparer<double>.Default.Compare(nodeX.SemiMajorAxis, nodeY.SemiMajorAxis);
            }
            else
            {
                // One of the two nodes was null, cannot compare so return 0 indicating equivalency
                return 0;
            }
        }
    }
}
