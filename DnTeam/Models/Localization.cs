using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;

namespace DnTeam.Models
{
    public class Language
    {
        public string Flag { get; set; }
        public string Title { get; set; }
        public string Culture { get; set; }
    }

    public class Labels
    {
        public string Culture { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }
    

    public static class Localization
    {
        private static string GetCulture(string name)
        {
            name = name.Substring(0, name.IndexOf(".resx"));
            name = name.Substring(name.LastIndexOf('.') + 1);

            return name;
        }

        private static string GetFlag(string name)
        {
            return XElement.Load(name).Elements("data").First(o => o.Attribute("name").Value == "Flag").Value;
        }

        private static string GetTitle(string name)
        {
            return XElement.Load(name).Elements("data").First(o => o.Attribute("name").Value == "Title").Value;
        }

        private static string GetPath(string lang)
        {
            return HostingEnvironment.MapPath(string.Format("~/App_GlobalResources/Labels.{0}.resx", lang));
        }

        public static IEnumerable<Language> GetLanguages()
        {
           return new DirectoryInfo(HostingEnvironment.MapPath("~/App_GlobalResources/")).GetFiles("*.resx", SearchOption.TopDirectoryOnly).Where(o=>o.Name != "Labels.resx")
               .Select(o => new Language
               {
                   Culture = GetCulture(o.Name),
                   Flag = GetFlag(o.FullName),
                   Title = GetTitle(o.FullName)
               });
        }

        public static Labels GetLabels(string lang)
        {
            return new Labels
                       {
                           Culture = lang,
                           Values = XElement.Load(GetPath(lang)).Elements("data").ToDictionary(o => o.Attribute("name").Value, x => x.Value)
                       };
        }

        internal static string SaveValue(string lang, string name, string value)
        {
            try
            {
                var filename = GetPath(lang);
                var file = XElement.Load(filename);
                file.Elements("data").SingleOrDefault(o => o.Attribute("name").Value == name).Element("value").SetValue(value);
                file.Save(filename);
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        internal static void AddNewLanguage(string lang)
        {
            System.Web.HttpContext.Current.Cache["LangList"] = GetLanguages();
            throw new System.NotImplementedException();
        }
    }
}