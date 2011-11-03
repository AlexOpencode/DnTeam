using System.Linq;
using DnTeamData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DnTeamData.Models;
using System.Collections.Generic;
using MongoDB.Driver;

namespace DnTeam.Tests
{
    
    /// <summary>
    ///This is a test class for DepartamentRepositoryTest and is intended
    ///to contain all DepartamentRepositoryTest Unit Tests
    ///</summary>
    [TestClass]
    public class DepartamentRepositoryTest
    {
        private const string CollectionName = "Departaments_Test";

        #region Additional test attributes

        [TestInitialize]
        public void MyTestInitialize()
        {
            DepartamentRepository.SetTestCollection(CollectionName);
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            MongoDatabase db = Mongo.Init();
            MongoCollection<Departament> coll = db.GetCollection<Departament>(CollectionName);
            coll.Drop();
        }

        #endregion

        /// <summary>
        ///A test for CreateDepartament
        ///</summary>
        [TestMethod]
        public void CreateDepartamentTest()
        {
            const string name = "Test_Departament"; 
            string parentDepartamentId = string.Empty; 
            DepartamentRepository.CreateDepartament(name, parentDepartamentId);
        }

        /// <summary>
        ///A test for GetAllDepartaments
        ///</summary>
        [TestMethod]
        public void GetAllDepartamentsTest()
        {
            const int expectedCount = 10;

            #region Create test departaments
            for (int i = 0; i < expectedCount; i++)
            {
                string name = "Test_Departament" + i;
                string parentDepartamentId = string.Empty;
                DepartamentRepository.CreateDepartament(name, parentDepartamentId);


            }
            #endregion

            var actual = DepartamentRepository.GetAllDepartaments();
            Assert.AreEqual(expectedCount, actual.Count());
        }


        /// <summary>
        ///A test for GetDepartamentsTree
        ///</summary>
        [TestMethod]
        public void GetDepartamentsTreeTest()
        {
            const int expectedCount = 10;

            #region Create test departaments
            for (int i = 0; i < expectedCount; i++)
            {
                string name = "Test_Departament" + i;
                string parentDepartamentId = string.Empty;
                DepartamentRepository.CreateDepartament(name, parentDepartamentId);

                
            }

            var created = DepartamentRepository.GetAllDepartaments();

            foreach (Departament departament in created)
            {
                for (int j = 0; j < 10; j++)
                {
                    string subName = "Test_SubDepartament" + j + "Of" + departament.Id;
                    string subParentDepartamentId = departament.Id.ToString();
                    DepartamentRepository.CreateDepartament(subName, subParentDepartamentId);
                }
            }
            #endregion 

            IEnumerable<TDepartament> actual = DepartamentRepository.GetDepartamentsTree();
            Assert.AreEqual(expectedCount, actual.Count());
        }

        
    }
}
