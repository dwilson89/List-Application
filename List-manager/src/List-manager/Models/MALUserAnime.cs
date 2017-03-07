using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace List_manager.Models
{
    public class MALUserAnime
    {
        [XmlElement("series_animedb_id")]
        public int Series_Animedb_Id { get; set; }
        [XmlElement("series_title")]
        public string Series_Title { get; set; }
        [XmlElement("series_synonyms")]
        public string Series_Synonyms { get; set; }
        [XmlElement("series_type")]
        public int Series_Type { get; set; }
        [XmlElement("series_episodes")]
        public int Series_Episodes { get; set; }
        [XmlElement("series_status")]
        public int Series_Status { get; set; }
        [XmlElement("series_start")]
        public string Series_Start { get; set; }
        [XmlElement("series_end")]
        public string Series_End { get; set; }
        [XmlElement("series_image")]
        public string Series_Image { get; set; }
        [XmlElement("my_id")]
        public int My_Id { get; set; }
        [XmlElement("my_watched_episodes")]
        public int My_Watched_Episodes { get; set; }
        [XmlElement("my_start_date")]
        public string My_Start_Date { get; set; }
        [XmlElement("my_finish_date")]
        public string My_Finish_Date { get; set; }
        [XmlElement("my_score")]
        public int My_Score { get; set; }
        //1/watching, 2/completed, 3/onhold, 4/dropped, 6/plantowatch
        [XmlElement("my_status")]
        public string My_Status { get; set; }
        [XmlElement("my_rewatching")]
        public string My_Rewatching { get; set; }
        [XmlElement("my_rewatching_ep")]
        public string My_Rewatching_Ep { get; set; }
        [XmlElement("my_last_updated")]
        public string My_Last_Updated { get; set; }
        [XmlElement("my_tags")]
        public string My_Tags { get; set; }

        public string StatusToString()
        {
            string my_status;
            switch (My_Status)
            {
                case "1":
                    my_status = "Watching";
                    break;
                case "2":
                    my_status = "Completed";
                    break;
                case "3":
                    my_status = "On Hold";
                    break;
                case "4":
                    my_status = "Dropped";
                    break;
                case "6":
                    my_status = "Plan to Watch";
                    break;
                default:
                       
                    my_status = My_Status;
                    break;
            }

            return my_status;
        }
        
}
}
