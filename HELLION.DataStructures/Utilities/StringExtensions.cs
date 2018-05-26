using System;

namespace HELLION.DataStructures
{
    public static class StringExtensions
    {
        public static bool Contains(this string str1, string str2, StringComparison compType)
        {
            return str1?.IndexOf(str2, compType) >= 0;
        }
    }



}
