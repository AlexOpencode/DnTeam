using System.Linq;
using DnTeamData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DnTeamData.Models;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace DnTeam.Tests
{
    
    
    /// <summary>
    ///This is a test class for PersonRepositoryTest and is intended
    ///to contain all PersonRepositoryTest Unit Tests
    ///</summary>
    [TestClass]
    public class PersonRepositoryTest
    {
        private const string CollectionName = "People_Test";
        private static readonly MongoDatabase Db = Mongo.Init();
        private static readonly MongoCollection<Client> Coll = Db.GetCollection<Client>(CollectionName);

        #region Additional test attributes

        [TestInitialize]
        public void MyTestInitialize()
        {
            PersonRepository.SetTestCollection(CollectionName);
            Coll.Drop();

            Coll.EnsureIndex(new IndexKeysBuilder().Ascending("Name"), IndexOptions.SetUnique(true));
            Coll.EnsureIndex(new IndexKeysBuilder().Ascending("IsActive"), IndexOptions.SetUnique(false));
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            Coll.Drop();
        }

        #endregion


        /// <summary>
        ///A test for GetTechnologySpecialties
        ///</summary>
        [TestMethod]
        public void GetTechnologySpecialtiesTest()
        {
            PersonRepository.CreatePerson("user1", string.Empty, string.Empty);
            var id = PersonRepository.GetAllPeople(true).First().Id.ToString();
            var expected = new List<Specialty>
                                           {
                                               new Specialty
                                                   {
                                                       FirstUsed = DateTime.Now.Date,
                                                       LastProjectNote = "note1",
                                                       LastUsed = DateTime.Now.Date,
                                                       Level = "level1",
                                                       Name = "name1"
                                                   },
                                               new Specialty
                                                   {
                                                       FirstUsed = DateTime.Now.Date,
                                                       LastProjectNote = "note2",
                                                       LastUsed = DateTime.Now.Date,
                                                       Level = "level2",
                                                       Name = "name2"
                                                   },
                                               new Specialty
                                                   {
                                                       FirstUsed = DateTime.Now.Date,
                                                       LastProjectNote = "note3",
                                                       LastUsed = DateTime.Now.Date,
                                                       Level = "level3",
                                                       Name = "name3"
                                                   }
                                           };

            foreach (var spec in expected)
                PersonRepository.CreateTechnologySpecialty(id, spec.Name, spec.Level, spec.FirstUsed.ToShortDateString(), spec.LastUsed.ToShortDateString(), spec.LastProjectNote);
            
            
            var actual = PersonRepository.GetTechnologySpecialties(id).ToList();
            
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        /// <summary>
        ///A test for AddValueToPropertySet
        ///</summary>
        [TestMethod]
        public void AddValueToPropertySetTest()
        {
            PersonRepository.CreatePerson("user1", string.Empty, string.Empty);
            var id = PersonRepository.GetAllPeople(true).First().Id.ToString();
            
            string name = "Links";
            string value = "http://www.mongodb.org/";
            const PersonEditStatus expected = PersonEditStatus.Ok;

            var actual = PersonRepository.AddValueToPropertySet(id, name, value);
            
            Assert.AreEqual(expected, actual);


            name = "LikesToWorkWith";
            value = ObjectId.GenerateNewId().ToString();

            actual = PersonRepository.AddValueToPropertySet(id, name, value);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreatePerson
        ///</summary>
        [TestMethod]
        public void CreatePersonTest()
        {
            string userName = "user1"; 
            string locatedIn = string.Empty; 
            string primaryManager = string.Empty; 

            PersonEditStatus expected = PersonEditStatus.Ok;
            PersonEditStatus actual = PersonRepository.CreatePerson(userName, locatedIn, primaryManager);
            
            Assert.AreEqual(expected, actual);


            expected = PersonEditStatus.ErrorDuplicateName;
            actual = PersonRepository.CreatePerson(userName, locatedIn, primaryManager);

            Assert.AreEqual(expected, actual);


            userName = "name2";
            locatedIn = "foo location";
            expected = PersonEditStatus.ErrorInvalidLocation;
            actual = PersonRepository.CreatePerson(userName, locatedIn, primaryManager);

            Assert.AreEqual(expected, actual);


            locatedIn = string.Empty;
            primaryManager = "foo manager";
            expected = PersonEditStatus.ErrorInvalidPrimaryManager;
            actual = PersonRepository.CreatePerson(userName, locatedIn, primaryManager);

            Assert.AreEqual(expected, actual);


            primaryManager = string.Empty;
            expected = PersonEditStatus.Ok;
            actual = PersonRepository.CreatePerson(userName, locatedIn, primaryManager);

            Assert.AreEqual(expected, actual);


            int count = PersonRepository.GetAllPeople(true).Count();
            Assert.AreEqual(2, count);
        }

        /// <summary>
        ///A test for CreateTechnologySpecialty
        ///</summary>
        [TestMethod]
        public void CreateTechnologySpecialtyTest()
        {

            PersonRepository.CreatePerson("user1", string.Empty, string.Empty);
            var id = PersonRepository.GetAllPeople(true).First().Id.ToString();
            var spec = new List<Specialty>
                                           {
                                               new Specialty
                                                   {
                                                       FirstUsed = DateTime.Now.Date,
                                                       LastProjectNote = "note1",
                                                       LastUsed = DateTime.Now.Date,
                                                       Level = "level1",
                                                       Name = "name1"
                                                   },
                                               new Specialty
                                                   {
                                                       FirstUsed = DateTime.Now.Date,
                                                       LastProjectNote = "note2",
                                                       LastUsed = DateTime.Now.Date,
                                                       Level = "level2",
                                                       Name = "name2"
                                                   },
                                               new Specialty
                                                   {
                                                       FirstUsed = DateTime.Now.Date,
                                                       LastProjectNote = "note3",
                                                       LastUsed = DateTime.Now.Date,
                                                       Level = "level3",
                                                       Name = "name2"
                                                   }
                                           };

            var expected = PersonEditStatus.Ok;
            
            var actual = PersonRepository.CreateTechnologySpecialty(id, spec[0].Name, spec[0].Level, spec[0].LastUsed.ToShortDateString(), 
                spec[0].FirstUsed.ToShortDateString(), spec[0].LastProjectNote);

            Assert.AreEqual(expected, actual);
            

            expected = PersonEditStatus.Ok;

            actual = PersonRepository.CreateTechnologySpecialty(id, spec[1].Name, spec[1].Level, spec[1].LastUsed.ToShortDateString(),
                spec[1].FirstUsed.ToShortDateString(), spec[1].LastProjectNote);

            Assert.AreEqual(expected, actual);


            expected = PersonEditStatus.ErrorDuplicateSpecialtyName;

            actual = PersonRepository.CreateTechnologySpecialty(id, spec[2].Name, spec[2].Level, spec[2].LastUsed.ToShortDateString(),
                spec[2].FirstUsed.ToShortDateString(), spec[2].LastProjectNote);

           Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DeletePeople
        ///</summary>
        [TestMethod]
        public void DeletePeopleTest()
        {
            PersonRepository.CreatePerson("user1", string.Empty, string.Empty);
            PersonRepository.CreatePerson("user2", string.Empty, string.Empty);
            PersonRepository.CreatePerson("user3", string.Empty, string.Empty);
            var values = PersonRepository.GetAllPeople(true).Select(o=>o.Id.ToString());
            const int expected = 2;

            PersonRepository.DeletePeople(values.Take(expected));

            int actual = PersonRepository.GetAllPeople(false).Count();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DeleteTechnologySpecialties
        ///</summary>
        [TestMethod]
        public void DeleteTechnologySpecialtiesTest()
        {
            PersonRepository.CreatePerson("user1", string.Empty, string.Empty);
            var id = PersonRepository.GetAllPeople(true).First().Id.ToString();
            var values = new List<Specialty>
                                           {
                                               new Specialty
                                                   {
                                                       FirstUsed = DateTime.Now.Date,
                                                       LastProjectNote = "note1",
                                                       LastUsed = DateTime.Now.Date,
                                                       Level = "level1",
                                                       Name = "name1"
                                                   },
                                               new Specialty
                                                   {
                                                       FirstUsed = DateTime.Now.Date,
                                                       LastProjectNote = "note2",
                                                       LastUsed = DateTime.Now.Date,
                                                       Level = "level2",
                                                       Name = "name2"
                                                   },
                                               new Specialty
                                                   {
                                                       FirstUsed = DateTime.Now.Date,
                                                       LastProjectNote = "note3",
                                                       LastUsed = DateTime.Now.Date,
                                                       Level = "level3",
                                                       Name = "name3"
                                                   }
                                           };

            foreach (var spec in values)
                PersonRepository.CreateTechnologySpecialty(id, spec.Name, spec.Level, spec.FirstUsed.ToShortDateString(), spec.LastUsed.ToShortDateString(), spec.LastProjectNote);


            PersonRepository.DeleteTechnologySpecialties(id, values.Select(o => o.Name).Take(2));

            var count = PersonRepository.GetTechnologySpecialties(id).Count();
            Assert.AreEqual(1, count);
        }

        /// <summary>
        ///A test for DeleteValueFromPropertySet
        ///</summary>
        [TestMethod]
        public void DeleteValueFromPropertySetTest()
        {
            PersonRepository.CreatePerson("user1", string.Empty, string.Empty);
            var id = PersonRepository.GetAllPeople(true).First().Id.ToString();
            string name = "Links";
            string value = "http://www.mongodb.org/";
            PersonRepository.AddValueToPropertySet(id, name, value);
            
            PersonRepository.DeleteValueFromPropertySet(id, name, value);

            var count = PersonRepository.GetPerson(id).Links.Count();
            Assert.AreEqual(0, count);


            name = "LikesToWorkWith";
            value = ObjectId.GenerateNewId().ToString();
            PersonRepository.AddValueToPropertySet(id, name, value);

            PersonRepository.DeleteValueFromPropertySet(id, name, value);

            count = PersonRepository.GetPerson(id).LikesToWorkWith.Count();
            Assert.AreEqual(0, count);
        }

        /// <summary>
        ///A test for DeleteValueFromPropertySet with empty value
        ///</summary>
        [TestMethod]
        public void DeleteValueFromPropertySetWithEmptyValueTest()
        {
            PersonRepository.CreatePerson("user1", string.Empty, string.Empty);
            var id = PersonRepository.GetAllPeople(true).First().Id.ToString();
            const string name = "Links";
            const string value = "http://www.mongodb.org/";
            PersonRepository.AddValueToPropertySet(id, name, value);

            PersonRepository.DeleteValueFromPropertySet(id, name, string.Empty);

            var count = PersonRepository.GetPerson(id).Links.Count();
            Assert.AreEqual(1, count);
        }
        
        /// <summary>
        ///A test for GetActivePersonsList
        ///</summary>
        [TestMethod]
        public void GetActivePersonsListTest()
        {
            PersonRepository.CreatePerson("user1", string.Empty, string.Empty);
            PersonRepository.CreatePerson("user2", string.Empty, string.Empty);
            PersonRepository.CreatePerson("user3", string.Empty, string.Empty);
            var people = PersonRepository.GetAllPeople(true);
            PersonRepository.DeletePeople(people.Where(o=>o.Name == "user1").Select(o=>o.Id.ToString()));
            Dictionary<string, string> expected = people.Where(o => o.Name != "user1").ToDictionary(o => o.Id.ToString(), x => x.Name);
            
            Dictionary<string, string> actual = PersonRepository.GetActivePersonsList();
            
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        /// <summary>
        ///A test for GetAllPeople
        ///</summary>
        [TestMethod]
        public void GetAllPeopleTest()
        {
            PersonRepository.CreatePerson("user1", string.Empty, string.Empty);
            PersonRepository.CreatePerson("user2", string.Empty, string.Empty);
            PersonRepository.CreatePerson("user3", string.Empty, string.Empty);
            var people = PersonRepository.GetAllPeople(true);
            
            PersonRepository.DeletePeople(people.Where(o => o.Name == "user1").Select(o => o.Id.ToString()));

            var actual = PersonRepository.GetAllPeople(true).Count();
            Assert.AreEqual(2, actual);

            actual = PersonRepository.GetAllPeople(false).Count();
            Assert.AreEqual(1, actual);
        }

        /// <summary>
        ///A test for GetPerson
        ///</summary>
        [TestMethod]
        public void GetPersonTest()
        {
            PersonRepository.CreatePerson("user1", string.Empty, string.Empty);
            var people = PersonRepository.GetAllPeople(true);

            Person expected = people[0];
            Person actual = PersonRepository.GetPerson(people[0].Id.ToString());

            Assert.AreEqual(expected.Id, actual.Id);
        }

        /// <summary>
        ///A test for GetPersonName
        ///</summary>
        [TestMethod]
        public void GetPersonNameTest()
        {
            const string expected = "user1";
            PersonRepository.CreatePerson(expected, string.Empty, string.Empty);
            var people = PersonRepository.GetAllPeople(true);

            string actual = PersonRepository.GetPersonName(people[0].Id.ToString());
            
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateProperty
        ///</summary>
        [TestMethod]
        public void UpdatePropertyTest()
        {
            const string name = "LocatedIn";
            string value = ObjectId.GenerateNewId().ToString();
            PersonRepository.CreatePerson("user1", string.Empty, string.Empty);
            var id = PersonRepository.GetAllPeople(true).First().Id.ToString();
            const PersonEditStatus expected = PersonEditStatus.Ok;
            
            var actual = PersonRepository.UpdateProperty(id, name, value);
            
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateTechnologySpecialty
        ///</summary>
        [TestMethod]
        public void UpdateTechnologySpecialtyTest()
        {
            PersonRepository.CreatePerson("name1", string.Empty, string.Empty);
            var id = PersonRepository.GetAllPeople(true).First().Id.ToString();
            
            const string name = "sname1";
            string value = "value1";
            string lastUsed = DateTime.Now.ToString();
            string firstUsed = DateTime.Now.ToString();
            string note = "note1";
            PersonRepository.CreateTechnologySpecialty(id, name, value, lastUsed, firstUsed, note);

            //PersonEditStatus.ErrorDateIsNotValid//
            lastUsed = "foo date";
            var expected = PersonEditStatus.ErrorDateIsNotValid;

            var actual = PersonRepository.UpdateTechnologySpecialty(id, name, value, lastUsed, firstUsed, note);
            
            Assert.AreEqual(expected, actual);

            //PersonEditStatus.ErrorDateIsNotValid//
            lastUsed = DateTime.Now.ToString();
            firstUsed = "foo date";

            actual = PersonRepository.UpdateTechnologySpecialty(id, name, value, lastUsed, firstUsed, note);

            Assert.AreEqual(expected, actual);


            //PersonEditStatus.Ok------------//
            value = "value2";
            lastUsed = DateTime.Now.ToString();
            firstUsed = DateTime.Now.ToString();
            note = "note2";
            expected = PersonEditStatus.Ok;

            actual = PersonRepository.UpdateTechnologySpecialty(id, name, value, lastUsed, firstUsed, note);
            
            Assert.AreEqual(expected, actual);
            var actulalSpecialty = PersonRepository.GetAllPeople(true).First().TechnologySpecialties.First();
            Assert.AreEqual(value, actulalSpecialty.Level);
            Assert.AreEqual(lastUsed, actulalSpecialty.LastUsed.ToString());
            Assert.AreEqual(firstUsed, actulalSpecialty.FirstUsed.ToString());
            Assert.AreEqual(note, actulalSpecialty.LastProjectNote);
        }

        /// <summary>
        ///A test for ValidateIdentifier
        ///</summary>
        [TestMethod]
        public void ValidateIdentifierTest()
        {
            const string expected = "name1";
            string openId = "test.myopenid.com";
            PersonRepository.CreatePerson(expected, string.Empty, string.Empty);
            var id = PersonRepository.GetAllPeople(true).First().Id.ToString();
            PersonRepository.UpdateProperty(id, "OpenId", openId);

            string actual = PersonRepository.ValidateIdentifier(openId);

            Assert.AreEqual(expected, actual);
            
            openId = "foo openId";
            actual = PersonRepository.ValidateIdentifier(openId);

            Assert.IsTrue(string.IsNullOrEmpty(actual));
        }
    }
}
