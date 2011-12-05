using System;
using System.Collections.Generic;
using System.Linq;

namespace DnTeamData
{
    /// <summary>
    /// A static class containing common actions
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// Splits string of values separated by "~"
        /// </summary>
        /// <param name="values">Values string</param>
        /// <returns>The list of values</returns>
        public static IEnumerable<string> SplitValues(string values)
        {
            return values.Split('~').Where(o => !String.IsNullOrEmpty(o)).Select(o => o.Trim());
        }
    }
}
