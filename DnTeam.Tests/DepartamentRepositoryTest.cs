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

        #region Additional test attributes

        [TestInitialize]
        public void MyTestInitialize()
        {
            DepartmentRepository.SetTestDepartmentCollection(CollectionName);
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            MongoDatabase db = Mongo.Init();
            MongoCollection<Department> coll = db.GetCollection<Department>(CollectionName);
            coll.Drop();
        }

        #endregion

        /// <summary>
        ///A test for CreateDepartment
        ///</summary>
        [TestMethod]
        public void CreateDepartmentTest()
        {
            const string name = "Test_Department"; 
            string parentDepartmentId = string.Empty; 
            DepartmentRepository.CreateDepartment(name, parentDepartmentId);
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
                string parentDepartmentId = string.Empty;
                DepartmentRepository.CreateDepartment(name, parentDepartmentId);


            }
            #endregion

            var actual = DepartmentRepository.GetAllDepartments();
            Assert.AreEqual(expectedCount, actual.Count());
        }
     
    }
}
