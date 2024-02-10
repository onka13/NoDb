using NoDb.Data.Domain.Base;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.SearchModels;
using System.Collections.Generic;
using System.Linq;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbTable : NoDbBase
    {
        public NoDbTableDetail Detail { get; set; }
        public List<NoDbColumn> Columns { get; set; }
        public List<NoDbIndex> Indices { get; set; }
        public List<NoDbRelation> Relations { get; set; }
        public List<NoDbSearchItem> SearchItems { get; set; }

        public NoDbTable()
        {
            Detail = new NoDbTableDetail();
            Columns = new List<NoDbColumn>();
            Indices = new List<NoDbIndex>();
            Relations = new List<NoDbRelation>();
            SearchItems = new List<NoDbSearchItem>();
        }

        public override string ToString()
        {
            return Detail?.Name ?? "?";
        }

        private NoDbTable GetBaseTable()
        {
            if (!string.IsNullOrEmpty(Detail.BaseTable))
            {
                return StaticManager.Tables.FirstOrDefault(x => x.Detail.Name == Detail.BaseTable);
            }

            return null;
        }

        public bool IsPrimaryKey(string column)
        {
            return IndicesWithRelated().Any(x => x.IsPrimaryKey && x.Columns.Any(y => y.ColumnName == column));
        }

        public bool IsIndexColumn(string column)
        {
            return IndicesWithRelated().Any(x => x.Columns.Any(y => y.ColumnName == column));
        }

        public bool IsForeignKey(string column)
        {
            return RelationsWithRelated().Any(x => x.Items.Any(y => y.ColumnName == column));
        }

        public List<NoDbColumn> GetPkColumns()
        {
            var pk = IndicesWithRelated().FirstOrDefault(y => y.IsPrimaryKey);
            if (pk == null) return new List<NoDbColumn>();
            return ColumnsWithRelated().Where(x => pk.Columns.Any(y => y.ColumnName == x.Name)).ToList();
        }

        public List<NoDbColumn> ColumnsWithRelated()
        {
            var columns = new List<NoDbColumn>(Columns);
            var baseTable = GetBaseTable();
            if (baseTable != null)
            {
                columns.AddRange(baseTable.Columns);
            }
            return columns;
        }
        
        public List<NoDbIndex> IndicesWithRelated()
        {
            var indices = new List<NoDbIndex>(Indices);
            var baseTable = GetBaseTable();
            if (baseTable != null)
            {
                indices.AddRange(baseTable.Indices);
            }
            return indices;
        }
        
        public List<NoDbRelation> RelationsWithRelated()
        {
            var relations = new List<NoDbRelation>(Relations);
            var baseTable = GetBaseTable();
            if (baseTable != null)
            {
                relations.AddRange(baseTable.Relations);
            }
            return relations;
        }
    }
}
