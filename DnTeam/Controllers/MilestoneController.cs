using System.Linq;
using System.Web.Mvc;
using DnTeamData;
using Telerik.Web.Mvc;
using DnTeam.Models;

namespace DnTeam.Controllers
{
    public class MilestoneController : Controller
    {
        [NonAction]
        private static GridModel Return(string id)
        {
            return new GridModel(ProjectRepository.GetMilestones(id).Select(o => new MilestoneModel
                                                                                     {
                                                                                        MilestoneId = o.MilestoneId,
                                                                                        Index = o.Index,
                                                                                        ActualDate = o.ActualDate,
                                                                                        TargetDate = o.TargetDate,
                                                                                        Name = o.Name
                                                                                     }).OrderBy(x=>x.Index));
        }

        [GridAction]
        public ActionResult Select(string projectId)
        {
            return View(Return(projectId));
        }

        [GridAction]
        public ActionResult Insert(string projectId)
        {
            var model = new MilestoneModel();
            if (TryUpdateModel(model))
            {
                ProjectRepository.CreateMilestone(projectId, model.Index, model.ActualDate, model.TargetDate, model.Name);
            }
            return View(Return(projectId));
        }

        [GridAction]
        public ActionResult Save(string id, string projectId)
        {
            var model = new MilestoneModel();
            if (TryUpdateModel(model))
            {
                ProjectRepository.UpdateMilestone(projectId, id, model.Index, model.ActualDate, model.TargetDate, model.Name);
            }
            return View(Return(projectId));
        }

        [GridAction]
        public ActionResult Delete(string id, string projectId)
        {
            ProjectRepository.DeleteMilestone(projectId, id);
            return View(Return(projectId));
        }
    }
}
