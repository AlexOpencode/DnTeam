using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DnTeamData;
using Telerik.Web.Mvc;
using DnTeam.Models;
using DnTeamData.Models;
namespace DnTeam.Controllers
{
    public class DepartmentController : Controller
    {

        [HttpPost]
        public ActionResult UpdateDapartament(string id, string name)
        {
            return new JsonResult { Data = DepartmentRepository.UpdateDepartment(id, name) };
        }

        [HttpPost]
        public ActionResult UpdateSubsidiary(string id, string location, float baseCost, float baseRate, string parentId)
        {
            return new JsonResult { Data = DepartmentRepository.UpdateSubsidiary(id, location, baseCost, baseRate, parentId) };
        }

        [HttpPost]
        public ActionResult DeleteDapartament(string id)
        {
            return new JsonResult { Data = DepartmentRepository.DeleteDepartment(id) };
        }

        [HttpPost]
        public ActionResult DeleteSubsidiary(string id, string departmentId)
        {
            return new JsonResult { Data = DepartmentRepository.DeleteSubsidiary(id, departmentId) };
        }

        [HttpPost]
        public ActionResult SaveSubsidiary(string location, float baseCost, float baseRate, string departmentId)
        {
            return new JsonResult { Data = DepartmentRepository.CreateSubsidiary(location, baseCost, baseRate, departmentId) };
        }

        [HttpPost]
        public ActionResult SaveDapartament(string name, string departmentOf)
        {
            return new JsonResult { Data = DepartmentRepository.CreateDepartment(name, departmentOf.Trim()) };
        }

        [NonAction]
        private void BuildDepartmentsTree(IEnumerable<TDepartment> departments, StringBuilder sb)
        {
            foreach (TDepartment department in departments)
            {
                sb.Append(string.Format("<li>{0} <input class=\"t-button t-icon t-edit\" type=\"button\" Value=\"Edit\" onclick=\"updateDepartment('{1}','{0}')\"/> <input class=\"t-button t-icon t-delete\" type=\"button\" Value=\"Delete\" onclick=\"deleteDepartment('{1}')\"/>",
                    department.Name, department.Id));

                sb.Append(string.Format("<li style=\"list-style: none;\"><input class=\"t-button\" type=\"button\" Value=\"+ Add Sub-Department\" onclick=\"addDepartment('{0}')\"/></li>",
                    department.Id));
                
                if (department.SubDepartments.Count() > 0)
                {
                    sb.Append("<ul>");
                    BuildDepartmentsTree(department.SubDepartments, sb);
                    sb.Append("</ul>");
                }
                //Subsidiaries----------------------------------------------->
                sb.Append(string.Format("<ul><li style=\"list-style: none;\">Location rates:</li><li style=\"list-style: none;\"><input class=\"t-button\" type=\"button\" Value=\"+ Add Location Rates\" onclick=\"addSubsidiary('{0}')\"/></li>",
                    department.Id));
                
                foreach (Subsidiary sub in department.Subsidiaries)
                {
                    sb.Append(string.Format("<li>{0} /Base cost: {1}, Base rate: {2}/ <input class=\"t-button t-icon t-edit\" type=\"button\" Value=\"Edit\" onclick=\"updateSubsidiary('{4}','{3}','{0}','{1}','{2}')\"/> <input class=\"t-button t-icon t-delete\" type=\"button\" Value=\"Delete\" onclick=\"deleteSubsidiary('{3}','{4}')\"/></li>", 
                        sub.Location, sub.BaseCost, sub.BaseRate, sub.SubsidiaryId, department.Id));
                }
                
                sb.Append("</ul>");
                //<----------------------------------------------------------

                sb.Append("</li>");
            }
            
            
        }

        public ActionResult Index()
        {
            var departments = DepartmentRepository.GetDepartmentsTree();
            var sb = new StringBuilder();
            sb.Append("<ul id=\"DepartmentsTree\"><li style=\"list-style: none;\"><input class=\"t-button\" type=\"button\" Value=\"+ Add Department\" onclick=\"addDepartment('')\"/></li>");
            BuildDepartmentsTree(departments, sb);
            sb.Append("</ul>");

            ViewBag.DepartmentsTree = sb.ToString();
            ViewBag.Locations = SettingsRepository.GetAllLocations();
            return View();
        }

        [NonAction]
        private IEnumerable<EditableProductModel> Return(List<Product> products)
        {
            return products.Select(o => new EditableProductModel { Name = o.Name, Id = o.ProductId, Client = o.ClientName() });
        }

        [GridAction]
        public ActionResult Select()
        {
            return View(new GridModel(Return(ProductRepository.GetAllProducts())));
        }

        [GridAction]
        public ActionResult Insert(EditableProductModel model)
        {
            if (TryUpdateModel(model))
            {
                ProductRepository.InsertProduct(model.Name, model.Client);
            }
            return View(new GridModel(Return(ProductRepository.GetAllProducts())));
        }

        [GridAction]
        public ActionResult Save(string id, EditableProductModel model)
        {
            if (TryUpdateModel(model))
            {
                ProductRepository.UpdateProduct(id, model.Name, model.Client);
            }
            return View(new GridModel(Return(ProductRepository.GetAllProducts())));
        }

        [GridAction]
        public ActionResult Delete(string id)
        {
            ProductRepository.DeleteProduct(id);
            return View(new GridModel(Return(ProductRepository.GetAllProducts())));
        }
    }
}
