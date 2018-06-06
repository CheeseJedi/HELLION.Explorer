using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HELLION.DataStructures;
using HELLION.DataStructures.StaticData;
using HELLION.DataStructures.Utilities;
using Newtonsoft.Json.Linq;

namespace HELLION.CrewSync
{
    static class CrewSyncProgram
    {
        /// <summary>
        /// Logging handler object.
        /// </summary>
        internal static LogFileHandler Logging = new LogFileHandler();

        /// <summary>
        /// The suffix appended to the current time date to create a unique log file name.
        /// </summary>
        internal const string LogFileNameSuffix = "_HELLION.CrewSync";

        /// <summary>
        /// The Hellion Dedicated Server file we're working on.
        /// </summary>
        internal static Json_File hellionSaveFile = null;

        /// <summary>
        /// The FileInfo object for the 
        /// </summary>
        internal static FileInfo hellionSaveFileInfo = null;

        /// <summary>
        /// The group prefix we'll be looking for.
        /// </summary>
        internal static string groupPrefix = null;

        /// <summary>
        /// The Steam GroupID64 we're getting membership from.
        /// </summary>
        internal static long? groupID64 = null;

        /// <summary>
        /// Whether the app will create a backup of the save file.
        /// </summary>
        internal static bool createBackup = true;

        /// <summary>
        /// Whether the app output is verbose.
        /// </summary>
        internal static bool verboseOutput = false;

        /// <summary>
        /// Stores the group membership info.
        /// </summary>
        internal static List<long> groupMembers = null;

        /// <summary>
        /// The master crew list - list of in-game characters that are also members of the Steam group.
        /// </summary>
        internal static List<AuthorisedPerson> crewList = null;

        /// <summary>
        /// The master vessel list.
        /// </summary>
        internal static List<JToken> vesselList = null;

