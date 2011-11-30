using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DnTeamData;
using Telerik.Web.Mvc;
using DnTeam.Models;

namespace DnTeam.Controllers
{
    public class PersonSpecialtyController : Controller
    {
        [NonAction]
        private static GridModel Return(string id)
        {
            return new GridModel(PersonRepository.GetTechnologySpecialties(id).Select(o => new SpecialtyModel
                                                                                     {
                                                                                        Name = o.Name,
                                                                                        Level = o.Level,
                                                                                        FirstUsed = o.FirstUsed,
                                                                                        LastUsed = o.LastUsed,
                                                                                        LastProjectNote = o.LastProjectNote
                                                                                     }));
        }

        [GridAction]
        public ActionResult Select(string id)
        {
            return View(Return(id));
        }

        [HttpPost]
        public ActionResult Save(string id, string name, string value, string lastUsed, string firstUsed, string note, bool update = false)
        {
            if(update) return new JsonResult { Data = PersonRepository.UpdateTechnologySpecialty(id, name, value, lastUsed, firstUsed, note) };

            return new JsonResult { Data = PersonRepository.CreateTechnologySpecialty(id, name, value, lastUsed, firstUsed, note) };
        }


        [HttpPost]
        public ActionResult Delete(string id, List<string> values)
        {
            return new JsonResult { Data = PersonRepository.DeleteTechnologySpecialties(id, values) };
        }
    }
}
