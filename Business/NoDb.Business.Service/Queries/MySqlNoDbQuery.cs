using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;

namespace NoDb.Business.Service.Queries
{
    public class MySqlNoDbQuery : SqlNoDbQueryBase
    {
        public override string Escape(string name)
        {
            return "`" + name + "`";
        }

        public override string ColumnDataType(NoDbColumn column)
        {
            if (column == null) return "";
            var output = column.DataType switch
            {
                NoDbDataType.BYTE => "TINYINT",
                NoDbDataType.SHORT => "SMALLINT",
                NoDbDataType.INT => "MEDIUMINT",
                NoDbDataType.LONG => "BIGINT",
                NoDbDataType.BOOL => "BIT",
                NoDbDataType.FLOAT => "FLOAT",
                NoDbDataType.DECIMAL => "DECIMAL(" + column.Precision + "," + column.Scale + ")",
                NoDbDataType.DATE => "DATE",
                NoDbDataType.DATETIME => "DATETIME",
                NoDbDataType.TIMESPAN => "TIME",
                NoDbDataType.GUID => "uuid",
                NoDbDataType.STRING => column.Length == 0 ? "TEXT" : "TEXT(" + column.Length + ")",
                _ => throw new ArgumentOutOfRangeException(),
            };
            return output;
        }

        public override NoDbDataType DbTypeToNoDbDataType(string columnDbType)
        {
            throw new NotImplementedException();
        }

        public override string ColumnQuery(NoDbColumn column)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} {1} ", Escape(column.Name), ColumnDataType(column));
            if (column.Required) stringBuilder.AppendFormat("NOT NULL ");
            else stringBuilder.AppendFormat("NULL ");

            if (column.IsAutoIncrement) stringBuilder.AppendFormat("AUTO_INCREMENT ");
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
            stringBuilder.AppendFormat("ALTER TABLE {2}.{0} DROP FOREIGN KEY {1};", Escape(table.Detail.GetTableDbName()), Escape(relation.Name), schema);
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
            stringBuilder.AppendFormat("ALTER TABLE {3}.{0} CHANGE COLUMN {1} {2};", Escape(table.Detail.GetTableDbName()), Escape(oldColumn.Name), ColumnQuery(newColumn), schema);
            return stringBuilder.ToString();
        }

        public override string RenameIndexQuery(NoDbTable table, NoDbIndex oldIndex, NoDbIndex newIndex)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("ALTER TABLE {3}.{0} RENAME INDEX {1} TO {2};", Escape(table.Detail.GetTableDbName()), Escape(oldIndex.Name), Escape(newIndex.Name), schema);
            return stringBuilder.ToString();
        }

        public override string RenameRelationQuery(NoDbTable table, NoDbRelation oldRelation, NoDbRelation newRelation)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(DeleteRelationQuery(table, oldRelation) + "\n");
            stringBuilder.AppendFormat(CreateRelationQuery(table, newRelation));
            return stringBuilder.ToString();
        }

        public override string UpdateColumnQuery(NoDbTable table, NoDbColumn column)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("ALTER TABLE {2}.{0} MODIFY {1};", Escape(table.Detail.GetTableDbName()), ColumnQuery(column), schema);
            return stringBuilder.ToString();
        }
    }
}