        /// <summary>
        /// Processes any command line arguments issued to the program.
        /// </summary>
        /// <param name="arguments"></param>
        internal static bool ProcessCommandLineArguments(string[] arguments)
        {
            if (arguments.Length > 0)
            {
                // There are arguments 

                string helpText = Application.ProductName 
                    + ".exe <full path to .save file to process> "
                    + "/prefix <the name prefix of vessel to apply to> "
                    + "/groupid64 <the GroupID64 of the Steam Group> "
                    + "[/logfolder <path to log file directory>] "
                    + "[/nobackup] [/verbose] ";

                for (int i = 0; i < arguments.Length; i++)
                {
                    // Try to figure out what's in this argument
                    if (arguments[i].EndsWith(".save", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's a .save file
                        hellionSaveFileInfo = new FileInfo(arguments[i]);
                        Logging.WriteLine("Argument: Save File " + hellionSaveFileInfo.FullName);

                        if (!hellionSaveFileInfo.Exists)
                        {
                            Logging.WriteLine("Specified Save File does not exist.");
                            PauseIfDebuggerAttached();
                            return false;
                        }
                    }
                    else if (arguments[i].Equals("/prefix", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Increment i by one to prevent the next element being processed in case there are other(?) arguments.
                        i++;
                        groupPrefix = arguments[i].ToUpper();
                        Logging.WriteLine("Argument: Vessel Prefix " + groupPrefix);

                        if (String.IsNullOrEmpty(groupPrefix))
                        {
                            Logging.WriteLine("Invalid prefix.");
                            PauseIfDebuggerAttached();
                            return false;
                        }

                    }
                    else if (arguments[i].Equals("/groupid64", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Increment i by one to prevent the next element being processed in case there are other(?) arguments.
                        i++;
                        groupID64 = Convert.ToInt64(arguments[i]);
                        Logging.WriteLine("Argument: Steam GroupID64 " + groupID64);
                        if (!(groupID64 > 0))
                        {
                            Logging.WriteLine("Problem with Steam GroupID64.");
                            PauseIfDebuggerAttached();
                            return false;
                        }
                    }
                    else if (arguments[i].Equals("/logfolder", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // Increment i by one to prevent the next element being processed in case there are other(?) arguments.
                        i++;
                        DirectoryInfo _loggingPath = new DirectoryInfo(arguments[i]);
                        Logging.WriteLine("Argument: Log File Path " + _loggingPath.FullName);
                        if (!(_loggingPath.Exists))
                        {
                            Logging.WriteLine("Invalid logging path specified.");
                            PauseIfDebuggerAttached();
                            return false;
                        }
                        Logging.LogFile = new FileInfo(Path.Combine(_loggingPath.FullName, Logging.GenerateLogFileName(LogFileNameSuffix)));
                        Logging.WriteLine("Logging to: " + Logging.LogFile.FullName);

                        Logging.Mode = LogFileHandler.LoggingOperationType.ConsoleAndLogFile;

                    }
                    else if (arguments[i].Equals("/nobackup", StringComparison.CurrentCultureIgnoreCase))
                    {
                        createBackup = false;
                        Logging.WriteLine("Argument: Backup file will NOT be created.");
                    }
                    else if (arguments[i].Equals("/verbose", StringComparison.CurrentCultureIgnoreCase))
                    {
                        verboseOutput = true;
                        Logging.WriteLine("Argument: Verbose output ON.");
                    }
                    else if (arguments[i].Equals("/?") || arguments[i].ToLower().Contains("help"))
                    {
                        Logging.WriteLine(helpText);
                        PauseIfDebuggerAttached();
                        return false;
                    }
                    else
                    {
                        Logging.WriteLine("Unexpected Argument: " + arguments[i]);
                        Logging.WriteLine("Use /? or /help to show available arguments.");
                        PauseIfDebuggerAttached();
                        return false;
                    }
                }
                // We got here so everything checked out so far.
                return true;
            }
            Logging.WriteLine("No parameters specified.");
            Logging.WriteLine("Use /? or /help to show available arguments.");
            PauseIfDebuggerAttached();
            return false;
        }
        
        /// <summary>
        /// Triggers loading of the save file.
        /// </summary>
        /// <returns></returns>
        internal static bool FileOpen()
        {
            hellionSaveFile = new Json_File(null, hellionSaveFileInfo);

            if (hellionSaveFile.LoadError) return false;
            return true;
        }

        /// <summary>
        /// Triggers writing of the save file.
        /// </summary>
        internal static void FileSave()
        {
            hellionSaveFile.SaveFile(createBackup);
        }

        /// <summary>
        /// Gets the list of members for the Steam Group.
        /// </summary>
        /// <returns></returns>
        internal static bool RetrieveSteamGroupMembership()
        {
            groupMembers = SteamIntegration.GetGroupMembers((long)groupID64);
            return (groupMembers.Count > 0) ? true : false;
        }

        /// <summary>
        /// Builds the master crew list.
        /// </summary>
        /// <returns></returns>
        internal static bool BuildCrewList()
        {
            List<AuthorisedPerson> _tmpList = new List<AuthorisedPerson>();

            JToken _playersCollection = hellionSaveFile.JData["Players"];

            foreach (JToken player in _playersCollection)
            {
                string markerChar = " ";
                if (groupMembers.Contains((long)player["SteamId"]))
                {
                    markerChar = "@";
                    AuthorisedPerson newPerson = new AuthorisedPerson
                    {
                        SteamID = (long)player["SteamId"],
                        Name = (string)player["Name"],
                        PlayerGUID = (long)player["GUID"],
                        Rank = AuthorisedPersonRank.Crewman
                    };
                    _tmpList.Add(newPerson);
                }
                if (verboseOutput)
                    Logging.WriteLine(String.Format(" {0} SteamID64: {1,-17} GUID: {2,-19} PlayerName: {3}",
                        markerChar, player["SteamId"], player["GUID"], player["Name"]));
            }
            crewList = _tmpList;

            Logging.WriteLine(String.Format("Save file Players collection contains {0} member(s).", _playersCollection.Children().Count()));
            Logging.WriteLine(String.Format("{0} Player(s) are members of the group and were added to the Master Crew List.", _tmpList.Count));

            return true;
        }

        /// <summary>
        /// Builds the filtered master vessel list.
        /// </summary>
        /// <returns></returns>
        internal static bool BuildVesselList()
        {
            List<JToken> _tmpList = new List<JToken>();

            JToken _shipsCollection = hellionSaveFile.JData["Ships"];

            foreach (JToken vessel in _shipsCollection)
            {
                string markerChar = " ";
                if (vessel["Name"].ToString().ToUpper().StartsWith(groupPrefix.ToUpper() +" "))
                {
                    markerChar = "@";
                    _tmpList.Add(vessel);
                }
                if (verboseOutput)
                    Logging.WriteLine(String.Format(" {0} Reg: {1,-25} GUID: {2,-13} Name: {3}", 
                        markerChar, vessel["Registration"], vessel["GUID"], vessel["Name"]));
            }
            vesselList = _tmpList;

            Logging.WriteLine(String.Format("Save file Ships collection contains {0} vessel(s).", _shipsCollection.Children().Count()));
            Logging.WriteLine(String.Format("{0} Vessels(s) have name prefixes that match and have been added to the Master Vessel List.", _tmpList.Count));

            // foreach (AuthorizedPerson person in tmpList) Logging.WriteLine(JToken.FromObject(person).ToString());


            return true;
        }

        /// <summary>
        /// Processes the (already filtered) vessel list.
        /// </summary>
        /// <returns></returns>
        internal static bool ProcessVesselList()
        {
            foreach (JToken vessel in vesselList)
            {
                // Check there's an owner - a ship with no owner or crew won't get updated.
                if (vessel["AuthorizedPersonel"].Children().Count() > 0)
                {
                    // Convert the authorised personnel JToken to a list
                    List<AuthorisedPerson> _vesselAuthorisedPersonnel = vessel["AuthorizedPersonel"]
                        .ToObject<List<AuthorisedPerson>>();

                    Logging.WriteLine(String.Format("Vessel: {0}  Commanding Officer: {1}  Existing Crew: {2}",
                        vessel["Name"], GetCommandingOfficerName(vessel), _vesselAuthorisedPersonnel.Count));

                    // Create a list of all members of the Master Crew List except 
                    // this vessel's CO and those that are already on ships crew.
                    List<AuthorisedPerson> _PlayersToAdd = crewList
                        .Except(_vesselAuthorisedPersonnel, new AuthorisedPersonSteamIDComparer())
                        .Where(p => p.Rank != AuthorisedPersonRank.CommandingOfficer)
                        .ToList();

                    Logging.WriteLine(String.Format("Adding {0} Players to vessel crew.", _PlayersToAdd.Count));

                    foreach (var player in _PlayersToAdd)
                    {
                        if (verboseOutput) Logging.WriteLine(" + " + player.SteamID + " " + player.Name);
                        AddAuthorisedPerson(vessel, player);
                    }

                    Logging.WriteLine("Done.");
                }
            }
            return true;

            /// <summary>
            /// Adds an authorised person to the vessel's crew list.
            /// </summary>
            void AddAuthorisedPerson (JToken vessel, AuthorisedPerson person)
            {
                JToken _serialisedPerson = JToken.FromObject(person);
                vessel["AuthorizedPersonel"].Last.AddAfterSelf(_serialisedPerson);
            }

            /// <summary>
            /// Gets the Commanding Officer of the specified vessel.
            /// </summary>
            /// <returns>The SteamID64 of the CO, or null if no CO defined.</returns>
            string GetCommandingOfficerName(JToken vessel)
            {
                IEnumerable<JToken> results = vessel["AuthorizedPersonel"].Children().Where(p => (int)p["Rank"] == 1);
                return (results.Count() > 0) ? (string)results.First()["Name"] : null;
            }

        }

        /// <summary>
        /// Pauses if the program is running with an attached debugger.
        /// </summary>
        internal static void PauseIfDebuggerAttached()
        {
            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Make a record of the starting time.
            DateTime operationStartTime = DateTime.Now;
            Logging.WriteLine(Application.ProductName + " - " + Application.ProductVersion);
#if DEBUG
            Logging.WriteLine("Mode=Debug");
#else
            Logging.WriteLine("Mode=Release"); 
#endif
            Logging.WriteLine("Part of HELLION.Explorer - https://github.com/CheeseJedi/HELLION.Explorer");

            // Process the command line arguments. If there's an issue (returned false) then exit.
            if (!ProcessCommandLineArguments(args)) return;

            Logging.WriteLine("Loading save file...");
            if (!FileOpen())
            {
                Logging.WriteLine("Problem loading save file.");
                PauseIfDebuggerAttached();
                return;
            }
            Logging.WriteLine("Complete.");

            Logging.WriteLine("Querying Steam Public API for group membership...");
            string groupName = SteamIntegration.GetGroupName((long)groupID64);
            if (!RetrieveSteamGroupMembership())
            {
                Logging.WriteLine("Problem querying Steam.");
                PauseIfDebuggerAttached();
                return;
            }
            Logging.WriteLine("Complete.");
            Logging.WriteLine(String.Format("Steam Group {0} ({1}) has {2} member(s).", 
                groupID64, groupName, groupMembers.Count));

            Logging.WriteLine("Building in-game player list...");
            // Build in-game character list containing only members of the steam group.
            BuildCrewList();
            Logging.WriteLine("Complete.");

            Logging.WriteLine("Building in-game vessel list...");
            // Build list of vessels whose names contain the specified prefix.
            BuildVesselList();
            Logging.WriteLine("Complete.");

            Logging.WriteLine("Processing vessel list...");
            ProcessVesselList();
            Logging.WriteLine("Complete.");

            // Save the changes.
            FileSave();

            TimeSpan timeElapsed = DateTime.Now - operationStartTime;

            Logging.WriteLine(String.Format("Operation completed in {0}.{1} second(s).",
                timeElapsed.Seconds, timeElapsed.Milliseconds));

            Logging.FlushBuffer();



#if DEBUG
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
#endif


        }
        
    }
}
