using System.Linq;
using DnTeamData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DnTeamData.Models;
using System.Collections.Generic;
using MongoDB.Driver;

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
        private static readonly MongoCollection<Client> Coll = Db.GetCollection<Client>(CollectionName);

        #region Additional test attributes

        [TestInitialize]
        public void MyTestInitialize()
        {
            DepartmentRepository.SetTestCollection(CollectionName);
            Coll.Drop();
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            Coll.Drop();
        }

        #endregion

        /// <summary>
        ///A test for CreateDepartment
        ///</summary>
        [TestMethod]
        public void CreateDepartmentTest()
        {
            const string name = "Test_Department";
            const string location = "Test_Location";
            const decimal rate = 10;
            const decimal cost = 10;
            DepartmentRepository.SaveDepartment(string.Empty, location, name, string.Empty, string.Empty, rate, cost);
        }

        /// <summary>
        ///A test for GetAllDepartments
        ///</summary>
        [TestMethod]
        public void GetAllDepartmentsTest()
        {
            const int expectedCount = 10;

            #region Create test Departments
            for (int i = 0; i < expectedCount; i++)
            {
                string name = "Test_Department" + i;
                const string location = "Test_Location";
                const decimal rate = 10;
                const decimal cost = 10;
                DepartmentRepository.SaveDepartment(string.Empty, location, name, string.Empty, string.Empty, rate, cost);


            }
            #endregion

            var actual = DepartmentRepository.GetAllDepartments();
            Assert.AreEqual(expectedCount, actual.Count());
        }
     
    }
}
