using System.Linq;
using DnTeamData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MongoDB.Bson;
using DnTeamData.Models;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeam.Tests
{
    
    /// <summary>
    ///This is a test class for DepartmentRepositoryTest and is intended
    ///to contain all DepartmentRepositoryTest Unit Tests
    ///</summary>
    [TestClass]
    public class DepartmentRepositoryTest
    {

        private const string CollectionName = "Departments_Test";
        private static readonly MongoDatabase Db = Mongo.Init();
        private static readonly MongoCollection<Department> Coll = Db.GetCollection<Department>(CollectionName);

        #region Additional test attributes

        [TestInitialize]
        public void MyTestInitialize()
        {
            DepartmentRepository.SetTestCollection(CollectionName);
            Coll.Drop();
            Coll.EnsureIndex(new IndexKeysBuilder().Ascending("Name").Ascending("Location"), IndexOptions.SetUnique(true));
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            Coll.Drop();
        }

        #endregion

        /// <summary>
        ///A test for GetDepartmentsDictionary
        ///</summary>
        [TestMethod]
        public void GetDepartmentsDictionaryTest()
        {
            DepartmentRepository.SaveDepartment(string.Empty, "location1", "name1", string.Empty, string.Empty, 11.11m, 11.11m);
            DepartmentRepository.SaveDepartment(string.Empty, "location2", "name2", string.Empty, string.Empty, 11.11m, 11.11m);
            var expected = new List<string> { "location1, name1", "location2, name2" };
            
            Dictionary<string, string> actual = DepartmentRepository.GetDepartmentsDictionary();
            
            Assert.IsTrue(expected.Except(actual.Select(o=>o.Value)).Count() == 0);
            Assert.IsTrue(actual.Select(o => o.Value).Except(expected).Count() == 0);
            foreach (KeyValuePair<string, string> keyValuePair in actual)
            {
                ObjectId id;
                Assert.IsTrue(ObjectId.TryParse(keyValuePair.Key, out id));
            }
        }

        /// <summary>
        ///A test for GetDepartmentNameTest
        ///</summary>
        [TestMethod]
        public void GetDepartmentNameTestTest()
        {
            ObjectId id = ObjectId.GenerateNewId();
            var values = new [] {"location1", "Name1" };
            DepartmentRepository.SaveDepartment(id.ToString(), values[0], values[1], string.Empty, string.Empty, 11.11m, 11.11m);
            DepartmentRepository.SaveDepartment(string.Empty, "location2", "name2", string.Empty, string.Empty, 11.11m, 11.11m);
            string expected = string.Format("{0}, {1}", values[0], values[1]);

            string actual = DepartmentRepository.GetDepartmentNameTest(id);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetDepartmentDescription
        ///</summary>
        [TestMethod]
        public void GetDepartmentDescriptionTest()
        {
            ObjectId id = ObjectId.GenerateNewId();
            var values = new [] { "location1", "Name1", "22.2", "33.3" };
            DepartmentRepository.SaveDepartment(id.ToString(), values[0], values[1], string.Empty, string.Empty, decimal.Parse(values[2]), decimal.Parse(values[3]));
            DepartmentRepository.SaveDepartment(string.Empty, "location2", "name2", string.Empty, string.Empty, 11.11m, 11.11m);
            string expected = string.Format("{0}, {1} (Cost: {2}, Rate: {3})", values[0], values[1], decimal.Parse(values[3]), decimal.Parse(values[2]));

            string actual = DepartmentRepository.GetDepartmentDescription(id.ToString());
            
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetAllDepartments
        ///</summary>
        [TestMethod]
        public void GetAllDepartmentsTest()
        {
            var expected = new List<Department>
            {
                new Department { Id = ObjectId.GenerateNewId(), Location = "location1", Name = "name1", DepartmentOf = ObjectId.GenerateNewId(),
                    ParentDepartment = string.Empty, Cost = 11.1m, Rate = 22.1m},
                new Department { Id = ObjectId.GenerateNewId(), Location = "location2", Name = "name2", DepartmentOf = ObjectId.GenerateNewId(), 
                    ParentDepartment = string.Empty, Cost = 11.2m, Rate = 22.2m},
                new Department { Id = ObjectId.GenerateNewId(), Location = "location2", Name = "name3", DepartmentOf = ObjectId.GenerateNewId(), 
                    ParentDepartment = string.Empty, Cost = 11.3m, Rate = 22.3m}
            };
            foreach (Department dep in expected)
            {
                DepartmentRepository.SaveDepartment(dep.Id.ToString(), dep.Location, dep.Name, dep.DepartmentOf.ToString(), string.Empty, dep.Rate, dep.Cost);    
            }
            
            var actual = DepartmentRepository.GetAllDepartments();

            Assert.IsTrue(expected.SequenceEqual(actual)); 
        }

        /// <summary>
        ///A test for Exists
        ///</summary>
        [TestMethod]
        public void ExistsTest()
        {
            ObjectId id = ObjectId.GenerateNewId();
            DepartmentRepository.SaveDepartment(id.ToString(), "location1", "name1", string.Empty, string.Empty, 11.11m, 11.11m);
            
            Assert.IsTrue(DepartmentRepository.ExistsTest(id));
            Assert.IsFalse(DepartmentRepository.ExistsTest(ObjectId.GenerateNewId()));
        }

        /// <summary>
        ///A test for DeleteDepartments
        ///</summary>
        [TestMethod]
        public void DeleteDepartmentsTest()
        {
            ObjectId id1 = ObjectId.GenerateNewId();
            ObjectId id2 = ObjectId.GenerateNewId();
            DepartmentRepository.SaveDepartment(id1.ToString(), "location1", "name1", string.Empty, string.Empty, 11.11m, 11.11m);
            DepartmentRepository.SaveDepartment(id2.ToString(), "location2", "name2", string.Empty, string.Empty, 11.11m, 11.11m);
            DepartmentRepository.SaveDepartment(string.Empty, "location3", "name3", id1.ToString(), string.Empty, 11.11m, 11.11m);

            DepartmentRepository.DeleteDepartments(new List<string> { id1.ToString(), id2.ToString() });

            var actual = DepartmentRepository.GetAllDepartments().Count();
            Assert.AreEqual(2, actual);
        }

        ///// <summary>
        /////A test for GetFilteredDepartments
        /////</summary>
        //[TestMethod]
        //public void GetFilteredDepartmentsTest()
        //{
        //    var querys = new List<string> { "Location~location1,location2", "Name~name1,name3" };
        //    var expected = new List<ObjectId> {ObjectId.GenerateNewId()};
        //    DepartmentRepository.SaveDepartment(expected[0].ToString(), "location1", "name1", string.Empty, string.Empty, 11.11m, 11.11m);
        //    DepartmentRepository.SaveDepartment(ObjectId.GenerateNewId().ToString(), "location2", "name2", string.Empty, string.Empty, 11.11m, 11.11m);
        //    DepartmentRepository.SaveDepartment(ObjectId.GenerateNewId().ToString(), "location3", "name3", string.Empty, string.Empty, 11.11m, 11.11m);

        //    var actual = DepartmentRepository.GetFilteredDepartments(querys);
            
        //    Assert.IsTrue(expected.SequenceEqual(actual.Select(o=>o.Id)));
        //}

        /// <summary>
        ///A test for GetParentDependantDepartments
        ///</summary>
        [TestMethod]
        public void GetParentDependantDepartmentsTest()
        {
            var id1 = ObjectId.GenerateNewId();
            var id2 = ObjectId.GenerateNewId();
            var id3 = ObjectId.GenerateNewId();
            var id4 = ObjectId.GenerateNewId();
            var id5 = ObjectId.GenerateNewId();
            DepartmentRepository.SaveDepartment(id1.ToString(), "location1", "name1", string.Empty, string.Empty, 11.11m, 11.11m);
            DepartmentRepository.SaveDepartment(id2.ToString(), "location2", "name2", id1.ToString(), string.Empty, 11.11m, 11.11m);
            DepartmentRepository.SaveDepartment(id3.ToString(), "location3", "name3", id2.ToString(), string.Empty, 11.11m, 11.11m);
            DepartmentRepository.SaveDepartment(id4.ToString(), "location4", "name4", string.Empty, string.Empty, 11.11m, 11.11m);
            DepartmentRepository.SaveDepartment(id5.ToString(), "location5", "name5", id4.ToString(), string.Empty, 11.11m, 11.11m);
            var departments = new List<ObjectId> {id1, id2, id3, id4};
            var expected = new List<ObjectId> { id4 };
            
            var actual = DepartmentRepository.GetParentDependantDepartmentsTest(departments);
            
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        /// <summary>
        ///A test for GetTopSearchOffers
        ///</summary>
        [TestMethod]
        public void GetTopSearchOffersTest()
        {
            DepartmentRepository.SaveDepartment(ObjectId.GenerateNewId().ToString(), "location1", "name1", string.Empty, string.Empty, 11.11m, 11.11m);
            DepartmentRepository.SaveDepartment(ObjectId.GenerateNewId().ToString(), "location2", "name2", string.Empty, string.Empty, 11.11m, 11.11m);
            DepartmentRepository.SaveDepartment(ObjectId.GenerateNewId().ToString(), "location3", "not name", string.Empty, string.Empty, 11.11m, 11.11m);
            const string column = "Name";
            const string query = "name";
            IEnumerable<string> expected = new List<string> { "name1", "name2"} ;
            
            IEnumerable<string> actual  = DepartmentRepository.GetTopSearchOffers(column, query);
            
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        /// <summary>
        ///A test for SaveDepartment
        ///</summary>
        [TestMethod]
        public void SaveDepartmentTest()
        {
            const string location = "location1";
            const string name = "name1";
            const string emptyName = "";
            string emptyParentName = string.Empty;
            const string parentName = "parent";

            DepartmentEditStatus expected = DepartmentEditStatus.Ok;
            DepartmentEditStatus actual = DepartmentRepository.SaveDepartment(ObjectId.GenerateNewId().ToString(), location, name, string.Empty, emptyParentName, 11.11m, 11.11m);
            Assert.AreEqual(expected, actual);

            expected = DepartmentEditStatus.ErrorDuplicate;
            actual = DepartmentRepository.SaveDepartment(ObjectId.GenerateNewId().ToString(), location, name, string.Empty, emptyParentName, 11.11m, 11.11m);
            Assert.AreEqual(expected, actual);

            
            expected = DepartmentEditStatus.ErrorNoName;
            actual = DepartmentRepository.SaveDepartment(ObjectId.GenerateNewId().ToString(), location, emptyName, string.Empty, emptyParentName, 11.11m, 11.11m);
            Assert.AreEqual(expected, actual);

            
            expected = DepartmentEditStatus.ErrorParentUndefined;
            actual = DepartmentRepository.SaveDepartment(ObjectId.GenerateNewId().ToString(), location, name, string.Empty, parentName, 11.11m, 11.11m);
            Assert.AreEqual(expected, actual);
        }
    }
}
