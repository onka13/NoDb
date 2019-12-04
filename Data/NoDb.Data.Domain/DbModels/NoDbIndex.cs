using NoDb.Data.Domain.Base;
using NoDb.Data.Domain.Converters;
using System.Collections.Generic;
using System.ComponentModel;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbIndex : NoDbBase
    {
        [Description("Index name")]
        [Category("Identity")]
        public string Name { get; set; }

        [Description("True if index is also a primary key.")]
        [Category("General")]
        public bool IsPrimaryKey { get; set; }

        [Description("Is index unique?")]
        [Category("General")]
        public bool IsUnique { get; set; }

        [Category("General")]
        [TypeConverter(typeof(NoDbCollectionTypeConverter))]
        public List<NoDbIndexColumn> Columns { get; set; }

        public NoDbIndex()
        {
            Columns = new List<NoDbIndexColumn>();
            Name = "Index_" + Hash;
        }

        public override string ToString()
        {
            return Name ?? "Index";
        }
    }
}
