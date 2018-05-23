using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HELLION.DataStructures;
using HELLION.DataStructures.Utilities;
using Newtonsoft.Json.Linq;
using static HELLION.DataStructures.StaticDataHelper;

namespace HELLION.CrewSync
{
    static class CrewSyncProgram
    {
        
        internal static LogFileHandler Logging = new LogFileHandler();
        
        
        /// <summary>
        /// The Hellion Dedicated Server file we're working on.
        /// </summary>
        internal static HEJsonFile hellionSaveFile = null;

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
                    + "[/logfilepath <full path to log file directory>]"
                    + "[/nobackup] [/verbose]";

                for (int i = 0; i < arguments.Length; i++)
                {
                    // Try to figure out what's in this argument
                    if (arguments[i].EndsWith(".save", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's a .save file
                        hellionSaveFileInfo = new FileInfo(arguments[i]);
                        Console.WriteLine("Argument: Save File " + hellionSaveFileInfo.FullName);

                        if (!hellionSaveFileInfo.Exists)
                        {
                            Console.WriteLine("Specified Save File does not exist.");
                            return false;
                        }

                    }
                    else if (arguments[i].Equals("/prefix", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's the /prefix argument, increment i by one to prevent the next element being
                        // processed in case there are other(?) arguments.
                        i++;
                        groupPrefix = arguments[i].ToUpper();
                        Console.WriteLine("Argument: Vessel Prefix " + groupPrefix);

                        if (String.IsNullOrEmpty(groupPrefix))
                        {
                            Console.WriteLine("Invalid prefix.");
                            return false;
                        }

                    }
                    else if (arguments[i].Equals("/groupid64", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's the /groupid64 argument, increment i by one to prevent the next element being
                        // processed in case there are other(?) arguments.
                        i++;
                        groupID64 = Convert.ToInt64(arguments[i]);
                        Console.WriteLine("Argument: Steam GroupID64 " + groupID64);
                        if (!(groupID64 > 0))
                        {
                            Console.WriteLine("Problem with Steam GroupID64.");
                            return false;
                        }
                    }
                    else if (arguments[i].Equals("/logfilepath", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's the /logfilepath argument, increment i by one to prevent the next element being
                        // processed in case there are other(?) arguments.
                        i++;
                        DirectoryInfo _loggingPath = new DirectoryInfo(arguments[i]);
                        Console.WriteLine("Argument: Log File Path " + _loggingPath.FullName);
                        if (!(_loggingPath.Exists))
                        {
                            Console.WriteLine("Invalid logging path specified.");
                            return false;
                        }
                        Logging.LogFile = new FileInfo(Path.Combine(_loggingPath.FullName, Logging.GenerateLogFileName()));
                        Console.WriteLine("Logging to: " + Logging.LogFile.FullName);

                        Logging.Mode = LogFileHandler.LoggingOperationType.ConsoleAndLogFile;


                    }
                    else if (arguments[i].Equals("/nobackup", StringComparison.CurrentCultureIgnoreCase))
                    {
                        createBackup = false;
                        Console.WriteLine("Argument: No backup file will be created.");
                    }
                    else if (arguments[i].Equals("/verbose", StringComparison.CurrentCultureIgnoreCase))
                    {
                        verboseOutput = true;
                        Console.WriteLine("Argument: Verbose output ON.");
                    }
                    else if (arguments[i].Equals("/?") || arguments[i].ToLower().Contains("help"))
                    {
                        Console.WriteLine(helpText);
                    }
                    else
                    {
                        Console.WriteLine("Unexpected Argument: " + arguments[i]);
                        Console.WriteLine("Use /? or /help to show available arguments.");
                        return false;
                    }
                }

                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Triggers loading of the save file.
        /// </summary>
        /// <returns></returns>
        internal static bool FileOpen()
        {
            hellionSaveFile = new HEJsonFile(null, hellionSaveFileInfo);

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
                    Console.WriteLine(" {0} SteamID64: {1,-17} GUID: {2,-19} PlayerName: {3}",
                        markerChar, player["SteamId"], player["GUID"], player["Name"]);

            }
            crewList = _tmpList;

            Console.WriteLine("Save file Players collection contains {0} member(s).", _playersCollection.Children().Count());
            Console.WriteLine("{0} Player(s) are members of the group and were added to the Master Crew List.", _tmpList.Count);

            // foreach (AuthorizedPerson person in tmpList) Console.WriteLine(JToken.FromObject(person).ToString());


            return true;
        }

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
                Console.WriteLine(" {0} Reg: {1,-25} GUID: {2,-13} Name: {3}", 
                    markerChar, vessel["Registration"], vessel["GUID"], vessel["Name"] );

            }
            vesselList = _tmpList;

            Console.WriteLine("Save file Ships collection contains {0} vessel(s).", _shipsCollection.Children().Count());
            Console.WriteLine("{0} Vessels(s) have name prefixes that match and have been added to the Master Vessel List.", _tmpList.Count);

            // foreach (AuthorizedPerson person in tmpList) Console.WriteLine(JToken.FromObject(person).ToString());


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
                // Check there's an owner - a ship with no owner won't get updated.
                Console.WriteLine("Vessel: {0}  Commanding Officer: {1}", vessel["Name"], GetCommandingOfficerName(vessel));
                if (vessel["AuthorizedPersonel"].Children().Count() > 0)
                {
                    // Convert the authorised personnel JToken to a list
                    List<AuthorisedPerson> _vesselAuthorisedPersonnel = vessel["AuthorizedPersonel"].ToObject<List<AuthorisedPerson>>();
                    Console.WriteLine("_vesselAuthorisedPersonnel count {0}", _vesselAuthorisedPersonnel.Count);

                    // Create a list of all members of the Master Crew List except this vessel's CO and those that are already on ships crew.
                    List<AuthorisedPerson> _PlayersToAdd = crewList
                        .Except(_vesselAuthorisedPersonnel, new AuthorisedPersonSteamIDComparer())
                        .Where(p => p.Rank != AuthorisedPersonRank.CommandingOfficer) //   GetCommandingOfficer(vessel) })
                        .ToList();
                    Console.WriteLine("Players to add {0}", _PlayersToAdd.Count);

                    foreach (var player in _PlayersToAdd)
                        AddAuthorisedPerson(vessel, player);
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
                Console.WriteLine(" + " + person.SteamID + " " + person.Name);
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



        static void Main(string[] args)
        {
            Console.WriteLine(Application.ProductName + " - " + Application.ProductVersion);

#if DEBUG
                Console.WriteLine("Mode=Debug");
#else
                Console.WriteLine("Mode=Release"); 
#endif
            DateTime operationStartTime = DateTime.Now;

            if (!ProcessCommandLineArguments(args))
            {
                Console.WriteLine("Problem processing command line arguments.");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
                return;
            }

            Console.Write("Loading save file...");
            if (!FileOpen())
            {
                Console.WriteLine("Problem loading save file.");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("  Complete.");

            Console.Write("Querying Steam Public API for group membership...");
            string groupName = SteamIntegration.GetGroupName((long)groupID64);
            if (!RetrieveSteamGroupMembership())
            {
                Console.WriteLine("Problem querying Steam.");
                return;
            }
            Console.WriteLine("  Complete.");
            Console.WriteLine("Steam Group {0} ({1}) has {2} member(s).", 
                groupID64, groupName, groupMembers.Count);

            Console.WriteLine("Building in-game player list...");
            // Build in-game character list containing only members of the steam group.
            BuildCrewList();
            Console.WriteLine("Complete.");

            Console.WriteLine("Building in-game vessel list...");
            // Build list of vessels whose names contain the specified prefix.
            BuildVesselList();
            Console.WriteLine("Complete.");

            Console.WriteLine("Processing vessel list...");
            ProcessVesselList();
            Console.WriteLine("Complete.");

            // Save the changes.
            FileSave();


            TimeSpan timeElapsed = DateTime.Now - operationStartTime;

            Console.WriteLine("Operation completed in {0}.{1} second(s).", timeElapsed.Seconds, timeElapsed.Milliseconds);

#if DEBUG
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
#endif


        }

        
    }



}
