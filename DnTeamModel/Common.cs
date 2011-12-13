using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using MongoDB.Bson;

namespace DnTeamData
{
    /// <summary>
    /// A static class containing common actions
    /// </summary>
    public static class Common
    {
        private static string GetPropertyType(Type type, string propertyName, out PropertyInfo info, bool isList)
        {
            info = type.GetProperty(propertyName);
            var propType = isList ? info.PropertyType.GetGenericArguments()[0] : info.PropertyType;
            
            return propType.Name == "Nullable`1" 
                ? string.Format("{0}?", info.PropertyType.GetGenericArguments()[0].Name) 
                : propType.Name;
        }

        /// <summary>
        /// Splits string of values separated by "~"
        /// </summary>
        /// <param name="values">Values string</param>
        /// <returns>The list of values</returns>
        public static IEnumerable<string> SplitValues(string values)
        {
            return values.Split('~').Where(o => !String.IsNullOrEmpty(o)).Select(o => o.Trim());
        }

        /// <summary>
        /// Returns the object's ptoperty value of the valid type
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        /// <param name="type">Type of the object</param>
        /// <param name="val">Object's ptoperty value of the valid type</param>
        /// <param name="isList">Defines whether to get value of the [T] in the list</param>
        /// <returns>True - if value is converted successfuly</returns>
        public static dynamic GetTypedPropertyValue(string name, string value, Type type, out dynamic val, bool isList = false)
        {
            PropertyInfo info;
            return GetTypedPropertyValue(name, value, type, out info, out val, isList);
        }

        /// <summary>
        /// Returns the object's ptoperty value of the valid type
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="value">Property value</param>
        /// <param name="type">Type of the object</param>
        /// <param name="info">PropertyInfo of the value</param>
        /// <param name="val">Object's ptoperty value of the valid type</param>
        /// <param name="isList">Defines whether to get value of the [T] in the list</param>
        /// <returns>True - if value is converted successfuly</returns>
        public static bool GetTypedPropertyValue(string name, string value, Type type, out PropertyInfo info, out dynamic val, bool isList = false)
        {
            switch (GetPropertyType(type, name, out info, isList))
            {
                case "string":
                    val = value;
                    return true;

                case "ObjectId":
                    if(string.IsNullOrEmpty(value))
                    {
                        val = ObjectId.Empty;
                        return true;
                    }
                    ObjectId objectId;
                    if (ObjectId.TryParse(value, out objectId))
                    {
                        val = objectId;
                        return true;
                    }
                    val = null;
                    return false;

                case "DateTime":
                    DateTime dateTime;
                    if (DateTime.TryParse(value, out dateTime))
                    {
                        val = dateTime;
                        return true;
                    }
                    val = null;
                    return false;
                
                case "DateTime?":
                    DateTime? dateTimeNullable;
                    if (TryParseNullableDate(value, out dateTimeNullable))
                    {
                        val = dateTimeNullable;
                        return true;
                    }
                    val = null;
                    return false;

                case "decimal":
                    decimal dcml;
                    if (decimal.TryParse(value, out dcml))
                    {
                        val = dcml;
                        return true;
                    }
                    val = null;
                    return false;

                default:
                    val = value;
                    return true;
            }
            
        }

        /// <summary>
        /// Parses nullable date. If string is empty returns true and emty date
        /// </summary>
        /// <param name="text">Text to be parsed</param>
        /// <param name="nDate">Output DateTime?</param>
        /// <returns>True - if parsed. </returns>
        public static bool TryParseNullableDate(string text, out DateTime? nDate)
        {
            if (string.IsNullOrEmpty(text))
            {
                nDate = new DateTime?();
                return true;
            }

            DateTime date;
            bool isParsed = DateTime.TryParse(text, out date);
            nDate = isParsed ? date : new DateTime?();
            return isParsed;
        }
    }
}
