using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using KillBug.Models;
using KillBug.ViewModels;
using System.Configuration;
using KillBug.Classes;
using System.Web.Services.Description;
using System.Data.Entity;
using Blog.Classes;
using System.IO;

namespace KillBug.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        public ActionResult UserProfile(ManageMessage? message)
        {
            ViewBag.StatusMessage = message == ManageMessage.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessage.ChangeEmailSuccess ? "Your email has been changed."
                : message == ManageMessage.Error ? "There has been an error."
                : message == ManageMessage.UpdateProfileSuccess ? "You have successfully updated your profile."
                : message == ManageMessage.UpdateProfileError ? "There has been an error updateing your profile."
                : "";

            var user = db.Users.Find(User.Identity.GetUserId());
            var UserProfile = new UserProfileViewModel(user);

            return View(UserProfile);
        }

        //
        // POST: /Manage/UpdateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser([Bind(Include ="Id,FirstName,LastName,AvatarPath,AddressLine1,AddressLine2,AddressCity,AddressState,AddressZip,PhoneNumber,AboutMe")] ApplicationUser profile, HttpPostedFileBase image)
        {
            if(profile != null)
            {
                try
                {
                    var dbUser = db.Users.Find(profile.Id);

                    dbUser.FirstName = profile.FirstName;
                    dbUser.LastName = profile.LastName;
                    dbUser.AvatarPath = profile.AvatarPath;
                    dbUser.AddressLine1 = profile.AddressLine1;
                    dbUser.AddressLine2 = profile.AddressLine2;
                    dbUser.AddressCity = profile.AddressCity;
                    dbUser.AddressState = profile.AddressState;
                    dbUser.AddressZip = profile.AddressZip;
                    dbUser.PhoneNumber = profile.PhoneNumber;
                    dbUser.AboutMe = profile.AboutMe;
                    if (ImageUploadValidator.IsWebFriendlyImage(image))
                    {
                        var fileName = $"avatar-{DateTime.Now.Ticks}{Path.GetExtension(image.FileName)}";
                        image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/Avatars/"), fileName));
                        dbUser.AvatarPath = $"Uploads/Avatars/{ fileName }";
                    } else
                    {
                        dbUser.AvatarPath = profile.AvatarPath;
                    }

                    db.Entry(dbUser).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("UserProfile", "Manage", new { Message = ManageMessage.UpdateProfileSuccess });
                }
                catch(Exception ex)
                {
                    return RedirectToAction("UserProfile", "Manage", new { Message = ManageMessage.UpdateProfileError });
                }
            }

            return RedirectToAction("UserProfile", "Manage", new { Message = ManageMessage.UpdateProfileError });
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessage? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessage.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessage.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessage.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessage.Error ? "An error has occurred."
                : message == ManageMessage.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessage? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessage.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessage.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangeEmail(string newEmail)
        {
            if (newEmail == null)
            {
                return RedirectToAction("UserProfile", "Manage", new { message = ManageMessage.Error });
            }
            else
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(User.Identity.GetUserId());

                user.Email = newEmail;
                user.UserName = newEmail;

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("UserProfile", new { message = ManageMessage.ChangeEmailSuccess });
            }
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword([Bind(Include = "OldPassword,NewPassword,ConfirmPassword")] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("UserProfile", "Manage", model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("UserProfile", "Manage", new { Message = ManageMessage.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessage.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessage? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessage.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessage.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessage.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessage.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
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

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessage
        {
            ChangeEmailSuccess,
            ChangePasswordSuccess,
            UpdateProfileSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error,
            UpdateProfileError
        }

        #endregion
    }
}