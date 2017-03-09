﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace List_manager.Models
{
    /**
     * Class for the datatype being sent back to the MAL API in order to be used to update/Add an entry to the users list.
     * Use will be a form a user can input values into before posting the request 
     */
    [XmlRoot("entry")]
    public class UserAnimeData
    {
        [Required]
        [XmlElement("episode")]
        public int Episode { get; set; }
        //1/watching, 2/completed, 3/onhold, 4/dropped, 6/plantowatch
        [Required]
        [XmlElement("status")]
        public int Status { get; set; }
        [Required]
        [XmlElement("score")]
        public int Score { get; set; }

        //unknown that this point in time
        [XmlElement("storage_type")]
        public int? Storage_Type { get; set; }
        [XmlElement("storage_value")]
        public float? Storage_Value { get; set; }
        [XmlElement("times_rewatched")]
        public int? Times_Rewatched { get; set; }
        [XmlElement("rewatch_value")]
        public int? Rewatch_Value { get; set; }

        [XmlElement("date_start")]
        public string Date_Start_Str { get; set; }
        [XmlElement("date_finish")]
        public string Date_Finish_Str { get; set; }

        [XmlElement("priority")]
        public int? Priority { get; set; }

        //1=enable, 0=disable
        [XmlElement("enable_discussion")]
        public int Enable_Discussion {
            get;

            set;
        }

        //1=enable, 0=disable
        [XmlElement("enable_rewatching")]
        public int Enable_Rewatching { get; set; }

        [XmlElement("comments")]
        public string Comments { get; set; }

        //seperated by commma's
        [XmlElement("tags")]
        public string Tags { get; set; }

        [XmlIgnoreAttribute]
        public int Rewatch_Episodes { get; set; }

        [XmlIgnoreAttribute]
        public DateTime? Date_Start
        {
            get { return date_start; }
            set
            {
                date_start = value;

                if (value != null)
                {
                    Date_Start_Str = value?.Date.ToString("MMddyyyy");
                }
            }
        }

        [XmlIgnoreAttribute]
        public DateTime? Date_Finish
        {
            get { return date_finish; }
            set
            {
                date_finish = value;

                if(value != null)
                {
                    Date_Finish_Str = value?.Date.ToString("MMddyyyy");
                }

               
            }
        }

        [XmlIgnoreAttribute]
        private DateTime? date_start;
        [XmlIgnoreAttribute]
        private DateTime? date_finish;

        public UserAnimeData() { }

        public UserAnimeData(MALUserAnime malAnime)
        {
            if (!String.IsNullOrEmpty(malAnime.My_Finish_Date) && (malAnime.My_Finish_Date != "0000-00-00"))
                this.Date_Finish = Convert.ToDateTime(malAnime.My_Finish_Date);
            if (!String.IsNullOrEmpty(malAnime.My_Start_Date) && (malAnime.My_Start_Date != "0000-00-00"))
                this.Date_Start = Convert.ToDateTime(malAnime.My_Start_Date);

            this.Episode = malAnime.My_Watched_Episodes;
            this.Score = malAnime.My_Score;
            this.Status = Convert.ToInt32(malAnime.My_Status);
            this.Tags = malAnime.My_Tags;
            if (!String.IsNullOrEmpty(malAnime.My_Rewatching))
            {
                this.Times_Rewatched = Convert.ToInt32(malAnime.My_Rewatching);

            }
            if (!String.IsNullOrEmpty(malAnime.My_Rewatching_Ep))
            {
                this.Rewatch_Episodes = Convert.ToInt32(malAnime.My_Rewatching_Ep);
                if (this.Rewatch_Episodes > 0)
                {
                    this.Enable_Rewatching = 1;
                }
            }
        }

    }
}
