using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DnTeamData.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeamData
{
    /// <summary>
    /// Manages data manipulation of Departments
    /// </summary>
    public static class DepartmentRepository
    {
        private static readonly MongoDatabase Db = Mongo.Init();
        private static MongoCollection<Department> _coll = Db.GetCollection<Department>("Departments");

        #if DEBUG //Test area
        /// <summary>
        /// Set the name of the Departments collection
        /// </summary>
        /// <param name="collectionName">Departments collection name</param>
        public static void SetTestCollection(string collectionName)
        {
            _coll = Db.GetCollection<Department>(collectionName);
        }
        /// <summary>
        /// Public function to call internal Exists while testing
        /// </summary>
        public static bool ExistsTest(ObjectId id)
        {
            return Exists(id);
        }
        /// <summary>
        /// Public function to call internal GetDepartmentName while testing
        /// </summary>
        public static string GetDepartmentNameTest(ObjectId id)
        {
            return GetDepartmentName(id);
        }
        /// <summary>
        /// Public function to call private GetParentDependantDepartments while testing
        /// </summary>
        public static IEnumerable<ObjectId> GetParentDependantDepartmentsTest(List<ObjectId> departments)
        {
            return GetParentDependantDepartments(departments);
        }
        #endif

        /// <summary>
        /// Returns the list of departments that are being used as parent and departments that use them are not in the list
        /// </summary>
        /// <param name="departments">The list of departments</param>
        /// <returns>List of Departments' ObjectId</returns>
        private static IEnumerable<ObjectId> GetParentDependantDepartments(List<ObjectId> departments)
        {
            var query = Query.In("DepartmentOf", new BsonArray(departments));
            var cursor = _coll.Find(query);
            cursor.Fields = Fields.Include(new[] { "_id", "DepartmentOf" });
            var parents = cursor.Select(o => o.DepartmentOf).Distinct().ToList();

            RemoveIfAllChildrenArePresentOrThereAreNoChildren(departments, cursor, parents);
            
            return parents;
        }

        /// <summary>
        /// Recursive method that removes id from the list of departments if all of its children are present in the list and their children don't have children ...
        /// </summary>
        /// <param name="departments">The list of all department ids</param>
        /// <param name="cursor">Mongo Cursor, providing acces to the list of all departments</param>
        /// <param name="parents"></param>
        private static void RemoveIfAllChildrenArePresentOrThereAreNoChildren(List<ObjectId> departments, MongoCursor<Department> cursor, List<ObjectId> parents)
        {
            var localParents = new List<ObjectId>(parents);
            foreach (var dep in localParents)
            {
                ObjectId pId = dep;
                //Get all children
                var children = cursor.Where(o => o.DepartmentOf == pId).Select(o => o.Id).Distinct().ToList();
                //If no children, remove department
                if (children.Count() == 0)
                {
                    parents.Remove(dep);
                }
                else
                {
                    //Validate whether all children are present in the departments
                    if (children.Except(departments).Count() == 0)
                    {
                        //Validate if their children have children
                        RemoveIfAllChildrenArePresentOrThereAreNoChildren(departments, cursor, children);

                        //If no children, remove department
                        if (children.Count() == 0)
                        {
                            parents.Remove(dep);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Validates whether deparment exists in the database
        /// </summary>
        /// <param name="id">Department Id</param>
        /// <returns>True if deparment exists</returns>
        internal static bool Exists(ObjectId id)
        {
            var query = Query.EQ("_id", id);

            return _coll.Count(query) > 0;
        }

        /// <summary>
        /// Retuns the name of the defined department
        /// </summary>
        /// <param name="id">Department Id</param>
        /// <returns>"Location, Name"</returns>
        internal static string GetDepartmentName(ObjectId id)
        {
            var department = _coll.FindOneById(id);
            return string.Format("{0}, {1}", department.Location, department.Name);
        }

        /// <summary>
        /// Returns the list of all departments
        /// </summary>
        /// <returns>Departments list</returns>
        public static List<Department> GetAllDepartments()
        {
            return _coll.FindAll().ToList();
        }

        /// <summary>
        /// Returns the list of departments' key and location, name
        /// </summary>
        /// <returns>Dictionary Key and Location, Name</returns>
        public static Dictionary<string, string> GetDepartmentsDictionary()
        {
            return GetAllDepartments().ToDictionary(o => o.Id.ToString(), x => string.Format("{0}, {1}", x.Location, x.Name));
        }
        
        /// <summary>
        /// Saves department or creates new if id is empty
        /// </summary>
        /// <param name="id">Department Id</param>
        /// <param name="location">Department Location Name</param>
        /// <param name="name">Department Name</param>
        /// <param name="parentId">Parent Department Id</param>
        /// <param name="parentName">Parent Department Location, Name</param>
        /// <param name="rate">Basic Rate</param>
        /// <param name="cost">Basic Rate</param>
        /// <returns>Department Edit Status</returns>
        public static DepartmentEditStatus SaveDepartment(string id, string location, string name, string parentId, string parentName, decimal rate, decimal cost)
        {
           
                if(string.IsNullOrEmpty(name))
                    return DepartmentEditStatus.ErrorNameIsEmpty;

                ObjectId departmentOf;
                if(!ObjectId.TryParse(parentId, out departmentOf) && !string.IsNullOrEmpty(parentName))
                    return  DepartmentEditStatus.ErrorParentUndefined;
                

                var department = new Department
                                     {
                                         Id = string.IsNullOrEmpty(id) ? ObjectId.GenerateNewId() : ObjectId.Parse(id),
                                         Name = name,
                                         DepartmentOf = departmentOf,
                                         Cost = cost,
                                         Rate = rate,
                                         Location = location,
                                         ParentDepartment = string.IsNullOrEmpty(parentName) ? string.Empty : parentName
                                     };
            try
            {
                _coll.Save(department, SafeMode.True);

                return DepartmentEditStatus.Ok;
            }
            catch (MongoSafeModeException ex)
            {
                if (ex.Message.Contains("duplicate")) return DepartmentEditStatus.ErrorDuplicate;
                throw;
            }
        }

        /// <summary>
        /// Deletes the list of the defined departments. Doesn't delete Departmensts that are linked to Person or that are parent.
        /// </summary>
        /// <param name="values">The list of departments' ids to be deleted.</param>
        public static void DeleteDepartments(List<string> values)
        {
            //convert ids to ObjectId
            List<ObjectId> departments = values.Where(o => !string.IsNullOrEmpty(o)).Select(ObjectId.Parse).ToList();
            if (departments.Count() <= 0) return;

            //validate department is not used on Person
            departments = departments.Except(PersonRepository.GetUsedDepartments(departments)).ToList();
            if (departments.Count() <= 0) return;

            //validate department is not parent
            departments = departments.Except(GetParentDependantDepartments(departments)).ToList();

            if (departments.Count() <= 0) return;

            var query = Query.In("_id", new BsonArray(departments));
            _coll.Remove(query);
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

        ///// <summary>
        ///// Returns the list of filtered parameters
        ///// </summary>
        ///// <param name="queries">the list of queries of kind ColumnName~value1,value2</param>
        ///// <returns>The list of departments</returns>
        //public static List<Department> GetFilteredDepartments(List<string> queries)
        //{
        //    var andQueryList = new List<IMongoQuery>();
        //    foreach (string query in queries)
        //    {
        //        var v = query.Split('~');
        //        var names = v[1].Split(',');
        //        andQueryList.Add(Query.Or(names.Select(name => Query.Matches(v[0], new BsonRegularExpression(string.Format("/^{0}/i", name)))).Cast<IMongoQuery>().ToArray()));
        //    }
        //    return _coll.Find(Query.And(andQueryList.ToArray())).ToList();
        //}

        /// <summary>
        /// Returns the string describing department
        /// </summary>
        /// <param name="id">Department Id</param>
        /// <returns>String containing: Location, Department Name (Cost, Rate)</returns>
        public static string GetDepartmentDescription(string id)
        {
            var department = _coll.FindOneById(ObjectId.Parse(id));
            if(department == null) return string.Empty;
            return string.Format("{0}, {1} (Cost: {2}, Rate: {3})", department.Location, department.Name, department.Cost, department.Rate);
        }
    }
}
