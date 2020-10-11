using NoDb.Data.Domain.Converters;
using System.ComponentModel;

namespace NoDb.Data.Domain.SearchModels
{
    public class NoDbSearchGridColumn
    {
        [TypeConverter(typeof(ColumnConverter))]
        public string ColumnName { get; set; }

        public string Title { get; set; }

        public string RelationColumnName { get; set; }

        public bool SkipForJs { get; set; }

        [Description("Custom Component Name")]
        public string Component { get; set; }

        public override string ToString()
        {
            return ColumnName + " " + Title;
        }
    }
}
