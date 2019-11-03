using NoDb.Data.Domain.Base;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbEnumDetail : NoDbBase
    {
        public string Name { get; set; }

        public NoDbDataEnumType EnumType { get; set; }

        [TypeConverter(typeof(NoDbCollectionTypeConverter))]
        public List<NoDbEnumItem> Items { get; set; }

        public NoDbEnumDetail()
        {
            Items = new List<NoDbEnumItem>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
