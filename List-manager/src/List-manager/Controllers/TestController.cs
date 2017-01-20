using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Http.Headers;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace List_manager.Controllers
{
    public class TestController : Controller
    {
        public async Task<IActionResult> Index(string searchString)
        {
            string userName = "Rh4istl1n";
            string password = "9279627759S";

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