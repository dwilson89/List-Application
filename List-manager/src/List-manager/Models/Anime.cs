using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace List_manager.Models
{
    public class Anime
    {
        //private static int dbId = 1;

        public Anime()
        {
            User_Status = "Plan to watch";
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        // MAL XML Fields
        [XmlElement("id")]
        public int MALID { get; set; }
        [XmlElement("title")]
        public string Title { get; set; }
        [XmlElement("english")]
        public string English { get; set; }
        [XmlElement("synonyms")]
        public string Synonyms { get; set; }
        [XmlElement("episodes")]
        public int Episodes { get; set; }
        [XmlElement("score")]
        public decimal Score { get; set; }
        [XmlElement("type")]
        public string Type { get; set; }
        [XmlElement("status")]
        public string Status { get; set; }
        [XmlElement("start_date")]
        public string Start_Date { get; set; }
        [XmlElement("end_date")]
        public string End_Date { get; set; }
        [XmlElement("synopsis")]
        public string Synopsis {
            get; set;
        }
        [XmlElement("image")]
        public string Image { get; set; }

        //Additional Fields - Will need user to submit these
        public string GetLink() { 
            return $"https://myanimelist.net/anime/{MALID}/{Title.Replace(" ", "_")}";
        }  

        public string User_Status { get; set; }

        private DateTime updated_date;

        //private bool isInitialized;
        
         //public string Genres { get; set; }
    }
}
