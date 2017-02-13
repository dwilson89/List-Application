using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using List_manager.Data;
using List_manager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;

namespace List_manager.Controllers
{
    [Authorize(Policy = "DefaultPolicy")]
    public class AnimesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IMemoryCache _cache;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnimesController(ApplicationDbContext context, IMemoryCache cache, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _cache = cache;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Animes
        public async Task<IActionResult> Index()
        {



            var user = await GetCurrentUserAsync();
            var userId = user?.Id;

            var anime = _context.UserAnimes.Where(d =>d.ApplicationUserId == userId).Include(d => d.Anime).Select(s => s.Anime);

            var anime2 = _context.UserAnimes.Where(d => d.ApplicationUserId == userId).Include(d => d.Anime);

            //Alternative method, needs a bit more manipulation though
            var appuser = await _context.Users.Include(s => s.UserAnime).ThenInclude(e => e.Anime).AsNoTracking().SingleOrDefaultAsync(m => m.Id == userId);

            return View(await anime2.ToListAsync());
           
        }

        // GET: Animes/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Animes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Animes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MALID,End_Date,English,Episodes,Image,Score,Start_Date,Status,Synonyms,Synopsis,Title,Type")] Anime anime)
        {
            if (ModelState.IsValid)
            {
                _context.Add(anime);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(anime);
        }

        // GET: Animes/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: Animes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MALID,End_Date,English,Episodes,Image,Score,Start_Date,Status,Synonyms,Synopsis,Title,Type")] Anime anime)
        {
            if (id != anime.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(anime);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimeExists(anime.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(anime);
        }

        // GET: Animes/Delete/5
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

        // POST: Animes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anime = await _context.Anime.SingleOrDefaultAsync(m => m.ID == id);
            _context.Anime.Remove(anime);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AnimeExists(int id)
        {
            return _context.Anime.Any(e => e.MALID == id);
        }


        private bool AnimeExistsForUser(int animeId, string userId)
        {
            return _context.UserAnimes.Any(e => e.AnimeID == animeId && e.ApplicationUserId.Equals(userId));
            
        }

        [HttpPost]
        public IActionResult Search(Anime anime)
        {
            TempData["returnUrl"] = Request.Headers["Referer"].ToString();
            return View("Add", anime);
        }




        //Might not be proper to rely on a cache, however given the nature for the time being this will do
        public IActionResult Add(int id)
        {
            try {
                ViewData["returnUrl"] = Request.Headers["Referer"].ToString();

                
                Anime anime = ((AnimeList)_cache.Get("SearchResults")).EntryList[id];
                

                return View(anime);

            } catch (Exception ex)//Accessing the _cache will fail
            {
                //log error
                //inform the user and redirect
            } 

            return RedirectToAction("Search");
        }

        // POST: Animes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToDB([Bind("MALID,End_Date,English,Episodes,Image,Score,Start_Date,Status,Synonyms,Synopsis,Title,Type")]Anime anime,string user_status, string returnUrl = null)
        {
            ViewData["return"] = returnUrl;
            if (ModelState.IsValid)
            {
                int f_id = 0;
                if(!AnimeExists(anime.MALID))
                {
                    _context.Add(anime);
                    await _context.SaveChangesAsync();

                    f_id = anime.ID;
                } else
                {
                    f_id = _context.Anime.First(f => f.MALID == anime.MALID).ID;
                }


                //Add to the relational table for the user and the anime added
                
                var user = await GetCurrentUserAsync();
                var userId = user?.Id;

                if (!AnimeExistsForUser(f_id, userId))
                {
                    _context.UserAnimes.Add(new UserAnime { AnimeID = f_id, ApplicationUserId = userId, User_Status = user_status });
                    await _context.SaveChangesAsync();
                }

                return Redirect(returnUrl);
            }
            return View(anime);
        }

        [Authorize(Policy = "MALApiPolicy")]
        public async Task<IActionResult> Search(string searchString)
        {

            //Add in code that checks the cache for a current search term, if it matches the previous saved searched load cached results

            //Might be an alternative way of doing this, look into it
            //var claims = HttpContext.User.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);
            //string userName = claims["Username"];
            //string password = claims["Secret"]
            
            var malCookiesAuth = HttpContext.Authentication.GetAuthenticateInfoAsync("MALCookie");

            if (malCookiesAuth.Result.Description.AuthenticationScheme == null)
            {
                return RedirectToAction("Login", "MALAccount",new { returnUrl = "/Animes/Search"});
            }

            //todo add in exception handling if something goes wrong
            var claims = malCookiesAuth.Result.Principal.Claims;
            string userName = claims.First(p => p.Type == "Username").Value;
            string password = claims.First(p => p.Type == "Secret").Value;

            Models.AnimeList list = new Models.AnimeList();
            if (!String.IsNullOrEmpty(searchString))
            {

                list = await MALApi.MALSearch(userName, password, searchString);
                _cache.CreateEntry("SearchResults");
                _cache.Set("SearchResults", list);

            }

            return View(list);
        }

    }
}

