using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace List_manager.Models
{
    public class AnimeCompare : IEqualityComparer<Anime>
    {
        public bool Equals(Anime x, Anime y)
        {
            return x.MALID == y.MALID;
        }
        public int GetHashCode(Anime codeh)
        {
            return codeh.MALID.GetHashCode();
        }

    }
}
