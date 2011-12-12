using System;
using System.Collections.Generic;
using System.Linq;
using DnTeamData.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeamData
{
    /// <summary>
    /// Manages Persons entity 
    /// </summary>
    public static class PersonRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static MongoCollection<Person> _coll = Db.GetCollection<Person>("Persons");

        #if DEBUG //Test area
        /// <summary>
        /// Set the name of the collection
        /// </summary>
        /// <param name="collectionName">Collection name</param>
        public static void SetTestCollection(string collectionName)
        {
            _coll = Db.GetCollection<Person>(collectionName);
        }
        #endif


        /// <summary>
        /// Returns the list of persons
        /// </summary>
        /// <param name="isActive">true - returns active persons, false - inActive persons</param>
        /// <returns>The list of Persons</returns>
        public static List<Person> GetAllPeople(bool isActive)
        {
            var query = Query.EQ("IsActive", isActive);
            return _coll.Find(query).ToList();
        }

        /// <summary>
        /// Returns dictioonary of persons
        /// </summary>
        /// <returns>The dictionary of persons (Id, Name)</returns>
        public static Dictionary<string, string> GetActivePersonsList()
        {
            return GetAllPeople(true).ToDictionary(o => o.Id.ToString(), o => o.Name);
        }

        /// <summary>
        /// Create a new person
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="locatedIn">Location id</param>
        /// <param name="primaryManager">Primary manager id</param>
        /// <returns>Creation status of type PersonCreateStatus</returns>
        public static PersonEditStatus CreatePerson(string userName, string locatedIn, string primaryManager)
        {
            ObjectId locationId;
            if (!ObjectId.TryParse(locatedIn, out locationId) && !string.IsNullOrEmpty(locatedIn))
                return PersonEditStatus.ErrorInvalidLocation;

            ObjectId managerId;
            if (!ObjectId.TryParse(primaryManager, out managerId) && !string.IsNullOrEmpty(primaryManager))
                return PersonEditStatus.ErrorInvalidPrimaryManager;
            
            var p = new Person
                        {
                            Name = userName,
                            LocatedIn = locationId,
                            PrimaryManager = managerId
                        };

            try
            {
                _coll.Insert(p, SafeMode.True);
                return PersonEditStatus.Ok;
            }
            catch (MongoSafeModeException ex)
            {
                if (ex.Message.Contains("duplicate")) return PersonEditStatus.ErrorDuplicateName;
                throw;
            }
            
        }

       /// <summary>
        /// Returns the name of the specified person
        /// </summary>
        /// <param name="id">Person id</param>
        /// <returns>Person name</returns>
        internal static string GetPersonName(ObjectId id)
        {
            if (id == ObjectId.Empty) return "wanted";
            
            return _coll.FindOneById(id).Name;
        }

        /// <summary>
        /// Returns the name of the specified person
        /// </summary>
        /// <param name="id">Person id</param>
        /// <returns>Person name</returns>
        public static string GetPersonName(string id)
        {
            ObjectId personId;
            if(ObjectId.TryParse(id, out personId))
            {
                return GetPersonName(personId);
            }

            return "wanted";
        }

        /// <summary>
        /// Returns the defined Person
        /// </summary>
        /// <param name="id">Person Id</param>
        /// <returns>Person</returns>
        public static Person GetPerson(string id)
        {
            return _coll.FindOneById(ObjectId.Parse(id));
        }

        /// <summary>
        /// Updates value of the specified Person's property
        /// </summary>
        /// <param name="id">Person Id</param>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        /// <returns>Update status</returns>
        public static PersonEditStatus UpdateProperty(string id, string name, string value)
        {
            dynamic val;
            if(!Common.GetTypedPropertyValue(name, value, typeof(Person), out val))
            {
                return PersonEditStatus.ErrorUndefinedFormat;
            }

            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set(name, val);

            try
            {
                var res = _coll.Update(query, update, SafeMode.True);

                return res.UpdatedExisting ? PersonEditStatus.Ok : PersonEditStatus.ErrorPropertyHasNotBeenUpdated;
            }
            catch(MongoSafeModeException ex)
            {
                if (ex.Message.Contains("duplicate")) return PersonEditStatus.ErrorDuplicateItem;

                throw;
            }
        }

        /// <summary>
        /// Adds value to the defined property of the Person
        /// </summary>
        /// <param name="id">Person Id</param>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        /// <returns>Update status</returns>
        public static PersonEditStatus AddValueToPropertySet(string id, string name, string value)
        {
            dynamic val;
            if (!Common.GetTypedPropertyValue(name, value, typeof(Person), out val, true))
            {
                return PersonEditStatus.ErrorUndefinedFormat;
            }

            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.AddToSet(name,  val);

            var res = _coll.Update(query, update, SafeMode.True);

            return  (res.DocumentsAffected == 0) ? PersonEditStatus.ErrorPropertyHasNotBeenAdded : PersonEditStatus.Ok; 
        }

        /// <summary>
        /// Deletes defined value from Person's property
        /// </summary>
        /// <param name="id">Person Id</param>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        public static void DeleteValueFromPropertySet(string id, string name, string value)
        {
            if(string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value)) return;

            var query = Query.EQ("_id", ObjectId.Parse(id));
            dynamic val;
            if (Common.GetTypedPropertyValue(name, value, typeof(Person), out val, true))
            {
                var update = Update.Pull(name, val);

                _coll.Update(query, update, SafeMode.True);
            }
        }

        /// <summary>
        /// Deactivates defined people
        /// </summary>
        /// <param name="values"> The list of people ids</param>
        public static void DeletePeople(IEnumerable<string> values)
        {
            var query = Query.In("_id", new BsonArray(values.Select(ObjectId.Parse)));
            var update = Update.Set("IsActive", false);
            
            _coll.Update(query, update, UpdateFlags.Multi);
        }

        /// <summary>
        /// Validates if the OpenId is assigned to a user
        /// </summary>
        /// <param name="id">OpenId (is "http" or "https" URI, or an XRI)</param>
        /// <returns>User name</returns>
        public static string ValidateIdentifier(string id)
        {
            var query = Query.And(new [] {Query.EQ("OpenId", id), Query.EQ("IsActive", true)});
            var res = _coll.FindOne(query);

            return (res == null) ? string.Empty : res.Name;
        }

        /// <summary>
        /// Returns the list of the departments used by Persons
        /// </summary>
        /// <param name="departments">The list of all departments</param>
        /// <returns>The list if used departments</returns>
        internal static IEnumerable<ObjectId> GetUsedDepartments(IEnumerable<ObjectId> departments)
        {
            var query = Query.In("LocatedIn", new BsonArray(departments));
            var result = _coll.Find(query);
            result.Fields = Fields.Include(new[] { "LocatedIn" });

            return result.Select(o => o.LocatedIn);
        }

        /// <summary>
        ///Returns technology specialties of the defined Person
        /// </summary>
        /// <param name="id">Person Id</param>
        ///<returns>Specialties list</returns>
        public static IEnumerable<Specialty> GetTechnologySpecialties(string id)
        {
            return GetPerson(id).TechnologySpecialties;
        }

        /// <summary>
        /// Add Technology Specialty to the defined Person
        /// </summary>
        /// <param name="id">Person Id</param>
        /// <param name="name">specilaty name</param>
        /// <param name="value">specialty level</param>
        /// <param name="lastUsed">last used date</param>
        /// <param name="firstUsed">first used date</param>
        /// <param name="note">last project notes</param>
        /// <returns>Creation status</returns>
        public static PersonEditStatus CreateTechnologySpecialty(string id, string name, string value, string lastUsed, string firstUsed, string note)
        {
            //validate dates are valid
            DateTime firstUsedDate;
            if (!DateTime.TryParse(firstUsed, out firstUsedDate))
            {
                return PersonEditStatus.ErrorDateIsNotValid;
            }

            DateTime lastUsedDate;
            if (!DateTime.TryParse(lastUsed, out lastUsedDate))
            {
                return PersonEditStatus.ErrorDateIsNotValid;
            }

            var query = Query.And(new[] { Query.EQ("_id", ObjectId.Parse(id)), Query.NE("TechnologySpecialties.Name", name) });
            
            var update = Update.AddToSet("TechnologySpecialties", new Specialty
            {
                Name = name,
                Level = value,
                LastUsed = lastUsedDate,
                FirstUsed = firstUsedDate,
                LastProjectNote = note
            }.ToBsonDocument());
            
            var res = _coll.Update(query, update, SafeMode.True);
            
            return (res.DocumentsAffected == 0) ? PersonEditStatus.ErrorDuplicateSpecialtyName : PersonEditStatus.Ok;
        }

        /// <summary>
        /// Updates Technology Specialty to the defined Person
        /// </summary>
        /// <param name="id">Person Id</param>
        /// <param name="name">specilaty name</param>
        /// <param name="value">specialty level</param>
        /// <param name="lastUsed">last used date</param>
        /// <param name="firstUsed">first used date</param>
        /// <param name="note">last project notes</param>
        /// <returns>Update status</returns>
        public static PersonEditStatus UpdateTechnologySpecialty(string id, string name, string value, string lastUsed, string firstUsed, string note)
        {
            //validate dates are valid
            //validate dates are valid
            DateTime firstUsedDate;
            if (!DateTime.TryParse(firstUsed, out firstUsedDate))
            {
                return PersonEditStatus.ErrorDateIsNotValid;
            }

            DateTime lastUsedDate;
            if (!DateTime.TryParse(lastUsed, out lastUsedDate))
            {
                return PersonEditStatus.ErrorDateIsNotValid;
            }

            var query = Query.And(new[] { Query.EQ("_id", ObjectId.Parse(id)), Query.EQ("TechnologySpecialties.Name", name) });
            var update = Update.Set("TechnologySpecialties.$.Level", value).Set("TechnologySpecialties.$.LastUsed", lastUsedDate)
                .Set("TechnologySpecialties.$.FirstUsed", firstUsedDate).Set("TechnologySpecialties.$.LastProjectNote", note);

            var res = _coll.Update(query, update, SafeMode.True);

            return (res.DocumentsAffected == 0) ? PersonEditStatus.ErrorUndefined : PersonEditStatus.Ok;
        }

        /// <summary>
        /// Deletes defined technology specialties from the Person
        /// </summary>
        /// <param name="id">Person Id</param>
        /// <param name="values">The list of specialties' names to delete</param>
        public static void DeleteTechnologySpecialties(string id, IEnumerable<string> values)
        {
            var update = Update.Pull("TechnologySpecialties", Query.In("Name", new BsonArray(values)));
            
            _coll.Update(Query.EQ("_id", ObjectId.Parse(id)), update, SafeMode.True);
        }
     
    }
}
