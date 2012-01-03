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
                    return Resources.Labels.Milestone_Error_Duplicate_Name;

                case MilestoneEditStatus.ErrorNoName:
                    return Resources.Labels.Milestone_Error_No_Name;

                case MilestoneEditStatus.ErrorNotUpdated:
                    return Resources.Labels.Milestone_Error_Not_Updated;

                case MilestoneEditStatus.ErrorActualDateFormat:
                    return Resources.Labels.Milestone_Error_Actual_Date_Format;

                case MilestoneEditStatus.ErrorTargetDateFormat:
                    return Resources.Labels.Milestone_Error_Target_Date_Format;

                default:
                    return Resources.Labels.Error_Default;
            }
        }
    }
}
