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
    public class MilestoneController : Controller
    {
        [NonAction]
        private static IEnumerable<MilestoneModel> Return(string id)
        {
            return ProjectRepository.GetMilestones(id).Select(o => new MilestoneModel
            {
                MilestoneId = o.ToString(),
                Index = o.Index,
                ActualDate = o.ActualDate,
                TargetDate = o.TargetDate,
                Name = o.Name
            }).OrderBy(x => x.Name);
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

        public ActionResult Save(string id, string milestoneId, string name, string targetDate, string actualDate)
        {
            if (string.IsNullOrEmpty(milestoneId))
                return new JsonResult { Data = GetTransactionStatusCode(ProjectRepository.InsertMilestone(id, name, targetDate, actualDate)) };

            return new JsonResult { Data = GetTransactionStatusCode(ProjectRepository.UpdateMilestone(id, milestoneId, targetDate, actualDate)) };
        }
        
        public ActionResult Delete(string projectId, List<string> values)
        {
            ProjectRepository.DeleteMilestones(projectId, values);
            
            return Content("");
        }

        public ActionResult Finish(string projectId, List<string> values)
        {
            ProjectRepository.FinishMilestones(projectId, values);

            return Content("");
        }

        [NonAction]
        private static string GetTransactionStatusCode(MilestoneEditStatus status)
        {
            switch (status)
            {
                case MilestoneEditStatus.Ok:
                    return null;

                case MilestoneEditStatus.ErrorDuplicateName:
                    return "Milestone with such name already exists. Please enter a different value.";

                case MilestoneEditStatus.ErrorNameIsEmpty:
                    return "Milestone name is empty. Please enter one.";

                case MilestoneEditStatus.ErrorMilestoneHasNotBeenUpdated:
                    return "Error occured. Property has not been updated.";

                case MilestoneEditStatus.ErrorActualDateFormat:
                    return string.Format("Actual Date has invalid format. Please enter date in proper format (for example today is {0})", DateTime.Now.ToShortDateString());

                case MilestoneEditStatus.ErrorTargetDateFormat:
                    return string.Format("Target Date has invalid format. Please enter date in proper format (for example today is {0})", DateTime.Now.ToShortDateString());

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}
