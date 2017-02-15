using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using List_manager.Data;
using List_manager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace List_manager.Controllers
{
    [Authorize(Policy = "DefaultPolicy")]
    public class UserAnimeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAnimeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: UserAnime
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var userId = user?.Id;

            var anime = _context.UserAnimes.Where(d => d.ApplicationUserId == userId).Include(u => u.Anime).Include(u => u.ApplicationUser);
            return View(await anime.ToListAsync());
        }

        // GET: UserAnime/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAnime = await _context.UserAnimes.SingleOrDefaultAsync(m => m.UserAnimeID == id);
            if (userAnime == null)
            {
                return NotFound();
            }

            return View(userAnime);
        }

        

        // GET: UserAnime/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAnime = await _context.UserAnimes.Include(u => u.Anime).SingleOrDefaultAsync(m => m.UserAnimeID == id);
            if (userAnime == null)
            {
                return NotFound();
            }

            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["Exists"] = true;
            ViewData["AnimeID"] = new SelectList(_context.Anime, "ID", "ID", userAnime.AnimeID);
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", userAnime.ApplicationUserId);

            return View(userAnime);
        }



        // POST: UserAnime/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserAnimeID,AnimeID,ApplicationUserId,User_Status")] UserAnime userAnime)
        {
            if (id != userAnime.UserAnimeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAnime);
                    await _context.SaveChangesAsync();
                    TempData["Alert"] = "<div class=\"alert alert-success\">" +
                        "   <b>Success!</b> Entry Updated! " + 
                        "</div>";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAnimeExists(userAnime.UserAnimeID))
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
            //ViewData["AnimeID"] = new SelectList(_context.Anime, "ID", "ID", userAnime.AnimeID);
            //ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", userAnime.ApplicationUserId);

            return View(userAnime);
        }

        // GET: UserAnime/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAnime = await _context.UserAnimes.Include(a => a.Anime).SingleOrDefaultAsync(m => m.UserAnimeID == id);
            if (userAnime == null)
            {
                return NotFound();
            }

            return View(userAnime);
        }

        // POST: UserAnime/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userAnime = await _context.UserAnimes.SingleOrDefaultAsync(m => m.UserAnimeID == id);
            _context.UserAnimes.Remove(userAnime);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: UserAnime/Create
        public async Task<IActionResult> Add(int animeId)
        {
            //ViewData["AnimeID"] = new SelectList(_context.Anime, "ID", "ID");
            //ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["returnUrl"] = Request.Headers["Referer"].ToString();
            var user = await GetCurrentUserAsync();
            var userId = user?.Id;

            var anime = await _context.Anime.SingleOrDefaultAsync(m => m.ID == animeId);

            UserAnime userAnime = new Models.UserAnime {AnimeID = animeId, ApplicationUserId = userId, Anime = anime };
            ViewData["Exists"] = false;
            ViewData["returnUrl"] = TempData["returnUrl"].ToString();
            return View(userAnime);
        }


        // POST: UserAnime/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("UserAnimeID,AnimeID,ApplicationUserId,User_Status")] UserAnime userAnime, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {

                var user = await GetCurrentUserAsync();
                var userId = user?.Id;

                if (!AnimeExistsForUser(userAnime.AnimeID, userId))
                {
                    _context.UserAnimes.Add(userAnime);
                    await _context.SaveChangesAsync();
                }

                TempData["Alert"] = "<div class=\"alert alert-success\">" +
                        "   <b>Success!</b> Entry Added! " +
                        "</div>";

                return Redirect(returnUrl);

                //return RedirectToAction("Index");
            }
            //ViewData["AnimeID"] = new SelectList(_context.Anime, "ID", "ID", userAnime.AnimeID);
            //ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", userAnime.ApplicationUserId);
            return View(userAnime);
        }

        private bool UserAnimeExists(int id)
        {
            return _context.UserAnimes.Any(e => e.UserAnimeID == id);
        }

        private bool AnimeExistsForUser(int animeId, string userId)
        {
            return _context.UserAnimes.Any(e => e.AnimeID == animeId && e.ApplicationUserId.Equals(userId));

        }



        /*
        // GET: UserAnime/Create
        public IActionResult Create()
        {
            ViewData["AnimeID"] = new SelectList(_context.Anime, "ID", "ID");
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: UserAnime/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserAnimeID,AnimeID,ApplicationUserId,User_Status")] UserAnime userAnime)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userAnime);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["AnimeID"] = new SelectList(_context.Anime, "ID", "ID", userAnime.AnimeID);
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", userAnime.ApplicationUserId);
            return View(userAnime);
        }

        */

    }
}
