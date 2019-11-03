using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NoDb.Data.Domain.SearchModels
{
    public class NoDbSearchColumnDetail
    {
        [Category("General")]
        [TypeConverter(typeof(ColumnConverter))]
        public string ColumnName { get; set; }

        [Category("General")]
        public NoDbSearchSign Sign { get; set; }

        public string Title { get; set; }

        [Category("General")]
        public NoDbSearchDisplayType Display { get; set; }

        [Category("UI")]
        public bool SkipForJs { get; set; }
        [Category("UI")]
        public bool AlwaysOn { get; set; }

        public string GetColumnPropertyName(List<NoDbSearchColumnDetail> Columns)
        {
            var parameterName = ColumnName;
            if (Columns.Count(x => x.ColumnName == parameterName) > 1)
            {
                var index = Columns.IndexOf(this);
                parameterName = ColumnName + index + Sign;
            }
            return parameterName;
        }

        public override string ToString()
        {
            return ColumnName + " " + Sign;
        }
    }
}
