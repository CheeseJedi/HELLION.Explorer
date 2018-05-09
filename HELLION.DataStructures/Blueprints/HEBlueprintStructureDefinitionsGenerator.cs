using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HELLION.DataStructures
{
    public class HEBlueprintStructureDefinitionsGenerator
    {

        //HEBlueprintStructureDefinitions


        public HEBlueprintStructureDefinitionsFile GenerateNewStructureDefinitionsFile(HEBaseJsonFile structuresJsonFile, string outputFileName)
        {
            // Check the reference to the Static Data's Structures.json file.
            if (structuresJsonFile == null) throw new NullReferenceException("structuresJsonFile was null.");

            HEBlueprintStructureDefinitionsFile newSDFile = new HEBlueprintStructureDefinitionsFile(this);








            return newSDFile;

        }














    }
}
