using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DnTeamData.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeamData
{
    public static class DepartmentRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static MongoCollection<Department> _coll = Db.GetCollection<Department>("Departments");
        private static MongoCollection<Person> _personColl = Db.GetCollection<Person>("Persons");

#if DEBUG //Sets collection name for tests
        public static void SetTestDepartmentCollection(string collectionName)
        {
            _coll = Db.GetCollection<Department>(collectionName);
        }
        public static void SetTestPersonCollection(string collectionName)
        {
            _personColl = Db.GetCollection<Person>(collectionName);
        }
#endif


        public static List<Department> GetAllDepartments()
        {
            return _coll.FindAll().ToList();
        }

        public static Dictionary<string, string> GetDepartmentsList()
        {
            return GetAllDepartments().ToDictionary(o => o.Id.ToString(), x => string.Format("{0}, {1}", x.Location,x.Name));
        }

        //public static IEnumerable<TDepartment> GetDepartmentsTree()
        //{

        //    var departments = GetAllDepartments();
        //    //Get all top Departments
        //    var topDepartments = departments.Where(o => o.DepartmentOf == ObjectId.Empty).ToList();
        //    //Reduse total Departments
        //    departments = departments.Except(topDepartments).ToList();
        //    //Convert Departments to TDepartment
        //    var resDepartments = topDepartments.Select(o => new TDepartment
        //    {
        //        Id = o.Id.ToString(),
        //        Name = o.Name,
        //        Subsidiaries = o.Subsidiaries
        //    }).ToList();

        //    //Retrive sub-Departments
        //    GetSubDepartments(departments, resDepartments);

        //    return resDepartments;
        //}

        //private static void GetSubDepartments(List<Department> departments, List<TDepartment> resDepartments)
        //{
        //    foreach (TDepartment department in resDepartments)
        //    {
        //        var subDepartments = departments.Where(o => o.DepartmentOf == ObjectId.Parse(department.Id)).ToList();
        //        if (subDepartments.Count() <= 0) continue;

        //        department.SubDepartments = subDepartments.Select(o => new TDepartment
        //                                                                      {
        //                                                                          Id = o.Id.ToString(),
        //                                                                          Name = o.Name,
        //                                                                          Subsidiaries = o.Subsidiaries
        //                                                                      }).ToList();

        //        departments = departments.Except(subDepartments).ToList();

        //        GetSubDepartments(departments, department.SubDepartments);
        //    }


        //}

        public static bool CreateDepartment(string name, string departmentOf)
        {
            var res = _coll.Insert(new Department
                                      {
                                          Name = name,
                                          DepartmentOf = string.IsNullOrEmpty(departmentOf) ? ObjectId.Empty : ObjectId.Parse(departmentOf)
                                          
                                      }, SafeMode.True);
            return res.Ok;
        }

        ////TODO: Validate if any user is linked to the susbsidary - prevent deletion
        ////TODO: Validate if element with such location exists in the given departament
        //public static bool CreateSubsidiary(string location, float baseCost, float baseRate, string departmentId)
        //{
        //    var query = Query.EQ("_id", ObjectId.Parse(departmentId));
        //    var update = Update.AddToSet("Subsidiaries", new Subsidiary
        //                                                     {
        //                                                         Location = location,
        //                                                         BaseCost = baseCost,
        //                                                         BaseRate = baseRate
        //                                                     }.ToBsonDocument());

        //    var res = _coll.Update(query, update, SafeMode.True);

        //    return res.Ok;
        //}

        //public static bool DeleteDepartment(string id)
        //{
        //    var query = Query.EQ("_id", ObjectId.Parse(id));
        //    var update = Update.Set("IsDeleted", true);

        //    var res = _coll.Update(query, update, SafeMode.True);

        //    return res.Ok;
        //}

        //public static bool DeleteSubsidiary(string id, string departmentId)
        //{

        //    using (Db.Server.RequestStart(Db))
        //    {
        //        var update = Update.Pull("Subsidiaries", Query.EQ("_id", ObjectId.Parse(id)));
        //        var res = _coll.Update(Query.EQ("_id", ObjectId.Parse(departmentId)), update, SafeMode.True);
        //        if(res.Ok)
        //        {
        //            var query = Query.EQ("LocatedIn", ObjectId.Parse(id));
        //            update = Update.Set("LocatedIn", ObjectId.Empty);
        //            res = _personColl.Update(query, update, SafeMode.True);

        //            return res.Ok;
        //        }
        //    }

        //    return false;
        //}

        //public static bool UpdateDepartment(string id, string name)
        //{
        //    var query = Query.EQ("_id", ObjectId.Parse(id));
        //    var update = Update.Set("Name", name);

        //    var res = _coll.Update(query, update, SafeMode.True);

        //    return res.Ok;
        //}

        //public static bool UpdateSubsidiary(string id, string location, float baseCost, float baseRate, string departmentId)
        //{
        //    var query = Query.And(new[] { Query.EQ("_id", ObjectId.Parse(departmentId)), Query.EQ("Subsidiaries._id", ObjectId.Parse(id)) });
        //    var update = Update.Set("Subsidiaries.$.Location", location).Set("Subsidiaries.$.BaseCost", baseCost).Set("Subsidiaries.$.BaseRate", baseRate);
            
        //    var res = _coll.Update(query, update, SafeMode.True);

        //    return res.Ok;
        //}

        //public static Dictionary<string, string> GetLocationsList()
        //{
        //    var res = new Dictionary<string, string>();
        //    var query = Query.Where("(this.Subsidiaries.length > 0)");
        //    var departments = _coll.Find(query);
        //    foreach (Department department in departments)
        //    {
        //        foreach (Subsidiary subsidiary in department.Subsidiaries)
        //        {
        //            res.Add(subsidiary.SubsidiaryId, string.Format("{0}, {1}", department.Name, subsidiary.Location));
        //        }
        //    }

        //    return res;
        //}

        //internal static string GetLocationName(ObjectId locatedIn)
        //{
        //    var query = Query.EQ("Subsidiaries._id", locatedIn);
        //    var departament = _coll.FindOne(query);

        //    return string.Format("{0}, {1}", departament.Name, departament.Subsidiaries.Single(o => o.Id == locatedIn).Location);
        //}

        /// <summary>
        /// Saves department or creates new if id is empty
        /// </summary>
        /// <param name="id">Department Id</param>
        /// <param name="location">Department Location Name</param>
        /// <param name="name">Department Name</param>
        /// <param name="parentId">Parent Department Id</param>
        /// /// <param name="parentId">Parent Department Location, Name</param>
        /// <param name="rate">Basic Rate</param>
        /// <param name="cost">Basic Rate</param>
        /// <returns>DepartmentEditStatus</returns>
        public static object[] SaveDepartment(string id, string location, string name, string parentId, string parentName, decimal rate, decimal cost)
        {
            try
            {
                ObjectId _parentId;
                ObjectId.TryParse(parentId, out _parentId);

                var department = new Department
                                     {
                                         Id = string.IsNullOrEmpty(id) ? ObjectId.GenerateNewId() : ObjectId.Parse(id),
                                         Name = name,
                                         DepartmentOf = _parentId,
                                         Cost = cost,
                                         Rate = rate,
                                         Location = location,
                                         ParentDepartment = string.IsNullOrEmpty(parentName) ? string.Empty : parentName.Split(',').ElementAt(1).Trim()
                                     };
                _coll.Save(department, SafeMode.True);

                return new object[] { DepartmentEditStatus.Created, department.Id.ToString() };
            }
            catch (MongoSafeModeException ex)
            {
                if (ex.Message.Contains("duplicate"))
                    return new object[] {DepartmentEditStatus.ErrorDuplicate, ex.Message};

                return new object[] { DepartmentEditStatus.ErrorUndefined, ex.Message };
            }
           
        }

        /// <summary>
        /// Deletes the list of the defined departments. Doesn't delete Departmensts that are linked to Person or that are parent.
        /// </summary>
        /// <param name="values">The list of departments' ids to be deleted.</param>
        public static void DeleteDepartments(List<string> values)
        {

            //conbert ids to ObjectId
            List<ObjectId> departments = values.Where(o => !string.IsNullOrEmpty(o)).Select(ObjectId.Parse).ToList();
            if (departments.Count() <= 0) return;

            //validate department is not used on Person
            departments = departments.Except(PersonsRepository.GetUsedDepartments(departments)).ToList();
            if (departments.Count() <= 0) return;

            //validate department is not parent
            departments = departments.Except(GetParentDepartments(departments)).ToList();

            if (departments.Count() <= 0) return;

            var query = Query.In("_id", new BsonArray(departments));
            _coll.Remove(query);
        }

        /// <summary>
        /// Returns the list of departments that are beaing used as parent and departments that use them are not in the list
        /// </summary>
        /// <param name="departments">The list of departments</param>
        /// <returns>List of Departments' ObjectId</returns>
        private static IEnumerable<ObjectId> GetParentDepartments(List<ObjectId> departments)
        {
            var query = Query.And(Query.In("DepartmentOf", new BsonArray(departments)), Query.NotIn("_id", new BsonArray(departments))); 
            var result = _coll.Find(query);
            result.Fields = Fields.Include(new[] { "DepartmentOf" });

            return result.Select(o => o.DepartmentOf);
        }


        /// <summary>
        /// Returns the list of offered values to be used for filter
        /// </summary>
        /// <param name="column">The name of the Department property to be searched in</param>
        /// <param name="query">Search query</param>
        /// <returns>The list of "column" values that respond to the search query</returns>
        public static IEnumerable<string> GetTopSearchOffers(string column, string query)
        {
            var q = Query.Matches(column, new BsonRegularExpression(string.Format("/^{0}/i", query)));

            var matches = _coll.Find(q).SetLimit(10);
            matches.Fields = Fields.Include(column);

            //select value of property using its name
            Type type = typeof(Department);
            PropertyInfo info = type.GetProperty(column);
            if (info == null) return  new List<string>();


            return matches.Select(department => info.GetValue(department, null).ToString()).Distinct();
        }

        /// <summary>
        /// Returns the list of filtered parameters
        /// </summary>
        /// <param name="querys">the list of queries of kind ColumnName~value1,value2</param>
        /// <returns>The list of departments</returns>
        public static List<Department> GetFilteredDepartments(List<string> querys)
        {
            var andQueryList = new List<IMongoQuery>();


            foreach (string query in querys)
            {
                var v = query.Split('~');
                var names = v[1].Split(',');

                andQueryList.Add(Query.Or(names.Select(name => Query.Matches(v[0], new BsonRegularExpression(string.Format("/^{0}/i", name)))).Cast<IMongoQuery>().ToArray()));

            }

            return _coll.Find(Query.And(andQueryList.ToArray())).ToList();

        }
    }
}
