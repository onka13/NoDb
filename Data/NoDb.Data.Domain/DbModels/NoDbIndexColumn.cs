using NoDb.Data.Domain.Base;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.Enums;
using System.ComponentModel;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbIndexColumn : NoDbBase
    {
        [Description("Index column name")]
        [TypeConverter(typeof(ColumnConverter))]
        public string ColumnName { get; set; }

        [Description("sort value of index. asc or desc.")]
        public NoDbIndexSort Sort { get; set; }

        public override string ToString()
        {
            return ColumnName ?? "Column";
        }
    }
}
