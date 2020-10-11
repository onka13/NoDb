using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.Enums;
using System.ComponentModel;

namespace NoDb.Data.Domain.SearchModels
{
    public class NoDbSearchColumn
    {
        [Category("General")]
        [TypeConverter(typeof(ColumnConverter))]
        public string Name { get; set; }

        [Category("General")]
        public string Label { get; set; }

        [Category("General")]
        [TypeConverter(typeof(EnumColumnConverter))]
        public string EnumName { get; set; }

        [Category("Display")] public bool DisplayInDetail { get; set; }
        [Category("Display")] public bool DisplayInEdit { get; set; }
        [Category("Display")] public bool DisplayInCreate { get; set; }

        [Category("Component")] public string EditComponent { get; set; }
        [Category("Component")] public string CreateComponent { get; set; }
        [Category("Component")] public string DetailComponent { get; set; }

        [Category("Advance")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public NoDbSearchColumnReference Reference { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public NoDbSearchColumn()
        {
            Reference = new NoDbSearchColumnReference();
        }
    }

    public class NoDbSearchColumnReference
    {
        public string Route { get; set; }
        public string FilterField { get; set; }
        public string DataField { get; set; }
    }
}
