using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;

namespace NoDb.Business.Service.Queries
{
    public class MsSqlNoDbQuery : SqlNoDbQueryBase
    {
        public override string Escape(string name)
        {
            return "[" + name + "]";
        }

        public override string ColumnDataType(NoDbColumn column)
        {
            if (column == null) return "";
            var output = column.DataType switch
            {
                NoDbDataType.BYTE => "tinyint",
                NoDbDataType.SHORT => "smallint",
                NoDbDataType.INT => "int",
                NoDbDataType.LONG => "bigint",
                NoDbDataType.BOOL => "bit",
                NoDbDataType.FLOAT => "float",
                NoDbDataType.DECIMAL => "decimal(" + column.Precision + "," + column.Scale + ")",
                NoDbDataType.DATE => "date",
                NoDbDataType.DATETIME => "datetime",
                NoDbDataType.TIMESPAN => "time",
                NoDbDataType.GUID => "uniqueidentifier",
                NoDbDataType.STRING => column.Length == 0 ? "nvarchar(MAX)" : "nvarchar(" + column.Length + ")",
                _ => throw new ArgumentOutOfRangeException(),
            };
            return output;
        }

        public override NoDbDataType DbTypeToNoDbDataType(string columnDbType)
        {
            var output = columnDbType switch
            {
                "tinyint" => NoDbDataType.BYTE,
                "smallint" => NoDbDataType.SHORT,
                "int" => NoDbDataType.INT,
                "bigint" => NoDbDataType.LONG,
                "bit" => NoDbDataType.BOOL,
                "float" => NoDbDataType.FLOAT,
                "numeric" => NoDbDataType.DECIMAL,
                "decimal" => NoDbDataType.DECIMAL,
                "date" => NoDbDataType.DATE,
                "datetime" => NoDbDataType.DATETIME,
                "time" => NoDbDataType.TIMESPAN,
                "uniqueidentifier" => NoDbDataType.GUID,
                "nvarchar" => NoDbDataType.STRING,
                "varchar" => NoDbDataType.STRING,
                _ => throw new ArgumentOutOfRangeException(),
            };
            return output;
        }

        public override string ColumnQuery(NoDbColumn column)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} {1} ", Escape(column.Name), ColumnDataType(column));
            if (column.IsAutoIncrement) stringBuilder.AppendFormat("IDENTITY(1,1) ");

            if (column.Required) stringBuilder.AppendFormat("NOT NULL ");
            else stringBuilder.AppendFormat("NULL ");
            return stringBuilder.ToString();
        }

        public override string CreateIndexQuery(NoDbTable table, NoDbIndex index)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            if (index.IsPrimaryKey)
            {
                stringBuilder.AppendFormat("ALTER TABLE {3}.{1} ADD CONSTRAINT {0} PRIMARY KEY ({2});",
                    Escape(index.Name),
                    Escape(table.Detail.GetTableDbName()),
                    string.Join(", ", index.Columns.Select(x => Escape(x.ColumnName))),
                    schema
                );
            }
            else
            {
                stringBuilder.AppendFormat("CREATE {3} INDEX {0} ON {4}.{1} ({2});",
                        Escape(index.Name),
                        Escape(table.Detail.GetTableDbName()),
                        string.Join(", ", index.Columns.Select(x => Escape(x.ColumnName) + " " + x.Sort)),
                        (index.IsUnique ? "UNIQUE" : ""),
                        schema
                    );
            }
            return stringBuilder.ToString();
        }

        public override string DeleteRelationQuery(NoDbTable table, NoDbRelation relation)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("ALTER TABLE {2}.{0} DROP CONSTRAINT {1};", Escape(table.Detail.GetTableDbName()), Escape(relation.Name), schema);
            return stringBuilder.ToString();
        }

        public override string DropIndexQuery(NoDbTable table, NoDbIndex index)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            if (index.IsPrimaryKey)
                stringBuilder.AppendFormat("ALTER TABLE {2}.{0} DROP CONSTRAINT {1};", Escape(table.Detail.GetTableDbName()), Escape(index.Name), schema);
            else
                stringBuilder.AppendFormat("DROP INDEX {0} ON {2}.{1};", Escape(index.Name), Escape(table.Detail.GetTableDbName()), schema);
            return stringBuilder.ToString();
        }

        public override string RenameColumnQuery(NoDbTable table, NoDbColumn oldColumn, NoDbColumn newColumn)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("EXEC sp_rename '{3}.{0}.{1}', '{2}', 'COLUMN';", table.Detail.GetTableDbName(), oldColumn.Name, newColumn.Name, schema);
            return stringBuilder.ToString();
        }

        public override string RenameIndexQuery(NoDbTable table, NoDbIndex oldIndex, NoDbIndex newIndex)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("EXEC sp_rename N'{0}.{1}', N'{2}', 'INDEX';", table.Detail.GetTableDbName(), oldIndex.Name, newIndex.Name);
            return stringBuilder.ToString();
        }

        public override string RenameRelationQuery(NoDbTable table, NoDbRelation oldRelation, NoDbRelation newRelation)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("EXEC sp_rename N'{2}.{0}', N'{1}';", Escape(oldRelation.Name), Escape(newRelation.Name));
            return stringBuilder.ToString();
        }

        public override string UpdateColumnQuery(NoDbTable table, NoDbColumn column)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("ALTER TABLE {2}.{0} ALTER COLUMN {1};", Escape(table.Detail.GetTableDbName()), ColumnQuery(column), schema);
            return stringBuilder.ToString();
        }
    }
}
