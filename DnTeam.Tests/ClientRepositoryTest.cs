using System.Linq;
using DnTeamData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DnTeamData.Models;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeam.Tests
{
    
    
    /// <summary>
    ///This is a test class for ClientRepositoryTest and is intended
    ///to contain all ClientRepositoryTest Unit Tests
    ///</summary>
    [TestClass]
    public class ClientRepositoryTest
    {

        private const string CollectionName = "Clients_Test";
        private static readonly MongoDatabase Db = Mongo.Init();
        private static readonly MongoCollection<Client> Coll = Db.GetCollection<Client>(CollectionName);
        
        #region Additional test attributes

        [TestInitialize]
        public void MyTestInitialize()
        {
            ClientRepository.SetTestCollection(CollectionName);
            Coll.Drop();
            Coll.EnsureIndex(new IndexKeysBuilder().Ascending("Name"), IndexOptions.SetUnique(true));
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            Coll.Drop();
        }

        #endregion

        /// <summary>
        ///A test for GetAllClients
        ///</summary>
        [TestMethod]
        public void GetAllClientsTest()
        {
            var expected = new List<string> { "Name1", "Name2", "Name3" };
            ClientRepository.InsertClients(expected);

            List<Client> actual = ClientRepository.GetAllClients().ToList();
            
            Assert.IsTrue(expected.Except(actual.Select(o=>o.Name)).Count() == 0);
            Assert.IsTrue(actual.Select(o => o.Name).Except(expected).Count() == 0);
            
        }

        /// <summary>
        ///A test for DeleteClients
        ///</summary>
        [TestMethod]
        public void DeleteClientsTest()
        {
            var values = new List<string> { "Name1", "Name2", "Name3" };
            ClientRepository.InsertClients(values);
            var clients = ClientRepository.GetAllClients();

            ClientRepository.DeleteClients(clients.Select(o=>o.Id.ToString()));

            long actual = ClientRepository.GetAllClients().Count();
            Assert.IsTrue(actual == 0);
        }

        /// <summary>
        ///A test for DeleteClients with empty list
        ///</summary>
        [TestMethod]
        public void DeleteClientsWithEmpltyListTest()
        {
            var values = new List<string> { "Name1", "Name2", "Name3" };
            ClientRepository.InsertClients(values);
            
            ClientRepository.DeleteClients(new List<string>());

            long actual = ClientRepository.GetAllClients().Count();
            Assert.IsTrue(actual == 3);
        }

        /// <summary>
        ///A test for GetClientsDictionary
        ///</summary>
        [TestMethod]
        public void GetClientsDictionaryTest()
        {
            var expected = new List<string> { "Name1", "Name2", "Name3" };
            ClientRepository.InsertClients(expected);
            
            Dictionary<string, string> actual = ClientRepository.GetClientsDictionary();


            Assert.IsTrue(expected.Except(actual.Select(o => o.Value)).Count() == 0);
            Assert.IsTrue(actual.Select(o => o.Value).Except(expected).Count() == 0);
            foreach (KeyValuePair<string, string> keyValuePair in actual)
            {
                ObjectId id;
                Assert.IsTrue(ObjectId.TryParse(keyValuePair.Key, out id));
            }
        }

        /// <summary>
        ///A test for GetName
        ///</summary>
        [TestMethod]
        public void GetNameTest()
        {
            var expected = new Client { Id = ObjectId.GenerateNewId(), Name = "Name1"};
            ClientRepository.InsertTestClient(expected);

            string actual = ClientRepository.GetName(expected.Id);
            
            Assert.AreEqual(expected.Name, actual);
        }

        /// <summary>
        ///A test for InsertClients
        ///</summary>
        [TestMethod]
        public void InsertClientsTest()
        {
            var values = new List<string> { "client 1", "client 2", "client 1", "client 3"}; 
            
            ClientRepository.InsertClients(values);

            var actual = ClientRepository.GetAllClients().ToList();
            Assert.IsTrue(values.Distinct().Except(actual.Select(o => o.Name)).Count() == 0);
            Assert.IsTrue(actual.Select(o => o.Name).Except(values.Distinct()).Count() == 0);
        }

        /// <summary>
        ///A test for UpdateClient
        ///</summary>
        [TestMethod]
        public void UpdateClientTest()
        {
            var client = new Client { Id = ObjectId.GenerateNewId(), Name = "Name1" };
            ClientRepository.InsertTestClient(client);
            const string expectedName = "Name2";

            //TransactionStatus.Ok------------------//
            var expectedStatus = TransactionStatus.Ok;
            TransactionStatus actualStatus = ClientRepository.UpdateClient(client.Id.ToString(), expectedName);

            Assert.AreEqual(expectedStatus, actualStatus);
            var actualName = ClientRepository.GetName(client.Id);
            Assert.AreEqual(expectedName, actualName);

            //TransactionStatus.DuplicateName--------------//
            expectedStatus = TransactionStatus.DuplicateName;
            const string duplicateName = "Name1";
            ClientRepository.InsertTestClient(new Client { Id = ObjectId.GenerateNewId(), Name = duplicateName });

            actualStatus = ClientRepository.UpdateClient(client.Id.ToString(), duplicateName);
            Assert.AreEqual(expectedStatus, actualStatus);
        }
    }
}
