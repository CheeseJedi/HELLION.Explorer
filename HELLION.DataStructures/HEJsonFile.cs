using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Diagnostics;

namespace HELLION.DataStructures
{
    // Define a custom class to hold data from a JSON file and associated metadata

    public class HEJsonFile
    {
        // Base class for a generic JSON data file

        public string FileName { get; set; }
        public JArray JData { get; set; } // Data files use JArrays (master save is loaded in to a JObject instead)

        // Externally read-only properties
        public bool IsFileLoaded { get; set; }
        public bool LoadError { get; set; }
        public bool SkipLoading { get; set; }

        public bool LogToDebug { get; set; }


        public HEJsonFile()
        {
            // Basic Constructor
            FileName = "";
            JData = null;
            IsFileLoaded = false;
            LoadError = false;
            SkipLoading = false;
            LogToDebug = false;
        }

        public HEJsonFile(string sFileName)
        {
            // Constructor that allows the file name to be set
            FileName = sFileName;
            JData = null;
            IsFileLoaded = false;
            LoadError = false;
            SkipLoading = false;
            LogToDebug = false;
        }

        public bool LoadFile()
        {
            // Load file data from FileName and parse to the JData JObject of type IOrderedEnumerable<JToken>
            // Returns true if there was a loading error
            if (!SkipLoading)
            {
                if (System.IO.File.Exists(FileName))
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(FileName))
                        {
                            // Process the stream with the JSON Text Reader in to a JArray; was previously an IOrderedEnumerable<JToken> JObject
                            using (JsonTextReader jtr = new JsonTextReader(sr))
                            {
                                JData = (JArray)JToken.ReadFrom(jtr);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        // Some error handling to be implemented here
                        LoadError = true;
                        if (LogToDebug) Debug.Print("Exception caught during StreamReader or JsonTextReader while processing " + FileName
                            + Environment.NewLine + e.ToString());
                    }

                    if (JData == null || JData.Count == 0)
                    {
                        // The data didn't load
                        LoadError = true;
                        if (LogToDebug) Debug.Print("JData is null or empty: " + FileName);
                    }
                    else
                    {
                        // We should have some data in the array
                        IsFileLoaded = true;
                        if (LogToDebug) Debug.Print("Processed " + JData.Count + " objects from: " + FileName);
                    }
                }
                else
                {
                    // Invalid file name
                    LoadError = true;
                    if (LogToDebug) Debug.Print("Invalid file name passed: " + FileName);
                }
            }
            else
            {
                if (LogToDebug) Debug.Print("Skipping file: " + FileName);
            } // End of SkipLoading check

            // Return the value of LoadError
            return LoadError;
        } // End of LoadFile()

    } // End of class HEjsonFile
} // End of namespace HELLION.DataStructures
