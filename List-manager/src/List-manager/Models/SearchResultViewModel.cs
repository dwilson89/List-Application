using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace List_manager.Models
{
    public class SearchResultViewModel
    {
        public SearchResultViewModel() { SearchResults = new List<Models.AnimeResult>(); }

        public List<AnimeResult> SearchResults { get; set; }
    }
}
