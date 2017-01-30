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
using System.Text;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using Microsoft.Extensions.Caching.Memory;

namespace List_manager.Controllers
{
    public class AnimesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IMemoryCache _cache;

        public AnimesController(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
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
        public async Task<IActionResult> Create([Bind("MALID,End_Date,English,Episodes,Image,Score,Start_Date,Status,Synonyms,Synopsis,Title,Type,User_Status")] Anime anime)
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
        public async Task<IActionResult> Edit(int id, [Bind("MALID,End_Date,English,Episodes,Image,Score,Start_Date,Status,Synonyms,Synopsis,Title,Type,User_Status")] Anime anime)
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
            return _context.Anime.Any(e => e.ID == id);
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
        public async Task<IActionResult> AddToDB([Bind("MALID,End_Date,English,Episodes,Image,Score,Start_Date,Status,Synonyms,Synopsis,Title,Type,User_Status")]Anime anime, string returnUrl = null)
        {
            ViewData["return"] = returnUrl;
            if (ModelState.IsValid)
            {
                _context.Add(anime);
                await _context.SaveChangesAsync();
                return Redirect(returnUrl);
            }
            return View(anime);
        }


        [Authorize(ActiveAuthenticationSchemes = "MALCookie,Default")]
        public async Task<IActionResult> Search(string searchString)
        {

            //Might be an alternative way of doing this, look into it
            //var claims = HttpContext.User.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);
            //string userName = claims["Username"];
            //string password = claims["Secret"];

            string userName = HttpContext.User.Claims.First(p => p.Type == "Username").Value;
            string password = HttpContext.User.Claims.First(p => p.Type == "Secret").Value;


            Models.AnimeList list = new Models.AnimeList();
            if (!String.IsNullOrEmpty(searchString))
            {
                const string uri = "https://myanimelist.net/api/anime/search.xml?q=";
                using (var httpClient = new System.Net.Http.HttpClient())
                {
                    //need to do this properly in the future
                    var byteArray = Encoding.ASCII.GetBytes($"{userName}:{password}");
                    var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    httpClient.DefaultRequestHeaders.Authorization = header;
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                    var result = await httpClient.GetStringAsync(uri + searchString);

                    list = XMLToObject(result);
                    _cache.CreateEntry("SearchString");
                    _cache.CreateEntry("SearchResults");

                    //_cache.Set("SearchString", searchString);
                    _cache.Set("SearchResults", list);
                }
            }

            return View(list);
        }

        //Might need to rewrite this in the future to use XmlSerializerInputFormatter - also put into using context -using the reader
        private static Models.AnimeList XMLToObject(string xml)
        {

            XmlSerializer serializer = null;
            XmlReader reader = null;
            Models.AnimeList animeList = new Models.AnimeList();
            try
            {
                MemoryStream s = new MemoryStream(Encoding.UTF8.GetBytes(xml));
                serializer = new XmlSerializer(typeof(Models.AnimeList));
                reader = XmlReader.Create(s);
                animeList = (Models.AnimeList)serializer.Deserialize(reader);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.StackTrace);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();

                }

            }

            return animeList;
        }

        

    }


}

