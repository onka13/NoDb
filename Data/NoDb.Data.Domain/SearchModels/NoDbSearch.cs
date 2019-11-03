using System;
using System.Collections.Generic;
using System.Text;

namespace NoDb.Data.Domain.SearchModels
{
    public class NoDbSearch
    {
        public List<NoDbSearchItem> Items { get; set; }

        public NoDbSearch()
        {
            Items = new List<NoDbSearchItem>();
        }
    }
}
