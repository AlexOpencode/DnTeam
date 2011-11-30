using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AccountController : Controller
    {
        private static readonly OpenIdRelyingParty openid = new OpenIdRelyingParty();

        [NonAction]
        private PersonModel MapPersonToModel(Person person, Dictionary<string, string> personsList)
        {
            return (person == null) ? new PersonModel() : new PersonModel
            {
                Id = person.PersonId,
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
                TechnologySpecialties = person.TechnologySpecialties,
                OpenId = person.OpenId
            };
        }

        [NonAction]
        private IEnumerable<PersonGridModel> Return(bool isActive = true)
        {
            return PersonRepository.GetAllPersons(isActive).Select(o => new PersonGridModel
                                                                     {
                UserId = o.PersonId,
                PrimaryManager = o.PrimaryManagerName,
                UserName = o.Name,
                Location = o.LocationName,
                TechnologySkills = (o.TechnologySpecialties.Count > 0) 
                    ? o.TechnologySpecialties.Select(s => s.Name).Aggregate((workingSentence, next) => next + ", " + workingSentence)
                    : string.Empty
            });
        }

        public ActionResult LogIn ()
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
            var response = openid.GetResponse();

            if (response == null)
            {
                //user submitting Identifier
                Identifier id;
                if (Identifier.TryParse(Request.Form["openid_identifier"], out id))
                {
                    //Validate Identifier is assigned to an active user in the database
                    

                    if (string.IsNullOrEmpty(PersonRepository.ValidateIdentifier(id.ToString())))
                    {
                        ViewData["Message"] = "User with such OpenId is not registered or is not active.";
                        return View("LogIn");
                    }

                    try
                    {
                        return openid.CreateRequest(Request.Form["openid_identifier"]).RedirectingResponse.AsActionResult();
                    }
                    catch (ProtocolException ex)
                    {
                        ViewData["Message"] = ex.Message;
                        return View("LogIn");
                    }
                }
                else
                {
                    ViewData["Message"] = "Invalid identifier";
                    return View("Login");
                }
            }
            else
            {
                // OpenID Provider sending assertion response
                switch (response.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        Session["FriendlyIdentifier"] = PersonRepository.ValidateIdentifier(response.ClaimedIdentifier);
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
            }
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult Details(string id)
        {
            var personsList = PersonRepository.GetActivePersonsList();
            var model = MapPersonToModel(PersonRepository.GetPerson(id), personsList);
            model.PhotoUrl = string.IsNullOrEmpty(model.PhotoUrl) ? "../../Content/noImage.jpg" : model.PhotoUrl;
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var personsList = PersonRepository.GetActivePersonsList();
            var model = MapPersonToModel(PersonRepository.GetPerson(id), personsList);
            ViewData["PersonsList"] = new SelectList(personsList, "key", "value");
            ViewData["LocationsList"] = new SelectList(DepartmentRepository.GetDepartmentsDictionary(), "key", "value"); 
            ViewData["TechnologySpecialtyNames"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.TechnologySpecialtyNames));
            ViewData["TechnologySpecialtyLevels"] = SettingsRepository.GetSettingValues(EnumName.TechnologySpecialtyLevels);
            return View(model);
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewData["PersonsList"] = new SelectList(PersonRepository.GetActivePersonsList(), "key", "value");
            ViewData["LocationsList"] = new SelectList(DepartmentRepository.GetDepartmentsDictionary(), "key", "value"); 
            return View();
        }

        [HttpGet]
        public ActionResult Inactive()
        {
            return View();
        }

        [GridAction]
        public ActionResult Insert(PersonGridModel model)
        {
            if (TryUpdateModel(model))
            {
                PersonRepository.CreatePerson(model.UserName, model.Location, model.PrimaryManager);
            }

            return View(new GridModel(Return()));
        }

        //[GridAction]
        //public ActionResult Save(string id, PersonGridModel model)
        //{
        //    if (TryUpdateModel(model))
        //    {
        //        PersonRepository.UpdatePerson(id, model.UserName, model.Location, model.PrimaryManager);
        //    }

        //    return View(new GridModel(Return()));
        //}

        [GridAction]
        public ActionResult Delete(string id)
        {
            PersonRepository.DeletePerson(id);
            return View(new GridModel(Return()));
        }

        [GridAction]
        public ActionResult Select()
        {
            return View(new GridModel(Return()));
        }

        [GridAction]
        public ActionResult SelectInActive()
        {
            return View(new GridModel(Return(false)));
        }

        #region Update Person Data
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
                        return new JsonResult { Data = "OpenId Identifier is user by other user." };

                    //assign valid OpenId format to save in the database
                    value = iden.ToString();
                }
                else
                {
                    return new JsonResult {Data = "OpenId Identifier is not valid."};
                }
            }

            return new JsonResult { Data = PersonRepository.UpdateProperty(id, name, value) };
        }

        [HttpPost]
        public ActionResult AddElementToPersonProperty(string id, string name, string value)
        {
            return new JsonResult { Data = PersonRepository.AddValueToPropertySet(id, name, value) };
        }

        [HttpPost]
        public ActionResult DeleteElementFromPersonProperty(string id, string name, string value)
        {
            return new JsonResult { Data = PersonRepository.DeleteValueFromPropertySet(id, name, value) };
        }
        #endregion

        #region Status Codes
        private static string ErrorCodeToString(PersonCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case PersonCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case PersonCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case PersonCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case PersonCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case PersonCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case PersonCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case PersonCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
