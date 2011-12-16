using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DnTeamData;
using Telerik.Web.Mvc;
using DnTeam.Models;
using DnTeamData.Models;
namespace DnTeam.Controllers
{

    [OpenIdAuthorize]
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
            return new JsonResult { Data = GetStatusCode(DepartmentRepository.SaveDepartment(id, location, name, parentId, parentName, rate, cost)) };
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
                       Id = department.ToString(),
                       Name = department.Name,
                       ParentDepartment = string.IsNullOrEmpty(department.ParentDepartment) ? "none" : department.ParentDepartment
                   };
        }

        [NonAction]
        private string GetStatusCode(DepartmentEditStatus status)
        {
            switch (status)
            {
                case DepartmentEditStatus.Ok:
                    return null;

                case DepartmentEditStatus.ErrorDuplicate:
                    return "Department with such name and location already exists. Please, enter other name or select other location.";

                case DepartmentEditStatus.ErrorParentUndefined:
                    return "Please, select parent department from the list or leave the field empty.";

                case DepartmentEditStatus.ErrorNameIsEmpty:
                    return "Please, enter the Name of the department.";

                default:
                    return "Undefined error occured. Please, contact your administrator.";
            }
        }
    }
}
