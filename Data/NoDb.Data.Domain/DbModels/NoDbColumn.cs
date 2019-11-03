using NoDb.Data.Domain.Base;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.Enums;
using System.ComponentModel;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbColumn : NoDbBase
    {
        [Category("General")]
        public string Name { get; set; }

        [Category("General")]
        public string Description { get; set; }

        [Category("General")]
        public NoDbDataType DataType { get; set; }

        [Category("General")]
        public bool Required { get; set; }

        [Category("General")]
        public bool IsAutoIncrement { get; set; }

        [Category("General")]
        [TypeConverter(typeof(EnumColumnConverter))]
        public string EnumName { get; set; }

        [Category("Extra")]
        public string DefaultValue { get; set; }

        [Category("Extra")]
        public int Precision { get; set; }

        [Category("Extra")]
        public int Scale { get; set; }

        [Category("Extra")]
        public int Length { get; set; }

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
