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

        public AnimesController(ApplicationDbContext context)
        {
            _context = context;
        }

        

        // GET: Animes
        public async Task<IActionResult> Index()
        { 

            return View(await _context.Anime.ToListAsync());
           
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

        /*
        public async Task<IActionResult> Add(int id)
        {
            try
            {
                TempData["returnUrl"] = Request.Headers["Referer"].ToString();


                Anime anime = ((AnimeList)_cache.Get("SearchResults")).EntryList[id];


                int f_id = 0;
                if (!AnimeExists(anime.MALID))
                {
                    _context.Add(anime);
                    await _context.SaveChangesAsync();

                    f_id = anime.ID;
                }
                else
                {
                    f_id = _context.Anime.First(f => f.MALID == anime.MALID).ID;
                }


                //Add to the relational table for the user and the anime added

                return RedirectToAction("Add", "UserAnime", new { animeId = f_id });

            }
            catch (Exception ex)//Accessing the _cache will fail
            {
                //log error
                //inform the user and redirect
            }

            return RedirectToAction("Search");
        }
        */

        [HttpPost, ActionName("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToDB([Bind("MALID,End_Date,English,Episodes,Image,Score,Start_Date,Status,Synonyms,Synopsis,Title,Type")]Anime anime)
        {
            TempData["returnUrl"] = Request.Headers["Referer"].ToString();
            if (ModelState.IsValid)
            {
                int f_id = 0;
                if (!AnimeExists(anime.MALID))
                {
                    _context.Add(anime);
                    await _context.SaveChangesAsync();

                    f_id = anime.ID;
                }
                else
                {
                    f_id = _context.Anime.First(f => f.MALID == anime.MALID).ID;
                }


                //Add to the relational table for the user and the anime added

                return RedirectToAction("Add", "UserAnime", new { animeId = f_id });
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}

