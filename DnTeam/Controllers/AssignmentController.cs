using System.Linq;
using System.Web.Mvc;
using DnTeamData;
using Telerik.Web.Mvc;
using DnTeam.Models;

namespace DnTeam.Controllers
{
    public class AssignmentController : Controller
    {
        [NonAction]
        private static GridModel Return(string id)
        {
            return new GridModel(ProjectRepository.GetAssignments(id).Select(o=> new AssignmentModel
                                                                                     {
                                                                                         Commitment = o.Commitment,
                                                                                         EndDate = o.EndDate,
                                                                                         Note = o.Note,
                                                                                         Person = o.PersonName,
                                                                                         Role = o.Role,
                                                                                         StartDate = o.StartDate,
                                                                                         AssignmentId = o.AssignmentId
                                                                                     }));
        }

        [GridAction]
        public ActionResult Select(string projectId)
        {
            return View(Return(projectId));
        }

        [GridAction]
        public ActionResult Insert(string projectId)
        {
            var model = new AssignmentModel();
            if (TryUpdateModel(model))
            {
                ProjectRepository.CreateAssignment(projectId, model.StartDate, model.EndDate, model.Person, model.Role, model.Commitment, model.Note);
            }
            return View(Return(projectId));
        }

        [GridAction]
        public ActionResult Save(string id, string projectId)
        {
            var model = new AssignmentModel();
            if (TryUpdateModel(model))
            {
                ProjectRepository.UpdateAssignment(projectId, id, model.StartDate, model.EndDate, model.Person, model.Role, model.Commitment, model.Note);
            }
            return View(Return(projectId));
        }

        [GridAction]
        public ActionResult Delete(string id, string projectId)
        {
            ProjectRepository.DeleteAssignment(projectId, id);
            return View(Return(projectId));
        }
    }
}
