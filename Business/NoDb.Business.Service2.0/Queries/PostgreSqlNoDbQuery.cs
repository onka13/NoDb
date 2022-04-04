using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;

namespace NoDb.Business.Service.Queries
{
    public class PostgreSqlNoDbQuery : SqlNoDbQueryBase
    {
        public override string Escape(string name)
        {
            return "\"" + name + "\"";
        }

        public override string ColumnDataType(NoDbColumn column)
        {
            if (column == null) return "";
            var output = column.DataType switch
            {
                NoDbDataType.BYTE => "smallint",
                NoDbDataType.SHORT => "smallint",
                NoDbDataType.INT => "int",
                NoDbDataType.LONG => "bigint",
                NoDbDataType.BOOL => "boolean",
                NoDbDataType.FLOAT => "real",
                NoDbDataType.DECIMAL => "numeric(" + column.Precision + "," + column.Scale + ")",
                NoDbDataType.DATE => "date",
                NoDbDataType.DATETIME => "timestamp",
                NoDbDataType.TIMESPAN => "time",
                NoDbDataType.GUID => "uuid",
                NoDbDataType.STRING => column.Length == 0 ? "text" : "character varying(" + column.Length + ")",
                _ => throw new ArgumentOutOfRangeException(),
            };
            return output;
        }

        public override string ColumnQuery(NoDbColumn column)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (column.IsAutoIncrement)
            {
                string dataType = "";
                if (column.DataType == NoDbDataType.LONG) dataType = "BIGSERIAL";
                else if (column.DataType == NoDbDataType.INT) dataType = "SERIAL";
                else dataType = "SMALLSERIAL";

                stringBuilder.AppendFormat("{0} {1}", Escape(column.Name), dataType);
            }
            else
            {
                stringBuilder.AppendFormat("{0} {1} ", Escape(column.Name), ColumnDataType(column));
                if (column.Required) stringBuilder.AppendFormat("NOT NULL ");
                else stringBuilder.AppendFormat("NULL ");
            }
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
                    Escape(table.Detail.Name),
                    string.Join(", ", index.Columns.Select(x => Escape(x.ColumnName))),
                    schema
                );
            }
            else
            {
                stringBuilder.AppendFormat("CREATE {3} INDEX {0} ON {4}.{1} ({2});",
                        Escape(index.Name),
                        Escape(table.Detail.Name),
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
            stringBuilder.AppendFormat("ALTER TABLE {2}.{0} DROP CONSTRAINT {1};", Escape(table.Detail.Name), Escape(relation.Name), schema);
            return stringBuilder.ToString();
        }

        public override string DropIndexQuery(NoDbTable table, NoDbIndex index)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            if (index.IsPrimaryKey)
                stringBuilder.AppendFormat("ALTER TABLE {2}.{0} DROP CONSTRAINT {1};", Escape(table.Detail.Name), Escape(index.Name), schema);
            else
                stringBuilder.AppendFormat("DROP INDEX {1}.{0};", Escape(index.Name), schema);
            return stringBuilder.ToString();
        }

        public override string RenameColumnQuery(NoDbTable table, NoDbColumn oldColumn, NoDbColumn newColumn)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("ALTER TABLE {3}.{0} RENAME COLUMN {1} TO {2};", Escape(table.Detail.Name), Escape(oldColumn.Name), Escape(newColumn.Name), schema);
            return stringBuilder.ToString();
        }

        public override string RenameIndexQuery(NoDbTable table, NoDbIndex oldIndex, NoDbIndex newIndex)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("ALTER INDEX {2}.{0} RENAME TO {1};", Escape(oldIndex.Name), Escape(newIndex.Name), schema);
            return stringBuilder.ToString();
        }

        public override string RenameRelationQuery(NoDbTable table, NoDbRelation oldRelation, NoDbRelation newRelation)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("ALTER TABLE {3}.{0} RENAME CONSTRAINT {1} TO {2}", Escape(table.Detail.Name), Escape(oldRelation.Name), Escape(newRelation.Name), schema);
            return stringBuilder.ToString();
        }

        public override string UpdateColumnQuery(NoDbTable table, NoDbColumn column)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("ALTER TABLE {0}.{1} " +
                        "ALTER COLUMN {2} TYPE {3} --USING ({2}::integer) \n" +
                        ",ALTER COLUMN {2} {4} NOT NULL;", schema, Escape(table.Detail.Name),
                        Escape(column.Name), ColumnDataType(column), column.Required ? "SET" : "DROP");
            return stringBuilder.ToString();
        }

        public override string DropTableQuery(NoDbTable table)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("DROP TABLE IF EXISTS {1}.{0};", Escape(table.Detail.Name), schema);
            return stringBuilder.ToString();
        }
    }
}
