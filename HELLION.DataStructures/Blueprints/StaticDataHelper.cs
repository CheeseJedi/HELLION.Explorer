using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HELLION.DataStructures
{
    public static class StaticDataHelper
    {
        /// <summary>
        /// The primary Enum for referencing the structures and their IDs and Descriptions.
        /// </summary>
        public enum HEStructureSceneID
        {
            //SolarSystemSetup = -3,
            //ItemScene = -2,
            //None = -1,

            Unspecified = 0,

            //Slavica = 1,
            //[Description("BRO")] // AltCorp Brontes?
            //AltCorp_Ship_Tamara = 2,

            [Description("CIM")]
            AltCorp_CorridorModule = 3,
            [Description("CTM")]
            AltCorp_CorridorIntersectionModule = 4,
            [Description("CLM")]
            AltCorp_Corridor45TurnModule = 5,

            [Description("ARG")]
            AltCorp_Shuttle_SARA = 6, // AltCorp Arges (aka Mule)
            [Description("PSM")]
            ALtCorp_PowerSupply_Module = 7,
            [Description("LSM")]
            AltCorp_LifeSupportModule = 8,
            [Description("CBM")]
            AltCorp_Cargo_Module = 9,

            [Description("CSM")]
            AltCorp_CorridorVertical = 10, // 0x0000000A
            [Description("CM")]
            AltCorp_Command_Module = 11, // 0x0000000B
            [Description("CRM")]
            AltCorp_Corridor45TurnRightModule = 12, // 0x0000000C
            [Description("OUTPOST")]
            AltCorp_StartingModule = 13, // 0x0000000D
            [Description("AM")]
            AltCorp_AirLock = 14, // 0x0000000E

            Generic_Debris_JuncRoom001 = 15, // 0x0000000F
            Generic_Debris_JuncRoom002 = 16, // 0x00000010
            Generic_Debris_Corridor001 = 17, // 0x00000011
            Generic_Debris_Corridor002 = 18, // 0x00000012

            [Description("IC")]
            AltCorp_DockableContainer = 19, // 0x00000013

            MataPrefabs = 20, // 0x00000014
            Generic_Debris_Outpost001 = 21, // 0x00000015

            [Description("CQM")]
            AltCorp_CrewQuarters_Module = 22, // 0x00000016

            Generic_Debris_Spawn1 = 23, // 0x00000017
            Generic_Debris_Spawn2 = 24, // 0x00000018
            Generic_Debris_Spawn3 = 25, // 0x00000019

            [Description("SPM")]
            AltCorp_SolarPowerModule = 26, // 0x0000001A
            [Description("STE")]
            AltCorp_Shuttle_CECA = 27, // 0x0000001B // AltCorp Steropes.
            [Description("FM")]
            AltCorp_FabricatorModule = 28, // 0x0000001C
            FlatShipTest = 29, // 0x0000001D
            // ActaeonProbe = 30, // 0x0000001E

            //Asteroid01 = 1000, // 0x000003E8
            //Asteroid02 = 1001, // 0x000003E9
            //Asteroid03 = 1002, // 0x000003EA
            //Asteroid04 = 1003, // 0x000003EB
            //Asteroid05 = 1004, // 0x000003EC
            //Asteroid06 = 1005, // 0x000003ED
            //Asteroid07 = 1006, // 0x000003EE
            //Asteroid08 = 1007, // 0x000003EF
        }

        /// <summary>
        /// Docking Port Types Enum.
        /// </summary>
        public enum HEDockingPortType
        {
            Unspecified = 0,
            StandardDockingPortA,
            StandardDockingPortB,
            StandardDockingPortC,
            StandardDockingPortD,
            AirlockDockingPort,
            Grapple,    // LEGACY ITEM - DEPRECATED
            IndustrialContainerPortA,
            IndustrialContainerPortB,
            IndustrialContainerPortC,
            IndustrialContainerPortD,
            CargoDockingPortA,
            CargoDockingPortB,
            CargoDock,  // Dockable Cargo (IC) module
            Anchor,
            DerelictPort1,
            DerelictPort2,
        }

        public static Dictionary<HEStructureSceneID, Dictionary<int, HEDockingPortType>> DockingPortHints
            = new Dictionary<HEStructureSceneID, Dictionary<int, HEDockingPortType>>()
        {

        { HEStructureSceneID.AltCorp_CorridorModule, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.StandardDockingPortA },
            { 2, HEDockingPortType.StandardDockingPortB },
            { 3, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.AltCorp_CorridorIntersectionModule,  new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.StandardDockingPortA },
            { 3, HEDockingPortType.StandardDockingPortB },
            { 2, HEDockingPortType.StandardDockingPortC },
            { 4, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.AltCorp_Corridor45TurnModule,  new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.StandardDockingPortA },
            { 2, HEDockingPortType.StandardDockingPortB },
            { 3, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.AltCorp_Shuttle_SARA, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.AirlockDockingPort },
            { 2, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.ALtCorp_PowerSupply_Module, new Dictionary<int, HEDockingPortType> {
            { 2, HEDockingPortType.StandardDockingPortA },
            { 1, HEDockingPortType.StandardDockingPortB },
            { 3, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.AltCorp_LifeSupportModule, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.StandardDockingPortA },
            { 2, HEDockingPortType.StandardDockingPortB },
            { 3, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.AltCorp_Cargo_Module,  new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.StandardDockingPortA },
            { 4, HEDockingPortType.IndustrialContainerPortA },
            { 5, HEDockingPortType.IndustrialContainerPortB },
            { 7, HEDockingPortType.IndustrialContainerPortC },
            { 6, HEDockingPortType.IndustrialContainerPortD },
            { 2, HEDockingPortType.CargoDockingPortA },
            { 3, HEDockingPortType.CargoDockingPortB },
            { 8, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.AltCorp_CorridorVertical, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.StandardDockingPortA },
            { 2, HEDockingPortType.StandardDockingPortB },
            { 3, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.AltCorp_Command_Module, new Dictionary<int, HEDockingPortType> {
            { 3, HEDockingPortType.StandardDockingPortA },
            { 2, HEDockingPortType.StandardDockingPortB },
            { 1, HEDockingPortType.StandardDockingPortC },
            { 4, HEDockingPortType.StandardDockingPortD },
            { 5, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.AltCorp_Corridor45TurnRightModule, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.StandardDockingPortA },
            { 2, HEDockingPortType.StandardDockingPortB },
            { 3, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.AltCorp_StartingModule, new Dictionary<int, HEDockingPortType> {
            { 2, HEDockingPortType.StandardDockingPortA },
            { 1, HEDockingPortType.StandardDockingPortB },
            { 3, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.Generic_Debris_JuncRoom001, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.DerelictPort1 },
            { 2, HEDockingPortType.DerelictPort2 }
        } },

        { HEStructureSceneID.Generic_Debris_JuncRoom002, new Dictionary<int, HEDockingPortType> { } },

        { HEStructureSceneID.Generic_Debris_Corridor001, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.DerelictPort1 }
        } },

        { HEStructureSceneID.Generic_Debris_Corridor002, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.DerelictPort1 },
            { 2, HEDockingPortType.DerelictPort2 }
        } },

        { HEStructureSceneID.AltCorp_AirLock, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.AirlockDockingPort },
            { 2, HEDockingPortType.StandardDockingPortA },
            { 3, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.AltCorp_DockableContainer, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.StandardDockingPortA },
            { 2, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.Generic_Debris_Outpost001, new Dictionary<int, HEDockingPortType> { } },

        { HEStructureSceneID.AltCorp_CrewQuarters_Module, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.StandardDockingPortA },
            { 2, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.Generic_Debris_Spawn1, new Dictionary<int, HEDockingPortType> { } },

        { HEStructureSceneID.Generic_Debris_Spawn2, new Dictionary<int, HEDockingPortType> { } },

        { HEStructureSceneID.Generic_Debris_Spawn3, new Dictionary<int, HEDockingPortType> { } },

        { HEStructureSceneID.AltCorp_SolarPowerModule, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.StandardDockingPortA },
            { 2, HEDockingPortType.StandardDockingPortB },
            { 3, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.AltCorp_Shuttle_CECA, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.AltCorp_FabricatorModule, new Dictionary<int, HEDockingPortType> {
            { 1, HEDockingPortType.StandardDockingPortA },
            { 2, HEDockingPortType.Anchor }
        } },

        { HEStructureSceneID.MataPrefabs, new Dictionary<int, HEDockingPortType> { } },

        { HEStructureSceneID.FlatShipTest, new Dictionary<int, HEDockingPortType> { } },

        };

        public static HEDockingPortType GetDockingPortType(HEStructureSceneID sceneID, int orderID)
        {
            // Attempt to locate the correct port Dictionary by sceneID.
            if (DockingPortHints.TryGetValue(sceneID, out Dictionary<int, HEDockingPortType> portDictionary))
            {
                // We should have a Dictionary object containing the structure's port OrderIDs and Names.
                if (portDictionary.TryGetValue(orderID, out HEDockingPortType portType))
                {
                    return portType;
                }
            }
            return HEDockingPortType.Unspecified;
        }

        public enum AuthorisedPersonRank
        {
            None = 0, // 0
            CommandingOfficer, // 1
            ExecutiveOfficer,
            Crewman, // 3
        }

        public class AuthorisedPerson
        {
            public AuthorisedPersonRank Rank;
            public long PlayerGUID;
            public long SteamID;
            public string Name;
        }

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

    /*
    public class SceneIDStringEnumConverter : StringEnumConverter
    {

        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
        

        
        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            bool isNullable = ReflectionUtils.IsNullableType(objectType);
            Type t = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;

            if (t != typeof(HEBlueprintStructureSceneID))
                throw new Exception("Was called, but wrong type.");

            try
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string enumText = reader.Value.ToString();

                    if (enumText == string.Empty && isNullable)
                    {
                        return null;
                    }

                    HEBlueprintStructureSceneID descriptionParseResult = EnumExtensions.ParseToEnumDescriptionOrEnumerator<HEBlueprintStructureSceneID>(enumText);
                    if (descriptionParseResult != HEBlueprintStructureSceneID.Unspecified) return descriptionParseResult;

                    return Enum.TryParse(enumText, out HEBlueprintStructureSceneID result) ? (object)result : null;


                    //return EnumUtils.ParseEnum(t, enumText, !AllowIntegerValues);
                }

                if (reader.TokenType == JsonToken.Integer)
                {
                    if (!AllowIntegerValues)
                    {
                        throw new JsonSerializationException(String.Format("Integer value {0} is not allowed.", reader.Value));
                    }

                    return (HEBlueprintStructureSceneID)reader.Value;
                }
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException(String.Format("Error converting value {0} to type '{1}'.{2}{3}",
                    reader.Value, objectType, Environment.NewLine, ex));
            }

            // we don't actually expect to get here.
            throw new JsonSerializationException(String.Format("Unexpected token {0} when parsing enum.", reader.TokenType));
        }

        internal static class ValidationUtils
        {
            public static void ArgumentNotNull(object value, string parameterName)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(parameterName);
                }
            }
        }


        internal static class ReflectionUtils
        {

            public static bool IsNullable(Type t)
            {
                ValidationUtils.ArgumentNotNull(t, nameof(t));

                if (t.IsValueType)
                {
                    return IsNullableType(t);
                }

                return true;
            }

            public static bool IsNullableType(Type t)
            {
                ValidationUtils.ArgumentNotNull(t, nameof(t));

                return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
            }
        }
    }
    */

    /*
    /// <summary>
    /// LEGACY Structure Types Enum.
    /// </summary>
    /// <remarks>
    /// The numeric values of these correspond to the game's ItemID in the structures.json
    /// to allow for easier cross-referencing.
    /// </remarks>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum aHEBlueprintStructureType
    {
        Unspecified = 0,
        //BRONTES = 2,
        CIM = 3,
        CTM = 4,
        CLM = 5,
        ARG = 6,
        PSM = 7,
        LSM = 8,
        CBM = 9,
        CSM = 10,
        CM = 11,
        CRM = 12,
        OUTPOST = 13,
        AM = 14,
        Generic_Debris_JuncRoom001 = 15, // 0x0000000F
        Generic_Debris_JuncRoom002 = 16, // 0x00000010
        Generic_Debris_Corridor001 = 17, // 0x00000011
        Generic_Debris_Corridor002 = 18, // 0x00000012
        IC = 19, // 0x00000013
        //MataPrefabs = 20, // 0x00000014
        //Generic_Debris_Outpost001 = 21, // 0x00000015
        CQM = 22, // 0x00000016
        // Generic_Debris_Spawn1 = 23, // 0x00000017
        // Generic_Debris_Spawn2 = 24, // 0x00000018
        // Generic_Debris_Spawn3 = 25, // 0x00000019
        SPM = 26, // 0x0000001A
        STEROPES = 27, // 0x0000001B
        FM = 28, // 0x0000001C
        // FlatShipTest = 29, // 0x0000001D
    }
    */

}
