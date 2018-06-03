using System.Collections.Generic;

namespace HELLION.DataStructures.StaticData
{
    /// <summary>
    /// Defines a authorised person for use on vessel cerw lists.
    /// </summary>
    public class AuthorisedPerson
    {
        public AuthorisedPersonRank Rank;
        public long PlayerGUID;
        public long SteamID;
        public string Name;
    }

    /// <summary>
    /// Enumeration for crew ranks.
    /// </summary>
    public enum AuthorisedPersonRank
    {
        None = 0, // 0
        CommandingOfficer, // 1
        ExecutiveOfficer,
        Crewman, // 3
    }

    /// <summary>
    /// A custom IEqualityComparer that looks at the Steam ID for equality comparisons.
    /// </summary>
    public class AuthorisedPersonSteamIDComparer : IEqualityComparer<AuthorisedPerson>
    {
        public bool Equals(AuthorisedPerson item1, AuthorisedPerson item2)
        {
            if (object.ReferenceEquals(item1, item2))
                return true;
            if (item1 == null || item2 == null)
                return false;
            return item1.SteamID.Equals(item2.SteamID);
        }
        public int GetHashCode(AuthorisedPerson item)
        {
            //int hCode = bx.Height ^ bx.Length ^ bx.Width;
            return item.SteamID.GetHashCode();
        }
    }

}
