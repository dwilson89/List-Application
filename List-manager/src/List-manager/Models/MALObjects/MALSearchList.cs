using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace List_manager.Models.MALObjects
{
    [XmlRoot("anime")]
    public class MALSearchList
    {
        public MALSearchList() { EntryList = new List<Entry>(); }
        [XmlElement("entry")]
        public List<Entry> EntryList { get; set; }

    }
}
