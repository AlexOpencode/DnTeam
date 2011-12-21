using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DnTeam
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Converts dictionary to select list and sets default values
        /// </summary>
        /// <param name="dictionary" type="">IEnumerable&lt;KeyValuePair&lt;string,string&gt;&gt; dictionary</param>
        /// <param name="value">Default value</param>
        /// <param name="text">Default text</param>
        /// <returns>SelectList</returns>
        public static SelectList ToSelectList(this IEnumerable<KeyValuePair<string,string>> dictionary, string value, string text)
        {
            var selectlist = new List<SelectListItem> {new SelectListItem { Text = text, Value = value }};
            selectlist.AddRange(dictionary.Select(d => new SelectListItem { Text = d.Value, Value = d.Key }));

            return new SelectList(selectlist, "value", "text");
        }

        /// <summary>
        /// Filters list
        /// </summary>
        /// <param name="value">The list to be filtered</param>
        /// <param name="filterQuery">The list of queries of kind: "PropertyName~FilterValue"</param>
        /// <returns>Filtered list</returns>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> value, List<string> filterQuery)
        {
            foreach (var filter in filterQuery)
            {
                if (value != null)
                {
                    var v = filter.Split('~');
                    value = value.Filter(v[0], v[1].Trim());
                }
            }

            return value;
        }

        /// <summary>
        /// Filters list
        /// </summary>
        /// <param name="value">The list to be filtered</param>
        ///<param name="propertyName">Property name</param>
        ///<param name="text">Filter text</param>
        ///<returns>Filtered list</returns>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> value, string propertyName, string text)
        {
            return value.Where(entity => entity.GetType().GetProperty(propertyName).GetValue(entity, null).ToString().IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }

}