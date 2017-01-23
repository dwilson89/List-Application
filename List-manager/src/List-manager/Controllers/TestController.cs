using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Http.Headers;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.AspNetCore.Identity;
using List_manager.Data;
using List_manager.Models;
using Microsoft.AspNetCore.Authorization;

namespace List_manager.Controllers
{
    public class TestController : Controller
    {
        /// <summary>
        /// Application DB context
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// User manager - attached to application DB context
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

       

        public TestController(ApplicationDbContext context, UserManager<ApplicationUser> UserManager)
        {
            _context = context;
            _userManager = UserManager;
            
        }

        [Authorize(ActiveAuthenticationSchemes = "MALCookie")]
        public async Task<IActionResult> Index(string searchString)
        {

            //Might be an alternative way of doing this, look into it
            var claims = HttpContext.User.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);

            string userName = claims["Username"];
            string password = claims["Secret"];

            var user = await _userManager.GetUserAsync(User);

            Models.AnimeList list = new Models.AnimeList();
            if (!String.IsNullOrEmpty(searchString))
            {
                const string uri = "https://myanimelist.net/api/anime/search.xml?q=";
                using (var httpClient = new HttpClient())
                {   
                    //need to do this properly in the future
                    var byteArray = Encoding.ASCII.GetBytes($"{userName}:{password}");
                    var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    httpClient.DefaultRequestHeaders.Authorization = header;
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                    var result = await httpClient.GetStringAsync(uri+searchString);

                    list = XMLToObject(result);
                   
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