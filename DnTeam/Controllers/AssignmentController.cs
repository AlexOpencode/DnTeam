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
    public class AssignmentController : Controller
    {
        [NonAction]
        private static IEnumerable<AssignmentModel> Return(string id)
        {
            return ProjectRepository.GetAssignments(id).Select(o=> new AssignmentModel
            {
                Commitment = o.Commitment,
                EndDate = o.EndDate,
                Note = o.Note,
                Person = o.PersonName,
                Role = o.Role,
                StartDate = o.StartDate,
                AssignmentId = o.ToString()
            });
        }

        [GridAction]
        public ActionResult Select(string projectId, List<string> filterQuery)
        {
            if (filterQuery != null && filterQuery.Count() > 0)
            {
                var result = Return(projectId).Filter(filterQuery);
                return View(new GridModel(result));
            }

            return View(new GridModel(Return(projectId)));
        }
        
        public ActionResult Save(string id, string assignmentId, string role, string person, string note, string startDate, string endDate, int commitment)
        {
            return string.IsNullOrEmpty(assignmentId) 
                ? new JsonResult { Data = GetTransactionStatusCode(ProjectRepository.InsertAssignment(id, role, person, note, startDate, endDate, commitment)) } 
                : new JsonResult { Data = GetTransactionStatusCode(ProjectRepository.UpdateAssignment(id, assignmentId, role, person, note, startDate, endDate, commitment)) };
        }
        
        public ActionResult Delete(string projectId, List<string> values)
        {
            ProjectRepository.DeleteAssignments(projectId, values);

            return Content("");
        }

        [NonAction]
        private static string GetTransactionStatusCode(AssignmentEditStatus status)
        {
            switch (status)
            {
                case AssignmentEditStatus.Ok:
                    return null;

                case AssignmentEditStatus.ErrorAssignmentHasNotBeenUpdated:
                    return "Assignment has not been updated. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case AssignmentEditStatus.ErrorAssignmentHasNotBeenInserted:
                    return "Assignment has not been inserted. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}
