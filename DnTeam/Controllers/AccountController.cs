using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DnTeam.Models;
using DnTeamData;
using DnTeamData.Models;
using Telerik.Web.Mvc;

namespace DnTeam.Controllers
{
    public class AccountController : Controller
    {
        [NonAction]
        private PersonModel MapPersonToModel(Person person, Dictionary<string, string> personsList)
        {
            return (person == null) ? new PersonModel() : new PersonModel
            {
                Id = person.PersonId,
                Name = person.Name,
                DoB = person.DoB,
                Comments = person.Comments,
                Email = person.Email,
                PhotoUrl = person.PhotoUrl,
                LocatedIn = person.LocationId,
                PrimaryManager = person.PrimaryManagerId,
                PrimaryPeer = person.PrimaryPeerId,
                OtherManagers = personsList.Where(o => person.OtherManagersList.Contains(o.Key)),
                OtherPeers = personsList.Where(o => person.OtherPeersList.Contains(o.Key)),
                LikesToWorkWith = personsList.Where(o => person.LikesToWorkWithList.Contains(o.Key)),
                DirectReports = personsList.Where(o => person.DirectReportsList.Contains(o.Key)),
                Links = person.Links,
                TechnologySpecialties = person.TechnologySpecialties
            };
        }

        [NonAction]
        private IEnumerable<PersonGridModel> Return()
        {
            return PersonsRepository.GetAllPersons().Select(o => new PersonGridModel
                                                                     {
                UserId = o.PersonId,
                Email = o.Email,
                PrimaryManager = o.PrimaryManagerName,
                UserName = o.Name,
                Location = o.LocationName,
                TechnologySkills = (o.TechnologySpecialties.Count > 0) 
                    ? o.TechnologySpecialties.Where(s => s.Level > 0).Select(s => s.Name).Aggregate((workingSentence, next) => next + ", " + workingSentence)
                    : string.Empty
            });
        }

        [HttpGet]
        public ActionResult Details(string id)
        {
            var personsList = PersonsRepository.GetPersonsList();
            var model = MapPersonToModel(PersonsRepository.GetPerson(id), personsList);
            model.PhotoUrl = string.IsNullOrEmpty(model.PhotoUrl) ? "../../Content/noImage.jpg" : model.PhotoUrl;
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var personsList = PersonsRepository.GetPersonsList();
            var model = MapPersonToModel(PersonsRepository.GetPerson(id), personsList);
            ViewData["PersonsList"] = new SelectList(personsList, "key", "value");
            ViewData["LocationsList"] = new SelectList(DepartmentRepository.GetLocationsList(), "key", "value");
            ViewData["TechnologySpecialties"] = new SelectList(SettingsRepository.GetSettingValues(EnumName.TechnologySpecialties));
            return View(model);
        }

        [HttpGet]
        public ActionResult List()
        {
            ViewData["PersonsList"] = new SelectList(PersonsRepository.GetPersonsList(), "key", "value");
            ViewData["LocationsList"] = new SelectList(DepartmentRepository.GetLocationsList(), "key", "value");
            return View();
        }

        [GridAction]
        public ActionResult Insert(PersonGridModel model)
        {
            if (TryUpdateModel(model))
            {
                PersonsRepository.CreatePerson(model.UserName, model.Location, model.PrimaryManager, model.Email);
            }

            return View(new GridModel(Return()));
        }

        [GridAction]
        public ActionResult Save(string id, PersonGridModel model)
        {
            if (TryUpdateModel(model))
            {
                PersonsRepository.UpdatePerson(id, model.UserName, model.Location, model.PrimaryManager, model.Email);
            }

            return View(new GridModel(Return()));
        }

        [GridAction]
        public ActionResult Delete(string id)
        {
            PersonsRepository.DeletePerson(id);
            return View(new GridModel(Return()));
        }

        [GridAction]
        public ActionResult Select()
        {
            return View(new GridModel(Return()));
        }

        #region Update Person Data
        [HttpPost]
        public ActionResult UpdatePersonProperty(string id, string name, string value)
        {
            return new JsonResult { Data = PersonsRepository.UpdateProperty(id, name, value) };
        }

        [HttpPost]
        public ActionResult AddElementToPersonProperty(string id, string name, string value)
        {
            return new JsonResult { Data = PersonsRepository.AddValueToPropertySet(id, name, value) };
        }

        [HttpPost]
        public ActionResult DeleteElementFromPersonProperty(string id, string name, string value)
        {
            return new JsonResult { Data = PersonsRepository.DeleteValueFromPropertySet(id, name, value) };
        }

        [HttpPost]
        public ActionResult AddTechnologySpecialty(string id, string name, int value, string lastUsed, string expSince, string note)
        {
            return new JsonResult { Data = PersonsRepository.AddTechnologySpecialty(id, name, value, lastUsed, expSince, note) };
        }

        [HttpPost]
        public ActionResult UpdateTechnologySpecialty(string id, string name, int value, string lastUsed, string expSince, string note)
        {
            return new JsonResult { Data = PersonsRepository.UpdateTechnologySpecialty(id, name, value, lastUsed, expSince, note) };
        }

        [HttpPost]
        public ActionResult DeleteTechnologySpecialty(string id, string name)
        {
            return new JsonResult { Data = PersonsRepository.DeleteTechnologySpecialty(id, name) };
        }
        #endregion

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            PersonModel model = new PersonModel();

            return View(model);
        }

        //
        // POST: /Account/Register

        //[HttpPost]
        //public ActionResult Register(RegisterModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Attempt to register the user
        //        UserCreateStatus createStatus;
        //        //Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

        //        PersonsRepository.CreateUser(model.Name, model.Email, model.Location, "password", 
        //            model.PrimaryManager,model.Comments, null, model.DoB, 
        //            false, out createStatus);
        //        if (createStatus == UserCreateStatus.Success)
        //        {
        //            //FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
        //            return RedirectToAction("List", "Account");
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", ErrorCodeToString(createStatus));
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(UserCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case UserCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case UserCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case UserCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case UserCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case UserCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case UserCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case UserCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
