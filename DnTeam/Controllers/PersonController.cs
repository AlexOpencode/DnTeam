using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DnTeam.Models;
using DnTeamData;
using DnTeamData.Models;
using Telerik.Web.Mvc;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.Messaging;


namespace DnTeam.Controllers
{
    public class PersonController : Controller
    {
        private static readonly OpenIdRelyingParty Openid = new OpenIdRelyingParty();

        [NonAction]
        private PersonModel MapPersonToModel(Person person, Dictionary<string, string> personsList)
        {
            return (person == null) ? new PersonModel() : new PersonModel
            {
                Id = person.ToString(),
                Name = person.Name,
                DoB = person.DoB,
                Comments = person.Comments,
                Phone = person.Phone,
                PhotoUrl = person.PhotoUrl,
                LocatedIn = person.LocationId,
                PrimaryManager = person.PrimaryManagerId,
                PrimaryPeer = person.PrimaryPeerId,
                OtherManagers = personsList.Where(o => person.OtherManagersList.Contains(o.Key)),
                OtherPeers = personsList.Where(o => person.OtherPeersList.Contains(o.Key)),
                LikesToWorkWith = personsList.Where(o => person.LikesToWorkWithList.Contains(o.Key)),
                DirectReports = personsList.Where(o => person.DirectReportsList.Contains(o.Key)),
                Links = person.Links,
                OpenId = person.OpenId,
                PrimaryManagerName = person.PrimaryManagerName,
                PrimaryPeerName = person.PrimaryPeerName
            };
        }

        [NonAction]
        private IEnumerable<PersonGridModel> Return(bool isActive = true)
        {
            return PersonRepository.GetAllPeople(isActive).Select(o => new PersonGridModel
                                                                     {
                                                                         Id = o.ToString(),
                                                                         PrimaryManager = o.PrimaryManagerName,
                                                                         Name = o.Name,
                                                                         Location = o.LocationName,
                                                                         TechnologySkills = (o.TechnologySpecialties.Count > 0)
                                                                             ? o.TechnologySpecialties.Select(s => s.Name).Aggregate((workingSentence, next) => next + ", " + workingSentence)
                                                                             : string.Empty
                                                                     });
        }

        public ActionResult LogIn()
        {
            Response.AppendHeader("X-XRDS-Location", new Uri(Request.Url, Response.ApplyAppPathModifier("~/Home/xrds")).AbsoluteUri);
            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("LogIn");
        }

        [ValidateInput(false)]
        public ActionResult Authenticate(string returnUrl)
        {
            var response = Openid.GetResponse();

            if (response == null)
            {
                //user submitting Identifier
                Identifier id;
                if (Identifier.TryParse(Request.Form["openid_identifier"], out id))
                {
                    //Validate Identifier is assigned to an active user in the database
                    if (string.IsNullOrEmpty(PersonRepository.ValidateIdentifier(id.ToString())))
                    {
                        ViewData["Message"] = Resources.Labels.People_Error_Not_Found_OpenId;
                        return View("LogIn");
                    }

                    try
                    {
                        return Openid.CreateRequest(Request.Form["openid_identifier"]).RedirectingResponse.AsActionResult();
                    }
                    catch (ProtocolException ex)
                    {
                        ViewData["Message"] = ex.Message;
                        return View("LogIn");
                    }
                }
                ViewData["Message"] = Resources.Labels.People_Error_Invalid_OpenId;
                return View("Login");
            }

            // OpenID Provider sending assertion response
            switch (response.Status)
            {
                case AuthenticationStatus.Authenticated:
                    //Session["FriendlyIdentifier"] = PersonRepository.ValidateIdentifier(response.ClaimedIdentifier);
                    FormsAuthentication.SetAuthCookie(response.ClaimedIdentifier, false);

                    if (!string.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");

                case AuthenticationStatus.Canceled:
                    ViewData["Message"] = "Canceled at provider";
                    return View("LogIn");
                case AuthenticationStatus.Failed:
                    ViewData["Message"] = response.Exception.Message;
                    return View("LogIn");
            }
            return new EmptyResult();
        }

        [HttpGet]
        [OpenIdAuthorize]
        public ActionResult Details(string id)
        {
            var personsList = PersonRepository.GetActivePersonsList();
            var model = MapPersonToModel(PersonRepository.GetPerson(id), personsList);

            //customize display values
            model.PhotoUrl = string.IsNullOrEmpty(model.PhotoUrl) ? VirtualPathUtility.ToAbsolute("~/Content/noImage.jpg") : model.PhotoUrl;
            model.PrimaryManagerName = (model.PrimaryManagerName == "wanted")
                                           ? model.PrimaryManagerName
                                           : string.Format("<a href=\"{0}\">{1}</a>", Url.Action("Details", new { id = model.PrimaryManager }), model.PrimaryManagerName);
            model.PrimaryPeerName = (model.PrimaryPeerName == "wanted")
                                           ? model.PrimaryPeerName
                                           : string.Format("<a href=\"{0}\">{1}</a>", Url.Action("Details", new { id = model.PrimaryPeer }), model.PrimaryPeerName);

            model.DepartmentDescription = DepartmentRepository.GetDepartmentDescription(model.LocatedIn);

            return View(model);
        }

        [HttpGet]
        [OpenIdAuthorize]
        public ActionResult Edit(string id)
        {
            var person = PersonRepository.GetPerson(id);

            //Prevent editing if person is not active
            if (!person.IsActive)
                return RedirectToAction("Index");

            var personsList = PersonRepository.GetActivePersonsList();
            var model = MapPersonToModel(person, personsList);
            ViewData["PersonsList"] = new SelectList(personsList.Where(o => o.Key != id), "key", "value");
            ViewData["NullablePersonsList"] = personsList.Where(o => o.Key != id).ToSelectList(string.Empty, "wanted");
            ViewData["LocationsList"] = DepartmentRepository.GetDepartmentsDictionary().ToSelectList(string.Empty, "none");
            ViewData["TechnologySpecialtyNames"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.TechnologySpecialtyNames));
            ViewData["TechnologySpecialtyLevels"] = SettingsRepository.GetSettingValues(EnumName.TechnologySpecialtyLevels);

            return View(model);
        }

