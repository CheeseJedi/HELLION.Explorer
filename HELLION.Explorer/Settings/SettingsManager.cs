using HELLION.DataStructures;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace HELLION.Explorer.Settings
{
    /// <summary>
    /// A class to handle loading and saving of settings from a json file.
    /// </summary>
    public class SettingsManager : IParent_Json_File
    {
        public readonly string coName;
        public readonly string appName;

        public string SettingsDirectoryPath
        {
            get => settingsDirectoryPath;
            set
            {
                if (settingsDirectoryPath != value)
                {
                    settingsDirectoryPath = value;

                }
            }
        }
        public string SettingsFileName
        {
            get => settingsFileName;
            set
            {
                if (settingsFileName != value)
                {
                    settingsFileName = value;

                }
            }
        }

        private string settingsDirectoryPath;
        private DirectoryInfo settingsDirectoryInfo;
        private string settingsFileName;
        private FileInfo settingsFileInfo;
        string settingsFilePath;
        private Settings_Json_File settings_Json_File;
        private bool loadOnInit = true;

        public SettingsManager(string coName, string appName)
        {
            this.coName = coName;
            this.appName = appName;
        }

        public void Init()
        {
            UpdateSettingsDirectoryInfo();
            UpdateSettingsFileInfo();
            if (loadOnInit)
            {
                Load();
            }
        }

        private void UpdateSettingsDirectoryInfo()
        {
            if (string.IsNullOrEmpty(settingsDirectoryPath))
            {
                // build path to the users settings file location.
                settingsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                   + @"\" + coName + @"\" + appName;
            }
            settingsDirectoryInfo = new DirectoryInfo(settingsDirectoryPath);
            if (!settingsDirectoryInfo.Exists) settingsDirectoryInfo.Create();
        }

        private void UpdateSettingsFileInfo()
        {
            settingsFilePath = Path.Combine(settingsDirectoryPath, settingsFileName);
            settingsFileInfo = new FileInfo(settingsFilePath);
            if (!settingsFileInfo.Exists)
            {
                Debug.WriteLine($"Settings file at {settingsFilePath} does not exist.");
                CreateDefaultSettingsFile(settingsFileInfo);
            }

        }

        private void CreateDefaultSettingsFile(FileInfo fileInfo)
        {
            JToken jdata = JToken.FromObject(defaultSettings);
            Settings_Json_File newFile = new Settings_Json_File(this, jdata, autoDeserialise: false);
            newFile.File = fileInfo;
            newFile.SaveFile();
            newFile.Close();
            newFile = null;
        }

        public string GetSetting(string name)
        {
            return settings_Json_File.GetSetting(name);
        }

        public void SetSetting(string name, string value)
        {
            settings_Json_File.SetSetting(name, value);
        }

        public void Load()
        {
            if (settingsFileInfo.Exists)
            {
                settings_Json_File = new Settings_Json_File(this)
                {
                    AutoLoadOnFileInfoSet = false,
                    AutoDeserialiseOnJdataModification = true,
                    File = settingsFileInfo,
                };
                settings_Json_File.LoadFile();
                Debug.WriteLine("SettingsManager.Init: Json_File load error = " + settings_Json_File.LoadError);
                return;
            }
            Debug.WriteLine("SettingsManager.Init: load error - settingsFileInfo does not exist.");
        }

        public void Save()
        {
            settings_Json_File.SaveFile();
        }

        private Dictionary<string, string> defaultSettings = new Dictionary<string, string>
        {
            {
                "GameDataFolder", null
            }
        };
    }
}
