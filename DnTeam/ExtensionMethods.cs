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

    }

}