using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.Enums;
using System.ComponentModel;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbTableDetail
    {
        [Description("Table name")]
        [Category("Identity")]
        public string Name { get; set; }

        [Description("Table System name")]
        [Category("Identity")]
        public string SystemName { get; set; }

        [Description("Table description")]
        [Category("Identity")]
        //[TypeConverter(typeof(MultilineStringConverter))]
        public string Description { get; set; }

        [Description("Ignore this item for generating!")]
        public bool Ignored { get; set; }

        [Description("A column name will be used for 1-n relations.")]
        [TypeConverter(typeof(ColumnConverter))]
        public string TitleColumn { get; set; }

        [Description("Table schema")]
        public string Schema { get; set; }

        [Description("Connection type")]
        [Category("Connection")]
        public NoDbConnectionType ConnectionType { get; set; }

        [Description("Connection name")]
        [Category("Connection")]
        public string ConnectionName { get; set; }

        //[Description("Custom base class")]
        //public string BaseFullName { get; set; }
        
        [Description("Custom base project name")]
        [TypeConverter(typeof(ProjectConverter))]
        public string BaseProject { get; set; }

        [Description("Custom base table name")]
        [TypeConverter(typeof(TableConverter))]
        public string BaseTable { get; set; }

        public bool IsModel { get; set; }

        public string GetTableDbName()
        {
            if (!string.IsNullOrEmpty(SystemName))
            {
                return SystemName;
            }

            return Name;
        }

        public override string ToString()
        {
            return Name;
        }        
    }
}
