using System.Linq;
using DnTeamData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DnTeamData.Models;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DnTeam.Tests
{
    public class Enum
    {
        [BsonId]
        public string Name { set; get; }
    }

    /// <summary>
    ///This is a test class for SettingsRepositoryTest and is intended
    ///to contain all SettingsRepositoryTest Unit Tests
    ///</summary>
    [TestClass]
    public class SettingsRepositoryTest
    {
        private const string CollectionName = "Enums_Test";

        #region Additional test attributes

        [TestInitialize]
        public void MyTestInitialize()
        {
            SettingsRepository.SetTestCollection(CollectionName);
            MongoDatabase db = Mongo.Init();
            MongoCollection<Enums> coll = db.GetCollection<Enums>(CollectionName);
            coll.Drop();

            var batch = new List<Enums>
                                    {
                                        new Enums {Name = EnumName.Locations.ToString()},
                                        new Enums {Name =  EnumName.ProjectRoles.ToString() },
                                        new Enums {Name =  EnumName.ProjectStatuses.ToString() },
                                        new Enums {Name =  EnumName.TechnologySpecialtyNames.ToString()},
                                        new Enums {Name =  EnumName.ProjectTypes.ToString() },
                                        new Enums {Name =  EnumName.ProjectMilestones.ToString() },
                                        new Enums {Name =  EnumName.ProjectNoiseTypes.ToString() },
                                        new Enums {Name =  EnumName.ProjectPriorityTypes.ToString()},
                                        new Enums {Name =  EnumName.TechnologySpecialtyLevels.ToString() }
                                    };

            coll.InsertBatch(batch);
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            MongoDatabase db = Mongo.Init();
            MongoCollection<Enums> coll = db.GetCollection<Enums>(CollectionName);
            coll.Drop();
        }

        #endregion


        /// <summary>
        ///A test for GetEnumName
        ///</summary>
        [TestMethod]
        public void GetEnumNameTest()
        {
            const string value = "Project Milestones";
            const EnumName expected = EnumName.ProjectMilestones;
            EnumName actual = SettingsRepository.GetEnumName(value);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetEnumNameWithUndefined value
        ///</summary>
        [TestMethod]
        public void GetUndefinedEnumNameTest()
        {
            const string value = "Undefined Setting";
            const EnumName expected = EnumName.Undefined;
            EnumName actual = SettingsRepository.GetEnumName(value);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BatchDeleteSettingValues
        ///</summary>
        [TestMethod]
        public void BatchDeleteSettingValuesTest()
        {
            const EnumName name = EnumName.Locations;
            IEnumerable<string> values = new List<string> { "value 1", "value 2", "value 3" };
            SettingsRepository.BatchAddSettingValues(name, values);

            SettingsRepository.BatchDeleteSettingValues(name, values);

            var actual = SettingsRepository.GetSettingValues(name);
            Assert.IsTrue(actual.Count() == 0);
        }

        /// <summary>
        ///A test for BatchAddSettingValues
        ///</summary>
        [TestMethod]
        public void BatchAddSettingValuesTest()
        {
            const EnumName name = EnumName.Locations;
            IEnumerable<string> values = new List<string> { "value 1", "value 2", "value 3", "value 3" };

            SettingsRepository.BatchAddSettingValues(name, values);

            var actual = SettingsRepository.GetSettingValues(name);
            Assert.IsTrue(actual.Count() == 3);
            Assert.IsTrue(actual.Except(values.Distinct()).Count() == 0);
            Assert.IsTrue(values.Distinct().Except(actual).Count() == 0);
        }

        /// <summary>
        ///A test for AddSettingValue
        ///</summary>
        [TestMethod]
        public void AddSettingValueTest()
        {
            const EnumName name = EnumName.Locations;
            const string expected = "test_value";
            SettingsRepository.AddSettingValue(name, expected);

            string actual = SettingsRepository.GetSettingValues(name).First();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetSettingValues
        ///</summary>
        [TestMethod]
        public void GetSettingValuesTest()
        {
            const EnumName name = EnumName.Locations;
            const string expected = "test_value";
            SettingsRepository.AddSettingValue(name, expected);

            List<string> actual = SettingsRepository.GetSettingValues(name);

            Assert.AreEqual(expected, actual.First());
            Assert.IsTrue(actual.Count() == 1);
        }
    }
}
