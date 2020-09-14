using NoDb.Data.Domain.Base;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbRelation : NoDbBase
    {
        [Description("Relation Name")]
        [Category("Identity")]
        public string Name { get; set; }

        [Description("Delete rule")]
        [Category("Table Designer")]
        public NoDbRelationRule DeleteRule { get; set; }

        [Description("Update Rule")]
        [Category("Table Designer")]
        public NoDbRelationRule UpdateRule { get; set; }

        string _foreignTable;
        [Category("General")]
        [TypeConverter(typeof(TableConverter))]
        public string ForeignTable
        {
            get { return _foreignTable; }
            set
            {
                _foreignTable = value;
                StaticManager.SelectedForeignTable = value;
            }
        }

        [Category("General")]
        [TypeConverter(typeof(NoDbCollectionTypeConverter))]
        public List<NoDbRelationItem> Items { get; set; }

        [Description("Relation type")]
        [Category("General")]
        public NoDbRelationType RelationType { get; set; }

        public NoDbRelation()
        {
            Name = "Relation_" + Hash;
            Items = new List<NoDbRelationItem>();
        }

        public string RelationProperyName()
        {
            return string.Join("", Items.Select(x => x.ColumnName)).Replace("Id", "");
        }

        public override string ToString()
        {
            return Name ?? "Relation";
        }
    }
}
