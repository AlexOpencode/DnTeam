using System.Linq;
using System.Web.Mvc;
using DnTeamData;
using DnTeamData.Models;
using Telerik.Web.Mvc;

namespace DnTeam.Controllers
{
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
            return View(new GridModel(SettingsRepository.GetSettingValues(enumName).Select(o => new Value { Name = o })));
        }

        [GridAction]
        public ActionResult Insert(string type, string name)
        {
            var enumName = SettingsRepository.GetEnumName(type);
            SettingsRepository.UpdateSetting(enumName, name);
            return View(new GridModel(SettingsRepository.GetSettingValues(enumName).Select(o => new Value { Name = o })));
        }

        [GridAction]
        public ActionResult Save(string type, string id)
        {
            return View(new GridModel(SettingsRepository.GetSettingValues(SettingsRepository.GetEnumName(type)).Select(o => new Value { Name = o })));
        }

        [GridAction]
        public ActionResult Delete(string type, string id)
        {
            var enumName = SettingsRepository.GetEnumName(type);
            SettingsRepository.DeleteSetting(enumName, id);
            return View(new GridModel(SettingsRepository.GetSettingValues(enumName).Select(o => new Value { Name = o })));
        }

    }
}
