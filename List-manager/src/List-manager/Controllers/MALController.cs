using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using List_manager.Data;
using Microsoft.AspNetCore.Identity;
using List_manager.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


/*TODO - Once Implemented on the Front End properly finish off*/
namespace List_manager.Controllers
{
    [Authorize(Policy = "MALApiPolicy")]
    public class MALController : Controller
    {

        private readonly ApplicationDbContext _context;
        private IMemoryCache _cache;
        private readonly UserManager<ApplicationUser> _userManager;


        public MALController(ApplicationDbContext context, IMemoryCache cache, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _cache = cache;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        //Add

        // GET: MAL/Add
        public IActionResult Add(int malid, string animeName)
        {
            ViewData["anime"] = animeName;
            ViewData["animeID"] = malid;
            return View(new UserAnimeData());
        }

        // POST: MAL/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("Episode,Status,Score,Storage_Type,Storage_Value,Times_Rewatched," +
            "Rewatch_Value,Date_Start,Date_Finish,Priority,Enable_Discussion,Enable_Rewatching,Comments,Tags")] UserAnimeData data, int animeId)
        {
            MALUserLogin malUser = GetUserCredentials();

            if (malUser == null)
            {
                return RedirectToAction("Login", "MALAccount", new { returnUrl = "/Animes/Search" });
            }

            if (ModelState.IsValid)
            {
                HttpResponseMessage res = await MALApi.MALAdd(malUser.Username, malUser.Password, animeId, data);

                if (res.IsSuccessStatusCode && res.ReasonPhrase == "Created")
                {
                    //Set something to indicate to the user that it was a success
                    
                } else if(!res.IsSuccessStatusCode)
                {
                    //try updating it
                    return await Update(data, animeId);

                } else
                {
                    //something went wrong
                }
               

            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        //Update
        
        // GET: MAL/Add
        public async Task<IActionResult> Update(int malid, string animeName)
        {
            ViewData["anime"] = animeName;
            ViewData["animeID"] = malid;

            MALUserLogin malUser = GetUserCredentials();

            if (malUser == null)
            {
                return RedirectToAction("Login", "MALAccount", new { returnUrl = "/Animes/Search" });
            }

            var malDict = await GetMALUserDictionary(malUser);

            UserAnimeData userAnimeData = new UserAnimeData(malDict.MALAnimeDictionary[malid]);

            return View(new UserAnimeData());
        }
        

        // POST: MAL/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([Bind("Episode,Status,Score,Storage_Type,Storage_Value,Times_Rewatched,Rewatch_Value,Date_Start,Date_Finish,Priority,Enable_Discussion,Enable_Rewatching,Comments,Tags")] UserAnimeData data, int animeId)
        {

            MALUserLogin malUser = GetUserCredentials();

            if(malUser == null)
            {
                return RedirectToAction("Login", "MALAccount", new { returnUrl = "/Animes/Search" });
            }

            if (ModelState.IsValid)
            {
                HttpResponseMessage res = await MALApi.MALUpdate(malUser.Username, malUser.Password, animeId, data);

                if (res.IsSuccessStatusCode)
                {
                    //Set something to indicate to the user that it was a success
                    
                } else
                {
                    string reqMess = await res.Content.ReadAsStringAsync();
                }

            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        // GET: MAL/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anime = await _context.Anime.SingleOrDefaultAsync(m => m.ID == id);
            if (anime == null)
            {
                return NotFound();
            }

            return View(anime);
        }

        // POST: MAL/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            MALUserLogin malUser = GetUserCredentials();

            if (malUser == null)
            {
                return RedirectToAction("Login", "MALAccount", new { returnUrl = "/Animes/Search" });
            }

            HttpResponseMessage res = await MALApi.MALDelete(malUser.Username, malUser.Password, id);

            if (res.IsSuccessStatusCode)
            {
                //Set something to indicate to the user that it was a success
                return View();
            }
            else
            {
                string reqMess = await res.Content.ReadAsStringAsync();
            }

            return View();

        }

        // GET: MAL/Search/Bleach
        public async Task<IActionResult> Search(string searchString)
        {

            MALUserLogin malUser = GetUserCredentials();

            if (malUser == null)
            {
                return RedirectToAction("Login", "MALAccount", new { returnUrl = "/Animes/Search" });
            }

            MALSearchList list = new Models.MALSearchList();
            SearchResultViewModel searchResults = new SearchResultViewModel(); 
            if (!String.IsNullOrEmpty(searchString))
            {

                list = await MALApi.MALSearch(malUser.Username, malUser.Password, searchString);
                _cache.CreateEntry("SearchResults");
                _cache.Set("SearchResults", list);

                var user = await GetCurrentUserAsync();
                var userId = user?.Id;

                List<UserAnime> anime = _context.UserAnimes.Where(d => d.ApplicationUserId == userId).Include(u => u.Anime).ToList();

                var malDict = await GetMALUserDictionary(malUser);

                List <MALUserAnime> malList = malDict.MALAnimeDictionary.Values.ToList();

                var combineResults = from entrylist in list.EntryList
                                     from malAnimeList in malList.Where(f => f.Series_Animedb_Id == entrylist.MALID).DefaultIfEmpty()
                                     from animeUser in anime.Where(aua => aua.Anime.MALID == entrylist.MALID).DefaultIfEmpty()
                                     select new AnimeResult { Anime = entrylist, MAL_User_Status = malAnimeList == null ? null : malAnimeList.StatusToString(), User_Status = animeUser == null ? null : animeUser.User_Status};

                searchResults.SearchResults.AddRange(combineResults.ToList());

            }

            return View(searchResults);
        }

        private async Task<MALUserDictionary> GetMALUserDictionary(MALUserLogin malUser)
        {
            
            MALUserDictionary malUserDict;

            if (!_cache.TryGetValue("MALAnimeList", out malUserDict))
            {
                MALUserList malUserList = await MALApi.MALUserInfo(malUser.Username, "all", "anime");

                malUserDict = new MALUserDictionary(malUserList);

                _cache.Set("MALAnimeList", malUserDict);
            }

            return malUserDict;
        }

        private MALUserLogin GetUserCredentials()
        {
            var malCookiesAuth = HttpContext.Authentication.GetAuthenticateInfoAsync("MALCookie");

            if (malCookiesAuth.Result.Description.AuthenticationScheme == null)
            {
                return null;
            }

            //todo add in exception handling if something goes wrong
            var claims = malCookiesAuth.Result.Principal.Claims;
            string userName = claims.First(p => p.Type == "Username").Value;
            string password = claims.First(p => p.Type == "Secret").Value; 

            return new MALUserLogin { Username = userName, Password = password };
        }



    }
}