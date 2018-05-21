using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HELLION.DataStructures;

using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace HELLION.CrewSync
{
    static class CrewSyncProgram
    {

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
        /// Stores the group membership info.
        /// </summary>
        internal static List<long> groupMembers = null;




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
                    + ".exe <full file name of .save file to open> "
                    + "/prefix <the prefix of the vessel name to apply to> "
                    + "/groupid64 <the GroupID64 of the Steam Group> ";

                for (int i = 0; i < arguments.Length; i++)
                {
                    // Try to figure out what's in this argument
                    if (arguments[i].EndsWith(".save", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's a .save file
                        hellionSaveFileInfo = new FileInfo(arguments[i]);
                        Console.WriteLine("Argument: Save File " + hellionSaveFileInfo.FullName);

                        if (!hellionSaveFileInfo.Exists) return false;

                    }
                    else if (arguments[i].Equals("/prefix", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's the /tag argument, increment i by one to prevent the next element being
                        // processed in case there are other(?) arguments.
                        i++;
                        groupPrefix = arguments[i].ToUpper();
                        Console.WriteLine("Argument: Vessel Prefix " + groupPrefix);

                        if (String.IsNullOrEmpty(groupPrefix)) return false;

                    }
                    else if (arguments[i].Equals("/groupid64", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's the /tag argument, increment i by one to prevent the next element being
                        // processed in case there are other(?) arguments.
                        i++;
                        groupID64 = Convert.ToInt64(arguments[i]);
                        Console.WriteLine("Argument: Steam GroupID64 " + groupID64);
                        // if (groupID64 == )
                    }

                    else if (arguments[i].Equals("/?") || arguments[i].ToLower().Contains("help"))
                    {
                        Console.WriteLine(helpText);
                    }
                    else
                    {
                        Console.WriteLine("Unexpected Argument: " + arguments[i]);
                        Console.WriteLine("Use /? or /help to show available arguments.");
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
        /// Gets the list of members for the Steam Group.
        /// </summary>
        /// <returns></returns>
        internal static bool RetrieveSteamGroupMembership()
        {
            groupMembers = SteamIntegration.GetGroupMembers((long)groupID64);
            return (groupMembers.Count > 0) ? true : false;
        }




        static void Main(string[] args)
        {
            Console.WriteLine(Application.ProductName + " - " + Application.ProductVersion);

#if DEBUG
                Console.WriteLine("Mode=Debug");
#else
                Console.WriteLine("Mode=Release"); 
#endif


            if (!ProcessCommandLineArguments(args)) return;

            Console.Write("Loading save file...");
            if (!FileOpen()) return;
            Console.WriteLine(" Complete.");

            DateTime operationStartTime = DateTime.Now;
            Console.Write("Querying Steam Public API for group membership...");
            string groupName = SteamIntegration.GetGroupName((long)groupID64);
            if (!RetrieveSteamGroupMembership()) return;
            Console.WriteLine(" Complete.");
            Console.WriteLine("Steam Group {0} ({1}) has {2} member(s).", 
                groupID64, groupName, groupMembers.Count);


            TimeSpan timeElapsed = DateTime.Now - operationStartTime;

            Console.WriteLine("Operation completed in {0}.{1} second(s).", timeElapsed.Seconds, timeElapsed.Milliseconds);

#if DEBUG
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
#endif


        }




    }
}
