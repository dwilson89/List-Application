using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using List_manager.Models;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;

namespace List_manager.Controllers
{
    public class MALAccountController : Controller
    {

        private IMemoryCache _cache;

        public MALAccountController(IMemoryCache cache)
        {
            _cache = cache;
        }

        // GET: /MALAccount/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // GET: /MALAccount/Login
        [HttpPost]
        public async Task<IActionResult> Login(MALUserLogin malUser, string returnUrl = null)
        {
            //query the api

            //if the response return is 200 then its aok
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var success = await MALApi.MALVerify(malUser.Username, malUser.Password);

                if (success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim("Username", malUser.Username, ClaimValueTypes.String),
                        new Claim("Secret", malUser.Password, ClaimValueTypes.String)
                    };

                    var userIdentity = new ClaimsIdentity(claims, "MAL");

                    var principal = new ClaimsPrincipal(userIdentity);

                    //TODO: might add in an expiration date
                    await HttpContext.Authentication.SignInAsync("MALCookie", principal);

                    _cache.CreateEntry("MALAnimeList");

                    _cache.Set("MALAnimeList", await MALApi.MALUserInfo(malUser.Username, "all", "anime"));

                    return Redirect(returnUrl);

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(malUser);
                }
            }

            return View(malUser);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.Authentication.SignOutAsync("MALCookie");
            
            //TODO redirect to index page
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


    }
}