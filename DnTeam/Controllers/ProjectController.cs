using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DnTeam.Models;
using DnTeamData;
using Telerik.Web.Mvc;

namespace DnTeam.Controllers
{
    public class ProjectController : Controller
    {
        public ActionResult Index()
        {
            ViewData["ProjectStatuses"] = new SelectList(SettingsRepository.GetAllProjectStatuses());
            return View();
        }

        public ActionResult Details(string id)
        {
            return View();
        }

        [GridAction]
        public ActionResult Select()
        {
            return View(GetAllProjectsModel());
        }


        [GridAction]
        public ActionResult Insert()
        {
            var product = new ProjectEditableModel();
            if (TryUpdateModel(product))
            {
                ProjectRepository.Insert(product.Name, product.Priority, product.CreatedDate,
                    product.ProjectStatus);
            }

            return View(GetAllProjectsModel());
        }

        [NonAction]
        private static GridModel GetAllProjectsModel()
        {
            return new GridModel(ProjectRepository.GetAllProjects()
                                     .Select(o => new ProjectEditableModel()
                                                      {
                                                          Id = o.ProjectId,
                                                          CreatedDate = o.CreatedDate,
                                                          Name = o.Name,
                                                          Priority = o.Priority,
                                                          ProjectStatus = o.Status
                                                      }).OrderByDescending(o=>o.Priority));
        }
    }
}
