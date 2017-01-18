using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace List_manager.Models
{
    [XmlRoot("anime")]
    public class AnimeList
    {
        public AnimeList() { EntryList = new List<Anime>(); }
        [XmlElement("entry")]
        public List<Anime> EntryList { get; set; }
    }
}
