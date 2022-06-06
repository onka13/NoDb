using CoreCommon.Data.EntityFrameworkBase.Components;
using Microsoft.EntityFrameworkCore;
using NoDb.Business.Service.Queries;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoDb.Business.Service.Managers
{
    public class QueryManager
    {
        public static void ExecuteQuery(NoDbSettingConnection connection, string query)
        {
            if (connection.ConnectionType == NoDbConnectionType.Mssql ||
                connection.ConnectionType == NoDbConnectionType.Mysql ||
                connection.ConnectionType == NoDbConnectionType.Postgres)
            {
                var context = EmptyDbContext.Init(connection.ConnectionType.ToString(), connection.ConnectionString);
                context.Database.ExecuteSqlRaw(query);
            }
        }
        
        public static NoDbQueryBase GetNoDbQueryService(NoDbConnectionType connectionType)
        {
            switch (connectionType)
            {
                case NoDbConnectionType.Mssql:
                    return new MsSqlNoDbQuery();
                case NoDbConnectionType.Mysql:
                    return new MySqlNoDbQuery();
                case NoDbConnectionType.Postgres:
                    return new PostgreSqlNoDbQuery();
                case NoDbConnectionType.ElasticSearch:
                    break;
            }
            throw new System.Exception("Query service not defined!");
        }

        public static string GetTableQueries(List<NoDbTable> tables, NoDbConnectionType connectionType, bool dropBefore)
        {
            var queryService = GetNoDbQueryService(connectionType);
            // TODO: order algorithm
            tables = tables.OrderBy(x => x, new NoDbTableComparer()).OrderBy(x => x, new NoDbTableComparer()).OrderBy(x => x, new NoDbTableComparer()).ToList();

            StringBuilder output = new StringBuilder();
            if (dropBefore)
            {
                // drop query must reverse for foreign keys
                for (int i = 0; i < tables.Count; i++)
                {
                    var table = tables[i];
                    output.AppendLine(queryService.DropTableQuery(table));
                }
                output.Append("\n");
            }
            for (int i = tables.Count - 1; i >= 0; i--)
            {
                var table = tables[i];
                output.AppendLine(queryService.CreateTableQuery(table) + "\n\n");
            }

            return output.ToString();
        }
    }
    public class NoDbTableComparer : IComparer<NoDbTable>
    {
        public int Compare(NoDbTable table1, NoDbTable table2)
        {
            //System.Diagnostics.Debug.WriteLine("table1 {0}-{2}, table2 {1}-{3}", table1.Detail.Name, table2.Detail.Name, table1.Relations.Count, table2.Relations.Count);

            if (table1.Detail.Name == table2.Detail.Name)
                return 0;

            if (table2.Relations.Count == 0)
                return -1;

            if (table1.Relations.Any(x => x.ForeignTable == table2.Detail.Name))
                return -1;

            return 1;
        }
    }
}
