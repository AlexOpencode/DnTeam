﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DnTeam.Models;
using DnTeamData;
using DnTeamData.Models;
using Telerik.Web.Mvc;

namespace DnTeam.Controllers
{
    [OpenIdAuthorize]
    public class ProjectController : Controller
    {
        private const string BlankPerson = "wanted";

        public ActionResult UpdateProjectProperty(string id, string name, string value)
        {
           return new JsonResult { Data = GetTransactionStatusCode(ProjectRepository.UpdateProjectProperty(id, name, value)) };
        }

        public ActionResult Index()
        {
            ViewData["ProjectStatuses"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectStatuses));
            ViewData["ProjectTypes"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectTypes));
            ViewData["Products"] = new SelectList(ProductRepository.GetAllProductsDictionary(), "key", "value");
            ViewData["ProjectNoiseTypes"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectNoiseTypes));
            ViewData["ProjectPriorityTypes"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectPriorityTypes));
            ViewData["PersonsList"] = PersonRepository.GetActivePersonsList().ToSelectList(string.Empty, BlankPerson);
            return View();
        }

        public ActionResult Details(string id)
        {
            ViewData["ProjectStatuses"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectStatuses));
            ViewData["ProjectTypes"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectTypes));
            ViewData["Products"] = new SelectList(ProductRepository.GetAllProductsDictionary(), "key", "value");
            ViewData["PersonsList"] = PersonRepository.GetActivePersonsList().ToSelectList(string.Empty, BlankPerson);
            ViewData["ProjectRoles"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectRoles));
            ViewData["ProjectNoiseTypes"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectNoiseTypes));
            ViewData["ProjectPriorityTypes"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectPriorityTypes));
            var project = ProjectRepository.GetProject(id);
            var model = new ProjectModel
                                     {
                                         Name = project.Name,
                                         CreatedDate = project.CreatedDate,
                                         Id = project.ToString(),
                                         Noise = project.Noise,
                                         ProductId = project.ProductName,
                                         Status = project.Status,
                                         Type = project.Type,
                                         IsDeleted = project.IsDeleted
                                     };

            return View(model);
        }

        [GridAction]
        public ActionResult Select(List<string> filterQuery)
        {
            if (filterQuery != null && filterQuery.Count() > 0)
            {
                var result = Return().Filter(filterQuery);
                return View(new GridModel(result));
            }

            return View(new GridModel(Return()));
        }
        
        public ActionResult Insert(ProjectGridModel model)
        {
            model.ProgramManager = (model.ProgramManager == BlankPerson) ? string.Empty : model.ProgramManager;
            model.TechnicalLead = (model.TechnicalLead == BlankPerson) ? string.Empty : model.TechnicalLead;

            return new JsonResult { Data = GetTransactionStatusCode(ProjectRepository.Insert(model.Name, model.Priority, model.CreatedDate, model.Status, 
                model.Noise, model.ProductId, model.Type, model.ProgramManager,  model.TechnicalLead)) };
        }

        public ActionResult Delete(List<string> values)
        {
            ProjectRepository.Delete(values);
            return Content("");
        }

        [NonAction]
        private static IEnumerable<ProjectGridModel> Return()
        {
            return ProjectRepository.GetAllProjects()
                                     .Select(o => new ProjectGridModel
                                                      {
                                                          Id = o.ToString(),
                                                          CreatedDate = o.CreatedDate,
                                                          Name = o.Name,
                                                          Priority = o.Priority,
                                                          Status = o.Status,
                                                          Noise = o.Noise,
                                                          Type = o.Type,
                                                          ProductId = o.ProductName,
                                                          ProgramManager = o.ProgramManagerName(),
                                                          TechnicalLead = o.TechnicalLeadName()
                                                      }).OrderByDescending(o=>o.Priority);
        }

        [NonAction]
        private static string GetTransactionStatusCode(ProjectEditStatus status)
        {
            switch (status)
            {
                case ProjectEditStatus.Ok:
                    return null;

                case ProjectEditStatus.ErrorDuplicateItem:
                    return Resources.Labels.Project_Error_Duplicate_Item;

                case ProjectEditStatus.ErrorNoName:
                    return Resources.Labels.Project_Error_No_Name;

                case ProjectEditStatus.ErrorPropertyNotUpdated:
                    return Resources.Labels.Project_Error_Propery_Not_Updated;

                case ProjectEditStatus.ErrorUndefinedFormat:
                    return Resources.Labels.Project_Error_Undefined_Format;

                default:
                    return Resources.Labels.Error_Default;
            }
        }
    }
}
