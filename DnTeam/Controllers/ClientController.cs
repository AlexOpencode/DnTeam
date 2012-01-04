using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web.Mvc;
using DnTeamData;
using Telerik.Web.Mvc;
using DnTeam.Models;
using DnTeamData.Models;

namespace DnTeam.Controllers
{

    [OpenIdAuthorize]
    public class ClientController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [NonAction]
        private IEnumerable<ClientModel> Return()
        {
            return ClientRepository.GetAllClients().Select(o => new ClientModel { Name = o.Name, Id = o.ToString() });
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

        public ActionResult MultipleInsert(string value)
        {
            ClientRepository.InsertClients(Common.SplitValues(value));
            return Content("");
        }
        
        public ActionResult Save(string id, string name)
        {
            return new JsonResult { Data = GetTransactionStatusCode(ClientRepository.UpdateClient(id, name)) };
        }
        
        public ActionResult Delete(List<string> values)
        {
            ClientRepository.DeleteClients(values);

            return Content("");
        }

        [NonAction]
        private string GetTransactionStatusCode(ClientEditStatus status)
        {
            switch (status)
            {
                case ClientEditStatus.Ok:
                    return null;

                case ClientEditStatus.DuplicateItem:
                    return Resources.Labels.Client_Error_Duplicate_Item;

                case ClientEditStatus.NameIsEmpty:
                    return string.Format("{0} {1}", Resources.Labels.Client_Name, Resources.Labels.Error_Is_Empty);

                default:
                    return Resources.Labels.Error_Default;
            }
        }
    }
}
