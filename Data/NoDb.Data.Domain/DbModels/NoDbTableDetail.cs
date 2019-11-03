using NoDb.Data.Domain.Enums;
using System.ComponentModel;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbTableDetail
    {
        [Category("Identity")]
        public string Name { get; set; }

        [Category("Identity")]
        //[TypeConverter(typeof(MultilineStringConverter))]
        public string Description { get; set; }

        [Description("Ignore this item for generating!")]
        public bool Ignored { get; set; }

        public string TitleColumn { get; set; }

        public string Schema { get; set; }

        [Category("Connection")]
        public NoDbConnectionType ConnectionType { get; set; }

        [Category("Connection")]
        public string ConnectionName { get; set; }

        public override string ToString()
        {
            return Name;
        }        
    }
}
