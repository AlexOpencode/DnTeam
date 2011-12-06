using System.Linq;
using System.Web.Mvc;
using DnTeam.Models;
using DnTeamData;
using DnTeamData.Models;
using Telerik.Web.Mvc;

namespace DnTeam.Controllers
{
    public class ProjectController : Controller
    {

        public ActionResult UpdateProjectProperty(string id, string name, string value)
        {
            return new JsonResult { Data = ProjectRepository.UpdateProjectProperty(id, name, value) };
        }

        public ActionResult Index()
        {
            ViewData["ProjectStatuses"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectStatuses));
            ViewData["ProjectTypes"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectTypes));
            ViewData["Products"] = new SelectList(ProductRepository.GetAllProductsDictionary(), "key", "value");
            ViewData["ProjectNoiseTypes"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectNoiseTypes));
            ViewData["ProjectPriorityTypes"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectPriorityTypes));
            ViewData["PersonsList"] = new SelectList(PersonRepository.GetActivePersonsList(), "key", "value");
            return View();
        }

        public ActionResult Details(string id)
        {
            ViewData["ProjectStatuses"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectStatuses));
            ViewData["ProjectTypes"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectTypes));
            ViewData["Products"] = new SelectList(ProductRepository.GetAllProductsDictionary(), "key", "value");
            ViewData["PersonsList"] = new SelectList(PersonRepository.GetActivePersonsList(), "key", "value");
            ViewData["ProjectRoles"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectRoles));
            ViewData["ProjectNoiseTypes"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectNoiseTypes));
            ViewData["ProjectPriorityTypes"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.ProjectPriorityTypes));
            var project = ProjectRepository.GetProject(id);
            ProjectModel model = new ProjectModel
                                     {
                                         Name = project.Name,
                                         CreatedDate = project.CreatedDate,
                                         Id = project.ProjectId,
                                         Noise = project.Noise,
                                         Product = project.ProductName,
                                         Status = project.Status,
                                         Type = project.Type,
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
                ProjectRepository.Insert(product.Name, product.Priority, product.CreatedDate, product.Status, product.Noise, product.Product, product.Type, 
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
                ProjectRepository.Save(id, product.Name, product.Priority, product.Status, product.Noise, product.Product, product.Type, 
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
                                                          Status = o.Status,
                                                          Noise = o.Noise,
                                                          Type = o.Type,
                                                          Product = o.ProductName,
                                                          ProgramManager = o.ProgramManagerName,
                                                          TechnicalLead = o.TechnicalLeadName
                                                      }).OrderByDescending(o=>o.Priority));
        }
    }
}
