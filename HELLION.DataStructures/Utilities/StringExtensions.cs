using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HELLION.DataStructures.Utilities
{
    public static class StringExtensions
    {
        /// <summary>
        /// Implements a Contains function that allows a StringComparison type to be specified.
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <param name="compType"></param>
        /// <returns></returns>
        public static bool Contains(this string str1, string str2, StringComparison compType)
        {
            return str1?.IndexOf(str2, compType) >= 0;
        }

        /// <summary>
        /// Attempts to parse a string to a JToken.
        /// </summary>
        /// <remarks>
        /// Uses portions of https://stackoverflow.com/questions/14977848/how-to-make-sure-that-string-is-valid-json-using-json-net
        /// </remarks>
        /// <param name="str">The string to be parsed.</param>
        /// <param name="mustBeObjectOrArray">Set to true to enable early object/array
        /// checks, false to allow parsing strings etc.</param>
        /// <returns>
        /// Returns true if the parse was successful. If successful the token created 
        /// during parsing is available via the out parameter.
        /// </returns>
        public static bool TryParseJson(this string str, out JToken jtoken, out JsonReaderException jrex, bool mustBeObjectOrArray = false)
        {
            // Set the JToken and JsonReaderException to null in case of returning due to an exception.
            jtoken = null;
            jrex = null;

            // Shortcut check for null or empty strings.
            if (string.IsNullOrEmpty(str)) return false;

            // Remove any leading or trailing white-space characters.
            string strToProcess = str.Trim();

            if (mustBeObjectOrArray)
            {
                // Check to see if the string starts and ends with valid characters for an object or array.
                if (!(
                    (strToProcess.StartsWith("{") && strToProcess.EndsWith("}")) || // Delimiters for Json object.
                    (strToProcess.StartsWith("[") && strToProcess.EndsWith("]"))    // Delimiters for Json array.
                    ))
                {
                    return false;
                }
                // The string starts and ends with either curly or square braces - it meets the requirement.                
            }

            // Attempt to parse the string.
            try
            {
                jtoken = JToken.Parse(strToProcess);
                // If we got here without an exception there's a pretty good chance the json was valid.
                return true;
            }
            catch (JsonReaderException jex)
            {
                // Exception caught during parsing.
                jrex = jex;
                Debug.Print("JsonReaderException: {0}", jex);
                return false;
            }
            catch (Exception ex)
            {
                // Other exception caught.
                Debug.Print("Exception: {0}", ex);
                return false;
            }
        }

    }
}
