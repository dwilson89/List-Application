﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace List_manager.Models
{
    public class UserAnime : AnimeResult
    {
        public int UserAnimeID { get; set; }
        public string ApplicationUserId { get; set; }
        public int AnimeID { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
       
    }
}
