using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using List_manager.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;

namespace List_manager.Controllers
{
    public class MALAccountController : Controller
    {
        // GET: /MALAccount/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // GET: /MALAccount/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(MALUser malUser, string returnUrl = null)
        {
            //query the api

            //if the response return is 200 then its aok
            ViewData["ReturnUrl"] = returnUrl;

            var success = false;

            const string uri = "https://myanimelist.net/api/account/verify_credentials.xml";
            using (var httpClient = new HttpClient())
            {
                //need to do this properly in the future
                var byteArray = Encoding.ASCII.GetBytes($"{malUser.Username}:{malUser.Password}");
                var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                httpClient.DefaultRequestHeaders.Authorization = header;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                var result = await httpClient.GetAsync(uri);

                success = result.IsSuccessStatusCode;
               
            }

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

                return Redirect(returnUrl);

            } else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(malUser);
            }

            return View(malUser);
        }

       


    }
}