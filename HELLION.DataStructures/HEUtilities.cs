using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HELLION.DataStructures
{
    public static class HEUtilities
    {
        // A class to hold utility functions


        public static int GetImageIndexByOrbitalObjectType(HETreeNodeType NodeType)
        {
            // Returns the defined image list index for the node type 

            switch (NodeType)
            {
                case HETreeNodeType.CelestialBody:
                    return (int)HEObjectTypesImageList.Contrast_16x;
                    //break;
                case HETreeNodeType.Asteroid:
                    return (int)HEObjectTypesImageList.CheckDot_16x;
                    //break;
                case HETreeNodeType.Ship:
                    return (int)HEObjectTypesImageList.AzureLogicApp_16x;
                    //break;
                case HETreeNodeType.Player:
                    return (int)HEObjectTypesImageList.Actor_16x;
                    //break;
                case HETreeNodeType.DynamicObject:
                    return (int)HEObjectTypesImageList.Property_16x;
                    //break;
                case HETreeNodeType.Scene:
                    return (int)HEObjectTypesImageList.a3DScene_16x;
                    //break;

                default:
                    return (int)HEObjectTypesImageList.Checkerboard_16x;
                    //break;
            }
        }







    }
}
