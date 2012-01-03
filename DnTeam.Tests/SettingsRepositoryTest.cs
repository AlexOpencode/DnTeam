using System.Linq;
using DnTeamData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DnTeamData.Models;
using System.Collections.Generic;
using MongoDB.Driver;

namespace DnTeam.Tests
{

    /// <summary>
    ///This is a test class for SettingsRepositoryTest and is intended
    ///to contain all SettingsRepositoryTest Unit Tests
    ///</summary>
    [TestClass]
    public class SettingsRepositoryTest
    {
        private const string CollectionName = "Enums_Test";
        private static readonly MongoDatabase Db = Mongo.Init();
        private static readonly MongoCollection<Client> Coll = Db.GetCollection<Client>(CollectionName);

        #region Additional test attributes

        [TestInitialize]
        public void MyTestInitialize()
        {
            SettingsRepository.SetTestCollection(CollectionName);
            Coll.Drop();

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

            Coll.InsertBatch(batch);
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            Coll.Drop();
        }

        #endregion

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
        ///A test for: while submiting empty list, shouldn't drop all records
        ///</summary>
        [TestMethod]
        public void BatchDeleteSettingValuesWithEmptyListTest()
        {
            const EnumName name = EnumName.Locations;
            IEnumerable<string> values = new List<string> { "value 1", "value 2", "value 3" };
            SettingsRepository.BatchAddSettingValues(name, values);

            SettingsRepository.BatchDeleteSettingValues(name, new List<string>());

            var actual = SettingsRepository.GetSettingValues(name);
            Assert.IsTrue(actual.Count() == 3);
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
            Assert.IsTrue(actual.SequenceEqual(values.Distinct()));
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
