using System.ComponentModel;
using System.Linq;
using DnTeamData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DnTeam.Tests
{
    /// <summary>
    ///This is a test class for CommonTest and is intended
    ///to contain all CommonTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CommonTest
    {
        /// <summary>
        ///A test for SplitValues
        ///</summary>
        [TestMethod()]
        public void SplitValuesTest()
        {
            const string values = "~value1~value2~~value3"; 
            var expected = new List<string> {"value1", "value2", "value3"} ; 
            List<string> actual = Common.SplitValues(values).ToList();

            Assert.IsTrue(expected.Except(actual).Count() == 0);
            Assert.IsTrue(actual.Except(expected).Count() == 0);
        }
    }
}
