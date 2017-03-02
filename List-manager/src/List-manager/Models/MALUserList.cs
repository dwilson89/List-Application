using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace List_manager.Models
{
    [XmlRoot("myanimelist")]
    public class MALUserList
    {
        public MALUserList() { MALAnimeList = new List<MALUserAnime>(); }
        [XmlElement("myinfo")]
        public MALUserInfo UserInfo { get; set; }
        [XmlElement("anime")]
        public List<MALUserAnime> MALAnimeList { get; set; }
    }
}
