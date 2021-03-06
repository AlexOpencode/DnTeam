﻿using System.Collections.Generic;
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
            if (filterQuery != null && filterQuery.Count() > 0)
            {
                var result = Return(DepartmentRepository.GetAllDepartments()).Filter(filterQuery);
                return View(new GridModel(result));
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
                    return Resources.Labels.Department_Error_Duplicate;

                case DepartmentEditStatus.ErrorParentUndefined:
                    return Resources.Labels.Department_Parent_Undefined;

                case DepartmentEditStatus.ErrorNoName:
                    return Resources.Labels.Department_No_Name;

                default:
                    return Resources.Labels.Error_Default;
            }
        }
    }
}
