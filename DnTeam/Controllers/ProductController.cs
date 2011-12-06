using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DnTeamData;
using DnTeamData.Models;
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
        private IEnumerable<ProductModel> Return()
        {
            return ProductRepository.GetAllProducts().Select(o => new ProductModel { Name = o.Name, Id = o.ToString(), Client = o.ClientName() });
        }
            
        [GridAction]
        public ActionResult Select()
        {
            return View(new GridModel(Return()));
        }

        public ActionResult Insert(string name, string client, bool isClientNew)
        {
            return new JsonResult {Data = GetTransactionStatusCode(ProductRepository.InsertProduct(name, client, isClientNew))};
        }


        public ActionResult Save(string id, string name, string client, bool isClientNew)
        {
            return new JsonResult { Data = GetTransactionStatusCode(ProductRepository.UpdateProduct(id, name, client, isClientNew)) };
        }

        public ActionResult Delete(List<string> values)
        {
            ProductRepository.DeleteProducts(values);
            return Content("");
        }

        [NonAction]
        private string GetTransactionStatusCode(TransactionStatus status)
        {
            switch (status)
            {
                case TransactionStatus.Ok:
                    return null;

                case TransactionStatus.DuplicateItem:
                    return "Product with such name of the defined client already exists. Please, enter other name or select other client.";

                default:
                    return "Undefined error occured. Please, contact your administrator.";
            }
        }
    }
}
