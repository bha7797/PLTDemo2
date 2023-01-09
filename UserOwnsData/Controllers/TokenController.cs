using Microsoft.Identity.Client;
using System;
using System.Security.Claims;
using System.Web.Mvc;

using UserOwnsData.Services.Security;

namespace UserOwnsData.Controllers
{
    public class TokenController : Controller
    {
        // GET: Token
        public ActionResult Index()
        {
            try
            {
                var userClaims = ClaimsPrincipal.Current.Identity as System.Security.Claims.ClaimsIdentity;

                // TenantId is the current organization's ID in Azure AD
                string tenantId = userClaims?.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

                // objectidentifier is GUID-based identifier for Azure AD User Account of current user
                string currentUserId = userClaims?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

                // parse together Home Account ID for current user
                string homeAccountId = currentUserId + "." + tenantId;

                IConfidentialClientApplication appConfidential = (IConfidentialClientApplication)System.Web.HttpContext.Current.Session["ConfidentialClient"];
                var UserID = appConfidential.GetAccountAsync(homeAccountId).Result;
                string newAccessToken = appConfidential.AcquireTokenSilent(PowerBIPermissionScopes.ReadUserWorkspaces, UserID).WithForceRefresh(true).ExecuteAsync().Result.AccessToken;

                System.Web.HttpContext.Current.Session.Add("AccessToken", newAccessToken);
                PLTController.storeUserDetails();
                return Json(new { success = true, responseText = "Successful!" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Logout()
        {
            try
            {
                TokenManager.ClearUserCache();
                return Json(new { success = true, responseText = "Successful!" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}