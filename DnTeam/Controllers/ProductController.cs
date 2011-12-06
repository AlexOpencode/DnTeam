using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DnTeamData;
using Telerik.Web.Mvc;
using DnTeam.Models;

namespace DnTeam.Controllers
{
    public class ProductController : Controller
    {

        public ActionResult Index()
        {
            ViewData["Clients"] = new SelectList(ClientRepository.GetClientsDictionary(), "key", "value");
            return View();
        }

        [NonAction]
        private IEnumerable<EditableProductModel> Return()
        {
            return ProductRepository.GetAllProducts().Select(o => new EditableProductModel { Name = o.Name, Id = o.ToString(), Client = o.ClientName() });
        }
            
        [GridAction]
        public ActionResult Select()
        {
            return View(new GridModel(Return()));
        }

        [GridAction]
        public ActionResult Insert(EditableProductModel model)
        {
            if (TryUpdateModel(model))
            {
                ProductRepository.InsertProduct(model.Name, model.Client);
            }
            return View(new GridModel(Return()));
        }

        [GridAction]
        public ActionResult Save(string id, EditableProductModel model)
        {
            if (TryUpdateModel(model))
            {
                ProductRepository.UpdateProduct(id, model.Name, model.Client);
            }
            return View(new GridModel(Return()));
        }

        [GridAction]
        public ActionResult Delete(string id)
        {
            ProductRepository.DeleteProduct(id);
            return View(new GridModel(Return()));
        }
    }
}
