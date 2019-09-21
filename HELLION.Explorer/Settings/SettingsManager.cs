using HELLION.DataStructures;
using Newtonsoft.Json;
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
        // Set through constructor.
        public readonly string coName;
        public readonly string appName;

        public const string GameDataFolder_Setting = "GameDataFolder";

        /// <summary>
        /// The default settings dictionary. Used to populate the main settings dictionary.
        /// </summary>
        public static readonly Dictionary<string, string> DefaultSettings = new Dictionary<string, string>
        {
            {
                GameDataFolder_Setting, null
            }
        };

        /// <summary>
        /// The main settings dictionary both key and value are strings.
        /// </summary>
        public Dictionary<string, string> Settings { get; private set; } = DefaultSettings;

        public string SettingsDirectoryPath
        {
            get => settingsDirectoryPath;
            set
            {
                if (settingsDirectoryPath != value)
                {
                    settingsDirectoryPath = value;
                    UpdateSettingsDirectoryInfo();
                    if (settingsFileName != null)
                    {
                        UpdateSettingsFileInfo();
                    }
                }
            }
        }
        private string settingsDirectoryPath;
        private DirectoryInfo settingsDirectoryInfo;

        public string SettingsFileName
        {
            get => settingsFileName;
            set
            {
                if (settingsFileName != value)
                {
                    settingsFileName = value;
                    if (settingsFileName != null)
                    {
                        UpdateSettingsFileInfo();
                    }
                }
            }
        }

        private string settingsFileName;
        private FileInfo settingsFileInfo;

        private Json_File settings_Json_File;

        private readonly bool loadOnInit = true;

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
            if (settingsDirectoryPath == null || string.IsNullOrEmpty(settingsFileName))
            {
                settingsFileInfo = null;
                return;
            }

            settingsFileInfo = new FileInfo(Path.Combine(settingsDirectoryPath, settingsFileName));
            if (!settingsFileInfo.Exists)
            {
                Debug.WriteLine($"Settings file at {settingsFileInfo.FullName} does not exist, creating new with default values.");
                CreateNewSettingsFile(settingsFileInfo);
            }

        }

        private void CreateNewSettingsFile(FileInfo fileInfo, JToken jdata = null)
        {
            Json_File newFile = new Json_File(this)
            {
                AutoLoadOnFileInfoSet = false,
                File = fileInfo,
                AutoDeserialiseOnJdataModification = false,
                JData = jdata ?? JToken.FromObject(DefaultSettings),
            };
            newFile.SaveFile(createBackup: false);
            newFile.Close();
        }

        public string GetSetting(string name)
        {
            if (Settings.TryGetValue(name, out string value))
            {
                return value;
            }
            return null;
        }

        public void SetSetting(string name, string value)
        {
            Debug.WriteLine($"Settings_Json_File.SetSetting: Called - {name}, {value}");
            Settings[name] = value;
        }

        public void Load()
        {
            if (settingsFileInfo.Exists)
            {
                settings_Json_File = new Json_File(this)
                {
                    AutoLoadOnFileInfoSet = false,
                    AutoDeserialiseOnJdataModification = false,
                    File = settingsFileInfo,
                };
                settings_Json_File.LoadFile();

                if (settings_Json_File.LoadError)
                {
                    Debug.WriteLine("SettingsManager.Load: Json_File load error.");
                }
                // Do the de-serialisation here.
                if (settings_Json_File.JData == null)
                {
                    Debug.WriteLine("SettingsManager.Load: load error - JData was null.");
                    return;
                }

                JsonConvert.PopulateObject(settings_Json_File.JData.ToString(), Settings); // must be a better way than this.

                //Settings = settings_Json_File.JData.ToObject<Dictionary<string, string>>();
                if (Settings == null)
                {
                    Debug.WriteLine("SettingsManager.Init: load error - failed to de-serialise.");
                }
                return;
            }
            Debug.WriteLine("SettingsManager.Load: load error - settingsFileInfo does not exist.");
        }

        public void Save()
        {
            if (!settingsFileInfo.Exists)
            {
                // The settings file has gone MIA!
                // create a new one with the current in-memory settings dictionary.
                CreateNewSettingsFile(settingsFileInfo, JToken.FromObject(Settings));
                return;
            }

            // Do the serialisation here.
            settings_Json_File.JData = JToken.FromObject(Settings);

            settings_Json_File.SaveFile(createBackup: false);
        }

    }
}
