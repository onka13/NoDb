using NoDb.Data.Domain.Base;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.Enums;
using System.ComponentModel;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbColumn : NoDbBase
    {
        [Category("General")]
        [Description("Column Name")]
        public string Name { get; set; }

        [Category("General")]
        [Description("Column description")]
        public string Description { get; set; }

        [Category("General")]
        [Description("Column data type")]
        public NoDbDataType DataType { get; set; }

        [Category("General")]
        [Description("Is required?")]
        public bool Required { get; set; }

        [Category("General")]
        [Description("If true, value will be incremented on after every new record.")]
        public bool IsAutoIncrement { get; set; }

        [Category("General")]
        [TypeConverter(typeof(EnumColumnConverter))]
        public string EnumName { get; set; }

        [Category("Extra")]
        [Description("Default value")]
        public string DefaultValue { get; set; }

        [Description("Precision for decimal types")]
        [Category("Extra")]
        public int Precision { get; set; }

        [Description("Scale for decimal types")]
        [Category("Extra")]
        public int Scale { get; set; }

        [Description("Length of string types")]
        [Category("Extra")]
        public int Length { get; set; }

        [Category("Extra")]
        [Description("Column Short Name. ex. for using mongoDB column name")]
        public string ShortName { get; set; }

        [Category("Advance")]
        [Description("Is list?")]
        public bool IsList { get; set; }
        
        [Category("Advance")]
        [Description("Custom Data Type")]
        public string CustomDataType { get; set; }

        public NoDbColumn()
        {
            DataType = NoDbDataType.STRING;
            Length = 50;
            Precision = 10;
            Scale = 2;
        }

        public override string ToString()
        {
            return Name ?? "Column";
        }
    }
}
