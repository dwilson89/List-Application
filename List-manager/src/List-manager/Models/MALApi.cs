using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace List_manager.Models
{
    public static class MALApi
    {

        public static async Task<AnimeList> MALSearch(string username, string password, string query)
        {
            string uri = $"https://myanimelist.net/api/anime/search.xml?q={query}";

            string result;

            using (var httpClient = new System.Net.Http.HttpClient())
            {
                //need to do this properly in the future
                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                httpClient.DefaultRequestHeaders.Authorization = header;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                result = await httpClient.GetStringAsync(uri);

            }

            return  XmlConvert.DeserializeObject<AnimeList>(result);
        }

        public static async Task<HttpResponseMessage> MALAdd(string username, string password, int id, UserAnimeData data)
        {
            string uri = $"https://myanimelist.net/api/animelist/add/{id}.xml"; // need to look at how to add data

            return await PostMALAPI(username, password, uri, XmlConvert.SerializeObject(data));
        }

        public static async Task<HttpResponseMessage> MALUpdate(string username, string password, int id, UserAnimeData data)
        {

            string uri = $"https://myanimelist.net/api/animelist/update/{id}.xml";

            return await PostMALAPI(username, password, uri, XmlConvert.SerializeObject(data));

        }

        public static async Task<HttpResponseMessage> MALDelete(string username, string password, int id)
        {
            string uri = $"https://myanimelist.net/api/animelist/delete/{id}.xml";

            return await PostMALAPI(username, password, uri);

        }

        //returns a boolean value to indicate success
        public static async Task<bool> MALVerify(string username, string password)
        {
            string uri = "https://myanimelist.net/api/account/verify_credentials.xml";

            bool success = false;

            using (var httpClient = new HttpClient())
            {
                //need to do this properly in the future
                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                httpClient.DefaultRequestHeaders.Authorization = header;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                var result = await httpClient.GetAsync(uri);

                success = result.IsSuccessStatusCode;

            }

            return success;

        }
        //status - all, etc, type: anime/manga
        public static async Task<MALUserList> MALUserInfo(string username, string status, string type)
        {

            string uri = $"https://myanimelist.net/malappinfo.php?u={username}&status={status}&type={type}";

            string result;

            using (var httpClient = new System.Net.Http.HttpClient())
            {
                //need to do this properly in the future
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                result = await httpClient.GetStringAsync(uri);

            }

            return XmlConvert.DeserializeObject<MALUserList>(result);

        }

        private static async Task<HttpResponseMessage> PostMALAPI(string username, string password, string uri, string data = null)
        {
            HttpResponseMessage result;

            string parsedXmlData = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + RemoveAllNamespaces(data);

            using (var httpClient = new HttpClient())
            {
                
                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                httpClient.DefaultRequestHeaders.Authorization = header;

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                //add data
                HttpContent content = new StringContent("data=" + parsedXmlData, UTF8Encoding.UTF8, "application/x-www-form-urlencoded");

                result = await httpClient.PostAsync(uri, content);
                

            }
            return result;
        }

        //Implemented based http://stackoverflow.com/questions/987135/how-to-remove-all-namespaces-from-xml-with-c
        public static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }

        //Core recursion function
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }

        /*
        static XElement stripNS(XElement root)
        {
            return new XElement(
                root.Name.LocalName,
                root.HasElements ?
                    root.Elements().Select(el => stripNS(el)) :
                    (object)root.Value
            );
        }
            */

    }
}
