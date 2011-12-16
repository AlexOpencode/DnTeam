using System;
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

                case PersonEditStatus.ErrorDuplicateName:
                    return "User name already exists. Please, enter a different user name.";

                case PersonEditStatus.ErrorDateIsNotValid:
                    return string.Format("Date is not in valid format. Please, enter a valid date (for example today is: {0}).", DateTime.Now.ToShortDateString());

                case PersonEditStatus.ErrorDuplicateSpecialtyName:
                    return "The selected specialty is already defined for the given person. Please, select other one.";
                    
                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}
