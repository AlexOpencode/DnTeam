using System.Linq;
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
            ViewData["ProjectTypes"] = new SelectList(SettingsRepository.GetAllProjectTypes());
            ViewData["Products"] = new SelectList(ProductRepository.GetAllProductsList(), "key", "value");
            ViewData["ProjectNoiseTypes"] = new SelectList(SettingsRepository.GetAllProjectNoiseTypes());
            ViewData["ProjectPriorityTypes"] = new SelectList(SettingsRepository.GetAllProjectPriorityTypes());
            ViewData["PersonsList"] = new SelectList(PersonsRepository.GetActivePersonsList(), "key", "value");
            return View();
        }

        public ActionResult Details(string id)
        {
            ViewData["ProjectStatuses"] = new SelectList(SettingsRepository.GetAllProjectStatuses());
            ViewData["ProjectTypes"] = new SelectList(SettingsRepository.GetAllProjectTypes());
            ViewData["Products"] = new SelectList(ProductRepository.GetAllProductsList(), "key", "value");
            ViewData["PersonsList"] = new SelectList(PersonsRepository.GetActivePersonsList(), "key", "value");
            ViewData["ProjectRoles"] = new SelectList(SettingsRepository.GetAllProjectRoles());
            ViewData["ProjectNoiseTypes"] = new SelectList(SettingsRepository.GetAllProjectNoiseTypes());
            ViewData["ProjectPriorityTypes"] = new SelectList(SettingsRepository.GetAllProjectPriorityTypes());
            var project = ProjectRepository.GetProject(id);
            ProjectModel model = new ProjectModel
                                     {
                                         Name = project.Name,
                                         CreatedDate = project.CreatedDate,
                                         Id = project.ProjectId,
                                         Noise = project.Noise,
                                         Product = project.ProductName,
                                         ProjectStatus = project.Status,
                                         ProjectType = project.Type,
                                         IsDeleted = project.IsDeleted
                                     };

            return View(model);
        }

        [GridAction]
        public ActionResult Select()
        {
            return View(Return());
        }


        [GridAction]
        public ActionResult Insert()
        {
            var product = new ProjectGridModel();
            if (TryUpdateModel(product))
            {
                ProjectRepository.Insert(product.Name, product.Priority, product.CreatedDate, product.ProjectStatus, product.Noise, product.Product, product.ProjectType, 
                    product.ProgramManager, product.TechnicalLead);
            }

            return View(Return());
        }

        [GridAction]
        public ActionResult Save(string id)
        {
            var product = new ProjectGridModel();
            if (TryUpdateModel(product))
            {
                ProjectRepository.Save(id, product.Name, product.Priority, product.ProjectStatus, product.Noise, product.Product, product.ProjectType, 
                    product.ProgramManager, product.TechnicalLead);
            }

            return View(Return());
        }

        [GridAction]
        public ActionResult Delete(string id)
        {
            ProjectRepository.Delete(id);
            return View(Return());
        }

        [NonAction]
        private static GridModel Return()
        {
            return new GridModel(ProjectRepository.GetAllProjects()
                                     .Select(o => new ProjectGridModel()
                                                      {
                                                          Id = o.ProjectId,
                                                          CreatedDate = o.CreatedDate,
                                                          Name = o.Name,
                                                          Priority = o.Priority,
                                                          ProjectStatus = o.Status,
                                                          Noise = o.Noise,
                                                          ProjectType = o.Type,
                                                          Product = o.ProductName,
                                                          ProgramManager = o.ProgramManagerName,
                                                          TechnicalLead = o.TechnicalLeadName
                                                      }).OrderByDescending(o=>o.Priority));
        }
    }
}
