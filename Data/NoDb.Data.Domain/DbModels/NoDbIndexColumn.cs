using NoDb.Data.Domain.Base;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.Enums;
using System.ComponentModel;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbIndexColumn : NoDbBase
    {
        [TypeConverter(typeof(ColumnConverter))]
        public string ColumnName { get; set; }

        public NoDbIndexSort Sort { get; set; }

        public override string ToString()
        {
            return ColumnName ?? "Column";
        }
    }
}
