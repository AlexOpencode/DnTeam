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
    ///This is a test class for ProductRepositoryTest and is intended
    ///to contain all ProductRepositoryTest Unit Tests
    ///</summary>
    [TestClass]
    public class ProductRepositoryTest
    {
        private const string CollectionName = "Products_Test";
        private static readonly MongoDatabase Db = Mongo.Init();
        private static readonly MongoCollection<Client> Coll = Db.GetCollection<Client>(CollectionName);

        #region Additional test attributes

        [TestInitialize]
        public void MyTestInitialize()
        {
            ProductRepository.SetTestCollection(CollectionName);
            Coll.Drop();
            Coll.EnsureIndex(new IndexKeysBuilder().Ascending("Name").Ascending("ClientId"), IndexOptions.SetUnique(true));
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            Coll.Drop();
        }

        #endregion


        /// <summary>
        ///A test for GetAllProducts
        ///</summary>
        [TestMethod]
        public void GetAllProductsTest()
        {
            var expectedProduct = new Product { ClientId = ObjectId.GenerateNewId(), Name = "Name1" };
            ProductRepository.InsertProduct(expectedProduct.Name, expectedProduct.ClientId.ToString(), false);
            
            List<Product> actual = ProductRepository.GetAllProducts();
            
            Assert.AreEqual(actual.Count(), 1);
            var actualProduct = actual.First();
            Assert.AreEqual(expectedProduct.Name, actualProduct.Name);
            Assert.AreEqual(expectedProduct.ClientId, actualProduct.ClientId);
        }

        /// <summary>
        ///A test for DeleteProducts
        ///</summary>
        [TestMethod]
        public void DeleteProductsTest()
        {
            ProductRepository.InsertProduct("Name1", ObjectId.Empty.ToString(), false);
            ProductRepository.InsertProduct("Name2", ObjectId.Empty.ToString(), false);
            ProductRepository.InsertProduct("Name3", ObjectId.Empty.ToString(), false);
            IEnumerable<string> values = ProductRepository.GetAllProducts().Select(o => o.Id.ToString()).Take(2);

            ProductRepository.DeleteProducts(values);

            var actual = ProductRepository.GetAllProducts().Count();
            Assert.AreEqual(1, actual);
        }

        /// <summary>
        ///A test for GetAllProductsDictionary
        ///</summary>
        [TestMethod]
        public void GetAllProductsDictionaryTest()
        {
            ProductRepository.InsertProduct("Name1", ObjectId.Empty.ToString(), false);
            ProductRepository.InsertProduct("Name2", ObjectId.Empty.ToString(), false);
            ProductRepository.InsertProduct("Name3", ObjectId.Empty.ToString(), false);
            var expectedNames = new List<string> { "Name1", "Name2", "Name3" };
            
            Dictionary<string, string> actual = ProductRepository.GetAllProductsDictionary();
            
            Assert.IsTrue(expectedNames.Except(actual.Select(o=>o.Value)).Count() == 0);
            Assert.IsTrue(actual.Select(o => o.Value).Except(expectedNames).Count() == 0);
            foreach (KeyValuePair<string, string> keyValuePair in actual)
            {
                ObjectId id;
                Assert.IsTrue(ObjectId.TryParse(keyValuePair.Key, out id));
            }
        }

        /// <summary>
        ///A test for InsertProduct
        ///</summary>
        [TestMethod]
        public void InsertProductTest()
        {
            const string name = "Name1"; 
            string client = ObjectId.Empty.ToString(); 
            const bool isClientNew = false;

            //TransactionStatus.Ok--------------------------//
            TransactionStatus expected = TransactionStatus.Ok;
            TransactionStatus actual = ProductRepository.InsertProduct(name, client, isClientNew);
            
            Assert.AreEqual(expected, actual);
            var products = ProductRepository.GetAllProducts();
            Assert.AreEqual(products.Count(), 1);
            Assert.AreEqual(products.First().Name,name);

            //TransactionStatus.DuplicateItem--------//
            expected = TransactionStatus.DuplicateItem;
            actual = ProductRepository.InsertProduct(name, client, isClientNew);

            Assert.AreEqual(expected, actual);
            products = ProductRepository.GetAllProducts();
            Assert.AreEqual(products.Count(), 1);
        }

        /// <summary>
        ///A test for UpdateProduct
        ///</summary>
        [TestMethod]
        public void UpdateProductTest()
        {
            const string firstName = "Name1";
            const string secondName = "Name2";
            const string otherName = "Name Other";
            ProductRepository.InsertProduct(firstName, ObjectId.Empty.ToString(), false);
            string id = ProductRepository.GetAllProducts().First().Id.ToString();
            ProductRepository.InsertProduct(secondName, ObjectId.Empty.ToString(), false);

            //TransactionStatus.Ok--------------------------//
            TransactionStatus expected = TransactionStatus.Ok;
            TransactionStatus actual = ProductRepository.UpdateProduct(id, otherName, ObjectId.Empty.ToString(), false);
            
            Assert.AreEqual(expected, actual);

            //TransactionStatus.DuplicateItem--------//
            expected = TransactionStatus.DuplicateItem;
            actual = ProductRepository.UpdateProduct(id, secondName, ObjectId.Empty.ToString(), false);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetName
        ///</summary>
        [TestMethod]
        public void GetNameTest()
        {
            ProductRepository.InsertProduct("Name1", ObjectId.Empty.ToString(), false);
            ProductRepository.InsertProduct("Name2", ObjectId.Empty.ToString(), false);
            ProductRepository.InsertProduct("Name3", ObjectId.Empty.ToString(), false);
            var product = ProductRepository.GetAllProducts().First();
            ObjectId id = product.Id;
            string expected = product.Name;
            
            string actual = ProductRepository.GetNameTest(id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetUsedClients
        ///</summary>
        [TestMethod]
        public void GetUsedClientsTest()
        {
            var expected = new List<ObjectId> { ObjectId.GenerateNewId(), ObjectId.GenerateNewId(), ObjectId.GenerateNewId() };
            ProductRepository.InsertProduct("Name1", expected[0].ToString(), false);
            ProductRepository.InsertProduct("Name2", expected[1].ToString(), false);
            ProductRepository.InsertProduct("Name3", expected[2].ToString(), false);
            
            var actual = ProductRepository.GetUsedClientsTest().ToList();
            
            Assert.IsTrue(expected.Except(actual).Count() == 0);
            Assert.IsTrue(actual.Except(expected).Count() == 0);
        }
    }
}
