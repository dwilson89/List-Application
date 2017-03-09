using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace List_manager.Models
{
    public class MALUserDictionary
    {
        public MALUserDictionary() { MALAnimeDictionary = new Dictionary<int, MALUserAnime>(); }
        public MALUserInfo UserInfo { get; set; }
        public Dictionary<int, MALUserAnime> MALAnimeDictionary { get; set; }

        public MALUserDictionary(MALUserList malList)
        {
            this.UserInfo = malList.UserInfo;
            MALAnimeDictionary = malList.MALAnimeList.ToDictionary(a => a.Series_Animedb_Id, a => a);
        }

    }
}
