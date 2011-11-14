using System;
using System.Collections.Generic;
using DnTeamData.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeamData
{
    public static class SettingsRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();

        public static EnumName GetEnumName(string value)
        {
            EnumName res;
            if (Enum.TryParse(value.Replace(" ", string.Empty), true, out res))
                return res;

            return EnumName.Undefined;
        }

        public static List<string> GetSettingValues(EnumName name)
        {
            MongoCollection<Enums> coll = Db.GetCollection<Enums>("Enums");
            var query = Query.EQ("Name", name.ToString());
            var collection = coll.FindOne(query);
            var res = (collection == null) ? new List<string>() : collection.Values;

            return res;
        }

        public static void UpdateSetting(EnumName name, string value)
        {
            MongoCollection<Enums> coll = Db.GetCollection<Enums>("Enums");
            var query = Query.EQ("Name", name.ToString());
            var update = Update.AddToSet("Values", value);
            coll.Update(query, update);
        }

        public static void DeleteSetting(EnumName name, string value)
        {
            MongoCollection<Enums> coll = Db.GetCollection<Enums>("Enums");
            var query = Query.EQ("Name", name.ToString());
            var update = Update.Pull("Values", value);
            coll.Update(query, update);
        }

        public static List<string> GetAllLocations()
        {
            return GetSettingValues(EnumName.Locations);
        }

        public static List<string> GetAllProjectStatuses()
        {
            return GetSettingValues(EnumName.ProjectStatuses);
        }

        public static List<string> GetAllProjectTypes()
        {
            return GetSettingValues(EnumName.ProjectTypes);
        }

        public static List<string> GetAllProjectRoles()
        {
            return GetSettingValues(EnumName.ProjectRoles);
        }
    }
}
