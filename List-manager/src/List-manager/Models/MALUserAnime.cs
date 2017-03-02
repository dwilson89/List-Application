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
        [XmlElement("my_status")]
        public int My_Status { get; set; }
        [XmlElement("my_rewatching")]
        public string My_Rewatching { get; set; }
        [XmlElement("my_rewatching_ep")]
        public string My_Rewatching_Ep { get; set; }
        [XmlElement("my_last_updated")]
        public string My_Last_Updated { get; set; }
        [XmlElement("my_tags")]
        public string My_Tags { get; set; }
    }
}
