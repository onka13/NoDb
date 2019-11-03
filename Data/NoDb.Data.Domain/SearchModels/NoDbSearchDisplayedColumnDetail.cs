using NoDb.Data.Domain.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NoDb.Data.Domain.SearchModels
{
    public class NoDbSearchDisplayedColumnDetail
    {
        [TypeConverter(typeof(ColumnConverter))]
        public string ColumnName { get; set; }

        public string Title { get; set; }

        public string RelationColumnName { get; set; }

        public bool SkipForJs { get; set; }

        public override string ToString()
        {
            return ColumnName + " " + Title;
        }
    }
}
