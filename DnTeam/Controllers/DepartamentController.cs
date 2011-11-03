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
    public class DepartamentController : Controller
    {

        [HttpPost]
        public ActionResult UpdateDapartament(string id, string name)
        {
            return new JsonResult { Data = DepartamentRepository.UpdateDepartament(id, name) };
        }

        [HttpPost]
        public ActionResult UpdateSubsidary(string id, string location, float baseCost, float baseRate, string parentId)
        {
            return new JsonResult { Data = DepartamentRepository.UpdateSubsidary(id, location, baseCost, baseRate, parentId) };
        }

        [HttpPost]
        public ActionResult DeleteDapartament(string id)
        {
            return new JsonResult { Data = DepartamentRepository.DeleteDepartament(id) };
        }

        [HttpPost]
        public ActionResult DeleteSubsidary(string id, string departamentId)
        {
            return new JsonResult { Data = DepartamentRepository.DeleteSubsidary(id, departamentId) };
        }

        [HttpPost]
        public ActionResult SaveSubsidary(string location, float baseCost, float baseRate, string departamentId)
        {
            return new JsonResult { Data = DepartamentRepository.CreateSubsidary(location, baseCost, baseRate, departamentId) };
        }

        [HttpPost]
        public ActionResult SaveDapartament(string name, string departamentOf)
        {
            return new JsonResult { Data = DepartamentRepository.CreateDepartament(name, departamentOf.Trim()) };
        }

        [NonAction]
        private void BuildDepartmentsTree(IEnumerable<TDepartament> departaments, StringBuilder sb, string parentId)
        {
            foreach (TDepartament departament in departaments)
            {
                sb.Append(string.Format("<li>{0} <input class=\"t-button t-icon t-edit\" type=\"button\" Value=\"Edit\" onclick=\"updateDepartament('{1}','{0}')\"/> <input class=\"t-button t-icon t-delete\" type=\"button\" Value=\"Delete\" onclick=\"deleteDepartament('{1}')\"/>",
                    departament.Name, departament.Id));

                sb.Append(string.Format("<li style=\"list-style: none;\"><input class=\"t-button\" type=\"button\" Value=\"+ Add Sub-departament\" onclick=\"addDepartament('{0}')\"/></li>",
                    departament.Id));
                
                if (departament.SubDepartaments.Count() > 0)
                {
                    sb.Append("<ul>");
                    BuildDepartmentsTree(departament.SubDepartaments, sb, departament.Id);
                    sb.Append("</ul>");
                }
                //Subsidaries----------------------------------------------->
                sb.Append(string.Format("<ul><li style=\"list-style: none;\">Subsidaries:</li><li style=\"list-style: none;\"><input class=\"t-button\" type=\"button\" Value=\"+ Add Subsidary\" onclick=\"addSubsidary('{0}')\"/></li>",
                    departament.Id));
                
                foreach (Subsidary sub in departament.Subsidaries)
                {
                    sb.Append(string.Format("<li>{0} /Base cost: {1}, Base rate: {2}/ <input class=\"t-button t-icon t-edit\" type=\"button\" Value=\"Edit\" onclick=\"updateSubsidary('{4}','{3}','{0}','{1}','{2}')\"/> <input class=\"t-button t-icon t-delete\" type=\"button\" Value=\"Delete\" onclick=\"deleteSubsidary('{3}','{4}')\"/></li>", 
                        sub.Location, sub.BaseCost, sub.BaseRate, sub.SubsidaryId, departament.Id));
                }
                
                sb.Append("</ul>");
                //<----------------------------------------------------------

                sb.Append("</li>");
            }
            
            
        }

        public ActionResult Index()
        {
            var departaments = DepartamentRepository.GetDepartamentsTree();
            var sb = new StringBuilder();
            sb.Append("<ul id=\"departamentsTree\"><li style=\"list-style: none;\"><input class=\"t-button\" type=\"button\" Value=\"+ Add Departament\" onclick=\"addDepartament('')\"/></li>");
            BuildDepartmentsTree(departaments, sb, string.Empty);
            sb.Append("</ul>");

            ViewBag.DepartamentsTree = sb.ToString();
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
