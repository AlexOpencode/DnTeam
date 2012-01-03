using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DnTeamData;
using DnTeamData.Models;
using Telerik.Web.Mvc;
using DnTeam.Models;

namespace DnTeam.Controllers
{
    [OpenIdAuthorize]
    public class PersonSpecialtyController : Controller
    {
       

        [GridAction]
        public ActionResult SelectTechnology(string id, List<string> filterQuery)
        {
            var specialties = PersonRepository.GetTechnologySpecialties(id).Select(o => new TechnologySpecialtyModel
            {
                Name = o.Name,
                Level = o.Level,
                FirstUsed = o.FirstUsed,
                LastUsed = o.LastUsed,
                LastProjectNote = o.LastProjectNote
            });

            if (filterQuery != null && filterQuery.Count() > 0)
                return View(new GridModel(specialties.Filter(filterQuery)));
            
            return View(new GridModel(specialties));
        }

        [GridAction]
        public ActionResult SelectProject(string id, List<string> filterQuery)
        {
            var specialties = ProjectRepository.GetPersonProjectSpecialties(id).Select(o => new FunctionalSpecialtyModel
            {
                Name = o.Name,
                Roles = o.Roles,
                FirstUsed = o.FirstUsed,
                LastUsed = o.LastUsed,
                ProjectId = o.ProjectId
            });

            if (filterQuery != null && filterQuery.Count() > 0)
                return View(new GridModel(specialties.Filter(filterQuery)));

            return View(new GridModel(specialties));
        }

        [HttpPost]
        public ActionResult Save(string id, string name, string value, string lastUsed, string firstUsed, string note, bool update = false)
        {
            return update 
                ? new JsonResult { Data = GetTransactionStatusCode(PersonRepository.UpdateTechnologySpecialty(id, name, value, lastUsed, firstUsed, note)) } 
                : new JsonResult { Data = GetTransactionStatusCode(PersonRepository.CreateTechnologySpecialty(id, name, value, lastUsed, firstUsed, note)) };
        }

        [HttpPost]
        public ActionResult Delete(string id, List<string> values)
        {
            PersonRepository.DeleteTechnologySpecialties(id, values);
            
            return Content("");
        }

        [NonAction]
        private static string GetTransactionStatusCode(PersonEditStatus status)
        {
            switch (status)
            {
                case PersonEditStatus.Ok:
                    return null;

               case PersonEditStatus.ErrorDateIsNotValid:
                    return Resources.Labels.Error_Invalid_Date_Format;

                case PersonEditStatus.ErrorDuplicateSpecialtyName:
                    return Resources.Labels.Specialty_Error_Duplicate_Name;
                    
                default:
                    return Resources.Labels.Error_Default;
            }
        }
    }
}
