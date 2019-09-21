using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HELLION.DataStructures;
using Newtonsoft.Json.Linq;

namespace HELLION.Explorer.Settings
{
    /// <summary>
    /// Defines a class to load and hold data from a JSON .save file and associated metadata.
    /// </summary>
    /// <remarks>
    /// Derived from the Base class for a generic JSON data file.
    /// </remarks>
    public class Settings_Json_File : Json_File
    {
        public Settings_Json_File(IParent_Json_File passedParentObject)
            : base (passedParentObject)
        {

        }
        
        /// <summary>
        /// Constructor that takes a FileInfo and, if the file exists, triggers the load.
        /// </summary>
        /// <param name="passedFileInfo">The FileInfo representing the file to be loaded.</param>
        public Settings_Json_File(IParent_Json_File passedParentObject, FileInfo passedFileInfo, bool autoDeserialise)
            : base(passedParentObject, passedFileInfo, autoDeserialise)
        {

        }

        public Settings_Json_File(IParent_Json_File passedParentObject, JToken jdata, bool autoDeserialise = false)
            : base(passedParentObject, jdata, autoDeserialise)
        {

        }


        private Dictionary<string, string> settings = new Dictionary<string, string>();

        public string GetSetting(string name)
        {
            if (settings.TryGetValue(name, out string value))
            {
                return value;
            }
            return null;
        }

        public void SetSetting(string name, string value)
        {
            Debug.WriteLine($"Settings_Json_File.SetSetting: Called - {name}, {value}");
            settings[name] = value;
            Serialise();
            IsDirty = true;
        }

        public override void Deserialise()
        {
            if (JData == null) Debug.Print("Settings_Json_File.Deserialise: JData was null");
            settings = JData.ToObject<Dictionary<string, string>>();
        }

        public override void Serialise()
        {
            JData = JToken.FromObject(settings);
        }

    }
}
