using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace List_manager.Models
{
    public class SearchResultViewModel
    {
        public SearchResultViewModel() { SearchResults = new List<Models.EntryResult>(); }

        public List<EntryResult> SearchResults { get; set; }
    }
}
