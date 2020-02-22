using Atlas.DataAccess.Entity;
using Atlas.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Atlas.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            Session.Abandon();
            Session.Clear();
            SetLoginCookie(null);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                var Result = AccountDAL.Login(model);
                if (!string.IsNullOrWhiteSpace(Result.UserName))
                {
                    FormsAuthentication.SetAuthCookie(Result.UserName, true);
                    SetLoginCookie(Result.UserName);
                    Session["UserId"] = Result.UserName;
                    string username = String.Format("{0} {1}", Result.FirstName, Result.LastName);
                    Session["LoggedInUser"] = username;
                    Session["CommId"] = Result.CommID;
                    Session["Role"] = Result.Role;
                    

                    if (Session["Role"] != null && (Convert.ToString(Session["Role"]) == "A" || Convert.ToString(Session["Role"]) == "M"))
                    {
                        return RedirectToAction("index", "Admin");
                    }

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", BusinessConstants.LoginFailed);
                return View();
            }
        }

        private void SetLoginCookie(string username)
        {
            string cookievalue;
            if (Request.Cookies["LoggedInUser"] != null)
            {
                cookievalue = Request.Cookies["LoggedInUser"].ToString();
            }
            else
            {
                Response.Cookies["LoggedInUser"].Value = username;
                Response.Cookies["LoggedInUser"].Expires = DateTime.Now.AddMinutes(60); // add expiry time
            }
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    if (AccountDAL.RegisterUser(model))
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                        string username = String.Format("{0} {1}", model.FirstName, model.LastName);
                        Session["LoggedInUser"] = username;
                        SetLoginCookie(username);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", BusinessConstants.DuplicateLogin);
                        return View(model);
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", BusinessConstants.contactAdmin);
                return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                if (AccountDAL.CheckforUser(model))
                {
                    return RedirectToAction("SetPassword", "manage", new { @name = model.UserName, @email = model.Email });
                }
                else
                {
                    ModelState.AddModelError("", BusinessConstants.UserNotFound);
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", BusinessConstants.contactAdmin);
                return View();
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.Clear();
            SetLoginCookie(null);
            FormsAuthentication.RedirectToLoginPage();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion Helpers
    }
}