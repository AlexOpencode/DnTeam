using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using DnTeam.Models;
using Telerik.Web.Mvc;

namespace DnTeam.Controllers
{
    public class LocalizationController : Controller
    {
        public ActionResult Change(string lang, string returnUrl)
        {
            Session["Culture"] = new CultureInfo(lang);
            return Redirect(returnUrl);
        }

        public static void FillLanguages()
        {
            System.Web.HttpContext.Current.Cache["LangList"] = Localization.GetLanguages();
        }

        [GridAction]
        public ActionResult Select()
        {
            if (System.Web.HttpContext.Current.Cache["LangList"] == null)
                FillLanguages();
            
            var result = System.Web.HttpContext.Current.Cache["LangList"] as IEnumerable<Language>;
            
            return View(new GridModel(result));
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(string lang)
        {
            var model = Localization.GetLabels(lang);
            return View(model);
        }

        public ActionResult SaveValue(string lang, string name, string value)
        {
            return new JsonResult {Data = Localization.SaveValue(lang, name, value)};
        }

        public ActionResult AddNewLanguage(string lang)
        {
            Localization.AddNewLanguage(lang);
            return Content("");
        }

    }
}
