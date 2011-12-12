using System.Web.Mvc;

namespace DnTeam
{
    public class OpenIdAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.HttpContext.Response.Redirect(string.Format("~/Person/LogIn?ReturnUrl={0}", filterContext.HttpContext.Request.Url));
            }
        }
    }
}