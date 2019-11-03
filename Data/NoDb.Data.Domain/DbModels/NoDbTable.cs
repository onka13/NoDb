using NoDb.Data.Domain.Base;
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

        public bool IsPrimaryKey(string column)
        {
            return Indices.Any(x => x.IsPrimaryKey && x.Columns.Any(y => y.ColumnName == column));
        }

        public List<NoDbColumn> GetPkColumns()
        {
            var pk = Indices.FirstOrDefault(y => y.IsPrimaryKey);
            return Columns.Where(x => pk.Columns.Any(y => y.ColumnName == x.Name)).ToList();
        }
    }
}
