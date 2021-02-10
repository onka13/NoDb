using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.Enums;
using System.Collections.Generic;
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
        [Category("Display")] public int? FieldSize { get; set; }

        [Category("Component")] public string EditComponent { get; set; }
        [Category("Component")] public string CreateComponent { get; set; }
        [Category("Component")] public string DetailComponent { get; set; }

        [Category("Advance")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public NoDbSearchColumnReference Reference { get; set; }
        
        [Category("Advance")]
        [TypeConverter(typeof(NoDbCollectionTypeConverter))]
        public List<NoDbSearchColumnDepend> Depends { get; set; }

        [Category("Advance")]
        public bool IsReadOnly { get; set; }

        [Category("Advance")]
        public bool IsReadOnlyCreate { get; set; }

        [Category("Advance")]
        public bool IsReadOnlyEdit { get; set; }

        [Category("Advance")]
        public string NullString { get; set; }
        
        [Category("Advance")]
        [Description("Commo seperated (email,)")]
        public string Validations { get; set; }       

        public override string ToString()
        {
            return Name;
        }

        public NoDbSearchColumn()
        {
            Reference = new NoDbSearchColumnReference();
            Depends = new List<NoDbSearchColumnDepend>();
        }
    }

    public class NoDbSearchColumnReference
    {
        public string Route { get; set; }
        public string FilterField { get; set; }
        public string DataField { get; set; }
        public string Limit { get; set; }
        public string SortField { get; set; }
        public string SortDirection { get; set; }
        public bool AddAllButton { get; set; }
        public string TreeParentFieldId { get; set; }
        public string TreeParentFieldName { get; set; }
        public bool DisplayParentWithNoChild { get; set; }
        public bool ParentIsAddable { get; set; }
    }

    public class NoDbSearchColumnDepend
    {
        public string Name { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
