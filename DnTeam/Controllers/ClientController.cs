using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DnTeamData;
using Telerik.Web.Mvc;
using DnTeam.Models;
using DnTeamData.Models;

namespace DnTeam.Controllers
{
    public class ClientController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [NonAction]
        private IEnumerable<EditableClientModel> Return(List<Client> clients)
        {
            return clients.Select(o => new EditableClientModel { Name = o.Name, Id = o.ClientId });
        }
            
        [GridAction]
        public ActionResult Select()
        {
            return View(new GridModel(Return(ClientRepository.GetAllClients())));
        }

        [GridAction]
        public ActionResult Insert(string name, List<string> links)
        {
            ClientRepository.InsertClient(name);
            return View(new GridModel(Return(ClientRepository.GetAllClients())));
        }

        [HttpPost]
        public ActionResult AddLink(string name, string link)
        {
            ClientRepository.AddLink(name, link);
            return Content("");
        }

        [HttpPost]
        public ActionResult RemoveLink(string name, string link)
        {
            ClientRepository.RemoveLink(name, link);
            return Content("");
        }

        [GridAction]
        public ActionResult Save(string id, string name)
        {
            ClientRepository.UpdateClient(id, name);
            return View(new GridModel(Return(ClientRepository.GetAllClients())));
        }

        [GridAction]
        public ActionResult Delete(string id)
        {
            ClientRepository.DeleteClient(id);
            return View(new GridModel(Return(ClientRepository.GetAllClients())));
        }
    }
}
