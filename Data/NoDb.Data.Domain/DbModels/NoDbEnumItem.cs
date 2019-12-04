using NoDb.Data.Domain.Base;
using System.ComponentModel;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbEnumItem : NoDbBase
    {
        [Description("Item name")]
        public string Name { get; set; }

        [Description("Description")]
        public string Description { get; set; }

        [Description("Item value")]
        public int Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
