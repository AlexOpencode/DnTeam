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
    public class ClientController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [NonAction]
        private IEnumerable<ClientModel> Return(IEnumerable<Client> clients)
        {
            return clients.Select(o => new ClientModel { Name = o.Name, Id = o.ToString()});
        }
            
        [GridAction]
        public ActionResult Select()
        {
            return View(new GridModel(Return(ClientRepository.GetAllClients())));
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
        private string GetTransactionStatusCode(TransactionStatus status)
        {
            switch (status)
            {
                case TransactionStatus.Ok:
                    return null;

                case  TransactionStatus.DuplicateItem:
                    return "Client with such name already exists. Please, enter other name.";

                default:
                    return "Undefined error occured. Please, contact your administrator.";
            }
        }
    }
}
