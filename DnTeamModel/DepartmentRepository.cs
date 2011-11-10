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
                Subsidiaries = o.Subsidiaries
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
                                                                                  Subsidiaries = o.Subsidiaries
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

        //TODO: Validate if any user is linked to the susbsidary - prevent deletion
        //TODO: Validate if element with such location exists in the given departament
        public static bool CreateSubsidiary(string location, float baseCost, float baseRate, string departmentId)
        {
            var query = Query.EQ("_id", ObjectId.Parse(departmentId));
            var update = Update.AddToSet("Subsidiaries", new Subsidiary
                                                             {
                                                                 Location = location,
                                                                 BaseCost = baseCost,
                                                                 BaseRate = baseRate
                                                             }.ToBsonDocument());

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

            using (Db.Server.RequestStart(Db))
            {
                var update = Update.Pull("Subsidiaries", Query.EQ("_id", ObjectId.Parse(id)));
                var res = _coll.Update(Query.EQ("_id", ObjectId.Parse(departmentId)), update, SafeMode.True);
                if(res.Ok)
                {
                    var query = Query.EQ("LocatedIn", ObjectId.Parse(id));
                    update = Update.Set("LocatedIn", ObjectId.Empty);
                    res = _personColl.Update(query, update, SafeMode.True);

                    return res.Ok;
                }
            }

            return false;
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

        public static Dictionary<string, string> GetLocationsList()
        {
            var res = new Dictionary<string, string>();
            var query = Query.Where("(this.Subsidiaries.length > 0)");
            var departments = _coll.Find(query);
            foreach (Department department in departments)
            {
                foreach (Subsidiary subsidiary in department.Subsidiaries)
                {
                    res.Add(subsidiary.SubsidiaryId, string.Format("{0}, {1}", department.Name, subsidiary.Location));
                }
            }

            return res;
        }

        internal static string GetLocationName(ObjectId locatedIn)
        {
            var query = Query.EQ("Subsidiaries._id", locatedIn);
            var departament = _coll.FindOne(query);

            return string.Format("{0}, {1}", departament.Name, departament.Subsidiaries.Single(o => o.Id == locatedIn).Location);
        }
    }
}