        [HttpGet]
        [OpenIdAuthorize]
        public ActionResult Index()
        {
            ViewData["PersonsList"] = new SelectList(PersonRepository.GetActivePersonsList(), "key", "value");
            ViewData["LocationsList"] = new SelectList(DepartmentRepository.GetDepartmentsDictionary(), "key", "value");
            return View();
        }

        [HttpGet]
        [OpenIdAuthorize]
        public ActionResult Inactive()
        {
            return View();
        }

        [OpenIdAuthorize]
        public ActionResult Insert(string name, string location, string primaryManager)
        {
            return new JsonResult { Data = GetTransactionStatusCode(PersonRepository.CreatePerson(name, location, primaryManager)) };
        }

        [OpenIdAuthorize]
        public ActionResult GetPeopleList()
        {
            return new JsonResult { Data = new SelectList(PersonRepository.GetActivePersonsList(), "key", "value") };
        }

        [OpenIdAuthorize]
        public ActionResult Delete(List<string> values)
        {
            PersonRepository.DeletePeople(values);

            return Content("");
        }

        [OpenIdAuthorize]
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

        [OpenIdAuthorize]
        [GridAction]
        public ActionResult SelectInActive(List<string> filterQuery)
        {
            if (filterQuery != null && filterQuery.Count() > 0)
            {
                var result = Return(false).Filter(filterQuery);
                return View(new GridModel(result));
            }

            return View(new GridModel(Return(false)));
        }

        #region Update Person Data
        [OpenIdAuthorize]
        [HttpPost]
        public ActionResult UpdatePersonProperty(string id, string name, string value)
        {
            //Validate OpenId
            if (name == "OpenId")
            {
                Identifier iden;
                if (Identifier.TryParse(value, out iden))
                {
                    if (!string.IsNullOrEmpty(PersonRepository.ValidateIdentifier(iden.ToString())))
                        return new JsonResult { Data = Resources.Labels.People_Error_Duplicate_OpenId };

                    //assign valid OpenId format to save in the database
                    value = iden.ToString();
                }
                else
                {
                    return new JsonResult { Data = Resources.Labels.People_Error_Invalid_OpenId };
                }
            }

            return new JsonResult { Data = GetTransactionStatusCode(PersonRepository.UpdateProperty(id, name, value)) };
        }

        [OpenIdAuthorize]
        [HttpPost]
        public ActionResult AddElementToPersonProperty(string id, string name, string value)
        {
            return new JsonResult { Data = PersonRepository.AddValueToPropertySet(id, name, value) };
        }

        [OpenIdAuthorize]
        [HttpPost]
        public ActionResult DeleteElementFromPersonProperty(string id, string name, string value)
        {
            PersonRepository.DeleteValueFromPropertySet(id, name, value);

            return new JsonResult { Data = null };
        }
        #endregion

        [NonAction]
        private static string GetTransactionStatusCode(PersonEditStatus status)
        {
            switch (status)
            {
                case PersonEditStatus.Ok:
                    return null;

                case PersonEditStatus.ErrorDuplicateName:
                    return Resources.Labels.People_Error_Duplicate_Name;

                case PersonEditStatus.ErrorDuplicateItem:
                    return Resources.Labels.People_Error_Duplicate_Item;

                case PersonEditStatus.ErrorInvalidPrimaryManager:
                    return Resources.Labels.People_Error_Invalid_Primary_Manager;

                case PersonEditStatus.ErrorInvalidLocation:
                    return Resources.Labels.People_Error_Invalid_Location;

                default:
                    return Resources.Labels.Error_Default;
            }
        }

    }
}
