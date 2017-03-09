using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace List_manager.Models
{
    public class AnimeResult
    {

        [Display(Name = "User Status")]
        public string User_Status { get; set; }

        [NotMapped]
        [Display(Name = "MAL User Status")]
        public string MAL_User_Status { get; set; }

        public Anime Anime { get; set; }

    }
}
