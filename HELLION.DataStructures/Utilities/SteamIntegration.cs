using System;
using System.Collections.Generic;
using System.Xml;

namespace HELLION.DataStructures.Utilities
{
    public static class SteamIntegration
    {
        /// <summary>
        /// Gets a Steam PlayerID (name) of the specified SteamID64
        /// </summary>
        /// <param name="steamID64">The ID of the player name to return.</param>
        /// <returns></returns>
        public static string GetPlayerName(long steamID64)
        {
            string uri = String.Format(@"http://steamcommunity.com/profiles/{0}/?xml=1", steamID64);

            XmlDocument document = new XmlDocument();
            document.Load(uri);

            XmlElement root = document.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("/profile/steamID");

            if (nodes.Count > 0) return nodes[0].InnerText;
            return null;
        }

        /// <summary>
        /// Gets a Steam GroupID (name) of the specified Steam Group name.
        /// </summary>
        /// <param name="steamID64">The ID of the player name to return.</param>
        /// <returns></returns>
        public static long? GetGroupID(string groupName)
        {
            string uri = String.Format(@"https://steamcommunity.com/groups/{0}/memberslistxml/?xml=1", groupName);

            XmlDocument document = new XmlDocument();
            document.Load(uri);

            XmlElement root = document.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("/memberList/groupID64");

            if (nodes.Count > 0) return Convert.ToInt64(nodes[0].InnerText);
            return null;
        }

        /// <summary>
        /// Gets a Steam Group Name by it's GroupID64.
        /// </summary>
        /// <param name="groupID64"></param>
        /// <returns></returns>
        public static string GetGroupName(long groupID64)
        {
            string uri = String.Format(@"https://steamcommunity.com/gid/{0}/memberslistxml/?xml=1", groupID64);

            XmlDocument document = new XmlDocument();
            document.Load(uri);

            XmlElement root = document.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("/memberList/groupDetails/groupName");

            return (nodes.Count > 0) ? nodes[0].InnerText : null;
        }

        /// <summary>
        /// Gets a Steam Group Custom URL by it's GroupID64.
        /// </summary>
        /// <param name="groupID64"></param>
        /// <returns></returns>
        public static string GetGroupURL(long groupID64)
        {
            string uri = String.Format(@"https://steamcommunity.com/gid/{0}/memberslistxml/?xml=1", groupID64);

            XmlDocument document = new XmlDocument();
            document.Load(uri);

            XmlElement root = document.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("/memberList/groupDetails/groupURL");

            if (nodes.Count > 0) return nodes[0].InnerText;
            return null;
        }
        
        /// <summary>
        /// Gets the SteamID64 of each member of the specified Steam Group name.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static List<long> GetGroupMembers(string groupName)
        {
            Uri uri = new Uri(String.Format(@"https://steamcommunity.com/groups/{0}/memberslistxml/?xml=1", groupName));
            return GetGroupMembers(uri);
        }

        /// <summary>
        /// Gets the SteamID64 of each member of the specified Steam GroupID64.
        /// </summary>
        /// <param name="groupID64"></param>
        /// <returns></returns>
        public static List<long> GetGroupMembers(long groupID64)
        {
            Uri uri = new Uri(String.Format(@"https://steamcommunity.com/gid/{0}/memberslistxml/?xml=1", groupID64));
            return GetGroupMembers(uri);
        }

        /// <summary>
        /// Gets the SteamID64 of each member of the specified Steam Group Uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static List<long> GetGroupMembers(Uri uri)
        {
            List<long> results = new List<long>();
            XmlDocument document = new XmlDocument();

            document.Load(uri.ToString());

            XmlElement root = document.DocumentElement;

            foreach (XmlNode node in root.SelectNodes("/memberList/members/steamID64"))
            {
                results.Add(Convert.ToInt64(node.InnerText));
            }
            return results;
        }

    }
}

