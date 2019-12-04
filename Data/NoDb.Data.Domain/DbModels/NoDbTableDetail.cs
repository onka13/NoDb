using NoDb.Data.Domain.Enums;
using System.ComponentModel;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbTableDetail
    {
        [Description("Table name")]
        [Category("Identity")]
        public string Name { get; set; }

        [Description("Table description")]
        [Category("Identity")]
        //[TypeConverter(typeof(MultilineStringConverter))]
        public string Description { get; set; }

        [Description("Ignore this item for generating!")]
        public bool Ignored { get; set; }

        [Description("A column name will be used for 1-n relations.")]
        public string TitleColumn { get; set; }

        [Description("Table schema")]
        public string Schema { get; set; }

        [Description("Connection type")]
        [Category("Connection")]
        public NoDbConnectionType ConnectionType { get; set; }

        [Description("Connection name")]
        [Category("Connection")]
        public string ConnectionName { get; set; }

        [Description("Custom base class")]
        public string BaseFullName { get; set; }

        public override string ToString()
        {
            return Name;
        }        
    }
}
