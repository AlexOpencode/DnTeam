using System.Collections.Generic;
using System.Linq;
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

        #if DEBUG //Sets collection name for tests
        public static void SetTestCollection(string collectionName)
        {
            _coll = Db.GetCollection<Department>(collectionName);
        }
        #endif


        public static List<Department> GetAllDepartments()
        {
            var query = Query.EQ("IsDeleted", false);
            return _coll.Find(query).ToList();
        }

        public static Dictionary<string, string> GetDepartmentsList()
        {
            return GetAllDepartments().ToDictionary(o => o.Id.ToString(), x => x.Name);
        }

        public static IEnumerable<TDepartment> GetDepartmentsTree()
        {
            
            var departments = GetAllDepartments();
            //Get all top Departments
            var topDepartments = departments.Where(o => o.DepartmentOf == ObjectId.Empty).ToList();
            //Reduse total Departments
            departments = departments.Except(topDepartments).ToList();
            //Convert Departments to TDepartment
            var resDepartments = topDepartments.Select(o => new TDepartment
            {
                Id = o.Id.ToString(),
                Name = o.Name,
                Subsidiaries = o.Subsidiaries.Where(x=>x.IsDeleted == false).ToList()
            }).ToList();

            //Retrive sub-Departments
            GetSubDepartments(departments, resDepartments);

            return resDepartments;
        }

        private static void GetSubDepartments(List<Department> departments, List<TDepartment> resDepartments)
        {
            foreach (TDepartment department in resDepartments)
            {
                var subDepartments = departments.Where(o => o.DepartmentOf == ObjectId.Parse(department.Id)).ToList();
                if (subDepartments.Count() <= 0) continue;

                department.SubDepartments = subDepartments.Select(o => new TDepartment
                                                                              {
                                                                                  Id = o.Id.ToString(),
                                                                                  Name = o.Name,
                                                                                  Subsidiaries = o.Subsidiaries.Where(x => x.IsDeleted == false).ToList()
                                                                              }).ToList();

                departments = departments.Except(subDepartments).ToList();

                GetSubDepartments(departments, department.SubDepartments);
            }

            
        }
        
        public static bool CreateDepartment(string name, string departmentOf)
        {
           var res = _coll.Insert(new Department
                                     {
                                          Name = name,
                                          DepartmentOf = string.IsNullOrEmpty(departmentOf) ? ObjectId.Empty : ObjectId.Parse(departmentOf)
                                      }, SafeMode.True);
            return res.Ok;
        }

        //TODO: change to { $addToSet : { field : value } }
        //TODO: Validate if any user is linked to the susbsidary - prevent deletion
        public static bool CreateSubsidiary(string location, float baseCost, float baseRate, string departmentId)
        {
            var query = Query.EQ("_id", ObjectId.Parse(departmentId));
            var dep = _coll.FindOne(query);

            //Verify location is unique
            if (dep.Subsidiaries.Any(o => o.Location == location && o.IsDeleted == false))
                return false; //TODO: Return user friendly error

            dep.Subsidiaries.Add(new Subsidiary
                                    {
                                        Location = location,
                                        BaseCost = baseCost,
                                        BaseRate = baseRate
                                    });

            var update = Update.Replace(dep);
            var res = _coll.Update(query, update, SafeMode.True);

            return res.Ok;
        }

        public static bool DeleteDepartment(string id)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("IsDeleted", true);

            var res = _coll.Update(query, update, SafeMode.True);

            return res.Ok;
        }

        public static bool DeleteSubsidiary(string id, string departmentId)
        {
            var query = Query.And(new[] { Query.EQ("_id", ObjectId.Parse(departmentId)), Query.EQ("Subsidiaries._id", ObjectId.Parse(id)) });
            var update = Update.Set("Subsidiaries.$.IsDeleted", true);
           
            var res = _coll.Update(query, update, SafeMode.True);

            return res.Ok;
        }


        public static bool UpdateDepartment(string id, string name)
        {
            var query = Query.EQ("_id", ObjectId.Parse(id));
            var update = Update.Set("Name", name);

            var res = _coll.Update(query, update, SafeMode.True);

            return res.Ok;
        }

        public static bool UpdateSubsidiary(string id, string location, float baseCost, float baseRate, string departmentId)
        {
            var query = Query.And(new[] { Query.EQ("_id", ObjectId.Parse(departmentId)), Query.EQ("Subsidiaries._id", ObjectId.Parse(id)) });
            var update = Update.Set("Subsidiaries.$.Location", location).Set("Subsidiaries.$.BaseCost", baseCost).Set("Subsidiaries.$.BaseRate", baseRate);
            
            var res = _coll.Update(query, update, SafeMode.True);

            return res.Ok;
        }
    }
}
