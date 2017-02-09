using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace List_manager.Models
{
    public class UserAnime
    {
        public int UserAnimeID { get; set; }
        public string ApplicationUserId { get; set; }
        public int AnimeID { get; set; }
        public string User_Status { get; set; }

        public Anime Anime { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
       
    }
}
