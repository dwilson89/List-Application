using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace List_manager.Models
{
    public class MALUserInfo
    {
        [XmlElement("user_id")]
        public int User_Id { get; set; }
        [XmlElement("user_name")]
        public string User_Name { get; set; }
        [XmlElement("user_watching")]
        public int User_Watching { get; set; }
        [XmlElement("user_completed")]
        public int User_Completed { get; set; }
        [XmlElement("user_onhold")]
        public int User_Onhold { get; set; }
        [XmlElement("user_dropped")]
        public int User_Dropped { get; set; }
        [XmlElement("user_plantowatch")]
        public int User_PlanToWatch { get; set; }
        [XmlElement("user_days_spent_watching")]
        public float User_Days_Spent_Watching { get; set; }
    }
}
