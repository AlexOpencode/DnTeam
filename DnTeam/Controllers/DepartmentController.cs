using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DnTeamData;
using Telerik.Web.Mvc;
using DnTeam.Models;
using DnTeamData.Models;
namespace DnTeam.Controllers
{
    public class DepartmentController : Controller
    {

        public ActionResult Index()
        {
            ViewData["LocationsList"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.Locations));
            ViewData["DepartmentsList"] = new SelectList(DepartmentRepository.GetDepartmentsDictionary(), "key", "value");
            return View();
        }


        public ActionResult DeleteDepartments(List<string> values)
        {
            DepartmentRepository.DeleteDepartments(values);
            return new EmptyResult();
        }

        [GridAction]
        public ActionResult Select(List<string> filterQuery)
        {
            //Apply filter, if any
            if(filterQuery != null && filterQuery.Count > 1)
            {
                filterQuery.RemoveAt(0);
                return View(new GridModel(Return(DepartmentRepository.GetFilteredDepartments(filterQuery))));
            }

            return View(new GridModel(Return(DepartmentRepository.GetAllDepartments())));
        }


        public ActionResult Save(string location, string name, string parentId, string parentName, decimal rate, decimal cost, string id = null)
        {
            return new JsonResult { Data = DepartmentRepository.SaveDepartment(id, location, name, parentId, parentName, rate, cost) };
        }

        
        public ActionResult GetDepartmentsList()
        {
            return new JsonResult {Data = new SelectList (DepartmentRepository.GetDepartmentsDictionary(), "key", "value")};
        }


        public ActionResult GetTopSearchOffers(string column, string query)
        {
            return new JsonResult { Data = DepartmentRepository.GetTopSearchOffers(column, query)};
        }

        [NonAction]
        public IEnumerable<DepartmentModel> Return(List<Department> departments)
        {
            return from department in departments
                   select new DepartmentModel
                   {
                       Cost = department.Cost,
                       Rate = department.Rate,
                       Location = department.Location,
                       Id = department.DepartmentId,
                       Name = department.Name,
                       ParentDepartment = string.IsNullOrEmpty(department.ParentDepartment) ? "none" : department.ParentDepartment
                   };
        }
    }
}
