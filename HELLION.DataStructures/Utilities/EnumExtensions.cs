using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace HELLION.DataStructures.Utilities
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets an Enum description from its value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Returns the description, or the element name if there isn't a description.</returns>
        /// <remarks>
        /// From: http://www.luispedrofonseca.com/unity-quick-tips-enum-description-extension-method/
        /// </remarks>
        public static string GetEnumDescription(this Enum value)
        {
            if (value == null) return null;
            DescriptionAttribute[] da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString()))
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return da.Length > 0 ? da[0].Description : value.ToString();
        }

        /// <summary>
        /// Attempts to parse a value to either an Enum's description or if that fails a regular parse.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        /// <remarks>
        /// Adapted From: https://stackoverflow.com/questions/4249632/string-to-enum-with-description
        /// </remarks>
        public static T ParseToEnumDescriptionOrEnumerator<T>(this string description)
        {
            MemberInfo[] fields = typeof(T).GetFields();

            foreach (var field in fields)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])field.
                    GetCustomAttributes(typeof(DescriptionAttribute), false);

                // Attempt to parse to the enumerator's description.
                if (attributes != null && attributes.Length > 0 && attributes[0].Description == description)
                    return (T)Enum.Parse(typeof(T), field.Name);
            }

            try
            {
                // Not found, attempt regular parse.
                return (T)Enum.Parse(typeof(T), description);
            }
            catch (NotSupportedException)
            {
                // Unable to parse, return the default for the enumeration.
                return default;
            }
           
        }

        /// <summary>
        /// Returns the values of an enum of given type T
        /// usage: var values = EnumUtil.GetValues<Foos>();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks>
        /// Seems like the source for this one https://stackoverflow.com/questions/972307/can-you-loop-through-all-enum-values
        /// </remarks>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

    }
}
