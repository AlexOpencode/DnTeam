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

        public ActionResult Index(EnumName type, string title)
        {
            ViewBag.SettingType = type;
            ViewBag.Title = title;
            
            return View();
        }
        
        [GridAction]
        public ActionResult Select(EnumName type)
        {
            var list = SettingsRepository.GetSettingValues(type);

            return View(new GridModel((list != null) ? list.Select(o => new Value {Name = o}).ToList() : new List<Value>()));
        }

        public ActionResult MultipleInsert(EnumName type, string value)
        {
            SettingsRepository.BatchAddSettingValues(type, Common.SplitValues(value));

            return Content("");
        }

        public ActionResult Delete(EnumName type, List<string> values)
        {
            SettingsRepository.BatchDeleteSettingValues(type, values);

            return Content("");
        }

    }
}
