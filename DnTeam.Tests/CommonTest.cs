using System.Linq;
using DnTeamData;
using DnTeamData.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace DnTeam.Tests
{
    /// <summary>
    ///This is a test class for CommonTest and is intended
    ///to contain all CommonTest Unit Tests
    ///</summary>
    [TestClass]
    public class CommonTest
    {

        /// <summary>
        ///A test for SplitValues
        ///</summary>
        [TestMethod]
        public void SplitValuesTest()
        {
            const string values = "~value1~value2~~value3"; 
            var expected = new List<string> {"value1", "value2", "value3"} ; 
            List<string> actual = Common.SplitValues(values).ToList();

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        #region GetTypedPropertyValue tests
        
        /// <summary>
        /// DateTime
        /// </summary>
        [TestMethod]
        public void GetTypedPropertyValueDateTimeTest()
        {
            const string name = "DoB";
            string value = DateTime.Now.ToString();
            Type type = typeof(Person);
            dynamic expectedValue = DateTime.Parse(value);
            dynamic actualValue;

            var actual = Common.GetTypedPropertyValue(name, value, type, out actualValue);

            Assert.AreEqual(true, actual);
            Assert.AreEqual(expectedValue, actualValue);
        }

        /// <summary>
        /// ObjectId
        /// </summary>
        [TestMethod]
        public void GetTypedPropertyValueObjectIdTest()
        {
            const string name = "LocatedIn";
            dynamic expectedValue = ObjectId.GenerateNewId();
            string value = expectedValue.ToString();
            Type type = typeof(Person);
            dynamic actualValue;

            var actual = Common.GetTypedPropertyValue(name, value, type, out actualValue);

            Assert.AreEqual(true, actual);
            Assert.AreEqual(expectedValue, actualValue);

        }

        /// <summary>
        /// List[ObjectId]
        /// </summary>
        [TestMethod]
        public void GetTypedPropertyValueListObjectIdTest()
        {
            const string name = "OtherPeers";
            dynamic expectedValue = ObjectId.GenerateNewId();
            string value = expectedValue.ToString();
            Type type = typeof(Person);
            dynamic actualValue;

            var actual = Common.GetTypedPropertyValue(name, value, type,out actualValue, true);

            Assert.AreEqual(true, actual);
            Assert.AreEqual(expectedValue, actualValue);
        }

        /// <summary>
        /// List[string]
        /// </summary>
        [TestMethod]
        public void GetTypedPropertyValueListStringTest()
        {
            const string name = "Links";
            const string expectedValue = "link";
            Type type = typeof(Person);
            dynamic actualValue;

            var actual = Common.GetTypedPropertyValue(name, expectedValue, type, out actualValue, true);

            Assert.AreEqual(true, actual);
            Assert.AreEqual(expectedValue, actualValue);
        }


        #endregion

        /// <summary>
        ///A test for TryParseNullableDate
        ///</summary>
        [TestMethod]
        public void TryParseNullableDateTest()
        {
            //Empty string-----------//
            string text = string.Empty; 
            DateTime? date;

            bool actual = Common.TryParseNullableDate(text, out date);
            
            Assert.AreEqual(null, date);
            Assert.AreEqual(true, actual);

            //Valid DateTime-------------//
            text = DateTime.Now.ToString();
            DateTime? dateExpected = DateTime.Parse(text);
            
            actual = Common.TryParseNullableDate(text, out date);
            
            Assert.AreEqual(dateExpected, date);
            Assert.AreEqual(true, actual);

            //Invalid DateTime-------------//
            text = "foo date";

            actual = Common.TryParseNullableDate(text, out date);

            Assert.AreEqual(null, date);
            Assert.AreEqual(false, actual);
        }
    }
}
