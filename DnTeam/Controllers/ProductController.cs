using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DnTeamData;
using DnTeamData.Models;
using Telerik.Web.Mvc;
using DnTeam.Models;

namespace DnTeam.Controllers
{
    [OpenIdAuthorize]
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
        public ActionResult Select(List<string> filterQuery)
        {
            if (filterQuery != null && filterQuery.Count() > 0)
            {
                var result = Return().Filter(filterQuery);
                return View(new GridModel(result));
            }

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
        private string GetTransactionStatusCode(ProductEditStatus status)
        {
            switch (status)
            {
                case ProductEditStatus.Ok:
                    return null;

                case ProductEditStatus.DuplicateItem:
                    return Resources.Labels.Product_Error_Duplicate_Item;

                case ProductEditStatus.NameIsEmpty:
                    return string.Format("{0} {1}", Resources.Labels.Products_Name, Resources.Labels.Error_Is_Empty);

                case ProductEditStatus.ClientIsInvalid:
                    return string.Format("{0} {1}", Resources.Labels.Products_Client, Resources.Labels.Error_Is_Empty);

                default:
                    return Resources.Labels.Error_Default;
            }
        }
    }
}
