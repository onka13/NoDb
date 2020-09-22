using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;

namespace NoDb.Business.Service.Queries
{
    public abstract class SqlNoDbQueryBase : NoDbQueryBase
    {
        public string ToRule(NoDbRelationRule rule)
        {
            switch (rule)
            {
                case NoDbRelationRule.NoAction:
                    return "NO ACTION";
                case NoDbRelationRule.Cascade:
                    return "CASCADE";
                case NoDbRelationRule.SetNull:
                    return "SET NULL";
                case NoDbRelationRule.SetDefault:
                    return "SET DEFAULT";
                default:
                    throw new Exception("Unknown RULE");
            }
        }

        public override string AddColumnQuery(NoDbTable table, NoDbColumn column)
        {
            var schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            if (column.Required)
            {
                stringBuilder.AppendFormat("ALTER TABLE {2}.{0} ADD {1};\n", Escape(table.Detail.Name), ColumnQuery(column), schema);
                column.Required = false;
                stringBuilder.AppendFormat("--ALTER TABLE {2}.{0} ADD {1};\n", Escape(table.Detail.Name), ColumnQuery(column), schema);
                stringBuilder.AppendFormat("--UPDATE {0}.{1} SET {2} = 0;\n", schema, Escape(table.Detail.Name), Escape(column.Name));
                column.Required = true;
                stringBuilder.AppendFormat("--" + UpdateColumnQuery(table, column).Replace("\n", "\n--"));
            }
            else
            {
                stringBuilder.AppendFormat("ALTER TABLE {2}.{0} ADD {1};", Escape(table.Detail.Name), ColumnQuery(column), schema);
            }
            return stringBuilder.ToString();
        }

        public override string CreateRelationQuery(NoDbTable table, NoDbRelation relation)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("ALTER TABLE {2}.{0} ADD CONSTRAINT {1}\n", Escape(table.Detail.Name), Escape(relation.Name), schema);
            stringBuilder.AppendFormat("FOREIGN KEY ({0}) REFERENCES {3}.{1} ({2})\n",
                                                string.Join(",", relation.Items.Select(x => Escape(x.ColumnName))),
                                                Escape(relation.ForeignTable),
                                                string.Join(",", relation.Items.Select(x => Escape(x.ForeignColumn))),
                                                schema
                                            );
            stringBuilder.AppendFormat("ON DELETE {0}\nON UPDATE {1};", ToRule(relation.DeleteRule), ToRule(relation.UpdateRule));
            return stringBuilder.ToString();
        }

        public override string CreateTableQuery(NoDbTable table)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("CREATE TABLE {1}.{0}(\n", Escape(table.Detail.Name), schema);

            stringBuilder.AppendFormat("\t{0}\n", string.Join(",\n\t", table.ColumnsWithRelated().Select(x => ColumnQuery(x))));

            stringBuilder.Append("\n);\n");
            foreach (var item in table.Indices)
            {
                stringBuilder.Append(CreateIndexQuery(table, item));
            }
            foreach (var item in table.Relations)
            {
                stringBuilder.Append(CreateRelationQuery(table, item));
            }
            return stringBuilder.ToString();
        }

        public override string DropColumnQuery(NoDbTable table, NoDbColumn column)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("ALTER TABLE {2}.{0} DROP COLUMN {1};", Escape(table.Detail.Name), Escape(column.Name), schema);
            return stringBuilder.ToString();
        }

        public override string DropTableQuery(NoDbTable table)
        {
            string schema = GetSchema(table);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("IF OBJECT_ID('{1}.{0}', 'U') IS NOT NULL\nDROP TABLE {1}.{0};", Escape(table.Detail.Name), schema);
            return stringBuilder.ToString();
        }

        public override string TableConstraintPrimaryKeyQuery(NoDbTable table)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var pkColumns = table.GetPkColumns();
            stringBuilder.AppendFormat("CONSTRAINT pk_{0} PRIMARY KEY ({1})", table.Hash, string.Join(", ", pkColumns.Select(x => Escape(x.Name))));
            return stringBuilder.ToString();
        }
    }
}
