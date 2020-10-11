using NoDb.Data.Domain.Converters;
using System.Collections.Generic;
using System.ComponentModel;

namespace NoDb.Data.Domain.SearchModels
{
    public class NoDbSearchItem
    {
        [Category("General")]
        public string Description { get; set; }

        [Category("Advance")]
        public bool IsExportable { get; set; }

        [Category("Advance")]
        public bool IsEditable { get; set; }

        [Category("Advance")]
        public bool IsCreateable { get; set; }

        [Category("Advance")]
        public bool IsDeleteable { get; set; }

        [Category("Advance")]
        public bool HasDetail { get; set; }

        [Category("Advance")]
        public string RepositoryMethod { get; set; }

        [Category("UI")]
        public bool HideMenu { get; set; }

        [Category("General")]
        [TypeConverter(typeof(NoDbCollectionTypeConverter))]
        public List<NoDbSearchColumn> AllColumns { get; set; }
        
        [Category("General")]
        [TypeConverter(typeof(NoDbCollectionTypeConverter))]
        public List<NoDbSearchFilterColumn> Columns { get; set; }

        [Category("General")]
        [TypeConverter(typeof(NoDbCollectionTypeConverter))]
        public List<NoDbSearchGridColumn> DisplayedColumns { get; set; }

        [Category("UI")]
        public string Menu { get; set; }

        [Category("UI")]
        public int MenuOrder { get; set; }

        public NoDbSearchItem()
        {
            AllColumns = new List<NoDbSearchColumn>();
            Columns = new List<NoDbSearchFilterColumn>();
            DisplayedColumns = new List<NoDbSearchGridColumn>();
            IsExportable = IsEditable = HasDetail = IsDeleteable = IsCreateable = true;
        }

        public override string ToString()
        {
            return RepositoryMethod;
        }
    }
}
