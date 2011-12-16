using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DnTeam.Models;
using DnTeamData;
using DnTeamData.Models;
using Telerik.Web.Mvc;

namespace DnTeam.Controllers
{
    [OpenIdAuthorize]
    public class SettingsController : Controller
    {
        public ActionResult Index(string type)
        {
            var e = SettingsRepository.GetEnumName(type);
            if (e == EnumName.Undefined)
                return View("Error");

            ViewBag.Title = type;
            return View();
        }
        
        [GridAction]
        public ActionResult Select(string type)
        {
            var enumName = SettingsRepository.GetEnumName(type);
            var list = SettingsRepository.GetSettingValues(enumName);

            return View(new GridModel((list != null) ? list.Select(o => new Value {Name = o}).ToList() : new List<Value>()));
        }

        public ActionResult MultipleInsert(string type, string value)
        {
            SettingsRepository.BatchAddSettingValues(SettingsRepository.GetEnumName(type), Common.SplitValues(value));

            return Content("");
        }

        public ActionResult Delete(string type, List<string> values)
        {
            var enumName = SettingsRepository.GetEnumName(type);
            SettingsRepository.BatchDeleteSettingValues(enumName, values);

            return Content("");
        }

    }
}
