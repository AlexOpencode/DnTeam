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

        public static  EnumName GetEnumName (string value)
        {
            EnumName res;
            if(Enum.TryParse(value.Replace(" ",string.Empty), true, out res))
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

        //public static List<string> GetAllUserRoles()
        //{
        //    return GetSettingValues(EnumName.ProjectRoles);
        //}

        //public static void InsertRole(string value)
        //{
        //    UpdateSetting(EnumName.ProjectRoles, value);
        //}

        //public static void DeleteRole(string value)
        //{
        //    DeleteSetting(EnumName.ProjectRoles, value);
        //}

        public static List<string> GetAllLocations()
        {
            return GetSettingValues(EnumName.Locations);
        }

        //public static void InsertLocation(string location)
        //{
        //   UpdateSetting(EnumName.Locations, location);
        //}

        //public static void DeleteLocation(string value)
        //{
        //    DeleteSetting(EnumName.Locations, value);
        //}

        //public static List<string> GetAllTechnologies()
        //{
        //    return GetSettingValues(EnumName.TechnologySpecialties);
        //}

        //public static void InsertTechnology(string value)
        //{
        //    UpdateSetting(EnumName.TechnologySpecialties, value);
        //}

        //public static void DeleteTechnology(string value)
        //{
        //    DeleteSetting(EnumName.TechnologySpecialties, value);
        //}

        public static List<string> GetAllProjectStatuses()
        {
            return GetSettingValues(EnumName.ProjectStatuses);
        }

        //public static void InsertProjectStatus(string value)
        //{
        //    UpdateSetting(EnumName.ProjectStatuses, value);
        //}

        //public static void DeleteProjectStatus(string value)
        //{
        //    DeleteSetting(EnumName.ProjectStatuses, value);
        //}

        //public static void InsertFunctionalSpecialty(string value)
        //{
        //    UpdateSetting(EnumName.FunctionalSpecialties, value);
        //}

        //public static List<string> GetAllFunctionalSpecialties()
        //{
        //    return GetSettingValues(EnumName.FunctionalSpecialties);
        //}

        //public static void DeleteFunctionalSpecialty(string value)
        //{
        //    DeleteSetting(EnumName.FunctionalSpecialties, value);
        //}
    }
}
