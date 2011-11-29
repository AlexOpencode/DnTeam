using System;
using System.Collections.Generic;
using System.Linq;
using DnTeamData.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeamData
{
    public static class SettingsRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static readonly MongoCollection<Enums> Coll = Db.GetCollection<Enums>("Enums");

        public static EnumName GetEnumName(string value)
        {
            EnumName res;
            if (Enum.TryParse(value.Replace(" ", string.Empty), true, out res))
                return res;

            return EnumName.Undefined;
        }

        public static List<string> GetSettingValues(EnumName name)
        {
            var query = Query.EQ("Name", name.ToString());
            var collection = Coll.FindOne(query);
            var res = (collection.Values == null) ? new List<string>() : collection.Values;

            return res;
        }

        public static void UpdateSetting(EnumName name, string value)
        {
            var query = Query.EQ("Name", name.ToString());
            var update = Update.AddToSet("Values", value);
            Coll.Update(query, update);
        }

        public static void DeleteSetting(EnumName name, string value)
        {
            var query = Query.EQ("Name", name.ToString());
            var update = Update.Pull("Values", value);
            Coll.Update(query, update);
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

        internal static List<string> GetAllMilestones()
        {
            return GetSettingValues(EnumName.ProjectMilestones);
        }

        public static void BatchUpdateSetting(EnumName name, string value)
        {
            var query = Query.EQ("Name", name.ToString());
            var update = Update.AddToSetEach("Values", BsonArray.Create(value.Split('~').Where(o => !string.IsNullOrEmpty(o)).Select(o => o.Trim())));

            Coll.Update(query, update);
        }

        public static List<string> GetAllProjectNoiseTypes()
        {
            return GetSettingValues(EnumName.ProjectNoiseTypes);
        }

        public static List<string> GetAllProjectPriorityTypes()
        {
            return GetSettingValues(EnumName.ProjectPriorityTypes);
        }
    }
}
