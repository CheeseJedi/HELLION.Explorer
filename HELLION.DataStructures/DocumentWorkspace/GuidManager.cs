using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace HELLION.DataStructures
{
    /// <summary>
    /// A class for handling in-game GUIDs.
    /// </summary>
    public static class GuidManager
    {
        public static List<long> ObservedGuids { get => observedGuids; }

        private static List<long> observedGuids = new List<long>();
        
        /// <summary>
        /// Adds a GUID to the observed list if it's not already on there.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>Returns true if the GUID was unique and was added.</returns>
        private static bool ObserveGuid(long guid)
        {
            if (observedGuids.Contains(guid)) return false;
            else
            {
                observedGuids.Add(guid);
                return true;
            }
        }

        /// <summary>
        /// Clears the observed GUIDs list.
        /// </summary>
        public static void ClearObservedGuidsList()
        {
            observedGuids = new List<long>();
        }

        public static void PopulateObservedGuidsList(JToken json)
        {
            if (json == null) throw new NullReferenceException("json was null.");
            else
            {
                List<JToken> interimResults = json.SelectTokens("..GUID").Distinct(JToken.EqualityComparer).ToList();

                if (interimResults.Count() == 0) throw new Exception("count was zero.");
                foreach (JToken token in interimResults)
                {
                    ObserveGuid((long)token);
                }

            }

        }




        private static Random rnd = new Random();

        /// <summary>
        /// Returns a new unique GUID 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static long NextGuid(long min, long max)
        {
            // Define a byte array
            byte[] buffer = new byte[8];
            // Fill the array with random data
            rnd.NextBytes(buffer);
            // Convert the array to an integer and modulo it with the difference between the 
            // maximum and the minimum to clamp it within this range, then add the minimum
            // to bring it back within the range of the maximum and minimum.
            long result = Math.Abs(BitConverter.ToInt64(buffer, 0) % (max - min)) + min;
            // If the new GUID is in the observed GUIDs list try again
            while (observedGuids.Contains(result))
            {
                // recursive call, hopefully this won't happen too often.
                return NextGuid(min, max);
            }
            return result;
        }

    }
}
